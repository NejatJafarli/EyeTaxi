using EyeTaxi.Models;
using EyeTaxi.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace EyeTaxi.Views
{
    /// <summary>
    /// Interaction logic for HistoryView.xaml

    /// </summary>

    public partial class HistoryView : Window
    {

        public ObservableCollection<History> Histories { get; set; }
        public HistoryView(User temp)
        {

            InitializeComponent();
            DataContext = this;
            Histories= new ObservableCollection<History>(temp.History);

        }
    }
}
