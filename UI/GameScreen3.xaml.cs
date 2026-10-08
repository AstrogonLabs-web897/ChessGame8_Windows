using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ChessGame8.Core;
namespace ChessGame8.UI
{
    /// <summary>
    /// Логика взаимодействия для GameScreen3.xaml
    /// </summary>
    public partial class GameScreen3 : UserControl
    {
        private GameScreenViewModel _viewModel;

        // Флаги
        private bool _gameOver = false;

        public CancellationTokenSource _cts; // Cancelation token source
        public GameScreen3()
        {
            InitializeComponent();
            // Создаем модель представления
            _viewModel = new GameScreenViewModel();
            _cts = new CancellationTokenSource();
            // Устанавливаем DataContext
            DataContext = _viewModel;
            // Загружаем доску при загрузке компонента
            Loaded += (sender, args) => _viewModel.LoadBoard();
            DoSomthing2();
        }
        public async  void DoSomthing2() 
        {
            // Загружаем шахматную доску
            await _viewModel.LoadBoardAsync();

            // Запускаем игровой процесс
            await StartingGameLoopAsync();
        }
        public async Task StartingGameLoopAsync() 
        {
            while (!_gameOver)
            {
                await _viewModel.WaitForPlayerMoveAsync();

                // Проверяем, не закончилась ли игра
                if (_viewModel.CheckIfGameIsOver())
                {
                    break;
                }

                // Ход AI (если игра против компьютера)
                if (_viewModel.GameMode1 == GameModeEnum.SinglePlayerVsComputer)
                {
                    await _viewModel.MakeAIMoveAsync();
                }

                // Проверяем, не закончилась ли игра
                if (_viewModel.CheckIfGameIsOver())
                {
                    break;
                }
                // Переключаем ход
                _viewModel.NextTurn();
            }
            // Игра закончилась
            await ShowGameOverAsync();
        }
        public async Task ShowGameOverAsync() 
        {
            // Логика показа сообщения
            // Устанавливаем флаг конца игры
            _viewModel.IsGameOver = true;

            // Имитация задержки
            await Task.Delay(1000, _cts.Token);

            // Сообщение о конце игры
            MessageBox.Show("Игра окончена!", "Шахматы", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}


