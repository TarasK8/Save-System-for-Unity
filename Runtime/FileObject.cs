using UnityEngine;

namespace TarasK8.SaveSystem
{
    [CreateAssetMenu(fileName ="New File", menuName = "Save System/File")]
    public class FileObject : ScriptableObject
    {
        [SerializeField] private string _json;

        public string GetJson()
        {
            return _json;
        }
    }
}