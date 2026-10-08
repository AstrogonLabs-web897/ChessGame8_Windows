using System;


namespace ChessGame8.Core
{
    public class King: Figure
    {
        public King(bool isWhite) : base(FigureType.King, isWhite ? Color.White : Color.Black, isWhite) { }

        public override bool CanMoveTo(int fromX, int fromY, int toX, int toY)
        {
            // Король ходит на одну клетку в любом направлении
            return (
                Math.Abs(toX - fromX) <= 1 &&
                Math.Abs(toY - fromY) <= 1
            );
        }
    }
}
