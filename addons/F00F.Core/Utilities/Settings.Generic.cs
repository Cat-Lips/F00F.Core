namespace F00F
{
    public class Settings<T>(bool autoSave = true) : Settings(typeof(T).Name, autoSave) { }
}
