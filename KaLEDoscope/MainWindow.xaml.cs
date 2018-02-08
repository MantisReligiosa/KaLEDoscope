using Logger;
using ServiceInterfaces;
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

namespace KaLEDoscope
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ApplicationInfrastructure _infrastructure = new ApplicationInfrastructure();
        public MainWindow()
        {
            InitializeComponent();
            ILogger logger = new SeviceLog();
            var viewModel = new ViewModel(logger);
            DataContext = trvMenu.DataContext = lbLog.DataContext = viewModel;
            viewModel.MakeNodes();
            _infrastructure.Tabs = tabControl.Items;
        }

    }
}
