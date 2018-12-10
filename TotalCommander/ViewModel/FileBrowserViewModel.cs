using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using TotalCommander.Model;

namespace TotalCommander.ViewModel
{
    class FileBrowserViewModel : Model.NotifyPropertyChanged, IDisposable
    {
        static FileBrowserViewModel()
        {
            AllFileBrowserViewModel = new List<FileBrowserViewModel>();
        }
        public static List<FileBrowserViewModel> AllFileBrowserViewModel { get; set; }
        public static bool CopyOrTransfer { get; set; }
        public static IEnumerable<Item> SelectionItemsForCopyOrTransfer { get; set; }

        public static void UpdateListsItems()
        {
            foreach (var item in AllFileBrowserViewModel)
            {
                Thread.Sleep(100);
                item.ListItems = Item.GetItems(item.GetCurrentDirectory.info as DirectoryInfo);
            }
        }

        public FileBrowserViewModel()
        {
            var Model = new FileBrowserManagers();
            ComboBoxItems = Model.GetComboBoxItems;
            SelectedDrive = comboBoxItems.First();
            Linq = Model.GetLinq;
            ListItems = Model.GetListItems;

            this.Selected = (sender,e) => {
                ListItems = Item.GetItems(new DirectoryInfo(((sender as ComboBox).SelectedItem as DriveInfo).Name));
                var drive = (sender as ComboBox).SelectedItem as DriveInfo;
                TextNearComboBox = $"{AdapterNumber.ToAdaptNumber(drive.TotalFreeSpace / 1024 / 1024)}Mбайт из {AdapterNumber.ToAdaptNumber(drive.TotalSize / 1024 / 1024)}Mбайт";
                Linq = drive.Name;
                GetCurrentDirectory = new Item(new DirectoryInfo(drive.Name));
            };
            this.DoubleClick = (sender, e) => {
                if ((sender as ListView).SelectedItem == null)
                    return;
                var selected = ((sender as ListView).SelectedItem as Item);
                try
                {
                    if (selected.info is DirectoryInfo)
                    {
                        GetCurrentDirectory = selected;
                        UpdateListsItems();
                    }
                    else
                    {
                        var a = new System.Diagnostics.Process();
                        a.StartInfo.FileName = selected.info.FullName;
                        a.StartInfo.UseShellExecute = true;
                        a.Start();
                    }
                }
                catch (Exception ex)
                {
                    TotalCommander.ViewModel.MainWindowViewModel.Alert.Call(ex.Message, Colors.Red);
                }
            };
            
            AllFileBrowserViewModel.Add(this);

            GetSelectionItems = new List<Item>();
        }
        public void Dispose()
        {
            AllFileBrowserViewModel.Remove(this);
        }

        public DriveInfo[] ComboBoxItems
        {
            get { return comboBoxItems; }
            set
            {
                comboBoxItems = value;
                OnPropertyChanged("ComboBoxItems");
            }
        }
        public DriveInfo[] comboBoxItems;

        public DriveInfo SelectedDrive
        {
            get { return selectedDrive; }
            set
            {
                selectedDrive = value;
                GetCurrentDirectory = new Item(new DirectoryInfo(selectedDrive.Name));
                OnPropertyChanged("SelectedDrive");
            }
        }
        public DriveInfo selectedDrive;
        
        public string TextNearComboBox
        {
            get { return textNearComboBox; }
            set
            {
                textNearComboBox = value;
                OnPropertyChanged("TextNearComboBox");
            }
        }
        private string textNearComboBox;

        public string Linq
        {
            get { return linq; }
            set
            {
                linq = value;
                OnPropertyChanged("Linq");
            }
        }
        private string linq;

        public IEnumerable<Item> ListItems
        {
            get { return listItems; }
            set
            {
                listItems = value;
                OnPropertyChanged("ListItems");
            }
        }
        private IEnumerable<Item> listItems;

        public Item SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
        private Item selectedItem;
        
        public Command CopyCommand { get { return new Command(obj => {
            CopyOrTransfer = true;
            SelectionItemsForCopyOrTransfer = GetSelectionItems.ToList();
        }); } }
        public Command CutCommand { get { return new Command(obj => {
            CopyOrTransfer = false;
            SelectionItemsForCopyOrTransfer = GetSelectionItems.ToList();
        }); } }
        public Command PasteCommand { get { return new Command(obj => 
        {
            DirectoryInfo Path = (GetSelectionItems.Count() == 1) 
                ? (GetSelectionItems.Single().info is DirectoryInfo) 
                    ? GetSelectionItems.Single().info as DirectoryInfo
                    : GetCurrentDirectory.info as DirectoryInfo
                : GetCurrentDirectory.info as DirectoryInfo;
            FileBrowserManagers.PasteAsync(CopyOrTransfer, SelectionItemsForCopyOrTransfer.ToList(), Path);
            UpdateListsItems();
        }); } }
        public Command DeleteCommand { get { return new Command(obj=> {
            FileBrowserManagers.Delete(GetSelectionItems);
            UpdateListsItems();
        }); } }
        
        public Action<object, EventArgs> Selected;
        public Action<object, EventArgs> DoubleClick;

        public void SelectionItems(object sender, EventArgs e)
        {
            GetSelectionItems = (sender as ListView).SelectedItems.Cast<Item>();
        }
        public IEnumerable<Item> GetSelectionItems { get; set; }
        public Item GetCurrentDirectory { get; set; }
    }
}
