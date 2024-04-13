namespace MamaSpellcheck;

public sealed class AppSettings
{
    private const string PathBaseDir = "settings";
    private const string PathDictionary = $"{PathBaseDir}/dict.txt";
    private const string PathSeparators = $"{PathBaseDir}/separators.txt";
    
    public AppSettings()
    {
        CustomDictionary = new List<string>()
        {
            
        };
        Separators = new[]
        {
            " ", ".", "!", "?", ":", ";", ",", "@", "#", "№", "$", "%", "^", "&", "*", "(", ")", "-", "+", "–", "„", "“",
            "\"", "”", "/", "\\", "…"
        };
    }

    public List<string> CustomDictionary { get; set; }
    public string[] Separators { get; set; }

    public void Load()
    {
        if (File.Exists(PathDictionary))
        {
            string[] lines = File.ReadAllLines(PathDictionary);
            CustomDictionary = CustomDictionary.Concat(lines).Distinct().ToList();
        }

        if (File.Exists(PathSeparators))
        {
            string[] lines = File.ReadAllLines(PathSeparators);
            Separators = Separators.Concat(lines).Distinct().ToArray();
        }
        
        EnsureCorrectness();
    }

    public void Save()
    {
        CustomDictionary.Sort();
        EnsureCorrectness();
        
        Directory.CreateDirectory(PathBaseDir);
        File.WriteAllLines(PathDictionary, CustomDictionary);
        File.WriteAllLines(PathSeparators, Separators);
    }

    public void EnsureCorrectness()
    {
        if (!Separators.Contains(" "))
        {
            Separators = Separators.Prepend(" ").ToArray();
        }

        CustomDictionary = CustomDictionary.Distinct().ToList();
        Separators = Separators.Distinct().ToArray();
    }
}