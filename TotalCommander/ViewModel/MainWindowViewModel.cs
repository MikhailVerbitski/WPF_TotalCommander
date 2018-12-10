using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TotalCommander.Model;

namespace TotalCommander.ViewModel
{
    class MainWindowViewModel : Model.NotifyPropertyChanged
    {
        public MainWindowViewModel(Window window)
        {
            this.window = window;
            Alert = new TotalCommander.Model.NotificationManager(this);

            FileBrowsers = new List<View.FileBrowser> { new View.FileBrowser() };
            PlusMinusButton = new Tuple<Command, string>[] {
                new Tuple<Command, string>(new Command(RemoveFileBrowser), "-"),
                new Tuple<Command, string>(new Command(AddFileBrowser), "+")
                };

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    NumberOfProcesses = $"Число потоков: {Process.GetCurrentProcess().Threads.Count.ToString()}";
                    Thread.Sleep(100);
                }
            });
        }
        public void AddFileBrowser(object sender)
        {
            FileBrowsers = FileBrowsers.Concat(new[] { new View.FileBrowser() }).ToList();

            window.Width = window.Width + 1;
            window.Width = window.Width - 1;
        }
        public void RemoveFileBrowser(object sender)
        {
            if (FileBrowsers.Count == 1)
            {
                TotalCommander.ViewModel.MainWindowViewModel.Alert.Call("Нельзя убрать последний ряд", Colors.Orange);
                return;
            }
            FileBrowserViewModel.AllFileBrowserViewModel.Remove((FileBrowsers[FileBrowsers.Count - 1].DataContext as FileBrowserViewModel));
            (FileBrowsers[FileBrowsers.Count - 1].DataContext as FileBrowserViewModel).Dispose();
            FileBrowsers = FileBrowsers.Take(FileBrowsers.Count - 1).ToList();

            window.Width = window.Width + 1;
            window.Width = window.Width - 1;
        }
        public Window window;
        public static Model.NotificationManager Alert { get; private set; }

        public List<View.FileBrowser> FileBrowsers
        {
            get { return fileBrowsers; }
            set
            {
                fileBrowsers = value;
                OnPropertyChanged("FileBrowsers");
            }
        }
        private List<View.FileBrowser> fileBrowsers;

        public Tuple<Model.Command, string>[] PlusMinusButton
        {
            get { return plusMinusButton; }
            set
            {
                plusMinusButton = value;
                OnPropertyChanged("PlusMinusButton");
            }
        }
        private Tuple<Model.Command, string>[] plusMinusButton;

        public string NotificationText
        {
            get { return notificationText; }
            set
            {
                notificationText = value;
                OnPropertyChanged("NotificationText");
            }
        }
        private string notificationText;

        public Brush NotificationBackground
        {
            get { return notificationBackground; }
            set
            {
                notificationBackground = value;
                OnPropertyChanged("NotificationBackground");
            }
        }
        private Brush notificationBackground;

        public string NumberOfProcesses
        {
            get { return numberOfProcesses; }
            set
            {
                numberOfProcesses = value;
                OnPropertyChanged("NumberOfProcesses");
            }
        }
        private string numberOfProcesses;
        
        public MenuItem[] MenuItemsCopyCutPasteDelete
        {
            get { return menuItemsCopyCutPasteDelete; }
            set
            {
                menuItemsCopyCutPasteDelete = value;
                OnPropertyChanged("MenuItemsCopyCutPasteDelete");
            }
        }
        private MenuItem[] menuItemsCopyCutPasteDelete = new MenuItem[] { };
        
        public void WindowSizeChange(object sender, SizeChangedEventArgs e)
        {
            foreach (var i in FileBrowsers)
            {
                i.Width = (e.NewSize.Width / FileBrowsers.Count);
            }
        }
    }
}
