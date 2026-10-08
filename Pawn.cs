using System;

namespace ChessGame8.Core
{
    public class Pawn : Figure
    {
        public Pawn(bool isWhite) : base(FigureType.Pawn, isWhite ? Color.White : Color.Black, isWhite) { }

        public override bool CanMoveTo(int fromX, int fromY, int toX, int toY)
        {
            // Простая логика пешки
            if (IsWhite)
            {
                return toY == fromY + 1 && toX == fromX;
            }
            else
            {
                return toY == fromY - 1 && toX == fromX;
            }
        }
    }
}
