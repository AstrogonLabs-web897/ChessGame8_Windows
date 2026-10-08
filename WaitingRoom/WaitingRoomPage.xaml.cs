using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace ChessGame8.WaitingRoom
{
    /// <summary>
    /// Логика взаимодействия для WaitingRoomPage.xaml
    /// </summary>
    public partial class WaitingRoomPage : Page
    {
        private readonly DispatcherTimer timer;
        private int progressValue = 0;
        private readonly TimeSpan Interval;
        public WaitingRoomPage()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            {
                Interval = TimeSpan.FromMilliseconds(50); // интервал обновления в миллисекундах
            }
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick([AllowNull] object sender, EventArgs e)
        {
            // Увеличиваем значение прогрессии
            progressValue++;
            if (progressValue > 100)
            {
                progressValue = 0; // цикл с 0 до 100
            }
            myProgressBar.Value = progressValue;
        }
        // Очищаем таймер при закрытии страницы
        private void UnloadPage(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }
        // Логика завершения ожидания и перехода на другую страницу
        private void ExitWaitingRoom_Click(object sender, RoutedEventArgs e)
        {
            // Остановим таймер
            timer.Stop();

            // Возвращаемся к предыдущей странице
            NavigationService.GoBack();
        }
#pragma warning disable IDE0301
        private static readonly object[] args = Array.Empty<object>();

        // Автоматическое перенаправление на страницу игры через некоторое время
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Через 5 секунд перенаправляем на главную страницу игры
            Dispatcher.BeginInvoke(
                new Action(() => NavigationService.Navigate(new MainWindow())),
                System.Windows.Threading.DispatcherPriority.Background,
                args);
        }
    }
}
