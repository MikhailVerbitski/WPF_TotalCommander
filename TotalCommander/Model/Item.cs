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
    class Item
    {
        public FileSystemInfo info { get; set; }
        public string Size { get { return (info is FileInfo) ? AdapterNumber.ToAdaptNumber((info as FileInfo).Length) + " байт" : ""; } set { } }
        public string Type { get { return (info is FileInfo) ? "Файл" : "Папка"; } set { } }
        public string Name { get { return (name == null) ? info.Name : name; } set { name = value; } }
        private string name = null;
        public string CreationTime { get { return info.CreationTime.ToShortDateString(); } set { } }
        private Item(FileSystemInfo info)
        {
            this.info = info;
        }

        public static List<Item> GetItems(DirectoryInfo directory)
        {
            return new Item[]
            {
                new Item((directory.Parent != null) ? directory.Parent : directory){ Name = "..." }
            }
            .AsEnumerable()
            .Concat(GetItems(directory.GetDirectories().AsEnumerable<FileSystemInfo>().Concat(directory.GetFiles().AsEnumerable())))
            .ToList();
        }
        public static List<Item> GetItems(IEnumerable<FileSystemInfo> Files)
        {
            return Files
                .Where(a => (a.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                .Select(a => new Item(a))
                .ToList();
        }
    }
}
