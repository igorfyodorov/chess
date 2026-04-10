namespace Model.ChessLogic;

public static class GameSerializer
{
    public static bool Save(GameState state)
    {
        string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "saved_games");
        string path = Path.Combine(folderPath, $"autosave_{DateTime.Now.ToString("dd.MM.yyyy_HH-mm-ss")}.json");
        string json = System.Text.Json.JsonSerializer.Serialize(state, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

        try
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            
            File.WriteAllText(path, json);
        }

        catch (Exception)
        {
            return false;
        }
        
        return true;
    }

    public static GameState Load(string path)
    {
        if (!File.Exists(path)) return null!;
        string json = File.ReadAllText(path);
        return System.Text.Json.JsonSerializer.Deserialize<GameState>(json)!;
    }
}