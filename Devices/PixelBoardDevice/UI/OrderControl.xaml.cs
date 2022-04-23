using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PixelBoardDevice.UI
{
    /// <summary>
    /// Логика взаимодействия для OrderControl.xaml
    /// </summary>
    public partial class OrderControl : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(int), typeof(OrderControl), new PropertyMetadata(0));

        public static readonly DependencyProperty DownCommandProperty = DependencyProperty.Register(
            "DownCommand", typeof(ICommand), typeof(OrderControl), new PropertyMetadata(null));

        public static readonly DependencyProperty UpCommandProperty = DependencyProperty.Register(
            "UpCommand", typeof(ICommand), typeof(OrderControl), new PropertyMetadata(null));

        public OrderControl()
        {
            InitializeComponent();
            down.Click += Down_Click;
            up.Click += Up_Click;
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            UpCommand?.Execute(element.DataContext);
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            DownCommand?.Execute(element.DataContext);
        }

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public ICommand DownCommand
        {
            get => (ICommand)GetValue(DownCommandProperty);
            set
            {
                SetValue(DownCommandProperty, value);
            }
        }

        public ICommand UpCommand
        {
            get => (ICommand)GetValue(UpCommandProperty);
            set
            {
                SetValue(UpCommandProperty, value);
            }
        }
    }
}
