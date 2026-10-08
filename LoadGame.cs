using System.IO;
using Newtonsoft.Json;
using ChessGame8.UI;
namespace ChessGame8.Core
{
    public static class LoadGame
    {
        // Метод для загрузки сохранённого состояния игры
        public static GameState? LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл с сохранённой партией не найден.", filePath);
            }

            // Читаем содержимое файла
            string json = File.ReadAllText(filePath);

            // Десериализуем JSON в объект GameState
            GameState? gameState = JsonConvert.DeserializeObject<GameState>(json);


            return gameState;
        }
        // Метод для восстановления состояния игры на доске
        public static void RestoreGameState(ChessBoard chessBoard, GameState gameState)
        {
            // Сбрасываем старую расстановку перед заполнением из файла
            chessBoard.SetupInitialPositions();

            // Применяем позиции фигур на шахматной доске
            foreach (var piece in gameState.Pieces)
            {
                // Преобразуем PieceType в Type
                Type convertedType = ConvertPieceTypeToCoreType(piece.Type);

                // Преобразуем Color в булево значение
                bool isWhite = ConvertColorToBool((Color)piece.Color);
                chessBoard.SetPiece(piece.X, piece.Y, convertedType, isWhite);
            }

            // Устанавливаем очередь хода
            chessBoard.currentPlayer = ConvertColorToCoreColor(gameState.CurrentPlayerColor);

            // Установим номер текущего хода
            chessBoard.TurnNumber = gameState.TurnNumber;

            // Повторим предыдущие ходы
            chessBoard.PreviousMoves = gameState.History;
        }
        // Вспомогательные методы для преобразования типов
        private static Type ConvertPieceTypeToCoreType(PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.Pawn: return Type.Pawn;
                case PieceType.Knight: return Type.Knight;
                case PieceType.Bishop: return Type.Bishop;
                case PieceType.Rook: return Type.Rook;
                case PieceType.Queen: return Type.Queen;
                case PieceType.King: return Type.King;
                default: throw new ArgumentOutOfRangeException(nameof(pieceType), "Неправильный тип фигуры");
            }
        }
        
        private static bool ConvertColorToBool(Color color)
        {
            return color == Color.White;
        }
        // Вспомогательный метод для преобразования цвета
        private static ChessGame8.Core.Color ConvertColorToCoreColor(PieceColor uiColor)
        {
            switch (uiColor)
            {
                case PieceColor.White: return ChessGame8.Core.Color.White;
                case PieceColor.Black: return ChessGame8.Core.Color.Black;
                default: throw new ArgumentOutOfRangeException(nameof(uiColor), "Неправильное значение цвета");
            }
        }
    }
}
