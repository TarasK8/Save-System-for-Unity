using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace TarasK8.SaveSystemEditor.JEditor
{
    public class JsonEditor
    {
        private JToken _token;
        private Editor _editor;
        public static bool AddMode { get; private set; } = false;

        private JPropertyListEditor _propertiesList;

        public JsonEditor(JToken token, Editor editor)
        {
            _token = token;
            _editor = editor;
            _propertiesList = new JPropertyListEditor(token, _editor);
        }

        public void DrawEditor()
        {
            EditorGUILayout.BeginHorizontal();
            AddMode = EditorGUILayout.ToggleLeft("Add Mode", AddMode, JLayout.WITH_80, JLayout.MAX_WITH_2000);
            if (GUILayout.Button("Show All", JLayout.WITH_80)) ShowAll();
            if (GUILayout.Button("Hide All", JLayout.WITH_80)) HideAll();
            EditorGUILayout.EndHorizontal();

            _propertiesList.Draw(AddMode);
        }

        public string GetJson()
        {
            return _token.ToString();
        }

        public void ShowAll()
        {
            _propertiesList?.ShowAll();
        }

        public void HideAll()
        {
            _propertiesList?.HideAll();
        }
    }
}