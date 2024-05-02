using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TarasK8.SaveSystemEditor.JEditor
{
    public class JArrayEditor
    {
        private JArray _array;
        private Dictionary<JToken, JTokenEditor> _childTokens;
        private Editor _editor;
        private AddData _addData;
        private int _displayElements = 15;
        private int _page = 0;

        public JArrayEditor(JArray array, Editor editor)
        {
            _array = array;
            _addData = new AddData();
            _childTokens = new Dictionary<JToken, JTokenEditor>();
            _editor = editor;
        }

        public void Draw(bool addMode)
        {
            if (_array.Count == 0)
            {
                JLayout.Empty();
                if (addMode)
                    DrawAddButton();
            }
            else
            {
                var arrList = _array.ToList();
                int displayClamped = Mathf.Max(_displayElements, 1);
                int maxPage = Mathf.Max(arrList.Count / displayClamped, 0);
                if (arrList.Count % displayClamped == 0) maxPage--;
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField($"{arrList[0].Type}'s List", JLayout.WITH_80, JLayout.MAX_WITH_2000);
                if (arrList.Count > 15)
                {
                    EditorGUILayout.LabelField($"{_page + 1}/{maxPage + 1}", JLayout.WITH_50);
                    if (GUILayout.Button("F", GUILayout.Width(25f))) _page = 0;
                    if (GUILayout.Button("L", GUILayout.Width(25f))) _page = maxPage;
                    if (GUILayout.Button("Prev", JLayout.WITH_50)) _page--;
                    if (GUILayout.Button("Next", JLayout.WITH_50)) _page++;
                    _displayElements = EditorGUILayout.IntField(Mathf.Clamp(_displayElements, 1, 100), GUILayout.Width(30f));
                }
                EditorGUILayout.EndHorizontal();

                _page = Mathf.Clamp(_page, 0, maxPage);
                int offset = _displayElements * _page;
                int endIndex = offset + Mathf.Min(arrList.Count - offset, _displayElements);
                for (int i = offset; i < endIndex; i++)
                {
                    GetTokenEditor(arrList[i]).Draw($"Element {i}", out var changedValue, addMode);
                    if (changedValue != null)
                    {
                        arrList[i].Replace(changedValue);
                    }
                }
                if (addMode)
                    DrawAddElementButton();
            }
        }

        public void ShowAll()
        {
            foreach (var item in _childTokens.Values)
            {
                item.Show();
            }
        }

        public void HideAll()
        {
            foreach (var item in _childTokens.Values)
            {
                item.Hide();
            }
        }

        private void DrawAddElementButton()
        {
            EditorGUILayout.BeginHorizontal("box");

            JToken lastElem = _array.Last;
            if (lastElem.Type == JTokenType.Array || lastElem.Type == JTokenType.Object)
            {
                if (GUILayout.Button("Add (Clone Last)"))
                {
                    _array.Add(lastElem.DeepClone());
                }
            }
            else
            {
                _addData.Value = JLayout.PropertyByType(_addData.Value, lastElem.Type);
                if (GUILayout.Button("Add"))
                {
                    _array.Add(JToken.FromObject(_addData.Value));
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawAddButton()
        {
            EditorGUILayout.BeginHorizontal("button");

            /*
            switch (_addData.Type)
            {
                case SupportType.Integer:
                    try { _addData.Value = EditorGUILayout.IntField(Convert.ToInt32(_addData.Value)); }
                    catch { _addData.Value = 0; }
                    break;
                case SupportType.Float:
                    try { _addData.Value = EditorGUILayout.FloatField(Convert.ToSingle(_addData.Value)); }
                    catch { _addData.Value = 0f; }
                    break;
                case SupportType.String:
                    try { _addData.Value = EditorGUILayout.TextField(Convert.ToString(_addData.Value)); }
                    catch { _addData.Value = string.Empty; }
                    break;
                case SupportType.Boolean:
                    try { _addData.Value = EditorGUILayout.Toggle(Convert.ToBoolean(_addData.Value)); }
                    catch { _addData.Value = false; }
                    break;
                case SupportType.Object:
                    EditorGUILayout.TextField("Structure object");
                    //data.Value = EditorGUILayout.ObjectField(((Object)data.Value));
                    break;
                case SupportType.Array:
                    _addData.Value = new int[0];
                    EditorGUILayout.TextField("Structure object");
                    break;
                default:
                    JLayout.Unsupported(_addData.Type.ToString());
                    break;
            }
            */

            /*
            UnityAction ifObjectAction = () =>
            {
                _addData.Value = new JObject();
                EditorGUILayout.TextField("Structure object");
            };
            UnityAction ifArrayAction = () =>
            {
                _addData.Value = new int[0];
                EditorGUILayout.TextField("Structure object");
            };
            _addData.Value = JLayout.PropertyByType(_addData.Value, JLayout.EnumConvert(_addData.Type), ifObject: ifObjectAction, ifArray: ifArrayAction) ?? _addData.Value;
            */

            _addData.Value = JLayout.PropertyForAdd(_addData.Value, _addData.Type);


            _addData.Type = (SupportType)EditorGUILayout.EnumPopup(_addData.Type, JLayout.WITH_80);
            if (GUILayout.Button("Add", GUILayout.MinWidth(50f)))
            {
                var newObj = JToken.FromObject(_addData.Value);
                _array.Add(newObj);
            }

            EditorGUILayout.EndHorizontal();
        }

        private JTokenEditor GetTokenEditor(JToken token)
        {
            if (_childTokens.TryGetValue(token, out var propertyEditor))
            {
                return propertyEditor;
            }
            else
            {
                var newProperty = new JTokenEditor(token, _editor);
                _childTokens.Add(token, newProperty);
                return newProperty;
            }
        }

        public class AddData
        {
            public object Value = 0;
            public SupportType Type = SupportType.Integer;
        }

    }
}