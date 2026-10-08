using ChessGame8.Core;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Windows.Media;
using Windows.UI.Input.Inking.Analysis;
using static ChessGame8.UI.GameScreen2;

namespace ChessGame8.UI
{
    public class GameState
    {
        public required List<Piece> Pieces { get; set; } = new List<Piece>();// Позиции фигур
        public PieceColor CurrentPlayerColor { get; set; } = PieceColor.White; // Очередь хода
       
        public int TurnNumber { get; set; } = 1; // Номер текущего хода                                 
        public List<Move> History { get; set; } = new List<Move>();// История ходов

        [Newtonsoft.Json.JsonIgnore]
        public  ChessBoard Board {get; set;} = new ChessBoard(); //наша шахматная доска

        [Newtonsoft.Json.JsonIgnore]
        public  GameController gameController {get; set; } = new GameController(); // наш Game Controller

        // Метод выполнения хода ExcuteMove3(Move move)
        public void ExecuteMove3(Move move)  
        {
            Pieces.RemoveAll(p => p.X == move.FromX && p.Y == move.FromY);
            Pieces.Add(new Piece
            {
                X = move.ToX,
                Y = move.ToY,
                Type = move.PieceType,
                Color = move.PieceColor
            });
            // Добавляем ход в историю
            History.Add(move);

            // Меняем очередь хода
            ChangeTurn();
        }
        // Метод смены очереди хода
        public void ChangeTurn()
        {
            CurrentPlayerColor = CurrentPlayerColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            TurnNumber++;
        }

        // Метод проверки окончания игры
        public bool IsGameOver()
        {
            var colorForCheck = CurrentPlayerColor == PieceColor.White ? PlayerColor.White : PlayerColor.Black;
            return IsCheckmate(colorForCheck) || gameController.IsDraw();
        }

        public bool IsCheckmate(ChessGame8.UI.GameScreen2.PlayerColor playerColor)
        {
            // Сначала преобразуем PlayerColor в ChessGame8.UI.Color
            var uiColor = playerColor.ToUIColor();
            var Color = (uiColor.UIColorToCoreColor()).ToPlayerColor();
            return Board.IsCheckmate(Color);
        }
    }
    public class Piece
    {
        public int X { get; set; } // Координата X
        public int Y { get; set; } // Координата Y
        public PieceType Type { get; set; } // Тип фигуры
        public PieceColor Color { get; set; } // Цвет фигуры
        // Метод преобразования Color в PlayerColor                             
        public PlayerColor GetPlayerColor()
        {
            return Color == PieceColor.White ? PlayerColor.White : PlayerColor.Black;
        }
        // Новый метод IsWhite
        public bool IsWhite()
        {
            return Color == PieceColor.White;
        }
        // Перечисление цветов игроков
        public enum PlayerColor
        {
            White,
            Black
        }

    }

    // Класс хода
    public class Move
    {
        public int FromX { get; set; } // Исходная координата X
        public int FromY { get; set; } // Исходная координата Y
        public int ToX { get; set; } // Конечная координата X
        public int ToY { get; set; } // Конечная координата Y
        public PieceType PieceType { get; set; } // Тип фигуры
        public PieceColor PieceColor { get; set; } // Цвет фигуры
        public Move(int fromX, int fromY, int toX, int toY)
        {
            FromX = fromX;
            FromY = fromY;
            ToX = toX;
            ToY = toY;
        }
    }

    
    public enum PieceType
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    public enum PieceColor
    {
        White,
        Black
    }
    public static class GameMode
    {
        // Статическое свойство для хранения выбранного режима
        public static GameModeEnum SelectedMode { get; set; } = GameModeEnum.Unknown;
    }

    // Перечисление возможных режимов игры
    public enum GameModeEnum
    {
        Unknown,
        SinglePlayerVsComputer,
        Miltiplayer2VsComputer,
        Multiplayer2v2, 
        Multiplayer4v4
    }
}
