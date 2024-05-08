using Newtonsoft.Json.Linq;
using TarasK8.SaveSystemEditor.JEditor;
using UnityEditor;
using UnityEngine;

public class JArrayElementEditor : JTokenEditor
{
    public JArrayElementEditor(JToken token, Editor editor) : base(token, editor)
    {
        
    }

    public override void Draw(string label, out JToken changedValue, bool addMode)
    {
        base.Draw(label, out changedValue, addMode);
    }
}