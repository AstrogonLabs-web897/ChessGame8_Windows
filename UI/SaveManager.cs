using System.IO;
using Newtonsoft.Json;
namespace ChessGame8.UI
{
    public static class SaveManager
    {
        // Сохранение состояния игры в файл
        public static void SaveGame(GameState gameState, string filePath)
        {
            string json = JsonConvert.SerializeObject(gameState, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        // Загрузка состояния игры из файла
        public static GameState LoadGame(string filePath)
        {
            string json = File.ReadAllText(filePath);
            GameState gameState = JsonConvert.DeserializeObject<GameState>(json) ?? throw new Exception("Не удалось загрузить игровое состояние");
            return gameState;
        }
    }
}
