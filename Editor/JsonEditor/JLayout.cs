using Newtonsoft.Json.Linq;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace TarasK8.SaveSystemEditor.JEditor
{
    public static class JLayout
    {
        public static readonly GUILayoutOption WITH_50 = GUILayout.Width(50f);
        public static readonly GUILayoutOption WITH_80 = GUILayout.Width(80f);
        public static readonly GUILayoutOption WITH_120 = GUILayout.Width(120f);
        public static readonly GUILayoutOption MAX_WITH_2000 = GUILayout.MaxWidth(2000f);

        public static void DeleteButton(JToken token)
        {
            if (GUILayout.Button("Delete", WITH_50))
            {
                token.Remove();
            }
        }

        public static void RenameField(JProperty property)
        {
            string newName = EditorGUILayout.TextField(property.Name, WITH_80);
            if (newName != property.Name)
            {
                var newProp = new JProperty(newName, property.Value);
                property.Replace(newProp);
                //Debug.Log($"Name Changed\nOld Token:\n{token}\nNew Token:\n{newProp}");
            }
        }

        public static void Empty()
        {
            EditorGUILayout.BeginVertical("box");
            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            EditorGUILayout.LabelField("Empty", style, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndVertical();
        }

        public static void Unsupported(string typeName)
        {
            EditorGUILayout.LabelField($"{typeName} type is not supported.", WITH_80, MAX_WITH_2000);
        }

        public static object PropertyByType(object obj, JTokenType type, string label = "", UnityAction ifObject = null, UnityAction ifArray = null)
        {
            switch (type)
            {
                case JTokenType.Integer:
                    try { return EditorGUILayout.IntField(label, Convert.ToInt32(obj), WITH_80, MAX_WITH_2000); }
                    catch { return 0; }
                case JTokenType.Float:
                    try { return EditorGUILayout.FloatField(label, Convert.ToSingle(obj), WITH_80, MAX_WITH_2000); }
                    catch { return 0f; }
                case JTokenType.String:
                    try { return EditorGUILayout.TextField(label, Convert.ToString(obj), WITH_80, MAX_WITH_2000); }
                    catch { return string.Empty; }
                case JTokenType.Boolean:
                    try { return EditorGUILayout.Toggle(label, Convert.ToBoolean(obj), WITH_80, MAX_WITH_2000); }
                    catch { return false; }
                case JTokenType.Object:
                    ifObject?.Invoke();
                    return null;
                case JTokenType.Array:
                    ifArray?.Invoke();
                    return null;
                default:
                    Unsupported(type.ToString());
                    return null;
            }
        }

        public static object PropertyForAdd(object obj, SupportType type)
        {
            UnityAction ifObjectAction = () =>
            {
                obj = new JObject();
                EditorGUILayout.TextField("Structure object", WITH_80, MAX_WITH_2000);
            };
            UnityAction ifArrayAction = () =>
            {
                obj = new int[0];
                EditorGUILayout.TextField("Structure object", WITH_80, MAX_WITH_2000);
            };
            return PropertyByType(obj, EnumConvert(type), ifObject: ifObjectAction, ifArray: ifArrayAction) ?? obj;
        }

        public static JTokenType EnumConvert(SupportType type)
        {
            switch (type)
            {
                case SupportType.Object: return JTokenType.Object;
                case SupportType.Integer: return JTokenType.Integer;
                case SupportType.Float: return JTokenType.Float;
                case SupportType.String: return JTokenType.String;
                case SupportType.Boolean: return JTokenType.Boolean;
                case SupportType.Array: return JTokenType.Array;
                default: return JTokenType.None;
            }
        }
    }

    public enum SupportType
    {
        Object,
        Integer,
        Float,
        String,
        Boolean,
        Array,
    }
}