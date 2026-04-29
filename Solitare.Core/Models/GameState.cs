using System;
namespace Solitare.Models;

public class GameState
{
    /// <summary>
    /// face down card in deck
    /// </summary>
    public Stack<Card> Stock { get; set; } = new();

    /// <summary>
    /// face up discard pile
    /// </summary>
    public Stack<Card> Waste { get; set; } = new();


    public Dictionary<Suit, Stack<Card>> Foundations { get; set; } = new()
    {
        [Suit.Hearts] = new Stack<Card>(),
        [Suit.Diamonds] = new Stack<Card>(),
        [Suit.Clubs] = new Stack<Card>(),
        [Suit.Spades] = new Stack<Card>(),

    };

    public List<List<Card>> Tablaeu { get; set; } =
        Enumerable.Range(0, 7).Select(_ => new List<Card>()).ToList();

    public int Score { get; set; }
    public int Moves { get; set; }
    public bool IsWon { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public int ElapsedSeconds => (int)(DateTime.UtcNow - StartedAt).TotalSeconds;

    public Stack<string> UndoHistory { get; set; } = new();
}



