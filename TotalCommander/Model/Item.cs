using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TotalCommander.Model
{
    public class Item
    {
        public FileSystemInfo info { get; set; }
        public string Size { get { return (info is FileInfo) ? AdapterNumber.ToAdaptNumber((info as FileInfo).Length) + " байт" : ""; } set { } }
        public string Type { get { return (info is FileInfo) ? "Файл" : "Папка"; } set { } }
        public string Name { get { return (name == null) ? info.Name : name; } set { name = value; } }
        private string name = null;
        public string CreationTime { get { return info.CreationTime.ToShortDateString(); } set { } }
        public Item(FileSystemInfo info)
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
