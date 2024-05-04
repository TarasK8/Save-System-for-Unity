using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TarasK8.SaveSystem
{
    public class AssetsContainer : ScriptableObject
    {
        public const string RESOURCES_PATH = "Main Asset Container";
        public const string PATH = "Assets/Resources/" + RESOURCES_PATH + ".asset";

        private static AssetsContainer _instance;

        [SerializeField] private string[] _paths;
        [SerializeField] private List<AssetEntry> _assets;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SetupInstance()
        {
            _instance = Resources.Load<AssetsContainer>(RESOURCES_PATH);
        }

        public static bool CanReference(Object asset, out string guid)
        {
            if(_instance == null)
            {
                guid = null;
                return false;
            }

            return _instance.TryGetGuid(asset, out guid);
        }

        public static bool TryResolveReference(string guid, out Object asset)
        {
            if (_instance == null)
            {
                asset = null;
                return false;
            }

            return _instance.TryGetAsset(guid, out asset);
        }

        private bool TryGetGuid(Object asset, out string guid)
        {
            foreach (var entry in _assets)
            {
                if (ReferenceEquals(asset, entry.Asset))
                {
                    guid = entry.Guid;
                    return true;
                }
            }
            guid = null;
            return false;
        }

        private bool TryGetAsset(string guid, out Object asset)
        {
            foreach (var entry in _assets)
            {
                if (guid == entry.Guid)
                {
                    asset = entry.Asset;
                    return true;
                }
            }
            asset = null;
            return false;
        }

#if UNITY_EDITOR
        public void Initialize() => _assets = new List<AssetEntry>();

        public void ClearAll() => _assets.Clear();

        public void LoadAssets()
        {
            var a = FindAssets(_paths);
            _assets.AddRange(a);
        }

        public void AddWithContextMenu(IEnumerable<Object> assets)
        {
            var toAdd = Filter(assets);
            int addedCount = toAdd.Count();
            if (addedCount == 0)
                Debug.Log("These assets are already added");
            else
                Debug.Log($"Added {addedCount} assets");
            _assets.AddRange(toAdd);
        }

        private IEnumerable<AssetEntry> Filter(IEnumerable<Object> assets)
        {
            foreach (var asset in assets)
            {
                if(TryGetGuid(asset, out _) == false)
                {
                    string path = AssetDatabase.GetAssetPath(asset);
                    string guid = AssetDatabase.GUIDToAssetPath(path);
                    AssetEntry assetEntry = new AssetEntry(guid, asset);
                    yield return assetEntry;
                }
            }
        }

        public IEnumerable<AssetEntry> FindAssets(params string[] paths)
        {
            string[] filteredPaths = paths.Where(x => string.IsNullOrEmpty(x) == false).ToArray();

            if(filteredPaths.Length == 0) yield break;

            string[] guids = AssetDatabase.FindAssets("t:Object", filteredPaths);

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (TryGetGuid(asset, out _) == false)
                {
                    AssetEntry assetEntry = new AssetEntry(guid, asset);
                    yield return assetEntry;
                }

            }
        }
#endif
    }
}