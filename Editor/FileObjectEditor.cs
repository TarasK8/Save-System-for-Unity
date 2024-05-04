using Newtonsoft.Json.Linq;
using TarasK8.SaveSystem;
using TarasK8.SaveSystemEditor.JEditor;
using UnityEditor;
using UnityEngine;

namespace TarasK8.SaveSystemEditor
{
    [CustomEditor(typeof(FileObject))]
    public class FileObjectEditor : Editor
    {
        private SerializedProperty _json_string;
        private JsonEditor _fileEditor;

        private void OnEnable()
        {
            _json_string = serializedObject.FindProperty("_json");

            string json = _json_string.stringValue;
            JToken token =  string.IsNullOrEmpty(json) ? new JObject() : JToken.Parse(json);
            _fileEditor = new JsonEditor(token, this);
        }

        public override void OnInspectorGUI()
        {
            _fileEditor.DrawEditor();
        }

        private void OnDisable()
        {
            _json_string.stringValue = _fileEditor.GetJson();
            serializedObject.ApplyModifiedProperties();
        }
    }
}