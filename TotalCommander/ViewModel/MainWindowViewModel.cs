using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace TotalCommander.ViewModel
{
    class MainWindowViewModel : Model.NotifyPropertyChanged
    {
        public MainWindowViewModel(Window window)
        {
            this.window = window;
            Model = new Model.MainWindowModel(this);
            Alert = new TotalCommander.Model.NotificationManager(this);
        }
        private Model.MainWindowModel Model;
        public Window window;
        public static Model.NotificationManager Alert { get; private set; }

        public View.FileBrowser[] FileBrowsers
        {
            get { return fileBrowsers; }
            set
            {
                fileBrowsers = value;
                OnPropertyChanged("FileBrowsers");
            }
        }
        private View.FileBrowser[] fileBrowsers;

        public Tuple<Model.Command, string>[] KeyButtons
        {
            get { return keyButtons; }
            set
            {
                keyButtons = value;
                OnPropertyChanged("KeyButtons");
            }
        }
        private Tuple<Model.Command, string>[] keyButtons;

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

        public Action<object, SizeChangedEventArgs> WindowSizeChange;
    }
}
