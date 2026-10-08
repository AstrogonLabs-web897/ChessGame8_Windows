using System;


namespace ChessGame8.Core
{
    public class Bishop: Figure
    {
        public Bishop(bool isWhite) :  base(FigureType.Bishop, isWhite ? Color.White : Color.Black, isWhite) { }
        public override bool CanMoveTo(int fromX, int fromY, int toX, int toY)
        {
            // Слон ходит только по диагоналям
            return Math.Abs(toX - fromX) == Math.Abs(toY - fromY);
        }
    }
}
