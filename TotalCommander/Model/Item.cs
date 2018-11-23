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
        public string Size { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public static List<Item> GetItems(DirectoryInfo directory)
        {
            return new Item[] { new Item() { info = (directory.Parent != null) ? directory.Parent : directory, Name= "..." } }.AsEnumerable().Concat(GetItems(directory.GetDirectories().AsEnumerable<FileSystemInfo>().Concat(directory.GetFiles().AsEnumerable()))).ToList();
        }
        public static List<Item> GetItems(IEnumerable<FileSystemInfo> Files)
        {
            return Files.Select(a => new Item() {
                info = a,
                Size = (a is FileInfo) ? AdapterNumber.ToAdaptNumber((a as FileInfo).Length) + " байт" : "",
                Name = a.Name,
                Type = (a is FileInfo) ? "File" : "Directory" }).ToList();
        }
        private Item() { }
    }
}
