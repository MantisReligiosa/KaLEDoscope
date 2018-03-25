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

namespace PixelBoardDevice.UI
{
    /// <summary>
    /// Interaction logic for PixelControl.xaml
    /// </summary>
    public partial class PixelControl : UserControl
    {
        public PixelControl()
        {
            InitializeComponent();
            preview.MouseWheel += Preview_MouseWheel;
        }

        private void Preview_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Point p = e.MouseDevice.GetPosition(this.preview);
            Matrix m = (this.preview).LayoutTransform.Value;
            if (e.Delta > 0)
            {
                m.ScaleAtPrepend(1.1, 1.1, 0, 0);
            }
            else
                m.ScaleAtPrepend(1 / 1.1, 1 / 1.1, 0, 0);

            (this.preview).LayoutTransform = new MatrixTransform(m);
            scroll.InvalidateScrollInfo();
        }
    }
}
