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
    class FileBrowserViewModel : Model.NotifyPropertyChanged
    {
        public FileBrowserViewModel()
        {
            this.Model = new Model.FileBrowserManagers(this);
        }
        public Model.FileBrowserManagers Model { get; set; }

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

        public IEnumerable<Model.Item> ListItems
        {
            get { return listItems; }
            set
            {
                listItems = value;
                OnPropertyChanged("ListItems");
            }
        }
        private IEnumerable<Model.Item> listItems;

        public Model.Item SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
        private Model.Item selectedItem;


        public Action<object, EventArgs> Selected;
        public Action<object, EventArgs> SelectionItems; 
        public Action<object, EventArgs> DoubleClick; 
    }
}
