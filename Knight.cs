using System;

namespace ChessGame8.Core
{
    public class Knight : Figure
    {
        public Knight(bool isWhite) : base(FigureType.Knight, isWhite ? Color.White : Color.Black, isWhite) { }

        public override bool CanMoveTo(int fromX, int fromY, int toX, int toY)
        {
            // Конь ходит буквой "Г"
            int dx = Math.Abs(toX - fromX);
            int dy = Math.Abs(toY - fromY);
            return (dx == 2 && dy == 1) || (dx == 1 && dy == 2);
        }
    }
}
