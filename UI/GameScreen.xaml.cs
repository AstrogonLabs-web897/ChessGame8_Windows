using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
namespace ChessGame8.UI
{
    /// <summary>
    /// Логика взаимодействия для GameScreen.xaml
    /// </summary>
    public partial class GameScreen : Page
    {
        private List<Player> _players;
        private GameModeEnum _gameMode;
        private readonly DispatcherTimer timer;
        private int progressValue = 0;
        public GameScreen(List<Player> players, GameModeEnum gameMode)
        {
            InitializeComponent();
            _players = players;
            _gameMode = gameMode; 
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50) // каждые 50 миллисекунд увеличиваем прогресс
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (sender == null)
            {
                Debug.WriteLine("Sender is unexpectedly null in Timer_Tick method.");
                return;
            }
            else
            {
                // Увеличиваем значение прогресса
                progressValue++;
                if (progressValue >= 100)
                {
                    // останавливаем таймер и переходим на следующую страницу
                    timer.Stop();
                    NavigationService.Navigate(new GameScreen2(_players, (GameScreen2.GameModeEnum)_gameMode));
                }
                else
                {
                    myProgressBar.Value = progressValue;
                }
            }
        }
    }
}
