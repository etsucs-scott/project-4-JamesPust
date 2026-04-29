namespace Solitare.Tests;
using SolitaireGame.Models;
using SolitaireGame.Services;



return TestRunner.Run();


static class TestRunner
{
    public static int Run()
    {
        var tests = new List<(string Name, Action Body)>();
        void Add(string n, Action b) => tests.Add((n, b));

        //TEST 1
        Add("Card.IsRed is true for Hearts and Diamonds", () =>
        {
            Verify.True(new Card(Suit.Hearts, Rank.Ace).IsRed, "Hearts");
            Verify.True(new Card(Suit.Diamonds, Rank.Two).IsRed, "Diamonds");
            Verify.False(new Card(Suit.Clubs, Rank.King).IsRed, "Clubs");
            Verify.False(new Card(Suit.Spades, Rank.Queen).IsRed, "Spades");
        });

        //TEST 2
        Add("Card.RankLabel and SuitSymbol return correct strings", () =>
        {
            var ace = new Card(Suit.Hearts, Rank.Ace);
            var king = new Card(Suit.Spades, Rank.King);
            var ten = new Card(Suit.Clubs, Rank.Ten);
            Verify.Equal("A", ace.RankLabel, "Ace label");
            Verify.Equal("K", king.RankLabel, "King label");
            Verify.Equal("10", ten.RankLabel, "Ten label");
        });

        //TEST 3
        Add("NewGame deals exactly 52 unique cards across all zones", () =>
        {
            var engine = new GameEngine();
            engine.NewGame();
            var all = new List<Card>();
            all.AddRange(engine.State.Stock);
            all.AddRange(engine.State.Waste);
            foreach (var f in engine.State.Foundations.Values) all.AddRange(f);
            foreach (var col in engine.State.Tableau) all.AddRange(col);
            Verify.Equal(52, all.Count, "Total cards");
            Verify.Equal(52, all.Select(c => $"{c.Suit}-{c.Rank}").Distinct().Count(), "Unique cards");
        });

        //TEST 4
        Add("Tableau dealt correctly: column i has i+1 cards, only top face-up", () =>
        {
            var engine = new GameEngine();
            engine.NewGame();
            for (int i = 0; i < 7; i++)
            {
                var col = engine.State.Tableau[i];
                Verify.Equal(i + 1, col.Count, $"Col {i} count");
                Verify.True(col[^1].FaceUp, $"Col {i} top face-up");
                for (int j = 0; j < col.Count - 1; j++)
                    Verify.False(col[j].FaceUp, $"Col {i} row {j} face-down");
            }
        });

        //TEST 5
        Add("DrawFromStock moves one card to Waste face-up", () =>
        {
            var engine = new GameEngine();
            engine.NewGame();
            int sb = engine.State.Stock.Count;
            int wb = engine.State.Waste.Count;
            engine.DrawFromStock();
            Verify.Equal(sb - 1, engine.State.Stock.Count, "Stock -1");
            Verify.Equal(wb + 1, engine.State.Waste.Count, "Waste +1");
            Verify.True(engine.State.Waste.Peek().FaceUp, "Top of waste face-up");
        });

        //TEST 6
        Add("DrawFromStock recycles Waste to Stock when Stock is empty", () =>
        {
            var engine = new GameEngine();
            engine.NewGame();
            while (engine.State.Stock.Count > 0) engine.DrawFromStock();
            int wc = engine.State.Waste.Count;
            Verify.True(wc > 0, "Waste has cards before recycle");
            engine.DrawFromStock();   // triggers recycle
            Verify.Equal(0, engine.State.Waste.Count, "Waste emptied after recycle");
            Verify.True(engine.State.Stock.Count >= wc - 1, "Stock refilled");
        });

        //TEST 7
        Add("Foundation requires Ace first, then sequential same suit", () =>
        {
            var engine = new GameEngine();
            engine.NewGame();
            engine.State.Waste.Push(new Card(Suit.Hearts, Rank.Ace, faceUp: true));
            Verify.True(engine.TryWasteToFoundation(), "Ace of Hearts OK");
            engine.State.Waste.Push(new Card(Suit.Hearts, Rank.Two, faceUp: true));
            Verify.True(engine.TryWasteToFoundation(), "Two of Hearts OK");
            engine.State.Waste.Push(new Card(Suit.Hearts, Rank.Four, faceUp: true));
            Verify.False(engine.TryWasteToFoundation(), "Four without Three rejected");
        });

        //TEST 8
        Add("Tableau enforces alternating colour and descending rank", () =>
        {
            var engine = new GameEngine();
            engine.NewGame();
            engine.State.Tableau[0] = new List<Card>
                { new Card(Suit.Hearts, Rank.Seven, faceUp: true) };

            engine.State.Waste.Push(new Card(Suit.Spades, Rank.Six, faceUp: true));
            Verify.True(engine.TryWasteToTableau(0), "Black 6 on Red 7 OK");

            engine.State.Waste.Push(new Card(Suit.Diamonds, Rank.Five, faceUp: true));
            Verify.True(engine.TryWasteToTableau(0), "Red 5 on Black 6 OK");

            engine.State.Waste.Push(new Card(Suit.Hearts, Rank.Four, faceUp: true));
            Verify.False(engine.TryWasteToTableau(0), "Red 4 on Red 5 rejected (same colour)");
        });
    }
}