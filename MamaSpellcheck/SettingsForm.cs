namespace MamaSpellcheck;

public partial class SettingsForm : Form
{
    public SettingsForm(AppSettings settings)
    {
        InitializeComponent();
        
        Text = "Настройки";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterScreen;
        MaximizeBox = false;
        MinimizeBox = false;
        Width = 400;
        Height = 400;

        TabPage dictPage = new("Речник");
        RichTextBox dictBox = new();
        dictBox.Lines = settings.CustomDictionary.ToArray();
        dictBox.Dock = DockStyle.Fill;
        dictPage.Controls.Add(dictBox);
        
        TabPage sepPage = new("Пунктуация");
        RichTextBox sepBox = new();
        sepBox.Lines = settings.Separators.Where(x => x.Trim() != "").ToArray();
        sepBox.Dock = DockStyle.Fill;
        sepPage.Controls.Add(sepBox);
        
        TabControl tabs = new()
        {
            Top = 10,
            Left = 10,
            Width = 365,
            Height = 305,
            TabPages = { dictPage, sepPage }
        };
        Controls.Add(tabs);
        
        Button btnSave = new()
        {
            Top = 325,
            Left = 300,
            Width = 70,
            Text = "Запази"
        };
        btnSave.Click += (_, _) =>
        {
            settings.CustomDictionary = dictBox.Lines.Where(x => x.Trim() != "").ToList();
            settings.Separators = sepBox.Lines.Prepend(" ").ToArray();
            settings.Save();
            Close();
        };
        Controls.Add(btnSave);
        
        Button btnCancel = new()
        {
            Top = 325,
            Left = 225,
            Width = 70,
            Text = "Отмени"
        };
        btnCancel.Click += (_, _) =>
        {
            Close();
        };
        Controls.Add(btnCancel);
    }
}