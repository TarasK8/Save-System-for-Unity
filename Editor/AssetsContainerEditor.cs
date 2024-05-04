using System;
using System.IO;
using TarasK8.SaveSystem;
using UnityEditor;
using UnityEngine;

namespace TarasK8.SaveSystemEditor
{
    public class AssetsContainerEditor : EditorWindow
    {
        private AssetsContainer _container;
        private SerializedObject _serializedObject;
        private SerializedProperty _paths;
        private SerializedProperty _assets;

        private Vector2 _assetsScroll;
        private Vector2 _pathsScroll;

        [MenuItem("Window/Saveable References")]
        public static void ShowWindow()
        {
            GetWindow<AssetsContainerEditor>("Saveable References").Show();
        }

        [MenuItem("Assets/Add to Saveable References")]
        public static void AddToContainer()
        {
            AssetsContainer container = GetOrCreateContainer();
            UnityEngine.Object[] objects = Selection.objects;
            if (objects.Length == 0)
                return;
            container.AddWithContextMenu(objects);
        }

        private void Initialize()
        {
            _container = GetOrCreateContainer();
            _serializedObject = new SerializedObject(_container);
            _paths = _serializedObject.FindProperty("_paths");
            _assets = _serializedObject.FindProperty("_assets");
        }

        private void OnEnable()
        {
            base.minSize = new Vector2(400f, 400f);
            Initialize();
        }

        public void OnGUI()
        {
            if(_container == null)
            {
                if(GUILayout.Button("Create Assets Container"))
                {
                    Initialize();
                }
                return;
            }
            _serializedObject.Update();
            DrawPaths();
            DrawAssetsList();
            _serializedObject.ApplyModifiedProperties();
        }

        private void DrawPaths()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Paths ({_paths.arraySize})", EditorStyles.boldLabel, GUILayout.MinWidth(120f));
            if (GUILayout.Button("Add", GUILayout.MinWidth(70f)))
                _paths.InsertArrayElementAtIndex(0);
            EditorGUILayout.EndHorizontal();
            _pathsScroll = EditorGUILayout.BeginScrollView(_pathsScroll, GUILayout.MaxHeight(160f), GUILayout.MinHeight(0f), GUILayout.Height(160f));
            for (int i = 0; i < _paths.arraySize; i++)
            {
                DrawPathElement(i);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawPathElement(int index)
        {
            var element = _paths.GetArrayElementAtIndex(index);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Path {index}", GUILayout.Width(60f));
            EditorGUILayout.PropertyField(element, new GUIContent(string.Empty));
            if(GUILayout.Button("Select", GUILayout.Width(50f)))
            {
                string path = EditorUtility.OpenFolderPanel("Select path", "Assets", "");

                if (path != "Assets" || path.Length != 0)
                    path = path.Substring(path.IndexOf("Assets"));

                if (AssetDatabase.IsValidFolder(path))
                    element.stringValue = path;
                else
                    _paths.DeleteArrayElementAtIndex(index);
            }
            DrawDeleteButton(_paths, index);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAssetsList()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Saved Assets ({_assets.arraySize})", EditorStyles.boldLabel, GUILayout.MinWidth(120f));
            if (GUILayout.Button("Load From Paths"))
            {
                _container.LoadAssets();
            }
            if (GUILayout.Button("Clear All"))
            {
                _container.ClearAll();
            }
            EditorGUILayout.EndHorizontal();
            _assetsScroll = EditorGUILayout.BeginScrollView(_assetsScroll, /*GUILayout.MaxHeight(200f), */GUILayout.MinHeight(0f));
            for (int i = 0; i < _assets.arraySize; i++)
            {
                DrawAssetElement(i);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawAssetElement(int index)
        {
            var element = _assets.GetArrayElementAtIndex(index);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent($"Asset {index}"), GUILayout.Width(70f));
            GUI.enabled = false;
            EditorGUILayout.PropertyField(element.FindPropertyRelative("_asset"), new GUIContent(string.Empty));
            GUI.enabled = true;
            if(GUILayout.Button("Copy Ref", GUILayout.Width(70f)))
            {
                string reference = element.FindPropertyRelative("_guid").stringValue;
                EditorGUIUtility.systemCopyBuffer = reference;
                Debug.Log($"Copied Reference: {reference}");
            }
            DrawDeleteButton(_assets, index);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawDeleteButton(SerializedProperty array, int index)
        {
            if (GUILayout.Button("Remove", GUILayout.Width(60f)))
            {
                array.DeleteArrayElementAtIndex(index);
            }
        }

        private static AssetsContainer GetOrCreateContainer()
        {
            var container = Resources.Load<AssetsContainer>(AssetsContainer.RESOURCES_PATH);

            if (container)
            {
                return container;
            }

            container = ScriptableObject.CreateInstance<AssetsContainer>();
            container.Initialize();

            string directory = Path.GetDirectoryName(AssetsContainer.PATH);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            AssetDatabase.CreateAsset(container, AssetsContainer.PATH);
            AssetDatabase.SaveAssets();

            return container;
        }
    }

    [CustomEditor(typeof(AssetsContainer))]
    public class AssetsContainerEditorTemp : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor"))
            {
                AssetsContainerEditor.ShowWindow();
            }
        }
    }
}