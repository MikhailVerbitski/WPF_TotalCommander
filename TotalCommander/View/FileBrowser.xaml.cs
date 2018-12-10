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

namespace TotalCommander.View
{
    public partial class FileBrowser : UserControl, IDisposable
    {
        public FileBrowser()
        {
            InitializeComponent();
            DataContext = new ViewModel.FileBrowserViewModel();

        }
        public void Dispose()
        {
            (this.DataContext as IDisposable).Dispose();
        }

        public void Selected(object sender, EventArgs e)
        {
            if ((this.DataContext as ViewModel.FileBrowserViewModel).Selected != null)
                (this.DataContext as ViewModel.FileBrowserViewModel).Selected(sender, e);
        }
        public void DoubleClick(object sender, EventArgs e)
        {
            if ((this.DataContext as ViewModel.FileBrowserViewModel).DoubleClick != null)
                (this.DataContext as ViewModel.FileBrowserViewModel).DoubleClick(sender, e);
        }
        public void SelectionItems(object sender, EventArgs e)
        {
            (this.DataContext as ViewModel.FileBrowserViewModel).SelectionItems(sender, e);
        }
    }
}
