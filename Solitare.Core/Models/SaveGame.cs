using System;
namespace Solitare.Models;

public class SaveGame
{
    public int Score { get; set; }
    public int Moves { get; set; }
    public DateTime StartedAt { get; set; }

    public List<string> StockCards { get; set; } = new();

    public List<string> WasteCards { get; set; } = new();

    public List<List<string>> FoundationCards { get; set; } =
        Enumerable.Range(0, 4).Select(_ => new List<string>()).ToList();

    public List<List<string>> TableauCards { get; set; } =
        Enumerable.Range(0, 7).Select(_ => new List<string>()).ToList();

    private const string SectionSeperation = "---";

    public string ToText()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"SCORE:{Score}");
        sb.AppendLine($"MOVES:{Moves}");
        sb.AppendLine($"STARTED:{StartedAt.Ticks}");
        sb.AppendLine(SectionSeperation);
        sb.AppendLine($"STOCK:{string.Join(",", StockCards)}");
        sb.AppendLine($"WASTE:{string.Join(",", WasteCards)}");
        sb.AppendLine(SectionSeperation);
        for (int i = 0; i < 4; i++)
            sb.AppendLine($"FOUND{i}: {string.Join(", ", FoundationCards[i])}");
        sb.AppendLine(SectionSeperation);
        for (int i = 0; i < 7; i++)
            sb.AppendLine($"TAB{i}:{string.Join(", ", TableauCards[i])}");
        return sb.ToString();

    }


    public static SaveGame FromText(string text)
    {
        var sg = new SaveGame();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (line == SectionSeperation || string.IsNullOrWhiteSpace(line)) continue;

            var colon = line.IndexOf(':');
            if (colon < 0) throw new FormatException($"Missing colon in line: '{line}'");

            var key = line[..colon];
            var val = line[(colon + 1)..];

            switch (key)
            {
                case "SCORE":
                    if (!int.TryParse(val, out var sc)) throw new FormatException("Bad SCORE");
                    sg.Score = sc;
                    break;

                case "MOVES":
                    if (!int.TryParse(val, out var mv)) throw new FormatException("Bad MOVES");
                    sg.Moves = mv;
                    break;

                case "STARTED":
                    if (!long.TryParse(val, out var ticks)) throw new FormatException("Bad STARTED");
                    sg.StartedAt = new DateTime(ticks, DateTimeKind.Utc);
                    break;

                case "STOCK":
                    sg.StockCards = ParseCardList(val);
                    break;

                case "WASTE":
                    sg.WasteCards = ParseCardList(val);
                    break;

                default:
                    if (key.StartsWith("FOUND") && int.TryParse(key[5..], out var fi) && fi is >= 0 and <= 3)
                        sg.FoundationCards[fi] = ParseCardList(val);
                    else if (key.StartsWith("TAB") && int.TryParse(key[3..], out var ti) && ti is >= 0 and <= 6)
                        sg.TableauCards[ti] = ParseCardList(val);
                    else
                        throw new FormatException($"Unknown key: {key}");
                    break;

            }
        }

        return sg;
    }

    private static List<string> ParseCardList(string val) =>
        string.IsNullOrWhiteSpace(val)
        ? new List<string>()
        : val.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

}
