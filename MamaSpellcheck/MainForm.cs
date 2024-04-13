using System.Text.RegularExpressions;
using NHunspell;

namespace MamaSpellcheck;

public partial class MainForm : Form
{

    public MainForm()
    {
        InitializeComponent();

        Text = "MamaSpellcheck";
        FormBorderStyle = FormBorderStyle.FixedSingle;
        StartPosition = FormStartPosition.CenterScreen;
        MaximizeBox = false;
        Icon = new Icon("MS.ico");
        Width = 900;

        _hunspell = new Hunspell("dict/bg_BG.aff", "dict/bg_BG.dic");

        _box = new RichTextBox
        {
            Top = 10,
            Left = 10,
            Height = 430,
            Width = 400,
            AutoWordSelection = false,
            Font = new Font(FontFamily.GenericSansSerif, 16)
        };
        Controls.Add(_box);

        _wrongWordsBox = new ListBox
        {
            Top = 30,
            Left = 510,
            Height = 385,
            Width = 150
        };
        Controls.Add(_wrongWordsBox);
        
        _suggestionsBox = new ListBox
        {
            Top = 30,
            Left = 665,
            Height = 385,
            Width = 205
        };
        Controls.Add(_suggestionsBox);
        
        LoadComponents2();

        _settings = new AppSettings();
        _settings.Load();

        Disposed += OnDisposed;
    }

    private readonly Hunspell _hunspell;
    private readonly RichTextBox _box;
    private readonly ListBox _wrongWordsBox;
    private readonly ListBox _suggestionsBox;

    private readonly AppSettings _settings;
    
    private void LoadComponents2()
    {
        string lastSelectedWrongWord = "";
        
        Label labelWrongWords = new()
        {
            Text = "Сгрешени:",
            Top = 10,
            Left = 510
        };
        Controls.Add(labelWrongWords);
        
        Label labelSuggestions = new()
        {
            Text = "Предложения:",
            Top = 10,
            Left = 665,
            Width = 205,
            AutoEllipsis = true
        };
        Controls.Add(labelSuggestions);

        _wrongWordsBox.SelectedValueChanged += (_, _) =>
        {
            if (_wrongWordsBox.SelectedItem is not string word)
            {
                labelSuggestions.Text = "Предложения:";
                HighlightAll(lastSelectedWrongWord, Color.Gold);
                lastSelectedWrongWord = "";
                return;
            }

            HighlightAll(lastSelectedWrongWord, Color.Gold);
            
            lastSelectedWrongWord = word;
            HighlightAll(word, Color.Fuchsia);
            _suggestionsBox.Items.Clear();
            labelSuggestions.Text = $"Предложения за {word}:";

            List<string> suggestions = _hunspell.Suggest(word);
            _suggestionsBox.Items.AddRange(suggestions.ToArray());
        };
        
        Button btnCheck = new()
        {
            Top = 10,
            Left = 415,
            Text = "Провери"
        };
        btnCheck.Click += (_, _) =>
        {
            _box.Text = _box.Text.Replace("\u00AD", "");
            _wrongWordsBox.Items.Clear();
            _suggestionsBox.Items.Clear();
            lastSelectedWrongWord = "";
            labelSuggestions.Text = "Предложения:";
            _box.SelectAll();
            _box.SelectionBackColor = Color.White;
            _box.SelectionColor = Color.Black;
            _box.DeselectAll();
            _box.SelectionStart = 0;
            Check();
        };
        Controls.Add(btnCheck);
        
        Button btnClear = new()
        {
            Top = 35,
            Left = 415,
            Text = "Изчисти"
        };
        btnClear.Click += (_, _) =>
        {
            _box.Clear();
            _box.Focus();
            _box.SelectionBackColor = Color.White;
            _wrongWordsBox.Items.Clear();
            _suggestionsBox.Items.Clear();
            lastSelectedWrongWord = "";
            labelSuggestions.Text = "Предложения:";
        };
        Controls.Add(btnClear);
        
        Button btnWhite = new()
        {
            Top = 100,
            Left = 415,
            Text = "Изч. марк.",
            BackColor = Color.White
        };
        btnWhite.Click += (_, _) =>
        {
            _box.SelectionBackColor = Color.White;
        };
        Controls.Add(btnWhite);
        
        Button btnYellow = new()
        {
            Top = 125,
            Left = 415,
            Text = "Маркирай",
            BackColor = Color.Gold
        };
        btnYellow.Click += (_, _) =>
        {
            _box.SelectionBackColor = Color.Gold;
        };
        Controls.Add(btnYellow);
        
        Button btnRed = new()
        {
            Top = 150,
            Left = 415,
            Text = "Маркирай",
            BackColor = Color.IndianRed
        };
        btnRed.Click += (_, _) =>
        {
            _box.SelectionBackColor = Color.IndianRed;
        };
        Controls.Add(btnRed);
        
        Button btnBlue = new()
        {
            Top = 175,
            Left = 415,
            Text = "Маркирай",
            BackColor = Color.CornflowerBlue
        };
        btnBlue.Click += (_, _) =>
        {
            _box.SelectionBackColor = Color.CornflowerBlue;
        };
        Controls.Add(btnBlue);
        
        Button btnGreen = new()
        {
            Top = 175,
            Left = 415,
            Text = "Маркирай",
            BackColor = Color.SeaGreen
        };
        btnGreen.Click += (_, _) =>
        {
            _box.SelectionBackColor = Color.SeaGreen;
        };
        Controls.Add(btnGreen);
        
        Button btnIgnore = new()
        {
            Top = 415,
            Left = 510,
            Width = 100,
            Text = "Игнорирай"
        };
        btnIgnore.Click += (_, _) =>
        {
            if (_wrongWordsBox.SelectedItem is not string word)
            {
                return;
            }
            
            _wrongWordsBox.Items.Remove(word);
            lastSelectedWrongWord = "";
            HighlightAll(word, Color.White);
            
            labelSuggestions.Text = "Предложения:";
            _suggestionsBox.Items.Clear();
        };
        Controls.Add(btnIgnore);
        
        Button btnAddDictionary = new()
        {
            Top = 415,
            Left = 615,
            Width = 120,
            Text = "Добави в речника"
        };
        btnAddDictionary.Click += (_, _) =>
        {
            if (_wrongWordsBox.SelectedItem is not string word)
            {
                return;
            }
            
            _settings.CustomDictionary.Add(word);
            _settings.Save();
            
            _wrongWordsBox.Items.Remove(word);
            lastSelectedWrongWord = "";
            HighlightAll(word, Color.White);
            
            labelSuggestions.Text = "Предложения:";
            _suggestionsBox.Items.Clear();
        };
        Controls.Add(btnAddDictionary);
        
        Button btnSettings = new()
        {
            Top = 415,
            Left = 780,
            Width = 90,
            Text = "Настройки"
        };
        btnSettings.Click += (_, _) =>
        {
            _settings.EnsureCorrectness();
            new SettingsForm(_settings).ShowDialog();
            _settings.EnsureCorrectness();
        };
        Controls.Add(btnSettings);
    }

    private void Check()
    {
        _wrongWordsBox.Items.Clear();
        
        _settings.EnsureCorrectness();
        
        string[] words = _box.Text.Split(_settings.Separators, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        List<string> completed = new(words.Length);
        
        // Run spell check
        foreach (string wordNonLowercase in words)
        {
            if (wordNonLowercase == "")
            {
                continue;
            }

            string word = wordNonLowercase.ToLower();

            if (completed.Contains(word))
            {
                continue;
            }
            
            completed.Add(word);
            
            // Custom dictionary
            if (_settings.CustomDictionary.Contains(word))
            {
                continue;
            }
            
            // Spellcheck
            bool isCorrect = _hunspell.Spell(word) || _hunspell.Spell(wordNonLowercase);
            if (isCorrect)
            {
                continue;
            }
            
            // Word is wrong
            _wrongWordsBox.Items.Add(wordNonLowercase);
            
            // Highlight
            HighlightAll(word, Color.Gold);

            _box.SelectionLength = 0;
            _box.SelectionBackColor = Color.White;
        }

        MessageBox.Show("Проверката приключи успешно.", "MamaSpellcheck", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void HighlightAll(string word, Color color)
    {
        Regex reg = new(@"\b" + word + @"(\b|s\b)",RegexOptions.IgnoreCase);

        foreach (Match match in reg.Matches(_box.Text))
        {
            _box.Select(match.Index, match.Length);
            _box.SelectionBackColor = color;
        }
    }

    private void OnDisposed(object? sender, EventArgs e)
    {
        _hunspell.Dispose();
    }
}