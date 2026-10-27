namespace PokemonMemoryBattle;

internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        if (args.Length == 3 && args[0] == "--language-probe")
        {
            LocalizationManager.Initialize(args[1]);
            File.WriteAllText(args[2], LocalizationManager.CurrentLanguage);
            return;
        }
        if (args.Contains("--self-test"))
        {
            SelfTests.Run();
            return;
        }
        LocalizationManager.Initialize();
        Application.Run(new MainMenuForm());
    }
}
