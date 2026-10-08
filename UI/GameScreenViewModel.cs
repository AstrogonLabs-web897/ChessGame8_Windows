using ChessGame8.Core;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static  ChessGame8.UI.GameScreen2;
using wpfMessageBox = System.Windows.MessageBox;

namespace ChessGame8.UI
{
    public class GameScreenViewModel : INotifyPropertyChanged
    {
        #region Fields
        // Семафора для синхронизации доступа
        public  readonly SemaphoreSlim _loadSemaphore = new SemaphoreSlim(1, 1);
        public  readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        // Булевый флаг для загрузки доски
        public bool _isLoadingBoard;
        // Булевый флаг для завершения игры
        public bool _isGameOver;
        // Флаг готовности хода
        public  volatile bool _isMoveReady;
        // Шахматная доска
        public ChessBoard _board;
        // наш AI
        private SmartAI _smartAI;
        private SmartAI _AI;
        // наш Game Controller
        private GameController _gameController;
        public CurrentPlayer currentPlayer; //наш класс CurrentPlayer                           
        private Player _currentPlayer; // Текущий игрок                   
        private DispatcherTimer _timer; // Источник таймера
        private TimeSpan _remainingTime;// Оставшееся время на ход
        // наши списки 
        public List<Player> Players { get;  set; }
        private List<Move> _movesHistory = new List<Move>(); // История ходов
        public  CancellationTokenSource _cts; // Cancelation token source
        // наши коллекции 
        private ObservableCollection<FigureViewModel> _figuresViewModel;
        public ObservableCollection<BoardSquare> BoardSquares { get; } = new();
        public ObservableCollection<PieceViewModel> Pieces { get; } = new();

        public GameModeEnum _gameMode = GameModeEnum.SinglePlayerVsComputer; // Режим игры (по умолчанию игрок против компьютера)
        public Canvas ChessCanvas2 { get; set; }
        #endregion
        #region Events
        // Событие завершения хода игрока
        public event EventHandler<PlayerMoveEventArgs> PlayerMoveCompleted;

        // Вспомогательный метод уведомления о событии
        public void RaisePlayerMoveCompleted(Move move)
        {
            PlayerMoveCompleted?.Invoke(this, new PlayerMoveEventArgs(move));
        }
        // Аргументы события для завершения хода игрока
        public class PlayerMoveEventArgs : EventArgs
        {
            public Move Move { get; }

            public PlayerMoveEventArgs(Move move)
            {
                Move = move;
            }
        }
        #endregion
        #region Properties
        // Признак готовности хода
        public bool IsMoveReady
        {
            get => _isMoveReady;
            set
            {
                _isMoveReady = value;
                OnPropertyChanged(nameof(IsMoveReady));
            }
        }
        public ObservableCollection<FigureViewModel> Figures
        {
            get => _figuresViewModel;
            private set 
            {
                _figuresViewModel = value;
                OnPropertyChanged(nameof(Figures));
            }
        }
        // Доска
        public ChessBoard Board
        {
            get => _board;
            private set
            {
                _board = value;
                OnPropertyChanged(nameof(Board));
            }
        }
        // Следующий игрок
        public Player NextPlayer
        {
            get
            {
                var index = Players.IndexOf(CurrentPlayer1);
                return Players[(index + 1) % Players.Count];
            }
        }

        // Текущий игрок
        public Player CurrentPlayer1
        {
            get => _currentPlayer;
            private set
            {
                _currentPlayer = value;
                OnPropertyChanged(nameof(CurrentPlayer1));
            }
        }
        public List<Move> MovesHistory
        {
            get => _movesHistory;
            set
            {
                _movesHistory = value;
                OnPropertyChanged(nameof(MovesHistory));
            }
        }
        // Признак загрузки доски
        public bool IsLoadingBoard
        {
            get => _isLoadingBoard;
            set
            {
                _isLoadingBoard = value;
                OnPropertyChanged(nameof(IsLoadingBoard));
            }
        }

        // Признак конца игры
        public bool IsGameOver
        {
            get => _isGameOver;
            set
            {
                _isGameOver = value;
                OnPropertyChanged(nameof(IsGameOver));
            }
        }

        public GameModeEnum GameMode1
        {
            get => _gameMode;
            set
            {
                _gameMode = value;
                OnPropertyChanged(nameof(GameMode1));
                LoadBoard(); // Перезагрузка доски при смене режима
            }
        }
        #endregion

        #region Commands

        // Команда старта игры
        public ICommand StartCommand { get; }

        // Команда завершения игры
        public ICommand ExitCommand { get; }

        #endregion

        #region Constructor
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        public GameScreenViewModel()
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        {
            // Инициализация доски
            _board = new ChessBoard();

            // Инициализация фигур
            Figures = new ObservableCollection<FigureViewModel>(_board.GetFigures().Select(f => new FigureViewModel(f)));

            // Переключаем черный цвет на белый и передаем в конструктор текущего игрока
            currentPlayer = new CurrentPlayer((PlayerColor)(int)ChessGame8.Core.ColorSwitcher.SwitchColor(ChessGame8.Core.Color.Black));


            // Инициализация cancelation token
            _cts = new CancellationTokenSource();
            // Инициализация команд
            StartCommand = new RelayCommand(StartGame);
            ExitCommand = new RelayCommand(ExitGame);
        }
        #endregion
        #region Methods
        // Метод проверки окончания игры
        public bool CheckIfGameIsOver()
        {
            var targetUiColor = (GameScreen2.PlayerColor)(int)_currentPlayer.Color;
            // Проверка мата
            if (_gameController.IsCheckmate(targetUiColor))
            {
                IsGameOver = true;
                return true;
            }

            // Проверка пата
            if (_gameController.NoLegalMoves())
            {
                IsGameOver = true;
                return true;
            }

            // Проверка по времени
            if (_remainingTime <= TimeSpan.Zero)
            {
                IsGameOver = true;
                return true;
            }

            // Проверка по количеству ходов (например, правило 50 ходов)
            if (_gameController.FiftyMoveRule())
            {
                IsGameOver = true;
                return true;
            }

            // Проверка недостатка фигур для победы
            if (_gameController.InsufficientMaterial())
            {
                IsGameOver = true;
                return true;
            }

            return false;
        }

        //  Асинхронная загрузка доски
        public async Task LoadBoardAsync()
        {
            // Устанавливаем флаг загрузки
            IsLoadingBoard = true;

            // Запрашиваем разрешение на доступ
            await _loadSemaphore.WaitAsync();

            try
            {
                // Имитация длительной операции
                await Task.Delay(2000, _cts.Token);

                // Загружаем доску
                Board = new ChessBoard();
                DoSomething();

                // Сбрасываем флаг загрузки
                IsLoadingBoard = false;
            }
            finally
            {
                // Освобождаем семафор
                _loadSemaphore.Release();
            }
        }
        // Асинхронный показ конца игры
        public async Task ShowGameOverAsync()
        {
            // Устанавливаем флаг конца игры
            IsGameOver = true;

            // Имитация задержки
            await Task.Delay(1000, _cts.Token);

            // Сообщение о конце игры
            wpfMessageBox.Show("Игра окончена!", "Шахматы", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Старт игры
        private void StartGame()
        {
            // Запускаем асинхронную загрузку доски

#pragma warning disable CS4014 // Так как этот вызов не ожидается, выполнение существующего метода продолжается до тех пор, пока вызов не будет завершен
            LoadBoardAsync();

            // Запускаем игровой цикл
            StartGameLoopAsync();
        }

        // Выход из игры
        private void ExitGame()
        {
            // Отмена текущих операций
            _cts.Cancel();
        }

        // Метод для автоматической загрузки доски
        public void DoSomething()
        {
            LoadBoard();
        }
        public void LoadBoard()
        {
            // Очищаем существующие фигуры
            Pieces.Clear();
            // Создаем квадраты доски
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var square = new BoardSquare
                    {
                        Left = x * 60,
                        Top = y * 60,
                        Fill = (x + y) % 2 == 0 ? Brushes.LightGray : Brushes.Gray
                    };
                    BoardSquares.Add(square);
                }
            }
            // Добавляем фигуры на доску
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var piece = _board.GetPieceAt(x, y);
                    if (piece != null)
                    {
                        // Формируем правильный URI для изображения фигуры
                        string imagePath = $"{(piece.IsWhite() ? "w" : "b")}{piece.Type}.png";

                        var pieceVm = new PieceViewModel
                        {
                            Left = x * 60,
                            Top = y * 60,
                            ImageSource = new BitmapImage(new Uri($"pack://application:,,,/res/images/{imagePath}", UriKind.Absolute))
                        };
                        Pieces.Add(pieceVm);
                    }
                    else
                    {
                        // Добавляем пустую клетку (если нужно)
                        var emptyPieceVm = new PieceViewModel
                        {
                            Left = x * 60,
                            Top = y * 60,
                            ImageSource = new BitmapImage(new Uri("pack://application:,,,/res/images/Empty.png", UriKind.Absolute))
                        };
                        Pieces.Add(emptyPieceVm);
                    }
                }
            }
            // Если игра против компьютера, создаем AI
            if (GameMode1 == GameModeEnum.SinglePlayerVsComputer)
            {
                CreateAI();
            }
        }
        public void CreateAI()
        {
            PlayerColor aiColor = currentPlayer.Color == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;

            _smartAI = new SmartAI(_board, aiColor);


            // Начальное состояние
            _isPlayersTurn = true;

            // Запускаем игровой цикл
            StartGameLoopAsync();
        }

        public bool _isPlayersTurn;

        // Игровой цикл
        public async void StartGameLoopAsync()
        {
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    if (_isPlayersTurn == true) 
                    { 
                        // Ждем хода игрока
                        await WaitForPlayerMoveAsync();

                    }

                    else
                    {
                        // Проверяем, чей ход
                        if (currentPlayer.Color == PlayerColor.Black && _gameMode == GameModeEnum.SinglePlayerVsComputer)
                        {
                        // Делаем ход AI
                        await MakeAIMoveAsync();
                        }
                        // Переключаем ход
                        _isPlayersTurn = !_isPlayersTurn;
                    }
                    
                    // Переключаем игрока
                    currentPlayer.SwitchPlayer();
                }
            }
            catch (OperationCanceledException) { }
        }

        // Ожидание хода игрока
        public async Task WaitForPlayerMoveAsync()
        {
            // Здесь можно добавить логику ожидания реального хода игрока
            // Например, через событие или флаг
            // Ждем разрешения
            await Task.Delay(TimeSpan.FromSeconds(1));
            await _semaphore.WaitAsync(_cts.Token);
            try
            {
                // Сбрасываем флаг готовности хода
                IsMoveReady = false;
                // Запускаем функцию, отвечающую за ход игрока
                var move = await GetPlayerMoveAsync();

                // Выполняем ход на доске
                ExecuteMove3(move);

                // Обновляем доску
                DisplayBoard2(_board);
                //Переключаем флаг готовности хода 
                IsMoveReady = !IsMoveReady;
            }
            finally
            {
                // Освобождаем семафор
                NotifyPlayerMoveCompleted();
            }

        }
        public async Task<Move> GetPlayerMoveAsync()
        {
            // Создаем TaskCompletionSource для ожидания хода игрока
            var tcs = new TaskCompletionSource<Move>();

            // Подписываемся на событие завершения хода игрока
            EventHandler<PlayerMoveEventArgs>? handler = null;
            handler = (sender, args) =>
            {
                // Отписываемся от события
                PlayerMoveCompleted -= handler;

                // Завершаем задачу с результатом
                tcs.TrySetResult(args.Move);
            };
            PlayerMoveCompleted += handler;

            // Ждем завершения хода игрока
            return await tcs.Task;
        }
       private async Task ProcessPlayerMoveAsync(MouseButtonEventArgs e)
        {
            // Обработка хода игрока
            var move = await GetPlayerMoveFromEvent(e);

            // Выполняем ход на доске
#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
            ExecuteMove3(move);
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.

            // Обновляем UI
            UpdateUI2();
        }
        public async void NextTurn() 
        {
            CurrentPlayer1 = NextPlayer;
            currentPlayer.SwitchPlayer();
        }

        // Метод получения хода из события
        private async Task<Move?> GetPlayerMoveFromEvent(MouseButtonEventArgs e)
        {
            const int CellSize = 60;
            // Логика получения хода из события
            // Например, определение начальной и конечной позиции фигуры
            await Task.Delay(100); // Временная задержка
            // Булевые флаги для отслеживания состояния хода
            bool hasSelectedFigure = false;
            bool canMove = false;
            // Получаем позицию клика относительно канваса
            Point clickPoint = e.GetPosition(ChessCanvas2);

            // Определяем клетку, на которую кликнули
            int clickedColumn = (int)(clickPoint.X / CellSize);
            int clickedRow = (int)(clickPoint.Y / CellSize);

            // Получаем фигуру с текущей позиции
            Figure _selectedFigure = _board.GetFigureAt(clickedColumn, clickedRow);
            // Проверяем, доступна ли фигура для хода
            if (_selectedFigure != null && _selectedFigure.Color == (Core.Color)currentPlayer.Color)
            {
                // Фигура выбрана, помечаем флаг
                hasSelectedFigure = true;
            }
            else if (hasSelectedFigure)
            {
                if (_selectedFigure != null)
                {
                    // Если была выбрана фигура, проверяем возможность хода
                    canMove = _selectedFigure.CanMoveTo(_selectedFigure.Position.X, _selectedFigure.Position.Y, clickedColumn, clickedRow);
                }
                else if (_selectedFigure == null || canMove == false) { return null; }
                    
            }
            // Если ход доступен, создаем объект Move
            if (canMove)
            {
                if (_selectedFigure != null)
                {
                    var move = new Move(_selectedFigure.Position.X, _selectedFigure.Position.Y, clickedColumn, clickedRow);
                    return move;
                }
                else if (_selectedFigure == null || canMove == false) { return null; }
            }
            // Если ход невозможен или фигура не выбрана, возвращаем null
            return null;
        }
        // Обновление доски
        public void DisplayBoard2(ChessBoard board)
        {
            // Обновляем доску
            Board = board;

            // Обновляем UI (если используется UI, например, в WPF-приложениях)
            UpdateUI2();
        }

        // Логика обновления UI
        public void UpdateUI2()
        {
            // Обновляем расположение фигур на доске
            Figures.Clear();
            foreach (var figure in _board.GetFigures())
            {
                Figures.Add(new FigureViewModel(figure));
            }

            // Обновляем историю ходов
            MovesHistory = _movesHistory;
        }
        // Метод выполнения хода на доске
        private void ExecuteMove3(Move move)
        {
            if (move == null) return;
            else
            {
               // Выполняем ход на доске
              _board.ExecuteMove(move.FromX, move.FromY, move.ToX, move.ToY);

              // Добавляем ход в историю
              _movesHistory.Add(move);
            }
            
            // Обновляем доску
            DisplayBoard2(_board);
        }
        // Метод, вызываемый при завершении хода игрока
        public void NotifyPlayerMoveCompleted()
        {
            _semaphore.Release();
        }
        // Ход AI
        public async Task MakeAIMoveAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            // Создаем AI, если его еще нет
            if (_AI == null)
            {
                _smartAI = new SmartAI(_board, currentPlayer.Color);
            }

            // Делаем ход AI
            var move = await Task.Run(() => _smartAI.CalculateBestMove(currentPlayer.Color), _cts.Token);

            // Выполняем ход на доске
            _board.ExecuteMove(move.FromX, move.FromY, move.ToX, move.ToY);

            // Обновляем UI
            DisplayBoard2 (_board);
            OnPropertyChanged(nameof(Pieces));
        }

        // Завершение игры
        public void StopGame()
        {
            _cts.Cancel();
        }
        #endregion

        #region MoreClasses
        public class FigureViewModel(Figure model)
        {
            public Figure Model { get; } = model;

            public int X { get; set; } = model.Position.X;
            public int Y { get; set; } = model.Position.Y;
            public string ImageSource { get; set; } = $"{(model.Color == ( Core.Color)PieceColor.White ? "w" : "b")}{model.Type}.png";
        }
        public class BoardSquare
        {
            public double Left { get; set; }
            public double Top { get; set; }
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
            public Brush Fill { get; set; }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
            public Brush Stroke { get; set; }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
            public double StrokeThickness { get; set; }
        }
        public class PieceViewModel
        {
            public double Left { get; set; }
            public double Top { get; set; }
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
            public BitmapImage ImageSource { get; set; }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        }
        // Пример перечисления режимов игры
            public enum GameMode
            {
               SinglePlayerVsComputer,
               Multiplayer2VsComputer,
               Multiplayer2v2,
               Multiplayer4v4
            }
        #endregion
        #region INotifyPropertyChanged implementation
        // Реализация INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
