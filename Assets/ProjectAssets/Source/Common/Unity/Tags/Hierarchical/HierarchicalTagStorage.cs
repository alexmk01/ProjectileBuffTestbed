using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Common.Tags;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions;

namespace Common.Unity.Tags.Hierarchical
{
    public static class HierarchicalTagStorage
    {
        private readonly struct TagDescription
        {
            public readonly Guid TagId;
            public readonly int TagIndex;

            public TagDescription(Guid tagId, int tagIndex)
            {
                TagId = tagId;
                TagIndex = tagIndex;
            }
        }

        private const string DataFileName = "TagsData";
        private const string BinaryDataExtension = ".dat";
        private const string ReadableDataExtension = ".json";

        //TODO: get rid of GUID and use id generation from Tag?
        private static readonly Dictionary<Guid, string> TagsById = new(128);
        //TODO: index order dependent storage may cause vcs conflicts
        private static readonly Dictionary<string, TagDescription> TagsByValue = new(128, Tag.TagValueComparer);
        private static readonly List<HierarchicalTagsData.SerializableTag> TagsSerializationBuffer = new(256);
        private static (string, bool)? tagsDataPathInfo = null;
        private static bool isActualTagsDataLoaded;
        private static DateTime? lastTagsModificationDate = null;
        private static bool isGameStarted;

        private static string GetTagsDataFolderPath(string streamingAssetsPath) => Path.Combine(streamingAssetsPath, "Tags");
        private static string GetTagsDataFolderPath() => GetTagsDataFolderPath(Application.streamingAssetsPath);

        private static bool TryGetCurrentTagsDataPath(out string dataPath, out bool isBinaryData)
        {
            if (tagsDataPathInfo.HasValue)
            {
                (dataPath, isBinaryData) = tagsDataPathInfo.Value;
                return !string.IsNullOrEmpty(dataPath);
            }

            string dataFolderPath = GetTagsDataFolderPath();
            dataPath = null;
            isBinaryData = false;

            if (Directory.Exists(dataFolderPath))
            {
                foreach (string path in Directory.EnumerateFiles(dataFolderPath))
                {
                    if (Path.GetFileNameWithoutExtension(path) != DataFileName)
                    {
                        continue;
                    }

                    string fileExtension = Path.GetExtension(path);

                    if (fileExtension == ReadableDataExtension)
                    {
                        dataPath = path;
                        tagsDataPathInfo = (dataPath, false);
                        return true;
                    }
                    else if (fileExtension == BinaryDataExtension)
                    {
                        dataPath = path;
                        isBinaryData = true;
                        tagsDataPathInfo = (dataPath, true);
                        return true;
                    }
                }
            }

            return false;
        }

        private static List<HierarchicalTagsData.SerializableTag> LoadTagsData(string dataPath, bool isBinaryData)
        {
            if (isBinaryData)
            {
                var decompressedData = new GZipStream(File.Open(dataPath, FileMode.Open), CompressionMode.Decompress);
                var dataReadingStream = new MemoryStream();
                decompressedData.CopyTo(dataReadingStream);
                decompressedData.Close();
                using var reader = new BinaryReader(dataReadingStream);
                List<HierarchicalTagsData.SerializableTag> data = new();
                dataReadingStream.Position = 0;

                while (dataReadingStream.Position < dataReadingStream.Length)
                {
                    data.Add(new HierarchicalTagsData.SerializableTag(reader));
                }
                
                return data;
            }
            else
            {
                string serializedTagsData = File.ReadAllText(dataPath);
                return JsonConvert.DeserializeObject<HierarchicalTagsData>(serializedTagsData).tags;
            }
        }

        private static void GetTags(out Dictionary<Guid, string> tagsById, out Dictionary<string, TagDescription> tagsByValue)
        {
            tagsById = TagsById;
            tagsByValue = TagsByValue;
            bool updateTags = !isActualTagsDataLoaded;
            bool isTagsDataFileExist = TryGetCurrentTagsDataPath(out string tagsDataPath, out bool isBinaryTagsData);

            if (!updateTags && isTagsDataFileExist && (!isGameStarted || !lastTagsModificationDate.HasValue))
            {
                DateTime tagsModificationDate = File.GetLastWriteTime(tagsDataPath);
                updateTags = lastTagsModificationDate != tagsModificationDate;
                lastTagsModificationDate = tagsModificationDate;
            }

            if (updateTags)
            {
                tagsById.Clear();
                tagsByValue.Clear();
                List<HierarchicalTagsData.SerializableTag> loadedTags = isTagsDataFileExist ? LoadTagsData(tagsDataPath, isBinaryTagsData) : null;

                if (loadedTags != null)
                {
                    int tagIndex = 0;

                    foreach (HierarchicalTagsData.SerializableTag loadedTag in loadedTags)
                    {
                        if (tagsByValue.ContainsKey(loadedTag.tagString))
                        {
                            Common.DebugUtils.LogWarning($"{loadedTag.tagString} Tag already exists.");
                            continue;
                        }

                        Assert.IsFalse(string.IsNullOrEmpty(loadedTag.tagId));
                        Assert.IsFalse(string.IsNullOrEmpty(loadedTag.tagString));
                        Guid tagId = new(loadedTag.tagId);
                        tagsById.Add(tagId, loadedTag.tagString);
                        tagsByValue.Add(loadedTag.tagString, new TagDescription(tagId, tagIndex++));
                    }
                }

                isActualTagsDataLoaded = true;
            }
        }

        private static HierarchicalTagsData.SerializableTag GetSerializableTag(Guid tagId, string tag)
        {
            return new HierarchicalTagsData.SerializableTag(tagId, tag);
        }

        internal static void SaveTagsData(List<HierarchicalTagsData.SerializableTag> data, bool isBinaryData = false, bool updateTags = true, string tagsDataFolderPath = null)
        {
            string dataFolderPath = string.IsNullOrEmpty(tagsDataFolderPath) ? GetTagsDataFolderPath() : tagsDataFolderPath;
            string dataPath = Path.Combine(dataFolderPath, $"{DataFileName}{(isBinaryData ? BinaryDataExtension : ReadableDataExtension)}");

            if (!File.Exists(dataPath))
            {
                string tagsFolderPath = Path.GetDirectoryName(dataPath);
                if (!Directory.Exists(tagsFolderPath)) Directory.CreateDirectory(tagsFolderPath);
                using (File.Create(dataPath)) { }
            }

            if (isBinaryData)
            {
                var zipStream = new GZipStream(File.Open(dataPath, FileMode.Open), CompressionMode.Compress);
                using var writer = new BinaryWriter(zipStream);

                for (int i = 0; i < data.Count; i++)
                {
                    data[i].WriteBinary(writer);
                }
            }
            else
            {
                var tagsData = new HierarchicalTagsData { tags = data };
                File.WriteAllText(dataPath, JsonConvert.SerializeObject(tagsData, Formatting.Indented));
            }

            if (updateTags) isActualTagsDataLoaded = false;
        }

        public static IReadOnlyDictionary<Guid, string> GetCurrentTags()
        {
            GetTags(out Dictionary<Guid, string> tagsById, out _);
            return tagsById;
        }

        public static string GetTagValue(Guid tagId)
        {
            GetTags(out Dictionary<Guid, string> tagsById, out _);
            tagsById.TryGetValue(tagId, out string tag);
            return tag;
        }

        public static Tag GetTag(Guid tagId)
        {
            string tagValue = GetTagValue(tagId);
            return string.IsNullOrEmpty(tagValue) ? Tag.Empty : new(tagValue);
        }

        public static bool TryGetTagId(string tag, out Guid tagId)
        {
            GetTags(out _, out Dictionary<string, TagDescription> tagsByValue);

            if (tagsByValue.TryGetValue(tag, out TagDescription tagDescription))
            {
                tagId = tagDescription.TagId;
                return true;
            }

            tagId = Guid.Empty;
            return false;
        }

        public static List<HierarchicalTagsData.SerializableTag> GetCurrentTagsSerializableData()
        {
            TagsSerializationBuffer.Clear();
            GetTags(out Dictionary<Guid, string> tagsById, out _);

            foreach (KeyValuePair<Guid, string> currentTagData in tagsById)
            {
                TagsSerializationBuffer.Add(GetSerializableTag(currentTagData.Key, currentTagData.Value));
            }

            return TagsSerializationBuffer;
        }
        
        public static bool RenameTag
        (
            HierarchicalTagReference tagReference,
            string newTagName,
            List<HierarchicalTagsData.SerializableTag> serializableTagsData = null,
            bool saveTags = true
        )
        {
            if (!string.IsNullOrEmpty(newTagName))
            {
                Assert.IsTrue(HierarchicalTagUtility.SplitTag(newTagName, out _) <= 1, newTagName);
                Tag tag = tagReference.GetValue();
                string tagValue = tag.ToString();

                if (TryGetTagId(tagValue, out Guid tagId))
                {
                    GetTags(out Dictionary<Guid, string> tagsById, out Dictionary<string, TagDescription> tagsByValue);
                    int tagDepth = HierarchicalTagUtility.SplitTag(tagValue, out Range[] tags);
                    Range localTagRange = tags[tagDepth - 1];
                    string localTag = tagValue[localTagRange];
                    
                    if (localTag != newTagName)
                    {
                        string newTagValue = tagDepth > 1 ? HierarchicalTagUtility.GetFullTag(tagValue[..localTagRange.Start], newTagName) : newTagName;

                        if (!tagsByValue.ContainsKey(newTagValue))
                        {
                            TagDescription lastTagDescription = tagsByValue[tagValue];

                            if (serializableTagsData != null)
                            {
                                int tagIndex = lastTagDescription.TagIndex;
                                HierarchicalTagsData.SerializableTag serializableData = serializableTagsData[tagIndex];
                                Assert.AreEqual(serializableData.tagString, tagValue);
                                serializableData.tagString = newTagValue;
                                serializableTagsData[tagIndex] = serializableData;
                            }

                            tagsById[tagId] = newTagValue;
                            tagsByValue[newTagValue] = lastTagDescription;
                            tagsByValue.Remove(tagValue);
                            if (saveTags) SaveTagsData();
                            return true;
                        }
                        else
                        {
                            Common.DebugUtils.LogError($"{newTagValue} already exists.");
                        }
                    }
                }
                else
                {
                    Common.DebugUtils.LogError($"There is no such Tag {tagValue}.");
                }
            }

            return false;
        }

        public static Guid GenerateTagId() => Guid.NewGuid();

        public static void AddTag
        (
            string tag,
            out HierarchicalTagReference registeredTag,
            List<HierarchicalTagsData.SerializableTag> serializableTagsData = null,
            bool saveTags = true
        )
        {
            registeredTag = new HierarchicalTagReference();
            if (string.IsNullOrWhiteSpace(tag)) return;
            GetTags(out Dictionary<Guid, string> tagsById, out Dictionary<string, TagDescription> tagsByValue);

            if (tagsByValue.TryGetValue(tag, out TagDescription registeredTagDescription))
            {
                registeredTag = new HierarchicalTagReference(new SerializableGUID(registeredTagDescription.TagId));
                return;
            }

            int localTagsCount = HierarchicalTagUtility.SplitTag(tag, out Range[] tags);
            if (localTagsCount == 0) return;
            string localTag = null;
            int parentTagIndex = tagsById.Count - 1;

            for (int i = 0; i < localTagsCount; i++)
            {
                if (localTag == null) localTag = HierarchicalTagUtility.GetTag(tag, in tags[i]);
                else localTag = HierarchicalTagUtility.GetFullTag(localTag, HierarchicalTagUtility.GetTag(tag, in tags[i]));

                if (!tagsByValue.TryGetValue(localTag, out TagDescription tagDescription))
                {
                    serializableTagsData ??= GetCurrentTagsSerializableData();
                    Guid tagId = GenerateTagId();
                    int newTagIndex = ++parentTagIndex;
                    tagsByValue.Add(localTag, new TagDescription(tagId, newTagIndex));
                    tagsById.Add(tagId, tag);
                    serializableTagsData.Insert(newTagIndex, GetSerializableTag(tagId, localTag));
                }
                else
                {
                    parentTagIndex = tagDescription.TagIndex;
                }
            }

            if (saveTags && serializableTagsData != null) SaveTagsData(serializableTagsData, updateTags: false);
            registeredTag = new HierarchicalTagReference(new SerializableGUID(tagsByValue[tag].TagId));
        }

        public static void SaveTagsData(List<HierarchicalTagsData.SerializableTag> data, bool updateTags = true)
        {
            SaveTagsData(data, false, updateTags);
        }
        
        public static void SaveTagsData(bool updateTags = true)
        {
            SaveTagsData(GetCurrentTagsSerializableData(), false, updateTags);
        }

        public static void BuildPackedTagsData(string buildPath)
        {
            string tagsDataFolderPath = GetTagsDataFolderPath(Path.Combine(Path.GetDirectoryName(buildPath), $"{Application.productName}_Data", "StreamingAssets"));

            if (Directory.Exists(tagsDataFolderPath))
            {
                string readableTagsDataPath = Path.Combine(tagsDataFolderPath, $"{DataFileName}{ReadableDataExtension}");
                Common.DebugUtils.Log(readableTagsDataPath);

                if (File.Exists(readableTagsDataPath))
                {
                    try
                    {
                        File.Delete(readableTagsDataPath);
                        SaveTagsData(GetCurrentTagsSerializableData(), true, tagsDataFolderPath: tagsDataFolderPath);
                    }
                    catch (Exception e)
                    {
                        Common.DebugUtils.LogError($"Unable to overwrite readable tags data {readableTagsDataPath}: " + e.Message);
                    }
                }
            }
        }
        
        public static void BoostrapRuntime()
        {
            tagsDataPathInfo = null;
            lastTagsModificationDate = null;
            isGameStarted = true;
        }
    }
}
