using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ChessGame8.Core
{
    public enum Type
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }
    public enum FigureType
    {
        None,
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King,
        Empty
    }

    public enum Color
    {
        
        White,
        Black,
        None, 
        Human,
        Computer,
        Transparent,
       
    }


    public abstract class Figure
    {
        public bool IsWhite { get; }
        public FigureType Type { get; }
        public Color Color { get; }

        // Координаты фигуры
        public Coordinate Position { get; set; }
        public Figure(FigureType type, Color color, bool isWhite)
        {
            Type = type;
            Color = color;
            IsWhite = isWhite;
        }

        public abstract bool CanMoveTo(int fromX, int fromY, int toX, int toY);
        // Метод проверки наличия доступных ходов
        public bool HasAvailableMoves()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (CanMoveTo(Position.X, Position.Y, x, y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public class EmptyFigure : Figure
        {
            public EmptyFigure() :  base(FigureType.Empty,Color.Transparent, false) { }

            public override bool CanMoveTo(int fromX, int fromY, int toX, int toY)
            {
                return false;
            }
        }
        public struct Coordinate(int x, int y)
        {
            public int X { get; set; } = x;
            public int Y { get; set; } = y;
        }
    }
}

