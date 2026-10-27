namespace PokemonMemoryBattle;

public static class ScoreManager
{
    public static int Match(int score) => score + 100;
    public static int Miss(int score) => Math.Max(0, score - 10);
    public static int Victory(int score, int seconds) => score + seconds * 5;
}
public sealed class GameManager
{
    public GameState State
    {
        get; private set;
    }
    public Difficulty Difficulty { get; private set; } = Difficulty.All[0];
    public List<CardModel> Cards { get; private set; } = [];
    public int Score
    {
        get; private set;
    }
    public int Moves
    {
        get; private set;
    }
    public int Clicks
    {
        get; private set;
    }
    public int Remaining
    {
        get; private set;
    }
    public int Pairs => Cards.Count(c => c.Matched) / 2;
    public bool InputLocked => second != null;
    private CardModel? first, second;
    public void Reset(Difficulty difficulty, Random? random = null)
    {
        Difficulty = difficulty;
        State = GameState.Ready;
        Score = Moves = Clicks = 0;
        Remaining = difficulty.Seconds;
        first = second = null;
        Cards = Enumerable.Range(0, difficulty.Rows * difficulty.Columns / 2).SelectMany(i => new[] { new CardModel(i), new CardModel(i) }).ToList();
        random ??= Random.Shared;
        // Fisher–Yates: each remaining position is equally likely.
        for (int i = Cards.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (Cards[i], Cards[j]) = (Cards[j], Cards[i]);
        }
    }
    public void Start()
    {
        if (State == GameState.Ready)
            State = GameState.Playing;
    }
    public void TogglePause()
    {
        if (State == GameState.Playing)
            State = GameState.Paused;
        else if (State == GameState.Paused)
            State = GameState.Playing;
    }
    public bool? Select(int index)
    {
        if (State != GameState.Playing || InputLocked || index < 0 || index >= Cards.Count)
            return null;
        var card = Cards[index];
        if (card.Revealed || card.Matched)
            return null;
        card.Revealed = true;
        Clicks++;
        if (first == null)
        {
            first = card;
            return null;
        }
        second = card;
        Moves++;
        if (first.Creature != second.Creature)
        {
            Score = ScoreManager.Miss(Score);
            return false;
        }
        first.Matched = second.Matched = true;
        first = second = null;
        Score = ScoreManager.Match(Score);
        if (Pairs == Cards.Count / 2)
        {
            Score = ScoreManager.Victory(Score, Remaining);
            State = GameState.Won;
        }
        return true;
    }
    public void ResolveMismatch()
    {
        if (State != GameState.Playing || second == null)
            return;
        first!.Revealed = second.Revealed = false;
        first = second = null;
    }
    public void Tick()
    {
        if (State != GameState.Playing)
            return;
        if (--Remaining <= 0)
        {
            Remaining = 0;
            State = GameState.Lost;
        }
    }
}
