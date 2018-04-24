using Logger;
using ServiceInterfaces;
using System;
using System.Windows;

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
            var viewModel = new MainViewModel(logger);
            viewModel.ShowOptions += new EventHandler(OnShowOptions);
            DataContext = trvMenu.DataContext = lbLog.DataContext = viewModel;
            _infrastructure.Tabs = tabControl.Items;
        }

        private void OnShowOptions(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
