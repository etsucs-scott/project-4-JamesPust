using System;
namespace Solitare.Models;


public enum Suit { Hearts, Diamonds, Clubs, Spades }

public enum Rank { Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King }


public class Card
{

    public Suit Suit { get; init; }
    public Rank Rank { get; init; }
    public bool FaceUp { get; set; }

    public Card(Suit suit, Rank rank, bool faceUp = false)
    {
        Suit = suit;
        Rank = rank;
        FaceUp = faceUp;
    }

    public bool IsRed => Suit is Suit.Hearts or Suit.Diamonds;

    public string SuitLabel => Suit switch
    {
        Suit.Hearts => "Hearts",
        Suit.Diamonds => "Diamonds",
        Suit.Clubs => "Clubs",
        Suit.Spades => "Spades",
        _ => "?"
    };
    public string RankLabel => Rank switch
    {
        Rank.Ace => "A",
        Rank.Jack => "J",
        Rank.Queen => "Q",
        Rank.King => "K",
        _ => ((int)Rank).ToString()

    };

    public override string ToString() => $"{RankLabel}{SuitLabel}";

    public string WriteToFile() => $"{(int)Suit}|{(int)Rank}|{(FaceUp ? 1 : 0)}";

    public static Card ReadFromFile(string data)
    {
        var parts = data.Split('|');
        if (parts.Length != 3)
            throw new FormatException($"Invalid Card Data: {data}");

        if (!int.TryParse(parts[0], out var suitInt) || !Enum.IsDefined(typeof(Suit), suitInt))
            throw new FormatException($"Invalid Suit Value: {parts[0]}");

        if (!int.TryParse(parts[1], out var rankInt) || !Enum.IsDefined(typeof(Rank), rankInt))
            throw new FormatException($"Invalid Rank Value: {parts[1]}");

        return new Card((Suit)suitInt, (Rank)rankInt, parts[2] == "1");


    }
}
