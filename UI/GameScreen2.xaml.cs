using ChessGame8.Core;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static ChessGame8.UI.GameScreen2;
using wpfMedia = System.Windows.Media;
using wpfMessageBox = System.Windows.MessageBox;
using wpfShapes = System.Windows.Shapes;

namespace ChessGame8.UI
{
    /// <summary>
    /// Логика взаимодействия для GameScreen2.xaml
    /// </summary>
    public partial class GameScreen2 : Page
    {
        #region Fields
        private const int CellSize = 60; // Размер одной клетки

        private const int BoardSize = 8; // Количество клеток на доске
        private ChessGame8.UI.GameScreen2.PlayerColor _turn;
        private bool IsDragging = false; // Флаг перетаскивания фигуры

        private UIElement DraggedElement; // Текущая перетаскиваемая фигура

        private System.Windows.Point OriginalPosition; // Исходная позиция фигуры
        private ChessBoard _board = new ChessBoard(); // Шахматная доска

        private Dictionary<(int, int), Image> _pieces = new Dictionary<(int, int), Image>();//Шахматные фигуры 
        public wpfShapes.Rectangle rectangle;//Новая клетка шахматной доски 

        public CurrentPlayer currentPlayer;
        #region Enums
        public enum GameModeEnum
        {
            Unknown,
            SinglePlayerVsComputer, 
            Multiplayer2VsComputer,
            Multiplayer2v2, 
            Multiplayer4v4
        }
        #endregion
        private Label _playersCountLabel;

        private Label _currentPlayerLabel;

        private Label _turnTimeLabel;

        private Label _gameStatusLabel;
        private TextBlock _currentPlayerInfoBlock;

        private TextBlock _allPlayersInfoBlock;

        private GameController _gameController;

        private SmartAI _engine;
        public List<Player> Players { get; set; } // Список игроков

        public GameModeEnum GameMode { get; set; } //Режимы игры

        private int currentPlayerIndex; // Индекс текущего игрока
                                       
        public Piece? _SelectedPiece; // Переменная для хранения выделенной фигуры

        public  SelectedPiece? _selectedPiece;//Приватное поле для хранения выбранной фигуры
        #endregion
        #region Constructor 
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        public GameScreen2(List<Player> players, GameModeEnum mode )
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
         // оставим "за кадором" Label playersCountLabel, Label currentPlayerLabel, Label turnTimeLabel,
         // Label gameStatusLabel,TextBlock currentPlayerInfoBlock, TextBlock allPlayersInfoBlock
        {
            InitializeComponent();
            // Сохраняем список игроков
            Players = players;
            // Сохраняем режим игры
            GameMode = mode;
            _playersCountLabel = new Label();
            _currentPlayerLabel = new Label();
            _turnTimeLabel = new Label();
            _gameStatusLabel = new Label();
            _currentPlayerInfoBlock = new TextBlock();
            _allPlayersInfoBlock = new TextBlock();
            _gameController = new GameController();
            rectangle = new wpfShapes.Rectangle();
            DraggedElement = new UIElement();
            currentPlayer = new CurrentPlayer(PlayerColor.White);
            // Подписываемся на событие изменения статуса игры
            _gameController.PropertyChanged += GameController_PropertyChanged;
            DrawChessBoard(); // Отображение доски и фигур на экране
            // Дополнительная логика инициализации
            SetupGame();
            Loaded += GameScreen2_Loaded;// Дополнительная логика при загрузке
        }
        #endregion

        #region Methods
        private void GameController_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GameController.GameStatus) ||
                e.PropertyName == nameof(GameController.CurrentPlayer) ||
                e.PropertyName == nameof(GameController.RemainingTime))
            {
                UpdateUI();
            }
        }
        public void UpdateUI()
        {
            // Обновляем индикаторы
            _playersCountLabel.Content = $"Количество игроков: {_gameController.Players.Count}";
            _currentPlayerLabel.Content = $"Текущий игрок: {_gameController.CurrentPlayer.Name}";
            _turnTimeLabel.Content = $"Время на ход: {_gameController.RemainingTime.TotalSeconds:F0} секунд";
            _gameStatusLabel.Content = _gameController.GameStatus;

            // Обновляем информацию о текущем игроке
            _currentPlayerInfoBlock.Text = $"Имя: {_gameController.CurrentPlayer.Name}\n" +
                                          $"Цвет: {_gameController.CurrentPlayer.Color}\n" +
                                          $"Рейтинг: {_gameController.CurrentPlayer.Rating}";

            // Обновляем информацию обо всех игроках
            _allPlayersInfoBlock.Text = string.Join("\n",
                _gameController.Players.Select(p => $"Имя: {p.Name}, Цвет: {p.Color}, Рейтинг: {p.Rating}")
            );
        }
        private void GameScreen2_Loaded(object sender, RoutedEventArgs e)
        {
            // Доступ к UserControl
            GameScreenControl.DoSomthing2(); // Пример метода

            // Или любая другая логика
        }
        private void DrawChessBoard() 
        {
            DisplayBoard(_board);
            AddPieces();
        }
        public void DisplayBoard(ChessBoard _board)
        {
            // Генерируем шахматную доску
            for (int row = 0; row < BoardSize; row++)
            {
                for (int column = 0; column < BoardSize; column++)
                {
                    {
                        Width = CellSize;
                        Height = CellSize;
                        rectangle.Fill = (row + column) % 2 == 0 ? wpfMedia.Brushes.Beige : wpfMedia.Brushes.DarkGray;
                    }
                    ;

                    Canvas.SetLeft(rectangle, column * CellSize);
                    Canvas.SetTop(rectangle, row * CellSize);
                    ChessCanvas.Children.Add(rectangle);
                }
            }
        }
        // Добавьте сюда логику обработки ходов, расстановки фигур и прочее
        private void AddPieces()
        {
            // Ставим фигуры на изначальные позиции
            // Пример расстановки белых фигур
            for (int column = 0; column < BoardSize; column++)
            {
                PlacePiece(column, 1, "010-icon-wPawn(1).png"); // Пешки
                PlacePiece(column, 6, "004-icon-bPawn(1).png");
            }

            PlacePiece(0, 0, "007-icon-wRook(1).png"); // Ладья
            PlacePiece(7, 0, "007-icon-wRook(1).png");
            PlacePiece(1, 0, "008-icon-wKnight(1).png"); // Конь
            PlacePiece(6, 0, "008-icon-wKnight(1).png");
            PlacePiece(2, 0, "009-icon-wBishop(1).png"); // Слон
            PlacePiece(5, 0, "009-icon-wBishop(1).png");
            PlacePiece(3, 0, "012-icon-wQueen(1).png"); // Ферзь
            PlacePiece(4, 0, "006-icon-wKing(1).png"); // Король

            PlacePiece(0, 7, "002-icon-bRook(1).png");
            PlacePiece(7, 7, "002-icon-bRook(1).png");
            PlacePiece(1, 7, "005-icon-bKnight(1).png");
            PlacePiece(6, 7, "005-icon-bKnight(1).png");
            PlacePiece(2, 7, "003-icon-bBishop(1).png");
            PlacePiece(5, 7, "003-icon-bBishop(1).png");
            PlacePiece(3, 7, "011-icon-bQueen(1).png");
            PlacePiece(4, 7, "001-icon-bKing(1).png");

            // Подписываемся на события мыши
            ChessCanvas.MouseDown += ChessCanvas_MouseDown;
            ChessCanvas.MouseMove += ChessCanvas_MouseMove;
            ChessCanvas.MouseUp += ChessCanvas_MouseUp;

            // Подписываемся на события клавиатуры
            ChessCanvas.KeyDown += ChessCanvas_KeyDown;
        }

        private void PlacePiece(int column, int row, string imagePath)
        {
            System.Windows.Controls.Image pieces = new()
            {
                Source = new BitmapImage(new Uri($"pack://application:,,,/res/images/png/{imagePath}")),
                Width = CellSize,
                Height = CellSize
            };

            Canvas.SetLeft(pieces, column * CellSize);
            Canvas.SetTop(pieces, row * CellSize);
            ChessCanvas.Children.Add(pieces);
        }
        private void ChessCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Начинаем перетаскивание фигуры
            if (e.Source is UIElement element)
            {
                IsDragging = true;
                DraggedElement = element;
                OriginalPosition = new System.Windows.Point(Canvas.GetLeft(element), Canvas.GetTop(element));
                ChessCanvas.CaptureMouse();
                // Подсвечиваем возможные ходы
                HighlightAllowedMoves(OriginalPosition);
            }
        }

        private void ChessCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (IsDragging && DraggedElement != null)
            {
                System.Windows.Point position = e.GetPosition(ChessCanvas);
                Canvas.SetLeft(DraggedElement, position.X - CellSize / 2);
                Canvas.SetTop(DraggedElement, position.Y - CellSize / 2);
            }
        }

        private void ChessCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (DraggedElement != null)
            {
                // Окончание перетаскивания
                ChessCanvas.ReleaseMouseCapture();
                IsDragging = false;

                // Возвращаем фигуру на ближайшую клетку
                System.Windows.Point finalPosition = SnapToNearestCell(e.GetPosition(ChessCanvas));
                int originalColumn = (int)(OriginalPosition.X / CellSize);
                int originalRow = (int)(OriginalPosition.Y / CellSize);
                int targetColumn = (int)(finalPosition.X / CellSize);
                int targetRow = (int)(finalPosition.Y / CellSize);

                if (_board.IsLegalMove(originalColumn, originalRow, targetColumn, targetRow))
                {
                    _board.MovePiece(originalColumn, originalRow, targetColumn, targetRow);
                    Canvas.SetLeft(DraggedElement, finalPosition.X);
                    Canvas.SetTop(DraggedElement, finalPosition.Y);
                }
                else
                {
                    Canvas.SetLeft(DraggedElement, OriginalPosition.X);
                    Canvas.SetTop(DraggedElement, OriginalPosition.Y);
                }

                ClearHighlightedCells(); // Снимаем выделение клеток
            }
        }
        private System.Windows.Point SnapToNearestCell(System.Windows.Point point)
        {
            int column = (int)Math.Round(point.X / CellSize);
            int row = (int)Math.Round(point.Y / CellSize);
            return new System.Windows.Point(column * CellSize, row * CellSize);
        }
        private void HighlightAllowedMoves(System.Windows.Point position)
        {
            int column = (int)(position.X / CellSize);
            int row = (int)(position.Y / CellSize);
            var piece = _board.GetPiece(column, row);
            if (piece != null)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    for (int x = 0; x < BoardSize; x++)
                    {
                        if (piece.CanMoveTo(column, row, x, y))
                        {
                            HighlightCell(x, y);
                        }
                    }
                }
            }
        }
        // Обработка нажатия клавиш
        private void ChessCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
            {
                // Определяем текущую позицию фигуры
                int currentColumn = (int)(OriginalPosition.X / CellSize);
                int currentRow = (int)(OriginalPosition.Y / CellSize);

                // Определяем направление движения
                int deltaX = 0, deltaY = 0;
                switch (e.Key)
                {
                    case Key.Up: deltaY = -1; break;
                    case Key.Down: deltaY = 1; break;
                    case Key.Left: deltaX = -1; break;
                    case Key.Right: deltaX = 1; break;
                }

                // Новая позиция
                int targetColumn = currentColumn + deltaX;
                int targetRow = currentRow + deltaY;

                // Проверяем, что ход возможен
                if (_board.IsLegalMove(currentColumn, currentRow, targetColumn, targetRow))
                {
                    _board.MovePiece(currentColumn, currentRow, targetColumn, targetRow);

                    // Обновляем позицию фигуры на канвасе
                    Image pieceImage = _pieces[(currentColumn, currentRow)];
                    Canvas.SetLeft(pieceImage, targetColumn * CellSize);
                    Canvas.SetTop(pieceImage, targetRow * CellSize);

                    // Обновляем внутреннюю структуру
                    _pieces.Remove((currentColumn, currentRow));
                    _pieces[(targetColumn, targetRow)] = pieceImage;
                }

                // Снимаем выделение клеток
                ClearHighlightedCells();
            }
        }
        private void HighlightCell(int x, int y)
        {
            var existingRectangles = ChessCanvas.Children.OfType<wpfShapes.Rectangle>().Where(r => Canvas.GetLeft(r) == x * CellSize && Canvas.GetTop(r) == y * CellSize);
            foreach (var rect in existingRectangles)
            {
                rect.Stroke = wpfMedia.Brushes.Green;
                rect.StrokeThickness = 2;
            }
        }

        private void ClearHighlightedCells()
        {
            foreach (var rect in ChessCanvas.Children.OfType<wpfShapes.Rectangle>())
            {
                rect.Stroke = null;
                rect.StrokeThickness = 0;
            }
        }
        public void SetupGame()
        {
            // Инициализация шахматной доски
            ChessBoard board = new ChessBoard();

            // Размещение фигур на доске
            board.SetupInitialPositions();


            // Установка AI (если выбран режим против компьютера)
            if (GameMode == GameModeEnum.SinglePlayerVsComputer)
            {
                // Получаем текущий цвет Игрока-компьютера в формате PlayerColor
                var aiColor = _turn == PlayerColor.White ? PlayerColor.White : PlayerColor.Black;
                var targetAiColor = aiColor == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
                _engine = new SmartAI(board, targetAiColor);
            }

            // Отображение доски и фигур на экране
            DisplayBoard(board);

            // Начало игры
            StartGame();
        }
        public void StartGame()
        {
            while (!IsGameOver())
            {
                // Ход игрока
                MakePlayerMove();

                // Проверка окончания игры
                if (IsGameOver())
                    break;

                // Ход компьютера (если выбран режим против компьютера)
                if (GameMode == GameModeEnum.SinglePlayerVsComputer)
                {
                    MakeAiMove();
                }

                // Проверка окончания игры
                if (IsGameOver())
                    break;
            }

            // Игра закончена, вызываем метод EndGame
            EndGame();
        }
        public  void MakePlayerMove()
        {
            // Ждём, пока игрок сделает ход
            // Предполагается, что ход игрока обрабатывается через UI (например, клики мышью)
            // Здесь можно добавить логику для обработки кликов и перемещения фигур

            // Пример:
             WaitForPlayerMove();

            // После хода игрока проверяем, не закончился ли матч
            if (IsCheckmate(currentPlayer.Color))
            {
                // Мат игроку, игра закончилась
                EndGame();
            }

            // Меняем активного игрока
            SwitchPlayers();
        }
        public async void WaitForPlayerMove()
        {
            // Ждём, пока игрок сделает ход
            // Предполагается, что ход игрока обрабатывается через UI (например, клики мышью)
            // Здесь можно добавить логику для обработки кликов и перемещения фигур

            // Пример:
            // WaitForPlayerMove() может быть асинхронным методом, который ожидает события от UI
            // Например, можно использовать TaskCompletionSource для ожидания события

            var tcs = new TaskCompletionSource<Move>();
            // Подписываемся на событие, которое сигнализирует о завершении хода игрока
            // Например, через событие MouseUp или MouseDown на канвасе

            // Пример подписки на событие:
            ChessCanvas.MouseUp += (sender, e) =>
            {
                // Обработка хода игрока
                var move = ProcessPlayerMove(e);
                if (move != null)
                {
                    tcs.SetResult(move);

                }
                
            };

            // Ждём завершения хода игрока
            var move1 = await tcs.Task;

            // Создаём объект нужного типа
            var coreMove = new ChessGame8.Core.ChessBoard.Move(move1.FromX, move1.FromY, move1.ToX, move1.ToY);

            // Выполняем ход на доске
            _board.ExecuteMove(move1.FromX, move1.FromY, move1.ToX, move1.ToY);

            // Обновляем UI
            DisplayBoard(_board);
        }

        public  Move? ProcessPlayerMove(MouseButtonEventArgs e)
        {
            // Логика обработки хода игрока
            // Например, определение начальной и конечной позиции фигуры
            // и создание объекта Move
            // Определяем размер клетки
            const int cellSize = 60;

            // Получаем позицию клика относительно канваса
            Point position = e.GetPosition(ChessCanvas);

            // Определяем клетку, на которую кликнул игрок
            int clickedColumn = (int)(position.X / cellSize);
            int clickedRow = (int)(position.Y / cellSize);

            // Определяем, является ли это началом хода (выбор фигуры) или концом хода (перемещение фигуры)
            if (_SelectedPiece == null)
            {
                // Начало хода: игрок выбирает фигуру
#pragma warning disable CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
                Piece piece = GetPieceAt(clickedColumn, clickedRow);
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                if (piece != null && (currentPlayer.IsWhitesTurn() && piece.GetPlayerColor() == Piece.PlayerColor.White) || (currentPlayer.IsBlacksTurn() && piece.GetPlayerColor() == Piece.PlayerColor.Black))
                {
                    // Создаем объект SelectedPiece
                    _selectedPiece = new SelectedPiece(clickedColumn, clickedRow, piece);
                    // Выделяем фигуру (визуально или логически)
                    HighlightPiece(_selectedPiece.Column, _selectedPiece.Row);
                }
            }
            else if (_SelectedPiece != null)
            {


                // Конец хода: игрок перемещает фигуру
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                int fromX = _selectedPiece.Column;
                int fromY = _selectedPiece.Row;
                int toX = clickedColumn;
                int toY = clickedRow;

                // Создаем объект Move
                ChessGame8.UI.Move move = new UI.Move(fromX, fromY, toX, toY);

                // Сбрасываем выделенную фигуру
                DeselectPiece();

                // Возвращаем созданный ход
                return move;
            }
            else
            {
                // Если ход не завершился, возвращаем null
                return null;
            }
            return null;
        }
        public Piece?  GetPieceAt(int column,int row)
        {
            // Логика получения фигуры по координатам
            // Например, из массива фигур на доске
            return _board.GetPieceAt(column, row);
        }
        public void HighlightPiece(int column, int row)
        {
            // Логика выделения фигуры (например, изменением цвета фона клетки)
            // Например, через UI-элементы на канвасе
            // Находим прямоугольник, представляющий клетку на канвасе
            var rectangles = ChessCanvas.Children.OfType<wpfShapes.Rectangle>().Where(r => Canvas.GetLeft(r) == column * CellSize && Canvas.GetTop(r) == row * CellSize);
            foreach (var rect in rectangles)
            {
                // Выделяем клетку зеленой рамкой толщиной 2px
                rect.Stroke = Brushes.Green;
                rect.StrokeThickness = 2;
            }
        }
        public void ClearHighlights()
        {
            // Снимаем выделение со всех клеток
            foreach (var rect in ChessCanvas.Children.OfType<wpfShapes.Rectangle>())
            {
                rect.Stroke = null;
                rect.StrokeThickness = 0;
            }
        }
        public void DeselectPiece()
        {
            // Логика снятия выделения с фигуры
            // Например, восстановление оригинального цвета фона клетки
            _SelectedPiece = null;
        }
        public void MakeAiMove()
        {
            var aiTargetColor = currentPlayer.Color == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
            // Ход компьютера
            ChessGame8.Core.Move aiMove = this._engine.CalculateBestMove(aiTargetColor);

            int fromX = aiMove.FromX;
            int fromY = aiMove.FromY;
            int toX = aiMove.ToX;
            int toY = aiMove.ToY;
            // Выполнение хода на доске
            _board.ExecuteMove(fromX,fromY, toX, toY);

            // Обновление UI
            DisplayBoard(_board);

            // Проверка, не закончился ли матч
            if (IsCheckmate(currentPlayer.Color))
            {
                // Мат компьютеру, игра закончилась
                EndGame();
            }

            // Меняем активного игрока
            SwitchPlayers();
        }
        public void EndGame()
        {
            // Определяем победителя
            string winner = DetermineWinner();

            // Выводим сообщение о победе или поражении
            wpfMessageBox.Show(winner, "Игра окончена", MessageBoxButton.OK, MessageBoxImage.Information);

            // Возвращаемся на домашнюю страницу
            NavigationService.Navigate(new MainWindow());

            // Обработка выхода из игры (ESC)
            if (Keyboard.Modifiers == ModifierKeys.None && Keyboard.IsKeyDown(Key.Escape))
            {
                CloseGame();
            }
        }

        public string DetermineWinner()
        {
            if (IsCheckmate(PlayerColor.White))
                return "Победил чёрный король!";
            else if (IsCheckmate(PlayerColor.Black))
                return "Поздравляем! Победил белый король!";
            else
                return "Ничья.";
        }

        public void CloseGame()
        {
            // Закрываем игру
            Application.Current.Shutdown();
        }
        public bool IsGameOver()
        {
            return IsCheckmate((PlayerColor)PlayerColor.White.ToCoreColor()) || IsCheckmate((PlayerColor)PlayerColor.Black.ToCoreColor());
        }

        public bool IsCheckmate(ChessGame8.UI.GameScreen2.PlayerColor playerColor)
        {
            // Сначала преобразуем PlayerColor в ChessGame8.UI.Color
            var uiColor=playerColor.ToUIColor();
            var Color = (uiColor.UIColorToCoreColor()).ToPlayerColor();
            return _board.IsCheckmate(Color);
        }

        public void SwitchPlayers()
        {
            currentPlayer.SwitchPlayer();
        }
        
        public enum PlayerColor
        {
            White,
            Black,
            None,
            Transparent
        }
    
        public class CurrentPlayer
        {
            // Индекс текущего игрока (0 или 1)
            public int Index { get; private set; }

            // Цвет текущего игрока (White или Black)
            public PlayerColor Color { get; private set; }

            // Конструктор для инициализации текущего игрока
            public CurrentPlayer(PlayerColor startingColor)
            {
                Index = startingColor == PlayerColor.White ? 0 : 1;
                Color = startingColor;
            }

            // Метод для переключения текущего игрока
            public void SwitchPlayer()
            {
                Index = 1 - Index; // Переключаем между 0 и 1
                Color = Color == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
            }

            // Метод для получения противника текущего игрока
            public PlayerColor OpponentColor()
            {
                return Color == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
            }

            // Метод для проверки, является ли текущий игрок белым
            public bool IsWhite()
            {
                return Color == PlayerColor.White;
            }

            // Метод для проверки, является ли текущий игрок черным
            public bool IsBlack()
            {
                return Color == PlayerColor.Black;
            }
            public PlayerColor GetCurrentPlayerColor()
            {
                return Color;
            }

            // Метод для проверки, чей сейчас ход
            public bool IsWhitesTurn()
            {
                return Color == PlayerColor.White;
            }

            // Метод для проверки, чей сейчас ход
            public bool IsBlacksTurn()
            {
                return  Color == PlayerColor.Black;
            }
            // Переопределение ToString для удобной отладки
            public override string ToString()
            {
                return $"CurrentPlayer(Index={Index}, Color={Color})";
            }
        }
        #endregion
        public ChessGame8.UI.PieceColor ConvertToUIColor(GameScreen2.PlayerColor playerColor)
        {
            switch (playerColor)
            {
                case GameScreen2.PlayerColor.White:
                    return ChessGame8.UI.PieceColor.White;
                case GameScreen2.PlayerColor.Black:
                    return ChessGame8.UI.PieceColor.Black;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerColor), "Неизвестный цвет игрока");
            }
        }
        public  class SelectedPiece
        {
            public int Column {  get; set; }
            public int Row { get; set; }
            public Piece Piece { get; set; }
            public SelectedPiece(int column, int row, Piece piece)
            {
                Column = column;
                Row = row;
                Piece = piece;
            }
        }

    }

    public static class ColorExtensions
    {
        public static ChessGame8.Core.Color UIColorToCoreColor(this ChessGame8.UI.PieceColor uiColor)
        {
            switch (uiColor)
            {
                case ChessGame8.UI.PieceColor.White:
                    return ChessGame8.Core.Color.White;
                case ChessGame8.UI.PieceColor.Black:
                    return ChessGame8.Core.Color.Black;
                default:
                    throw new ArgumentOutOfRangeException(nameof(uiColor), "Неизвестный цвет");
            }
        }
        public static ChessGame8.Core.Color ToCoreColor(this PlayerColor playerColor)
        {
            return playerColor == PlayerColor.White ? ChessGame8.Core.Color.White : ChessGame8.Core.Color.Black;
        }
        public static ChessGame8.UI.PieceColor ToUIColor(this ChessGame8.UI.GameScreen2.PlayerColor playerColor)
        {
            switch (playerColor)
            {
                case ChessGame8.UI.GameScreen2.PlayerColor.White:
                    return ChessGame8.UI.PieceColor.White;
                case ChessGame8.UI.GameScreen2.PlayerColor.Black:
                    return ChessGame8.UI.PieceColor.Black;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerColor), "Неизвестный цвет игрока");
            }
        }
        // Преобразование из ChessGame8.Core.Color в ChessGame8.UI.GameScreen2.PlayerColor
        public static ChessGame8.UI.GameScreen2.PlayerColor ToPlayerColor(this ChessGame8.Core.Color coreColor)
        {
            switch (coreColor)
            {
                case ChessGame8.Core.Color.White:
                    return ChessGame8.UI.GameScreen2.PlayerColor.White;
                case ChessGame8.Core.Color.Black:
                    return ChessGame8.UI.GameScreen2.PlayerColor.Black;
                default:
                    throw new ArgumentOutOfRangeException(nameof(coreColor), "Неизвестный цвет");
            }
        }
    }  
}

