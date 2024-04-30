using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace TarasK8.SaveSystem
{
    public class SFile
    {
        private Dictionary<string, object> _data;

        public SFile()
        {
            _data = new Dictionary<string, object>();
        }

        public void Write(string key, object value)
        {
            if (_data.ContainsKey(key))
            {
                _data[key] = value;
            }
            else
            {
                _data.Add(key, value);
            }
        }

        public T Read<T>(string key)
        {
            if (_data.TryGetValue(key, out var data))
            {
                return ToObject<T>(data);
            }
            else
            {
                Debug.LogWarning(@$"No value found for '{key}'.");
                return default;
            }
        }

        public T Read<T>(string key, T defaultValue)
        {
            if (_data.TryGetValue(key, out var data))
            {
                return ToObject<T>(data);
            }
            else
            {
                return defaultValue;
            }
        }

        public bool TryRead<T>(string key, out T value)
        {
            if(_data.TryGetValue(key, out var data))
            {
                value = ToObject<T>(data);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public bool HasKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public bool DeleteKey(string key)
        {
            return _data.Remove(key);
        }

        public void Save(string path, string encryptionPassword = null)
        {
            string json = GetJson();
            if (string.IsNullOrEmpty(encryptionPassword) == false)
            {
                json = EncryptDecrypt(json, encryptionPassword);
            }
            string directoryPath = Path.GetDirectoryName(path);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            File.WriteAllText(path, json);
        }

        public void Load(string path, string encryptionPassword = null)
        {
            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(encryptionPassword) == false)
            {
                json = EncryptDecrypt(json, encryptionPassword);
            }
            LoadFromJson(json);
        }

        public void LoadFromJson(string json)
        {
            bool success = true;
            var settings = new JsonSerializerSettings
            {
                Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                {
                    Debug.LogError("DeserializationError: Most likely, you have disabled or enabled encryption, so the file cannot be downloaded. This file was created as a new.");
                    success = false;
                    _data = new Dictionary<string, object>();
                    args.ErrorContext.Handled = true;
                }
            };
            Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, settings);

            if(success)
                _data = data;
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(_data, Formatting.Indented);
        }

        public Dictionary<string, object> GetAllRawData()
        {
            return _data;
        }

        private string EncryptDecrypt(string data, string password)
        {
            StringBuilder modifiedData = new StringBuilder(data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                modifiedData.Append((char)(data[i] ^ password[i % password.Length]));
            }
            return modifiedData.ToString();
        }

        private T ToObject<T>(object data)
        {
            var obj = JToken.FromObject(data);
            return obj.ToObject<T>();
        }

    }
}
