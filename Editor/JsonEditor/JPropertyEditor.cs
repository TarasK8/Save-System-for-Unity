using Newtonsoft.Json.Linq;
using UnityEditor;

namespace TarasK8.SaveSystemEditor.JEditor
{
    public class JPropertyEditor : JTokenEditor
    {
        protected JProperty _property;

        public JPropertyEditor(JProperty property, Editor editor) : base(property.Value, editor)
        {
            _property = property;
        }

        public override void Draw(string label, out JToken changedValue, bool addMode)
        {
            EditorGUILayout.BeginHorizontal();
            JLayout.RenameField(_property);
            DrawValueField(_property.Value, label, out changedValue);
            JLayout.DeleteButton(_property);
            EditorGUILayout.EndHorizontal();
            base.DrawObject(addMode);
            base.DrawArray(addMode);
        }
    }
}