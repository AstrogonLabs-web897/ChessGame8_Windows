using ChessGame8.UI;
using System;
using static ChessGame8.UI.GameScreen2;
namespace ChessGame8.Core
{
    public class ChessBoard
    {
        public const int SIZE = 8;
        private  Figure [,] _figures= new Figure[SIZE, SIZE];
       
        public Color currentPlayer; // Очередь хода
        public int TurnNumber { get; set; } // Номер текущего хода
        public List<ChessGame8.UI.Move> PreviousMoves { get; set; } = new List<ChessGame8.UI.Move>(); // История предыдущих ходов
        private Piece[,] _board = new Piece[SIZE, SIZE];// Представление доски (массив фигур)
        public ChessBoard()
        {
            SetupInitialPositions();
            currentPlayer = Color.White;
            TurnNumber = 1;
        }

        // Метод для получения фигуры по координатам
        public Piece? GetPieceAt(int column, int row)
        {
            if (column < 0 || column >= 8 || row < 0 || row >= 8) { return null;}
               
            else { return _board[column, row]; }
                                        
        }
        public Figure GetPiece(int x, int y)
        {
            if (x < 0 || x >= SIZE || y < 0 || y >= SIZE)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "Недопустимая координата клетки");
                throw new ArgumentOutOfRangeException(nameof(y), "Недопустимая координата клетки");
            }
            return _figures[x, y] ?? new EmptyFigure();
        }
        public void SetPiece(int x, int y, Type type, bool isWhite)
        {
            // Создаем подходящую фигуру исходя из типа и цвета
            Figure piece = new EmptyFigure();
            switch (type)
            {
                case Type.Pawn: piece = new Pawn(isWhite); break;
                case Type.Knight: piece = new Knight(isWhite); break;
                case Type.Bishop: piece = new Bishop(isWhite); break;
                case Type.Rook: piece = new Rook(isWhite); break;
                case Type.Queen: piece = new Queen(isWhite); break;
                case Type.King: piece = new King(isWhite); break;
            }

            // Если получилась валидная фигура, ставим её на доску
            _figures[x, y] = piece;
        }
        class EmptyFigure : Figure
        {
            public EmptyFigure() : base(FigureType.Empty, Color.Transparent, false)  { }
            public override bool CanMoveTo(int fromX, int fromY, int toX, int toY)
            {
                return false;
            }
        }
        // Метод для проверки легальности хода
        public bool IsLegalMove(int fromX, int fromY, int toX, int toY)
        {
            var piece = GetPiece(fromX, fromY);
            if (piece == null) return false; // Нельзя сдвинуть пустую клетку

            // Простая проверка на выход за пределы доски
            if (toX < 0 || toX >= SIZE || toY < 0 || toY >= SIZE)
            {
                return false;
            }

            // Простая проверка на правило каждого типа фигуры
            return piece.CanMoveTo(fromX, fromY, toX, toY);
        }
        // Метод для перемещения фигуры
        public void MovePiece(int fromX, int fromY, int toX, int toY)
        {
            if (!IsLegalMove(fromX, fromY, toX, toY))
            {
                throw new InvalidOperationException("Ход невозможен");
            }

            var piece = GetPiece(fromX, fromY);
            _figures[toX, toY] = piece;
#pragma warning disable CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.
            _figures[fromX, fromY] = null;
#pragma warning restore CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.
        }
        public void SetupInitialPositions()
        {
            // Инициализация фигур на шахматной доске
            // Очищаем доску перед расстановкой
            for (int y = 0; y < SIZE; y++)
                for (int x = 0; x < SIZE; x++)
                    _figures[x, y] = new EmptyFigure();

            // Инициализация пешек
            for (int col = 0; col < SIZE; col++)
            {
                _figures[col, 1] = new Pawn(true);
                _figures[col, 6] = new Pawn(false);
            }

            // Фигуры
            _figures[0, 0] = new Rook(true); _figures[7, 0] = new Rook(true);
            _figures[0, 7] = new Rook(false); _figures[7, 7] = new Rook(false);

            _figures[3, 0] = new Queen(true); _figures[3, 7] = new Queen(false);
            _figures[4, 0] = new King(true); _figures[4, 7] = new King(false);

            _figures[2, 0] = new Bishop(true); _figures[5, 0] = new Bishop(true);
            _figures[2, 7] = new Bishop(false); _figures[5, 7] = new Bishop(false);

            _figures[1, 0] = new Knight(true); _figures[6, 0] = new Knight(true);
            _figures[1, 7] = new Knight(false); _figures[6, 7] = new Knight(false);
        }
        public Figure GetFigureAt(int x, int y)
        {
            if (x < 0 || x >= SIZE || y < 0 || y >= SIZE) return new EmptyFigure();
            return _figures[x, y] ?? new EmptyFigure();
        }
        public void SetFigureAt(int x, int y, Figure figure)
        {
            _figures[x, y] = figure ?? new EmptyFigure();
        }
        public void ExecuteMove(int fromX, int fromY, int toX, int toY)
        {
            // Логика перемещения фигуры
            var figure = GetFigureAt(fromX, fromY);
            SetFigureAt(toX, toY, figure);
#pragma warning disable CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.
            SetFigureAt(fromX, fromY, null);
#pragma warning restore CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.
        }
        public void ExecuteMove2(ChessGame8.UI.Move move)
        {
            if (move == null) return;
            else
            {
                // Выполняем ход на основе объекта Move
                ExecuteMove(move.FromX, move.FromY, move.ToX, move.ToY);

            }
            
        }
        public void UndoMove(int fromX, int fromY, int toX, int toY)
        {
            // Восстанавливаем фигуру на исходную позицию
            var tempFigure = GetFigureAt(toX, toY);
            SetFigureAt(fromX, fromY, tempFigure);
#pragma warning disable CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.
            SetFigureAt(toX, toY, null);
#pragma warning restore CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.
        }
        public List<ChessGame8.Core.ChessBoard.Move> GetAllPossibleMoves(Color side)
        {
            var result = new List<ChessGame8.Core.ChessBoard.Move>();
            for (int y = 0; y < SIZE; y++)
            {
                for (int x = 0; x < SIZE; x++)
                {
                    var figure = GetFigureAt(x, y);
                    if (figure != null && figure.Color == side)
                    {
                        for (int tx = 0; tx < SIZE; tx++)
                        {
                            for (int ty = 0; ty < SIZE; ty++)
                            {
                                if (figure.CanMoveTo(x, y, tx, ty))
                                    result.Add(new Move(x, y, tx, ty));
                            }
                        }
                    }
                }
            }
            return result;
        }
        // Все фигуры на доске
        public IEnumerable<Figure> GetFigures()
        {
            {
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        yield return _figures[x, y];
                    }
                }
            }
        }
        // Все фигуры на доске 2
        public IEnumerable<Figure> Figures
        {
            get
            {
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        yield return _figures[x, y];
                    }
                }
            }
        }

        // Класс Move для удобства работы с ходами
        public class Move
        {
            public int FromX { get; }
            public int FromY { get; }
            public int ToX { get; }
            public int ToY { get; }
#pragma warning disable
            public Move(int fromX, int fromY, int toX, int toY)
            {
                FromX = fromX;
                FromY = fromY;
                ToX = toX;
                ToY = toY;
            }
        }
        public bool IsCheckmate(PlayerColor side)
        {
            // Преобразуем PlayerColor в Color (если нужно)
            var coreColor = side  == PlayerColor.White ? Color.White : Color.Black;
            // Нахождение положения короля
            var kingPosition = FindKingPosition((PlayerColor)coreColor);
            if (kingPosition == null)
            { 
                return false; // Нельзя определить мат, если короля нет на доске

            }
               

            // Проверка, находится ли король под шахом
            if (!IsUnderAttack(kingPosition.Value.x, kingPosition.Value.y, (PlayerColor)coreColor))
            { 
                return false; // Если король не под шахом, мата нет

            }  
               

            // Проверка, есть ли доступные ходы для защиты
            var availableMoves = GetAllPossibleMoves(coreColor);
            return availableMoves.Count == 0;
        }
        // Вспомогательный метод для поиска позиции короля
        private (int x, int y)? FindKingPosition(PlayerColor side)
        {
            var coreColor = side == PlayerColor.White ? Color.White : Color.Black;
            for (int y = 0; y < SIZE; y++)
            {
                for (int x = 0; x < SIZE; x++)
                {
                    var figure = GetFigureAt(x, y);
                    if (figure != null && figure.Type == FigureType.King && figure.Color == coreColor)
                        return (x, y); // или просто (x, y), если используете кортежи нового типа
                }
            }
            return null;
        }

        // Проверка, находится ли клетка под атакой
        private bool IsUnderAttack(int x, int y, PlayerColor defenderColor)
        {
            var coreColor = defenderColor == PlayerColor.White ? Color.White : Color.Black;
            // Проверяем угрозу от всех фигур противника
            var attackerColor = coreColor==Color.White ? Color.Black : Color.White;
            for (int tx = 0; tx < SIZE; tx++)
            {
                for (int ty = 0; ty < SIZE; ty++)
                {
                    var figure = GetFigureAt(tx, ty);
                    if (figure != null && figure.Color == attackerColor && figure.CanMoveTo(tx, ty, x, y))
                        return true;
                }
            }
            return false;
        }
    }
}
