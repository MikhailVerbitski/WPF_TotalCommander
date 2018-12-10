using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;

namespace TotalCommander.Model
{
    class FileBrowserManagers
    {
        public FileBrowserManagers()
        {
            CurrentDirectory = new DirectoryInfo(GetSelectedDrive.Name);
        }
        public DriveInfo[] GetComboBoxItems { get { return DriveInfo.GetDrives().Where(a => a.IsReady == true).ToArray();} }
        public DriveInfo GetSelectedDrive { get { return GetComboBoxItems.First(); } }
        public string GetLinq { get { return CurrentDirectory.FullName; } }
        public IEnumerable<Item> GetListItems { get { return Item.GetItems(new DirectoryInfo(GetSelectedDrive.Name)); } }
        public DirectoryInfo CurrentDirectory { get; set; }
        
        private static NotificationManager Alert { get { return TotalCommander.ViewModel.MainWindowViewModel.Alert; } }

        public static async void PasteAsync(bool CopyOrTransfer, IEnumerable<Item> SelectionItemsForCopyOrTransfer, DirectoryInfo Path)
        {
            var result = await Task.Factory.StartNew<bool>(() => Paste(CopyOrTransfer, SelectionItemsForCopyOrTransfer, Path));
            if (result == true)
            {
                Alert.Call("Операция завершена успешно", Colors.Green);
            }
            else
            {
                Alert.Call("Ошибка", Colors.Red);
            }
        }
        private static bool Paste(bool CopyOrTransfer, IEnumerable<Item> SelectionItemsForCopyOrTransfer, DirectoryInfo Path)
        {
            if (SelectionItemsForCopyOrTransfer.Count() == 0)
                return false;
            var path = SelectionItemsForCopyOrTransfer.First().info.FullName.Split('\\');
            Array.Resize(ref path, path.Length - 1);
            string StringLocationWhereSelectedFilesAreLocated = string.Join("\\", path);
            DirectoryInfo DirectoryInfoLocationWhereSelectedFilesAreLocated = new DirectoryInfo(StringLocationWhereSelectedFilesAreLocated);
            var AllSelectionForCopuOrTransfer = FindAll(SelectionItemsForCopyOrTransfer.Select(a => a.info));
            var LastPathWithNewPath = AllSelectionForCopuOrTransfer.Select(a =>
            {
                StringBuilder newPath = new StringBuilder(a.FullName);
                newPath.Replace(StringLocationWhereSelectedFilesAreLocated, Path.FullName);
                while (File.Exists(newPath.ToString()))
                {
                    newPath.Replace(newPath.ToString(), newPath.ToString() + " копия");
                }
                return new Tuple<FileSystemInfo, StringBuilder>(a, newPath);
            });
            try
            {
                if (CopyOrTransfer)
                {
                    foreach (var i in LastPathWithNewPath.OrderBy(b => b.Item2.Length))
                        if (i.Item1 is DirectoryInfo)
                            Directory.CreateDirectory(i.Item2.ToString());
                        else if (i.Item1.FullName != i.Item2.ToString())
                            (i.Item1 as FileInfo).CopyTo(i.Item2.ToString());
                }
                else
                {
                    foreach (var i in LastPathWithNewPath.Where(a => a.Item1 is DirectoryInfo).OrderBy(b => b.Item2.Length))
                        Directory.CreateDirectory(i.Item2.ToString());
                    foreach (var i in LastPathWithNewPath.Where(a => a.Item1 is FileInfo).OrderBy(a => -a.Item2.Length))
                        (i.Item1 as FileInfo).MoveTo(i.Item2.ToString());
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static void Delete(IEnumerable<Item> SelectionItemsForDelete)
        {
            try
            {
                IEnumerable<FileSystemInfo> list = FindAll(SelectionItemsForDelete.Select(a => a.info)).OrderBy(a => -a.FullName.Length);
                string str = list.Last().FullName.Substring(0, list.Last().FullName.LastIndexOf('\\'));
                foreach (var i in list)
                    i.Delete();
            }
            catch (Exception ex)
            {
                Alert.Call(ex.Message, Colors.Red);
            }
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
    }
}
