using System;
using SolitaireGame.Models;
using Solitare.Models;
namespace SolitaireGame.Services;

public class GameSerializer
{


    public static string Serialize(GmaState state)
    {
        var sg = ToSaveGame(state);
        return sg.ToText();
    }


    public static GameState Deserialize(string snapshot)
    {
        var sg = SaveGame.FromText(snapshot);
        return FromSaveGame(sg);
    }


    public static void SaveToFile(GmaeState state, string path)
    {
        var text = ToSaveGame(state).ToText();
        File.WriteAllText(path, text);
    }


    public static GameState LoadFromFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Save file not found: {path}");

        var text = File.ReadAllText(path);
        var sg = SaveGame.FromFile(text);
        return FromSaveGame(sg);
    }


    private static SaveGame ToSaveGame(GameState state)
    {
        var sg = new SaveGame
        {
            Score = state.Score,
            Moves = state.Moves,
            StartedAt = state.StartedAt,
            StockCards = state.Stock.Select(c => c.Serialize()).ToList(),
            WasteCards = state.WasteCards.Select(c => c.Serialize()).ToList(),
        };

        foreach (var suit in Enum.GetValues<Suit>())
            sg.FoundationCards[(int)suit] = state.Foundations[suit].Select(c => c.Serialize()).ToList();

        for (int i = 0; i < 7; i++)
            sg.TableauCards[i] = state.Tableau[i].Select(c => c.Serialize()).ToList();

        return sg;
    }


    private static GameState FromSaveGame(SaveGame sg)
    {
        var state = new GameState
        {
            Score = sg.Score,
            Moves = sg.Moves,
            StartedAt = sg.StartedAt,
        };

        //these three foreach's (Stock, Waste, Foundations) rebuild from file top to bottom as stack
        foreach (var s in sg.StockCards)
            state.Stock.Push(Card.Deserialize(s));

        foreach (var s in sg.WasteCards)
            state.Waste.Push(Card.Deserialize(s));

        foreach (var suit in Enum.GetValues<Suit>())
        {
            var list = sg.FoundationCards[(int)suit];
            foreach (var s in list)
                state.Foundations[suit].Push(Card.Deserialize(s));
        }

        //the same for Tableau cards but this is is bottom to top
        for (int i = 0; i < 7; i++)
            state.Tableau[i] = sg.TableauCards[i].Select(Card.Deserialize).ToList();

        return state;
    }
}
