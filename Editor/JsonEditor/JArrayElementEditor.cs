using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TarasK8.SaveSystemEditor.JEditor;
using UnityEditor;
using UnityEngine;

public class JArrayElementEditor : JTokenEditor
{
    private Dictionary<JToken, JArrayElementEditor> _childTokens;

    public JArrayElementEditor(JToken token, Editor editor, Dictionary<JToken, JArrayElementEditor> childTokens) : base(token, editor)
    {
        _childTokens = childTokens;
    }

    public override void Draw(string label, out JToken changedValue, bool addMode)
    {
        EditorGUILayout.BeginHorizontal();
        base.DrawValueField(_token, label, out changedValue);
        DrawDeleteButton();
        EditorGUILayout.EndHorizontal();
        base.DrawObject(addMode);
        base.DrawArray(addMode);
        //base.Draw(label, out changedValue, addMode);
    }

    public void DrawDeleteButton()
    {
        if (GUILayout.Button("Delete", JLayout.WITH_50))
        {
            if (_childTokens.Remove(_token))
            {
                base._token.Remove();
            }
            else
            {
                Debug.LogError("Не видалено");
            }
        }
    }
}