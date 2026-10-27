namespace PokemonMemoryBattle;

internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        if (args.Contains("--self-test"))
        {
            SelfTests.Run();
            return;
        }
        Application.Run(new MainMenuForm());
    }
}
