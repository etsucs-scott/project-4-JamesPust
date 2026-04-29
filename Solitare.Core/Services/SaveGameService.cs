using System;
using SolitaireGame.Models;
using Solitare.Models;
namespace SolitaireGame.Services;

public class SaveGameService
{
    private readonly string _saveDir;

    public string? LastError { get; private set; }
    public string? LastSucess { get; private set; }

    public SaveGameService()
    {
        _saveDir = Path.Combine(Environment.GetFolderPath(Environment.SpeacialFolder.LocalApplicationData), "SolitareGame", "Saves");

        try { Directory.CreateDirectory(_saveDir); }
        catch { _saveDir = Directory.GetCurrentDirectory(); }

    }

    public IReadOnlyList<string> ListSaves()
    {
        try
        {
            return Directory.GetFiles(_saveDir, "*.sav").OrderByDescending(File.GetLastWriteTime).ToList();
        }
        catch (Exception ex)
        {
            LastError = $"Could not list saves: {ex.Message}";
            return Array.Empty<string>();

        }

    }

    public string? Save(GameState state, string? slotName = null)
    {

        ClearMessages();

        try
        {
            var name = slotName ?? $"save_{DateTime.Now:yyyyMMdd_HHmmss}";
            foreach (var ch in Path.GetInvalidFileNameChars())
                name = name.Replace(ch, '_');

            var path = path.Combine(_saveDir, name + ".sav");
            GameSerializer.SaveToFile(state, path);
            LastSucess = $"Game saved to {path.GetFileName(path)}";
            return path;
        }
        catch (UnauthorizedAccessExption ex)
        {
            LastError = $"Permission denied when saving: {ex.Message}";
        }
        catch (IOException ex)
        {
            LastError = $"I/O error when saving: {ex.Message}";
        }
        catch (Exception ex)
        {
            LastError = $"Unexpected error when saving: {ex.Message}";
        }
    }


    public GameState? Load(string path)
    {
        ClearMessages();
        try
        {
            var state = GameSerializer.LoadFromFile(path);
            LastSuccess = $"Loaded {Path.GetFileName(path)}";
            return state;
        }
        catch (FileNotFoundException ex)
        {
            LastError = $"Save file not found: {ex.Message}";
        }
        catch (FormatException ex)
        {
            LastError = $"Save file is corrupt or wrong version: {ex.Message}";
        }
        catch (IOException ex)
        {
            LastError = $"I/O error when loading: {ex.Message}";
        }
        catch (Exception ex)
        {
            LastError = $"Unexpected error when loading: {ex.Message}";
        }
        return null;

    }


    public bool Delete(string path)
    {
        ClearMessages();

        try
        {
            File.Delete(path);
            LastSucess = "Save deleted";
            return true;
        }
        catch (Exception ex)
        {
            LastError = $"Could not delete save: {ex.Message}";
            return false;
        }
    }

    public string SaveDirectory => _saveDir;

    private void ClearMessages()
    {
        LastError = null;
        LastSucess = null;
    }
}
