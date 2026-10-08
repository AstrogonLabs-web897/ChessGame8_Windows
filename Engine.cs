using System;
using static ChessGame8.UI.GameScreen2;
namespace ChessGame8.Core
{
    public class SmartAI
    {
        private readonly ChessBoard _board;
        private readonly int _maxDepth;
        private  PlayerColor _color;
        public SmartAI(ChessBoard board,PlayerColor color, int maxDepth = 3 )
        {
#pragma warning disable 
            _board = board ?? throw new ArgumentNullException(nameof(board));
            _color = color;
            _maxDepth = maxDepth;
        }

        public Move CalculateBestMove(PlayerColor side)
        {
            ChessGame8.Core.Color coreSide = side == PlayerColor.White ? ChessGame8.Core.Color.White : ChessGame8.Core.Color.Black;
            var moves = _board.GetAllPossibleMoves(coreSide);
            if (moves.Count == 0)
            {
                throw new InvalidOperationException("Недоступны возможные ходы.");
            }

            var bestMove = moves[0];
            double bestScore = double.NegativeInfinity;

            foreach (var move in moves)
            {
                _board.ExecuteMove(move.FromX, move.FromY, move.ToX, move.ToY);
                var score = Negamax(-double.MaxValue, double.MaxValue, _maxDepth, coreSide == Color.White ? PlayerColor.Black : PlayerColor.White);
                _board.UndoMove(move.FromX, move.FromY, move.ToX, move.ToY);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        private double Negamax(double alpha, double beta, int depth, PlayerColor side)
        {
            ChessGame8.Core.Color coreSide = side == PlayerColor.White ? ChessGame8.Core.Color.White : ChessGame8.Core.Color.Black;
            if (depth == 0 || _board.IsCheckmate((PlayerColor)side))
                return EvaluatePosition(side);

            var moves = _board.GetAllPossibleMoves(coreSide);
            if (moves.Count == 0) return EvaluatePosition(side);

            double bestScore = double.NegativeInfinity;

            foreach (var move in moves)
            {
                _board.ExecuteMove(move.FromX, move.FromY, move.ToX, move.ToY);
                var score = -Negamax(-beta, -alpha, depth - 1, coreSide == Color.White ? PlayerColor.Black : PlayerColor.White);
                _board.UndoMove(move.FromX, move.FromY, move.ToX, move.ToY);

                bestScore = Math.Max(bestScore, score);
                alpha = Math.Max(alpha, bestScore);

                if (alpha >= beta)
                    break; // Отсекаем ветку благодаря альфа-бета-отсечению
            }

            return bestScore;
        }

        private double EvaluatePosition(PlayerColor side)
        {
            ChessGame8.Core.Color coreSide = side == PlayerColor.White ? ChessGame8.Core.Color.White : ChessGame8.Core.Color.Black;
            // Простая эвристика оценки позиции
            int materialBalance = MaterialHeuristic(side);
            int mobilityBonus = MobilityHeuristic(coreSide);
            return materialBalance + mobilityBonus;
        }

        private int MaterialHeuristic(PlayerColor side)
        {
            ChessGame8.Core.Color coreSide = side == PlayerColor.White ? ChessGame8.Core.Color.White : ChessGame8.Core.Color.Black;
            int whiteMaterial = SumFigureValues(Color.White);
            int blackMaterial = SumFigureValues(Color.Black);
            return side == PlayerColor.White ? whiteMaterial - blackMaterial : blackMaterial - whiteMaterial;
        }

        private int SumFigureValues(Color color)
        {
            int sum = 0;
            for (int y = 0; y < ChessBoard.SIZE; y++)
            {
                for (int x = 0; x < ChessBoard.SIZE; x++)
                {
                    var figure = _board.GetFigureAt(x, y);
                    if (figure != null && figure.Color == color)
                    {
                        sum += GetFigureValue(figure.Type);
                    }
                }
            }
            return sum;
        }
#pragma warning disable
        private int GetFigureValue(FigureType type)
        {
            return type switch
            {
                FigureType.Pawn => 1,
                FigureType.Knight => 3,
                FigureType.Bishop => 3,
                FigureType.Rook => 5,
                FigureType.Queen => 9,
                FigureType.King => 100,
                FigureType.None => 0,
                FigureType.Empty => 0,
                _ => 0

            };
        }

        private int MobilityHeuristic(Color side)
        {
            return _board.GetAllPossibleMoves(side).Count;
        }
    }

    public class Move
    {
        public int FromX { get; set; }
        public int FromY { get; set; }
        public int ToX { get; set; }
        public int ToY { get; set; }

        public Move(int fromX, int fromY, int toX, int toY)
        {
            FromX = fromX;
            FromY = fromY;
            ToX = toX;
            ToY = toY;
        }
#pragma warning disable 
        // Кастомное преобразование
        public static implicit operator ChessGame8.Core.Move(ChessGame8.Core.ChessBoard.Move source)
        {
            return new ChessGame8.Core.Move(source.FromX, source.FromY, source.ToX, source.ToY);
        }

        public static implicit operator ChessGame8.Core.ChessBoard.Move(ChessGame8.Core.Move source)
        {
            return new ChessGame8.Core.ChessBoard.Move(source.FromX, source.FromY, source.ToX, source.ToY);
        }
    }
}
