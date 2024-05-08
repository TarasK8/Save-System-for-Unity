using UnityEngine;

namespace TarasK8.SaveSystem
{
    [CreateAssetMenu(fileName ="New File Object", menuName = "Save System/File Object")]
    public class FileObject : ScriptableObject
    {
        [SerializeField] private string _json;

        public string GetJson()
        {
            return _json;
        }
    }
}