using System;

namespace ChessGame8.Core
{
    public class Queen : Figure
    {
        public Queen(bool isWhite) : base(FigureType.Queen, isWhite ? Color.White : Color.Black, isWhite) { }

        public override bool CanMoveTo(int fromX, int fromY, int toX, int toY)
        {
            // Ферзь ходит вертикально, горизонтально или по диагонали
            return fromX == toX || fromY == toY || Math.Abs(toX - fromX) == Math.Abs(toY - fromY);
        }
    }
}
