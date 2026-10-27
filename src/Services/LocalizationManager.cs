using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace PokemonMemoryBattle;

public static class LocalizationManager
{
    public const string DefaultLanguage = "vi-VN";
    public static readonly string[] SupportedLanguages = [DefaultLanguage, "en-US"];
    private static readonly Dictionary<string, Dictionary<string, string>> resources = LoadResources();
    private static string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PokemonMemoryBattle", "settings.json");
    public static string CurrentLanguage { get; private set; } = DefaultLanguage;
    public static CultureInfo Culture => CultureInfo.GetCultureInfo(CurrentLanguage);
    public static bool SettingsLoadFailed { get; private set; }
    public static event EventHandler? LanguageChanged;
    internal static int SubscriberCount => LanguageChanged?.GetInvocationList().Length ?? 0;
    internal static IReadOnlyDictionary<string, string> StringsFor(string culture) => resources[culture];

    private static Dictionary<string, Dictionary<string, string>> LoadResources()
    {
        var result = new Dictionary<string, Dictionary<string, string>>();
        foreach (string culture in SupportedLanguages)
        {
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                $"PokemonMemoryBattle.Resources.Strings.{culture}.json")
                ?? throw new InvalidOperationException($"Missing embedded language resource: {culture}");
            result[culture] = JsonSerializer.Deserialize<Dictionary<string, string>>(stream)!;
        }
        return result;
    }

    public static void Initialize(string? preferencePath = null)
    {
        if (preferencePath != null)
            settingsPath = preferencePath;
        SetLanguage(LoadLanguagePreference());
    }

    public static string GetString(string key, params object[] arguments)
    {
        // Missing translation is a development error, caught by key parity tests.
        // Native multiline Edit controls require CRLF on Windows.
        string value = resources[CurrentLanguage][key].ReplaceLineEndings(Environment.NewLine);
        return arguments.Length == 0 ? value : string.Format(Culture, value, arguments);
    }

    public static void SetLanguage(string cultureCode)
    {
        string supported = SupportedLanguages.Contains(cultureCode) ? cultureCode : DefaultLanguage;
        bool changed = CurrentLanguage != supported;
        CurrentLanguage = supported;
        CultureInfo.CurrentUICulture = Culture;
        CultureInfo.CurrentCulture = Culture;
        if (changed)
            LanguageChanged?.Invoke(null, EventArgs.Empty);
    }

    public static string LoadLanguagePreference()
    {
        SettingsLoadFailed = false;
        try
        {
            if (!File.Exists(settingsPath))
                return DefaultLanguage;
            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(settingsPath));
            if (document.RootElement.ValueKind == JsonValueKind.Object &&
                document.RootElement.TryGetProperty("language", out JsonElement language) &&
                language.ValueKind == JsonValueKind.String && SupportedLanguages.Contains(language.GetString()))
                return language.GetString()!;
            SettingsLoadFailed = true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            SettingsLoadFailed = true;
        }
        return DefaultLanguage;
    }

    public static bool SaveLanguagePreference()
    {
        string temporary = settingsPath + ".tmp";
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(settingsPath)!);
            File.WriteAllText(temporary, JsonSerializer.Serialize(new { language = CurrentLanguage }));
            File.Move(temporary, settingsPath, true);
            SettingsLoadFailed = false;
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }
}
