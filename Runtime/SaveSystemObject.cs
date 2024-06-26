using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace TarasK8.SaveSystem
{
    public class SaveSystemObject : MonoBehaviour
    {
        // [Header("Path")]
        [SerializeField] private string _fileName = "Saves/save.json";
        [SerializeField] private PathType _path = PathType.PersistentDataPath;
        [SerializeField] private string _customPath;

        // [Header("Options")]
        [SerializeField] private bool _initializeOnAwake = true;
        [SerializeField] private bool _collectSaveablesEverySave = false;
        [SerializeField] private bool _useEncryption = false;
        [SerializeField] private string _encryptionPassword = "password";
        [SerializeField] private bool _autoSaving = false;
        [SerializeField] private float _savePeriod = 30f;
        [SerializeField] private bool _enableDefaultFile = false;
        [SerializeField] private DefaultFileType _defaultFileType;
        [SerializeField] private Object _defaultFileObject = null;
        [SerializeField] private FileObject _defaultFile = null;

        // [Header("Events")]
        [SerializeField] public UnityEvent<SFile> OnSave;
        [SerializeField] public UnityEvent<SFile> OnLoad;

        public SFile File { get; private set; }

        private IEnumerable<ISaveable> _seveables;
        private Coroutine _autosavingRoutine;

        private void Awake()
        {
            if(_initializeOnAwake) Initialize();
        }

        public void Initialize()
        {
            File = new SFile();
            _seveables = GetObjectsForSave();
            Load();
            if (_autoSaving)
            {
                StartAutosaving(_savePeriod);
            }
        }

        [ContextMenu("Save")]
        public void Save()
        {
            if(_collectSaveablesEverySave) _seveables = GetObjectsForSave();
            if (_seveables != null)
            {
                foreach (var saveObject in _seveables)
                {
                    saveObject.OnSave(File);
                }
            }
            OnSave?.Invoke(File);
            File.Save(GetPath(), GetPassword());
        }

        public void Load()
        {
            if (_collectSaveablesEverySave) _seveables = GetObjectsForSave();

            File.Load(GetPath(), GetPassword());

            if (_enableDefaultFile)
            {
                File.MergeFromJson(GetDefaultDataJson());
            }

            foreach (var saveObject in _seveables)
            {
                saveObject.OnLoad(File);
            }

            OnLoad?.Invoke(File);
        }

        public void StartAutosaving(float period)
        {
            _autoSaving = true;
            _savePeriod = period;
            if (period < 1f) return;
            _autosavingRoutine = StartCoroutine(AutosavingRoutine(period));
        }

        public void StopAutosaving()
        {
            _autoSaving = false;
            StopCoroutine(_autosavingRoutine);
        }

        public string GetPath()
        {
            string root = _customPath;
            switch (_path)
            {
                case PathType.PersistentDataPath:
                    root = Application.persistentDataPath;
                    break;
                case PathType.DataPath:
                    root = Application.dataPath;
                    break;
                case PathType.CustomPath:
                    root = _customPath;
                    break;
            }
            return Path.Combine(root, _fileName);
        }

        private IEnumerable<ISaveable> GetObjectsForSave()
        {
            return FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();
        }

        private IEnumerator AutosavingRoutine(float period)
        {
            var wait = new WaitForSecondsRealtime(period);
            while (_autoSaving)
            {
                Save();
                yield return wait;
            }
        }

        public string GetPassword()
        {
            return _useEncryption ? _encryptionPassword : string.Empty;
        }

        private string GetDefaultDataJson()
        {
            if(_defaultFileType == DefaultFileType.File)
            {
                return _defaultFile.GetJson();
            }
            else
            {
                return JsonConvert.SerializeObject(_defaultFileObject);
            }
        }

        public enum PathType
        {
            PersistentDataPath = 1,
            DataPath = 2,
            CustomPath = 3
        }

        public enum DefaultFileType
        {
            File = 0,
            Object = 1,
        }
    }
}
