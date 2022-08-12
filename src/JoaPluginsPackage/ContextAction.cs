namespace JoaPluginsPackage;

public class ContextAction
{
    public ContextAction(string key, string name, Key keyBoardKey, string? link = null)
    {
        Key = key;
        Name = name;
        KeyBoardKey = keyBoardKey;
        Link = link;
    }

    public string Key { get; set; }
    public string Name { get; set; }
    public Key KeyBoardKey { get; set; }
    public string? Link { get; set; }
}