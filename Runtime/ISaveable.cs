namespace TarasK8.SaveSystem
{
    public interface ISaveable
    {
        public void OnSave(File file) { }
        public void OnLoad(File file) { }
    }
}
