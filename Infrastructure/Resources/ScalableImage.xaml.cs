using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Resources
{
    /// <summary>
    /// Interaction logic for ScalableImage.xaml
    /// </summary>
    public partial class ScalableImage : UserControl
    {
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(object), typeof(ScalableImage), new FrameworkPropertyMetadata(new object()));

        public object Image
        {
            get
            {
                return GetValue(ImageProperty);
            }
            set
            {
                SetValue(ImageProperty, value);
            }
        }

        public ScalableImage()
        {
            InitializeComponent();
            preview.MouseWheel += Preview_MouseWheel;
        }
        private void Preview_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Matrix m = preview.LayoutTransform.Value;
            if (e.Delta > 0)
            {
                m.ScaleAtPrepend(1.1, 1.1, 0, 0);
            }
            else
                m.ScaleAtPrepend(1 / 1.1, 1 / 1.1, 0, 0);

            preview.LayoutTransform = new MatrixTransform(m);
            scroll.InvalidateScrollInfo();
        }
    }
}
