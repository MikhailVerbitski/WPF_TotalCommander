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
using System.IO.Compression;

namespace TotalCommander.Model
{
    class FileBrowserManagers
    {
        public FileBrowserManagers(ViewModel.FileBrowserViewModel ViewModel)
        {
            this.ViewModel = ViewModel;
            Managercs.Add(this);

            ViewModel.ComboBoxItems = DriveInfo.GetDrives().Where(a => a.IsReady == true).ToArray();
            ViewModel.SelectedDrive = ViewModel.ComboBoxItems.First();
            CurrentDirectory = new DirectoryInfo(ViewModel.ComboBoxItems.First().Name);
            ViewModel.Linq = CurrentDirectory.FullName;
            ViewModel.ListItems = Item.GetItems(new DirectoryInfo(ViewModel.SelectedDrive.Name));

            ViewModel.Selected = Selected;
            ViewModel.DoubleClick = DoubleClick;
            ViewModel.SelectionItems = SelectionItems;
        }
        public ViewModel.FileBrowserViewModel ViewModel { get; set; }
        public DirectoryInfo CurrentDirectory { get; set; }

        public void Selected(object sender, EventArgs e)
        {
            ViewModel.ListItems = Item.GetItems(new DirectoryInfo(((sender as ComboBox).SelectedItem as DriveInfo).Name));
            var drive = (sender as ComboBox).SelectedItem as DriveInfo;
            ViewModel.TextNearComboBox = $"{AdapterNumber.ToAdaptNumber(drive.TotalFreeSpace / 1024 / 1024)}Mбайт из {AdapterNumber.ToAdaptNumber(drive.TotalSize / 1024 / 1024)}Mбайт";
            ViewModel.Linq = drive.Name;
            CurrentDirectory = new DirectoryInfo(drive.Name);
        }
        public void DoubleClick(object sender, EventArgs e)
        {
            if ((sender as ListView).SelectedItem == null)
                return;
            var selected = ((sender as ListView).SelectedItem as Item).info;
            try
            {
                if (selected is DirectoryInfo)
                {
                    ViewModel.ListItems = Item.GetItems(selected as DirectoryInfo);
                    CurrentDirectory = selected as DirectoryInfo;
                    ViewModel.Linq = CurrentDirectory.FullName;
                }
                else
                {
                    var a = new System.Diagnostics.Process();
                    a.StartInfo.FileName = selected.FullName;
                    a.StartInfo.UseShellExecute = true;
                    a.Start();
                }
            }
            catch(Exception ex)
            {
                TotalCommander.ViewModel.MainWindowViewModel.Alert.Call(ex.Message, Colors.Red);
            }
        }
        public void SelectionItems(object sender, EventArgs e)
        {
            BufSelectedItems.Clear();
            foreach (Item i in (sender as ListView).SelectedItems)
                BufSelectedItems.Add(i);
            foreach (var i in Managercs.Where(a => !a.Equals(this)))
                i.ViewModel.SelectedItem = null;
            SelectedFileBrowserManagers = this;
        }
        private List<Item> BufSelectedItems = new List<Item>();

        public static IEnumerable<FileSystemInfo> SelectedItems;
        public static bool CopyOrTransfer;
        public static List<FileBrowserManagers> Managercs = new List<FileBrowserManagers>();
        public static FileBrowserManagers SelectedFileBrowserManagers;
        private static NotificationManager Alert { get { return TotalCommander.ViewModel.MainWindowViewModel.Alert; } }

        public static void CommandSelectedItems()
        {
            SelectedItems = FindAll(SelectedFileBrowserManagers.BufSelectedItems.Select(a => a.info)).ToArray();
        }
        public static void Copy(object obj)
        {
            CommandSelectedItems();
            CopyOrTransfer = true;
        }
        public static void Transfer(object obj)
        {
            CommandSelectedItems();
            CopyOrTransfer = false;
        }
        public static async void PasteAsync(object obj)
        {
            var result = await Task.Factory.StartNew<bool>(() => Paste(obj));
            if (result == true)
            {
                Alert.Call("Операция завершена успешно", Colors.Green);
            }
        }
        public static bool Paste(object obj)
        {
            if (SelectedFileBrowserManagers.BufSelectedItems.Count > 1 || SelectedFileBrowserManagers.BufSelectedItems.Count == 0)
            {
                Alert.Call("Выбирите одну папку", Colors.Red);
                return false;
            }
            DirectoryInfo destination = (SelectedFileBrowserManagers.BufSelectedItems.Count == 0) 
                ? destination = SelectedFileBrowserManagers.CurrentDirectory
                : destination = SelectedFileBrowserManagers.BufSelectedItems.Single().info as DirectoryInfo;
            if(SelectedFileBrowserManagers.BufSelectedItems.Single().info is FileInfo)
            {
                var str = SelectedFileBrowserManagers.BufSelectedItems.Single().info.FullName.Split('\\');
                Array.Resize(ref str, str.Length - 1);
                destination = new DirectoryInfo(string.Concat(str));
            }
                
            string SoursStr = destination.FullName;
            var buf = SelectedItems.Select(b => new Tuple<FileSystemInfo, StringBuilder>(b, new StringBuilder(b.FullName))).ToArray();

            var Linqs = buf.Select(a => a.Item1.FullName.Split('\\').ToList()).ToList();
            while(Linqs.All(a => a.Count != 0) && Linqs.All(a => a.Count > 1))
                if (Linqs.All(a => a[0] == Linqs.First()[0]))
                    foreach (var i in Linqs)
                        i.RemoveAt(0);
                else
                    break;
            for (int i = 0; i < Linqs.Count; i++)
                buf[i].Item2.Replace(buf[i].Item2.ToString(), destination.FullName + '\\' + Linqs[i].Aggregate((a, b) => a + '\\' + b));

            foreach (var i in buf)
                i.Item2.Replace(i.Item2.ToString(), CreateAvailableName(i.Item2.ToString(), i.Item1 is FileInfo));
            try
            {
                if (CopyOrTransfer)
                {
                    foreach (var i in buf.OrderBy(b => b.Item2.Length))
                        if (i.Item1 is DirectoryInfo)
                            Directory.CreateDirectory(i.Item2.ToString());
                        else if (i.Item1.FullName != i.Item2.ToString())
                            (i.Item1 as FileInfo).CopyTo(i.Item2.ToString());
                }
                else
                {
                    foreach (var i in buf.Where(a => a.Item1 is DirectoryInfo).OrderBy(b => b.Item2.Length))
                        Directory.CreateDirectory(i.Item2.ToString());
                    foreach (var i in buf.Where(a => a.Item1 is FileInfo).OrderBy(a => -a.Item2.Length))
                        (i.Item1 as FileInfo).MoveTo(i.Item2.ToString());
                }
            }
            catch(Exception ex)
            {
                Alert.Call(ex.Message, Colors.Red);
                foreach (var i in Managercs)
                {
                    i.ViewModel.ListItems = Item.GetItems(i.CurrentDirectory);
                }
                return false;
            }
            foreach (var i in Managercs)
            {
                i.ViewModel.ListItems = Item.GetItems(i.CurrentDirectory);
            }
            return true;
        }
        public static void Delete(object obj)
        {
            try
            {
                IEnumerable<FileSystemInfo> list = FindAll(SelectedFileBrowserManagers.BufSelectedItems.Select(a => a.info)).OrderBy(a => -a.FullName.Length);
                string str = list.Last().FullName.Substring(0, list.Last().FullName.LastIndexOf('\\'));
                foreach (var i in list)
                    i.Delete();
            }
            catch (Exception ex)
            {
                Alert.Call(ex.Message, Colors.Red);
            }
            SelectedFileBrowserManagers.ViewModel.ListItems = Item.GetItems(SelectedFileBrowserManagers.CurrentDirectory);
        }
        
        private static IEnumerable<FileSystemInfo> FindAll(IEnumerable<FileSystemInfo> list)
        {
            return (list.Where(a => a is DirectoryInfo).Count() > 0)
                 ? list.Concat(FindAll(list.Where(a => a is DirectoryInfo)
                        .Select(a => (a as DirectoryInfo)
                        .GetFiles().Concat((a as DirectoryInfo).GetDirectories().Select(b => b as FileSystemInfo)))
                        .Aggregate((a, b) => a.Concat(b))))
                : list;
        }
        private static string CreateAvailableName(string name, bool IsFile = true)
        {
            FileSystemInfo file = (IsFile) ? new FileInfo(name) as FileSystemInfo : new DirectoryInfo(name);
            if (file.Exists)
            {
                var str = name.Split('.');
                string last = str[str.Length - 1];
                Array.Resize(ref str, str.Length - 1);
                name = string.Concat(str) + " Копия" + "." + last;
                name = CreateAvailableName(name, IsFile);
            }
            return name;
        }
    }
}
