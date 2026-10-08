using System;

namespace ChessGame8.Core
{
   public class Rook: Figure
    {
        public Rook(bool isWhite) : base(FigureType.Rook, isWhite ? Color.White : Color.Black, isWhite) { }

        public override bool CanMoveTo(int fromX, int fromY, int toX, int toY)
        {
            // Ладья ходит вертикально или горизонтально
            return fromX == toX || fromY == toY;
        }
    }
}
