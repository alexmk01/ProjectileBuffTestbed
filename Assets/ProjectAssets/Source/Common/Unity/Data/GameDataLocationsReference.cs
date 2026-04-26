using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace Common.Unity.Data
{
    [Serializable]
    public sealed class GameDataLocationsReference
    {
        private static void PassDataToStorage<T>(IList<T> loadedData, IDictionary<string, T> dataStorage) where T : Object
        {
            if (dataStorage == null || loadedData.IsNullOrEmpty()) return;

            for (int i = 0; i < loadedData.Count; i++)
            {
                T data = loadedData[i];
                //TODO: use local paths
                string dataPath = data.name;
                if (!dataStorage.TryGetValue(dataPath, out _)) dataStorage.Add(dataPath, data);
                else Common.DebugUtils.LogWarning($"{dataPath} of type {typeof(T)} is a duplicate and will be skipped.");
            }
        }

        public string[] dataPaths;
        public AssetLabelReference[] dataLabelReferences;
        
        private List<string> GetValidAddressablesKeys()
        {
            var addressablesKeysBuffer = new List<string>(dataLabelReferences.Length);

            for (int i = 0; i < dataLabelReferences.Length; i++)
            {
                if (dataLabelReferences[i].RuntimeKeyIsValid())
                {
                    addressablesKeysBuffer.Add(dataLabelReferences[i].labelString);
                }
            }

            return addressablesKeysBuffer;
        }

        private IList<IResourceLocation> GetResourceLocations(IList<string> dataKeys)
        {
            return Addressables.LoadResourceLocationsAsync(dataKeys, Addressables.MergeMode.Union).WaitForCompletion();
        }

        private bool TryStartAddressableAssetsLoading<T>(out AsyncOperationHandle<IList<T>> operationHandle) where T : Object
        {
            List<string> dataKeys = GetValidAddressablesKeys();
            //Otherwise it will throw execptions due to empty folders with a label
            //TODO: more sane way?
            if (!GetResourceLocations(dataKeys).IsNullOrEmpty())
            {
                operationHandle = Addressables.LoadAssetsAsync<T>(dataKeys, null, Addressables.MergeMode.Union);
                return true;
            }

            operationHandle = default;
            return false;
        }

        private IList<T> LoadAssets<T>() where T : Object
        {
            var loadedData = new List<T>(32);

            for (int i = 0; i < dataPaths.Length; i++)
            {
                string path = dataPaths[i];

                if (!string.IsNullOrEmpty(path))
                {
                    loadedData.AddRange(Resources.LoadAll<T>(path));
                }
            }

            return loadedData;
        }

        private IList<T> LoadComponents<T>(IList<GameObject> prefabs) where T : class
        {
            if (prefabs.Count == 0) return Array.Empty<T>();
            var components = new List<T>(prefabs.Count);

            for (int i = 0; i < prefabs.Count; i++)
            {
                //TODO: addressables system can load destroyed prefabs. Clear cache somehow?
                if (prefabs[i] != null && prefabs[i].TryGetComponent(out T component))
                {
                    components.Add(component);
                }
            }

            return components;
        }

        public IList<IResourceLocation> GetResourceLocations()
        {
            if (dataLabelReferences.IsNullOrEmpty())
            {
                if (!dataPaths.IsNullOrEmpty())
                {
                    var locations = new List<IResourceLocation>();

                    foreach (string path in dataPaths)
                    {
                        if (string.IsNullOrEmpty(path)) continue;
                        locations.Add(new ResourceLocationBase(path, path, "Resources", typeof(Object)));
                    }

                    return locations;
                }

                return Array.Empty<IResourceLocation>();
            }

            return Addressables.LoadResourceLocationsAsync(GetValidAddressablesKeys(), Addressables.MergeMode.Union).WaitForCompletion();
        }

        public IList<T> LoadData<T>(IDictionary<string, T> dataStorage = null) where T : Object
        {
            IList<T> loadedData = null;

            if (dataLabelReferences.Length != 0)
            {
                if (TryStartAddressableAssetsLoading(out AsyncOperationHandle<IList<T>> operationHandle))
                {
                    loadedData = operationHandle.WaitForCompletion();
                }
            }
            else if (dataPaths.Length != 0)
            {
                loadedData = LoadAssets<T>();
            }

            PassDataToStorage(loadedData, dataStorage);
            return loadedData ?? Array.Empty<T>();
        }

        public async Task<IList<T>> LoadDataAsync<T>(IDictionary<string, T> dataStorage = null) where T : Object
        {
            IList<T> loadedData = null;

            if (dataLabelReferences.Length != 0)
            {
                if (TryStartAddressableAssetsLoading(out AsyncOperationHandle<IList<T>> operationHandle))
                {
                    loadedData = await operationHandle.Task;
                }
            }
            else if (dataPaths.Length != 0)
            {
                loadedData = await Task.Run(() => LoadAssets<T>());
            }

            PassDataToStorage(loadedData, dataStorage);
            return loadedData ?? Array.Empty<T>();
        }

        public IList<T> LoadComponents<T>() where T : class
        {
            return LoadComponents<T>(LoadData<GameObject>());
        }

        public async Task<IList<T>> LoadComponentsAsync<T>() where T : class
        {
            IList<GameObject> prefabs = await LoadDataAsync<GameObject>();
            return LoadComponents<T>(prefabs);
        }
    }
}
