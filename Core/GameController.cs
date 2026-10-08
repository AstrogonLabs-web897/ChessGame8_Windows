using ChessGame8.UI;
using System;
using System.ComponentModel;
using static ChessGame8.UI.GameScreen2;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ChessGame8.Core
{
    public class GameController: INotifyPropertyChanged
    {
        #region Fields
        // Шахматная доска
        private ChessBoard _board;
        private GameScreenViewModel _viewModel;
        // Текущий игрок
        private Player _currentPlayer;

        // Очередь хода (Human или Computer)
        private Color _turn;

        // Время на ход
        private TimeSpan _remainingTime;

        // Статус игры
        private string _gameStatus = string.Empty;

        // Выделенная фигура 
        private Figure? _selectedFigure;
        // выбранная игороком позиция шахматной фигуры
        private (int, int) _selectedPosition;
        //AI
        private SmartAI _AI;
        // Таймер для контроля времени
        private DispatcherTimer _timer;
        #endregion

        #region Properties
        // Список всех игроков
        public List<Player> Players = new List<Player>();
        // Текущий игрок
        public Player CurrentPlayer
        {
            get => _currentPlayer;
            private set
            {
                _currentPlayer = value;
                OnPropertyChanged(nameof(CurrentPlayer));
            }
        }
        // Время на ход
        public TimeSpan RemainingTime
        {
            get => _remainingTime;
            private set
            {
                _remainingTime = value;
                OnPropertyChanged(nameof(RemainingTime));
            }
        }
        // Свойство следующего игрока
        public Player NextPlayer
        {
            get
            {
                var index = Players.IndexOf(CurrentPlayer);
                return Players[(index + 1) % Players.Count];
            }
        }

        // Статус игры
        public string GameStatus
        {
            get => _gameStatus;
            private set
            {
                _gameStatus = value;
                OnPropertyChanged(nameof(GameStatus));
            }
        }
        #endregion

        #region Constructors
        // Конструктор
        public GameController()
        {
            // Инициализация игроков
            InitializePlayers();
            _board = new ChessBoard();
            _selectedPosition = (-1, -1);
            _viewModel = new GameScreenViewModel();

            // Конвертируем текущий ход из Core.Color в PlayerColor для ИИ
            GameScreen2.PlayerColor aiColor = _turn.ToPlayerColor();

            // Инициализируем AI, передавая ему противоположный цвет
            _AI = new SmartAI(_board, ColorSwitcher.SwitchColor(aiColor));

            // Начальное время на ход
            RemainingTime = TimeSpan.FromMinutes(5);
             // Начальный статус игры
            GameStatus = "Игра началась";
            // Первый ход
            NextTurn();
            // Инициализируем таймер
            InitializeTimer();
        }
        #endregion

        #region Methods
        // Инициализация игроков
        private void InitializePlayers()
        {
            Players.Add(new Player("Alice",   (PlayerColor_1)PlayerColor.White,1500));
            Players.Add(new Player("Computer",(PlayerColor_1)PlayerColor.Black,2000));

            // Устанавливаем первого игрока
            CurrentPlayer = Players[0];

            _turn = ( Core.Color) CurrentPlayer.Color;
        }
        // Инициализация таймера
        private void InitializeTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += TimerTick;
            _timer.Start();
        }
        // Обработка тика таймера
        private void TimerTick(object? sender, EventArgs e)
        {
            if (_remainingTime > TimeSpan.Zero)
            {
                _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));
            }
            else
            {
                // Время вышло, пропускаем ход
                SkipTurn();
            }
        }
        // Пропуск хода
        private void SkipTurn()
        {
            GameStatus = $"{CurrentPlayer.Name} пропустил ход!";
            SwitchingTurns();
        }
        // Сброс таймера
        private void ResetTimer()
        {
            _remainingTime = TimeSpan.FromMinutes(5);
        }

        public bool TryMakeMove(int fromX, int fromY, int toX, int toY)
        {
            var figure = _board.GetFigureAt(fromX, fromY);
            if (figure == null || figure.Color != _turn)
            {
                return false;

            }
            // Проверка, что ход возможен
            if (!figure.CanMoveTo(fromX, fromY, toX, toY))
            { 
                return false;

            }

            // *** Условие для установки фигуры ***
            // Проверяем, что новая позиция не занята фигурой того же цвета
            var destinationFigure = _board.GetFigureAt(toX, toY);
            if (destinationFigure == null || destinationFigure.Color != figure.Color)
            {
                _board.SetFigureAt(toX, toY, figure);
                
            }
            else 
            { // Нельзя ставить фигуру на занятую позицию своего цвета
                return false;
            }

            // *** Условие для удаления фигуры с предыдущей позиции ***
            // Всегда удаляем фигуру с предыдущей позиции
            if (figure != null)
            {
                _board.SetFigureAt(fromX, fromY, new EmptyFigure());

            }
            else 
            {
                // Ничего не делаем, если фигура уже удалена
            }
            // Переключаем Игрока
            SwitchingTurns();
            return true;
        }

        private void SwitchTurnsInternal()
        {
            _turn = _turn == Core.Color.White ? Core.Color.Black : Core.Color.White;
        }


        public Figure GetFigureAt(int x, int y)
        {
            return _board.GetFigureAt(x, y);
        }

        public static class ColorSwitcher
        {
            // Метод переключения цветов
           public static PlayerColor SwitchColor(PlayerColor side)
           {
                    return side == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
           }
        }
        public void HandleClick(int x, int y)
        {
            // Получаем фигуру с текущей позиции
            Figure figure = _board.GetFigureAt(x, y);

            // Проверяем, чья сейчас очередь
            if (figure != null && figure.Color == _turn)
            {
                // Пометим фигуру как выбранную
                _selectedFigure = figure;
                _selectedPosition = new(x, y);
            }
            else if (_selectedFigure != null)
            {
                // Если была выбрана фигура, проверим возможность хода
                if (_selectedFigure.CanMoveTo(_selectedPosition.Item1, _selectedPosition.Item2, x, y))
                {
                    // Перемещаем фигуру
                    _board.ExecuteMove(_selectedPosition.Item1, _selectedPosition.Item2, x, y);
                    SwitchingTurns();
                }
                else
                {
                    // Иначе отменяем выбор
                    _selectedFigure = null;
                    _selectedPosition = (-1, -1);
                }
            }
        }
        // Вспомогательные методы
        public void MakeMove(ChessGame8.UI.Move move) 
        {
            // Проверяем, чей сейчас ход
            if (_turn == Color.Human) // Человек делает ход
            {
                // Выполняем ход игрока
                PerformPlayerMove(move);
            }
            else if (_turn == Color.Computer) // Компьютер делает ход
            {
                // Выполняем ход AI
                PerformAiMove();
            }
            else { }
            // Переключаем ход Игрока 
            SwitchingTurns();

            // Обновляем UI
            _viewModel.UpdateUI2();
        }
        // Метод выполнения хода игрока
        public void PerformPlayerMove(ChessGame8.UI.Move move) 
        {
            // Выполняем ход на доске
            _board.ExecuteMove(move.FromX, move.FromY, move.ToX, move.ToY);
            // Проверяем, не завершилась ли игра
            CheckGameOver();
        }
        // Метод выполнения хода AI
        private void PerformAiMove()
        {
            // Получаем текущий цвет Игрока-компьютера в формате PlayerColor
            GameScreen2.PlayerColor aiColor = _turn.ToPlayerColor();

            // Получаем лучший ход от AI
            var aiMove = _AI.CalculateBestMove(aiColor);

            // Выполняем ход на доске по координатам X и Y
            _board.ExecuteMove(aiMove.FromX, aiMove.FromY, aiMove.ToX, aiMove.ToY);

            // Проверяем, не завершилась ли игра
            CheckGameOver();
        }
        // Вспомогательный метод для проверки окончания игры
        private void CheckGameOver()
        {
            var uiColor = _turn == Core.Color.White ? PlayerColor.White : PlayerColor.Black;
            if (IsCheckmate(uiColor))
            {
                GameStatus = $"{CurrentPlayer.Name} проиграл! Мат!";
            }
            else if (IsDraw())
            {
                GameStatus = "Ничья!";
            }
            else
            {
                GameStatus = $"{NextPlayer.Name}'s turn";
            }
        }
        // Следующий ход
        public void NextTurn()
        {
            // Переключаем игрока
            var index = Players.IndexOf(CurrentPlayer);
            CurrentPlayer = Players[(index + 1) % Players.Count];

            // Обновляем статус игры
            GameStatus = $"{CurrentPlayer.Name}'s turn";

            // Сбрасываем таймер
            RemainingTime = TimeSpan.FromMinutes(5);
        }
        // Метод проверки ничьей
        public  bool IsDraw()
        {
            // Логика проверки ничьей
            // Правила ничьей в шахматах:
            // 1. Пат (нет доступных ходов)
            // 2. Троекратное повторение позиции
            // 3. 50 ходов без взятий и рокировок
            // 4. Недостаточно фигур для мата

            // 1. Пат (нет доступных ходов)
            if (NoLegalMoves())
            {
                return true;
            }

            // 2. Троекратное повторение позиции
            if (ThreefoldRepetition())
            {
                return true;
            }

            // 3. 50 ходов без взятий и рокировок
            if (FiftyMoveRule())
            {
                return true;
            }

            // 4. Недостаточно фигур для мата
            if (InsufficientMaterial())
            {
                return true;
            }

            return false;
        }
        // 1. Пат (нет доступных ходов)
        public  bool NoLegalMoves()
        {
            foreach (var figure in _board.GetFigures().Where(f => f.Color == Color.White || f.Color == Color.Black))
            {
                if (figure.HasAvailableMoves())
                {
                    return false; // Есть доступные ходы, не ничья
                }
            }
            return true; // Нет доступных ходов, объявляем ничью
        }

        // 2. Троекратное повторение позиции
        public bool ThreefoldRepetition()
        {
            // Простая реализация (реальная реализация требует хранения истории ходов)
            // Здесь мы просто имитируем проверку
            return false; // Пока не реализовано
        }

        // 3. 50 ходов без взятий и рокировок
       public bool FiftyMoveRule()
        {
            // Простая реализация (реальная реализация требует счетчика ходов)
            // Здесь мы просто имитируем проверку
            return false; // Пока не реализовано
        }
        // 4. Недостаточно фигур для мата
        public bool InsufficientMaterial()
        {
            // Логика проверки недостаточного материала
            var whitePieces = _board.GetFigures().Where(f => f.Color == Color.White).ToList();
            var blackPieces = _board.GetFigures().Where(f => f.Color == Color.Black).ToList();

            // Король против короля
            if (whitePieces.Count == 1 && blackPieces.Count == 1)
            {
                return true;
            }

            // Король и конь/лёгкая фигура против короля
            if ((whitePieces.Count == 2 && blackPieces.Count == 1) ||
                (whitePieces.Count == 1 && blackPieces.Count == 2))
            {
                return true;
            }

            return false;
        }
        // Логика обновления UI
        // Метод переключения хода
        private void SwitchingTurns()
        {
            CurrentPlayer = NextPlayer;
            SwitchTurnsInternal();
            ResetTimer();
            GameStatus = $"Ход игрока: {CurrentPlayer.Name}";
        }
        public bool IsCheckmate(ChessGame8.UI.GameScreen2.PlayerColor playerColor)
        {
            // Сначала преобразуем PlayerColor в ChessGame8.UI.Color
            var uiColor = playerColor.ToUIColor();
            var Color = (uiColor.UIColorToCoreColor()).ToPlayerColor();
            return _board.IsCheckmate(Color);
        }
        private class Position
        {
            public int X { get; }
            public int Y { get; }
            public Position(int x, int y)
            {
#pragma warning disable 
                X = x;
                Y = y;
            }
        }
        // Реализация INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
    #region MoreClasses
    public static class ColorSwitcher
    {
        // Метод переключения цветов
        public static ChessGame8.Core.Color SwitchColor(ChessGame8.Core.Color side)
        {
            return side == ChessGame8.Core.Color.White ? ChessGame8.Core.Color.Black : ChessGame8.Core.Color.White;
        }
    }
    
    public sealed class EmptyFigure : Figure
    {
        public EmptyFigure() : base(FigureType.Empty, Core.Color.Transparent,false) { }

        public override bool CanMoveTo(int fromX, int fromY, int toX, int toY) => false; // Пустая фигура не может двигаться
    }
    #endregion
}