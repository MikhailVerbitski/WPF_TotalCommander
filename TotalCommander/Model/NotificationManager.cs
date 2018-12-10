using System;
using System.Windows.Media;
using System.Windows.Threading;

namespace TotalCommander.Model
{
    class NotificationManager
    {
        public NotificationManager(ViewModel.MainWindowViewModel ViewModel)
        {
            this.ViewModel = ViewModel;
        }
        public TotalCommander.ViewModel.MainWindowViewModel ViewModel;
        private DispatcherTimer Timer = null;

        public void Call(string Text, Color color)
        {
            ViewModel.NotificationText = Text;
            ViewModel.NotificationBackground = new SolidColorBrush(color);

            if (Timer != null && Timer.IsEnabled)
                Timer.Stop();
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 2);
            Timer.Tick += (a, b) => { ViewModel.NotificationText = ""; ViewModel.NotificationBackground = new SolidColorBrush(Colors.White); };
            Timer.Start();
        }
    }
}
