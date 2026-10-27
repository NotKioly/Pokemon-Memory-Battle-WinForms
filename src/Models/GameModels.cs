namespace PokemonMemoryBattle;

public enum GameState
{
    Ready, Playing, Paused, Won, Lost
}
public record Difficulty(string Name, int Rows, int Columns, int Seconds)
{
    public static readonly Difficulty[] All = [new("Easy", 2, 2, 45), new("Normal", 2, 3, 60), new("Hard", 4, 4, 120)];
    public override string ToString() => LocalizationManager.GetString("DifficultyFormat", LocalizationManager.GetString("Difficulty" + Name), Rows, Columns);
}
public sealed class CardModel(int creature)
{
    public int Creature { get; } = creature;
    public bool Revealed
    {
        get; set;
    }
    public bool Matched
    {
        get; set;
    }
}
public record Result(string Name, int Score, int Moves, int CompletionSeconds, DateTime Date, string Difficulty, bool Won);
