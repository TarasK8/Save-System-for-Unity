using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace TarasK8.SaveSystemEditor.JEditor
{
    public class JTokenEditor
    {
        protected JToken _token;
        private AnimBool _animBool;
        private Editor _editor;
        private JPropertyListEditor _childObjectEditor;
        private JArrayEditor _childArrayEditor;

        public JTokenEditor(JToken token, Editor editor)
        {
            _token = token;
            _animBool = new AnimBool(false);
            _editor = editor;
            _animBool.valueChanged.AddListener(editor.Repaint);
        }

        public virtual void Draw(string label, out JToken changedValue, bool addMode)
        {
            EditorGUILayout.BeginHorizontal();
            DrawValueField(_token, label, out changedValue);
            JLayout.DeleteButton(_token);
            EditorGUILayout.EndHorizontal();
            DrawObject(addMode);
            DrawArray(addMode);
        }

        public void Show()
        {
            _animBool.target = true;
            _childArrayEditor?.ShowAll();
            _childObjectEditor?.ShowAll();
        }

        public void Hide()
        {
            _animBool.target = false;
            _childArrayEditor?.HideAll();
            _childObjectEditor?.HideAll();
        }

        protected void DrawValueField(JToken token, string label, out JToken changedValue)
        {
            changedValue = null;
            JTokenType type = token.Type;
            object newValue = null;

            UnityAction action = () =>
            {
                DrawFadeToggle();
                EditorGUILayout.LabelField($"{token.Count()} elems", JLayout.WITH_80);
            };
            var temp = JLayout.PropertyByType(token, type, label, action, action);
            if (temp != null) newValue = temp;

            if (newValue != null)
            {
                bool canReplace = true;

                if (type == JTokenType.Float && Mathf.Approximately(token.ToObject<float>(), (float)newValue)
                || JToken.DeepEquals(token, JToken.FromObject(newValue)))
                {
                    canReplace = false;
                }
                if (canReplace)
                {
                    var newToken = JToken.FromObject(newValue);
                    changedValue = newToken;
                }
            }
        }

        protected void DrawObject(bool addMode)
        {
            if (_token.Type == JTokenType.Object)
            {
                if (EditorGUILayout.BeginFadeGroup(_animBool.faded))
                {
                    if (_childObjectEditor == null) _childObjectEditor = new JPropertyListEditor(_token, _editor);
                    _childObjectEditor.Draw(addMode);
                }
                EditorGUILayout.EndFadeGroup();
            }
        }

        protected void DrawArray(bool addMode)
        {
            if (_token.Type == JTokenType.Array)
            {
                if (EditorGUILayout.BeginFadeGroup(_animBool.faded))
                {
                    if (_childArrayEditor == null) _childArrayEditor = new JArrayEditor((JArray)_token, _editor);
                    _childArrayEditor.Draw(addMode);
                }
                EditorGUILayout.EndFadeGroup();
            }
        }

        private void DrawFadeToggle()
        {
            _animBool.target = EditorGUILayout.ToggleLeft("Show/Hide", _animBool.target, JLayout.WITH_80, JLayout.MAX_WITH_2000);
        }
    }
}