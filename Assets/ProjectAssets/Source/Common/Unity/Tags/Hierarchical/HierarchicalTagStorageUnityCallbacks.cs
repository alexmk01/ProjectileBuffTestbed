using UnityEngine;
using static Common.Unity.Tags.Hierarchical.HierarchicalTagStorage;

namespace Common.Unity.Tags.Hierarchical
{
    public static class HierarchicalTagStorageUnityCallbacks
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            BoostrapRuntime();
        }

#if UNITY_EDITOR
        [UnityEditor.Callbacks.PostProcessBuild]
        private static void OnPostprocessBuild(UnityEditor.BuildTarget target, string pathToBuiltProject)
        {
            BuildPackedTagsData(pathToBuiltProject);
        }
#endif
    }
}