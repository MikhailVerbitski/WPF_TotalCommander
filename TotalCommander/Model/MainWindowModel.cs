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
using System.Diagnostics;
using System.Threading;

namespace TotalCommander.Model
{
    class MainWindowModel
    {
        public MainWindowModel(ViewModel.MainWindowViewModel ViewModel)
        {
            this.ViewModel = ViewModel;
            ViewModel.KeyButtons = new Tuple<Model.Command, string>[] {
                new Tuple<Model.Command, string>(new Model.Command(Model.FileBrowserManagers.Copy), "Копировать"),
                new Tuple<Model.Command, string>(new Model.Command(Model.FileBrowserManagers.Transfer), "Вырезать"),
                new Tuple<Model.Command, string>(new Model.Command(Model.FileBrowserManagers.PasteAsync), "Вставить"),
                new Tuple<Model.Command, string>(new Model.Command(Model.FileBrowserManagers.Delete), "Удалить")
            };
            ViewModel.WindowSizeChange = WindowSizeChange;

            ViewModel.FileBrowsers = new View.FileBrowser[] { new View.FileBrowser() };

            ViewModel.PlusMinusButton = new Tuple<Model.Command, string>[] {
                new Tuple<Model.Command, string>(new Command(RemoveFileBrowser), "-"),
                new Tuple<Model.Command, string>(new Command(AddFileBrowser), "+")
            };
            
            Task.Factory.StartNew(() =>
            {
                while(true)
                {
                    ViewModel.NumberOfProcesses = $"Число процессов: {Process.GetCurrentProcess().Threads.Count.ToString()}";
                    Thread.Sleep(100);
                }
            });
        }
        private ViewModel.MainWindowViewModel ViewModel;

        public void AddFileBrowser(object sender)
        {
            View.FileBrowser[] buf = ViewModel.FileBrowsers;
            Array.Resize(ref buf, ViewModel.FileBrowsers.Length + 1);
            buf[buf.Length - 1] = new View.FileBrowser();
            ViewModel.FileBrowsers = buf;

            ViewModel.window.Width = ViewModel.window.Width + 1;
            ViewModel.window.Width = ViewModel.window.Width - 1;
        }
        public void RemoveFileBrowser(object sender)
        {
            if (ViewModel.FileBrowsers.Length == 1)
            {
                TotalCommander.ViewModel.MainWindowViewModel.Alert.Call("Нельзя убрать последний ряд", Colors.Orange);
                return;
            }
            View.FileBrowser[] buf = ViewModel.FileBrowsers;
            Model.FileBrowserManagers.Managercs.Remove((buf[buf.Length - 1].DataContext as ViewModel.FileBrowserViewModel).Model);
            Array.Resize(ref buf, ViewModel.FileBrowsers.Length - 1);
            ViewModel.FileBrowsers = buf;

            ViewModel.window.Width = ViewModel.window.Width + 1;
            ViewModel.window.Width = ViewModel.window.Width - 1;
        }
        public void WindowSizeChange(object sender, SizeChangedEventArgs e)
        {
            foreach (var i in ViewModel.FileBrowsers)
            {
                i.Width = (e.NewSize.Width / ViewModel.FileBrowsers.Length);
            }
        }
    }
}
