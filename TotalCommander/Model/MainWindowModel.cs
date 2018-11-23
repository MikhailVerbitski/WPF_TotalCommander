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

namespace TotalCommander.Model
{
    class MainWindowModel
    {
        public MainWindowModel(ViewModel.MainWindowViewModel ViewModel)
        {
            this.ViewModel = ViewModel;
            ViewModel.KeyButtons = new Tuple<Model.Command, string>[] {
                new Tuple<Model.Command, string>(new Model.Command(Model.FileBrowserManagers.Copy), "F1 - Копировать"),
                new Tuple<Model.Command, string>(new Model.Command(Model.FileBrowserManagers.Transfer), "F2 - Вырезать"),
                new Tuple<Model.Command, string>(new Model.Command(Model.FileBrowserManagers.PasteAsync), "F3 - Вставить"),
                new Tuple<Model.Command, string>(new Model.Command(Model.FileBrowserManagers.Delete), "F4 - Удалить")
            };
            ViewModel.WindowSizeChange = WindowSizeChange;

            ViewModel.FileBrowsers = new View.FileBrowser[] { new View.FileBrowser() };

            ViewModel.PlusMinusButton = new Tuple<Model.Command, string>[] {
                new Tuple<Model.Command, string>(new Command(RemoveFileBrowser), "-"),
                new Tuple<Model.Command, string>(new Command(AddFileBrowser), "+")
            };
            ViewModel.KeyDown = KeyDown;
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
                i.Width = (e.NewSize.Width / ViewModel.FileBrowsers.Length);
        }
        public void KeyDown(object sender, KeyEventArgs e)
        {
            int Key = Convert.ToInt16(e.Key);
            if (Key > 89 && Key < 94)
                ViewModel.KeyButtons[Key - 90].Item1.execute(new object());
        }
    }
}
