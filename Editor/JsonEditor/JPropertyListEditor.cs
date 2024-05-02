using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TarasK8.SaveSystemEditor.JEditor
{
    public class JPropertyListEditor
    {
        private JToken _token;
        private AddData _addData;
        private Dictionary<JProperty, JPropertyEditor> _childProperties;
        private Editor _editor;

        public JPropertyListEditor(JToken token, Editor editor)
        {
            _token = token;
            _addData = new AddData();
            _childProperties = new Dictionary<JProperty, JPropertyEditor>();
            _editor = editor;
        }

        public void Draw(bool addMode)
        {
            List<JProperty> list = _token.OfType<JProperty>().ToList();

            foreach (var item in list)
            {
                EditorGUILayout.BeginVertical("box");
                GetPropertyEditor(item).Draw(item.Value.Type.ToString(), out var changedValue, addMode);
                if (changedValue != null)
                {
                    item.Value.Replace(changedValue);
                }
                EditorGUILayout.EndVertical();
            }

            if (list.Count == 0)
                JLayout.Empty();

            if (addMode)
                DrawAddButton();
        }

        public void ShowAll()
        {
            foreach (var item in _childProperties.Values)
            {
                item.Show();
            }
        }

        public void HideAll()
        {
            foreach (var item in _childProperties.Values)
            {
                item.Hide();
            }
        }

        private void DrawAddButton()
        {
            EditorGUILayout.BeginHorizontal("button");

            _addData.Key = EditorGUILayout.TextField(_addData.Key, JLayout.WITH_80);

            _addData.Value = JLayout.PropertyForAdd(_addData.Value, _addData.Type);

            _addData.Type = (SupportType)EditorGUILayout.EnumPopup(_addData.Type, JLayout.WITH_80);
            if (GUILayout.Button("Add", GUILayout.MinWidth(50f)))
            {
                JToken forReplace = _token[_addData.Key];
                if (forReplace != null)
                {
                    _childProperties.Remove((JProperty)(forReplace.Parent));
                }
                _token[_addData.Key] = JToken.FromObject(_addData.Value);
            }

            EditorGUILayout.EndHorizontal();
        }

        private JPropertyEditor GetPropertyEditor(JProperty property)
        {
            if (_childProperties.TryGetValue(property, out var propertyEditor))
            {
                return propertyEditor;
            }
            else
            {
                var newProperty = new JPropertyEditor(property, _editor);
                _childProperties.Add(property, newProperty);
                return newProperty;
            }
        }

        public class AddData
        {
            public string Key = "New key";
            public object Value = 0;
            public SupportType Type = SupportType.Integer;
        }
    }
}