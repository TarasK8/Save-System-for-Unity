using Newtonsoft.Json.Linq;
using System.IO;
using TarasK8.SaveSystem;
using TarasK8.SaveSystemEditor.JEditor;
using UnityEditor;
using UnityEngine;

namespace TarasK8.SaveSystemEditor
{
    [CustomEditor(typeof(SaveSystemObject))]
    [CanEditMultipleObjects]
    public class SaveSystemObjectEditor : Editor
    {
        private SaveSystemObject _sceneSave;
        private SFile _file;
        private JsonEditor _fileEditor;

        private SerializedProperty _fileName_string;
        private SerializedProperty _path_enum;
        private SerializedProperty _customPath_string;

        private SerializedProperty _initializeOnAwake_bool;
        private SerializedProperty _collectSaveablesEverySave_bool;
        private SerializedProperty _useEncryption_bool;
        private SerializedProperty _encryptionPassword_string;
        private SerializedProperty _autoSaving_bool;
        private SerializedProperty _savePerion_int;

        private SerializedProperty _onSave_event;
        private SerializedProperty _onLoad_event;

        private Vector2 _scrollPosition = Vector2.zero;

        private void OnEnable()
        {
            _sceneSave = (SaveSystemObject)target;
            _fileName_string = serializedObject.FindProperty("_fileName");
            _path_enum = serializedObject.FindProperty("_path");
            _customPath_string = serializedObject.FindProperty("_customPath");

            _initializeOnAwake_bool = serializedObject.FindProperty("_initializeOnAwake");
            _collectSaveablesEverySave_bool = serializedObject.FindProperty("_collectSaveablesEverySave");
            _useEncryption_bool = serializedObject.FindProperty("_useEncryption");
            _encryptionPassword_string = serializedObject.FindProperty("_encryptionPassword");
            _autoSaving_bool = serializedObject.FindProperty("_autoSaving");
            _savePerion_int = serializedObject.FindProperty("_savePeriod");

            _onSave_event = serializedObject.FindProperty("OnSave");
            _onLoad_event = serializedObject.FindProperty("OnLoad");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Path", EditorStyles.boldLabel);
#if UNITY_EDITOR_WIN
            if (GUILayout.Button("Show in Explorer"))
                ShowExplorer(_sceneSave.GetPath());
#endif
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_fileName_string);
            EditorGUILayout.PropertyField(_path_enum);
            if (_path_enum.enumValueIndex == 2)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_customPath_string);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(10f);

            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_initializeOnAwake_bool);
            EditorGUILayout.PropertyField(_collectSaveablesEverySave_bool);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_useEncryption_bool);
            if(_useEncryption_bool.boolValue)
                EditorGUILayout.PropertyField(_encryptionPassword_string, new GUIContent(string.Empty));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_autoSaving_bool);
            if(_autoSaving_bool.boolValue)
                EditorGUILayout.PropertyField(_savePerion_int, new GUIContent("Period (seconds)"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10f);

            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_onSave_event);
            EditorGUILayout.PropertyField(_onLoad_event);

            serializedObject.ApplyModifiedProperties();

            // File preview
            
            EditorGUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("File Preview", EditorStyles.boldLabel);
            if(_file != null && GUILayout.Button("Save Changes"))
            {
                _file.LoadFromJson(_fileEditor.GetJson());
                _file.Save(_sceneSave.GetPath(), _sceneSave.GetPassword());
            }
            if (GUILayout.Button(_file == null ? "Load" : "Refresh"))
            {
                _file = new SFile();
                _file.Load(_sceneSave.GetPath(), _sceneSave.GetPassword());

                _fileEditor = new JsonEditor(JToken.Parse(_file.GetJson()), this);
            }
            EditorGUILayout.EndHorizontal();

            if(_file != null)
            {
                EditorGUILayout.Space(10f);
                _fileEditor.DrawEditor();
            }
            
        }

        private void ShowExplorer(string itemPath)
        {
#if UNITY_EDITOR_WIN
            if (Directory.Exists(Path.GetDirectoryName(itemPath)))
            {
                itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
                System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
            }
            else
            {
                Debug.LogWarning($"Path {itemPath} not exists");
            }
#endif
        }
    }
}
