namespace TarasK8.SaveSystem
{
    public interface ISaveable
    {
        public void OnSave(SFile file);
        public void OnLoad(SFile file);
    }
}
