using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TarasK8.SaveSystem
{
    [CustomEditor(typeof(SaveSystemObject))]
    [CanEditMultipleObjects]
    public class SceneSaveEditor : Editor
    {
        private SaveSystemObject _sceneSave;
        private File _file;

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
            if(_path_enum.enumValueIndex == 2)
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

            // TODO
            /*
            EditorGUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("File Preview", EditorStyles.boldLabel);
            if (GUILayout.Button("Load"))
            {
                _file = new File(_sceneSave.GetPath(), true, _encryptionPassword_string.stringValue);
            }
            EditorGUILayout.EndHorizontal();

            if(_file != null)
            {
                var data = _file.GetAllRawData();
                foreach (var item in data)
                {
                    DrawFileValue(item);
                }
                EditorGUILayout.BeginHorizontal();
                GUILayout.Button("Save");
                GUILayout.Button("Delete");
                EditorGUILayout.EndHorizontal();
            }
            */
        }

        private void DrawFileValue(KeyValuePair<string, object> item)
        {
            var maxHeight = GUILayout.MaxHeight(30f);

            EditorGUILayout.BeginHorizontal(maxHeight);
            EditorGUILayout.TextArea(item.Key, GUILayout.Width(80f));
            object value = item.Value;
            if (value is JToken)
            {
                var obj = ((JToken)value);
                EditorGUILayout.TextArea(obj.ToString(), maxHeight);
                EditorGUILayout.LabelField(obj.Type.ToString());
            }
            else
            {
                EditorGUILayout.TextArea(value.ToString());
            }
            EditorGUILayout.LabelField(value.GetType().Name);
            EditorGUILayout.EndHorizontal();
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
