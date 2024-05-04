# Save System
 Easy save system for Unity, based on [Newtonsoft Json Package](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@3.2/manual/index.html) and [Newtonsoft.Json-for-Unity.Converters](https://github.com/applejag/Newtonsoft.Json-for-Unity.Converters) for serialization.
 ## Fetures
 - Сan save almost any type (List, Class, Struct, most Unity types, etc)
 - Can save asset references
 - Сan add default data (if the file does not contain a value for the key, the data is taken from the default data)
 - Simple data encryption
 - Easy to use
 - Json Constructor in Unity Editor (edit the save file directly in the editor)
 - Flexible (use an existing component, or create your own save system based on the SFile class)

## Warning!!!
The package is currently in an early stage of development, so some methods and concepts may change over time. Use at your own risk

## Installation
1. Open the `manifest.json` in your project by path `[Project Folder]\Packages\manifest.json` and add Scoped Registry:
   ```json
   "scopedRegistries": [
    {
      "name": "Packages from jillejr",
      "url": "https://npm.cloudsmith.io/jillejr/newtonsoft-json-for-unity/",
      "scopes": [
        "jillejr"
      ]
    }
   ],
   ```
   > Add this before the dependencies block.
   
   > This item is required to install a dependency that [fixes an error](https://github.com/applejag/Newtonsoft.Json-for-Unity.Converters?tab=readme-ov-file#what-does-it-solve) when serializing Unity objects such as Vector3, Quaternion, Color, and [many more](https://github.com/applejag/Newtonsoft.Json-for-Unity.Converters/blob/master/Doc/Compatability-table.md).
2. Open the Package Manager and "Add package from git URL...". Paste `https://github.com/TarasK8/Save-System-for-Unity.git` and Add.

## How to use
### If you need to save `MonoBehavior` objects
1. Create Empty GameObject and add `SaveSystemObject` component.
2. Extend the MonoBehaviour class for saving with the `ISaveable` interface and implement it
   ```csharp
   public void OnSave(SFile file) { }

   public void OnLoad(SFile file) { }
   ```
   > If necessary, you can implement only one of these methods
3. In the implemented methods, write the logic for saving and loading, for example:
   ```csharp
   private int _valueForSave;
   
   public void OnSave(SFile file)
   {
       file.Write("Key", _valueForSave);
   }

   public void OnLoad(SFile file)
   {
       _valueForSave = file.Read<int>("Key");
   }
   ```
4. After that, depending on the `SaveSystemObject` object settings, it will save all `ISaveable` objects. Or you can save the data manually, using the `SceneSave.Save()` method.
### Custom save system
- You can also make a custom save system by directly using the SFile class, for example:
  ```csharp
  string path = System.IO.Path.Combine(Application.persistentDataPath, "Saves/save.json");
  SFile file = new SFile(path);
  int valueToSave = 1234;
  file.Write("Key", valueToSave);
  file.Save();
  ```
