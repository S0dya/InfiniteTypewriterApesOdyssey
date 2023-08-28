using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    public Dictionary<string, long> longDictionary;
    public Dictionary<string, int> intDictionary;
    public Dictionary<string, bool> boolDictionary;
    public Dictionary<string, string> stringDictionary;
}
