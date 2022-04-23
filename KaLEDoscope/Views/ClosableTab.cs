using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace KaLEDoscope.Views
{
    public class ClosableTab : TabItem
    {
        // Constructor
        public ClosableTab()
        {
            // Create an instance of the usercontrol
            var closableTabHeader = new CloseableHeader();
            // Assign the usercontrol to the tab header
            Header = closableTabHeader;
            closableTabHeader.button_close.MouseEnter +=
                new MouseEventHandler(ButtonCloseMouseEnter);
            closableTabHeader.button_close.MouseLeave +=
               new MouseEventHandler(ButtonCloseMouseLeave);
            closableTabHeader.button_close.Click +=
               new RoutedEventHandler(ButtonCloseClick);
            closableTabHeader.label_TabTitle.SizeChanged +=
               new SizeChangedEventHandler(LabelTabTitleSizeChanged);
        }


        public event EventHandler OnTabCloseClick;

        public string Title
        {
            get
            {
                return (Header as CloseableHeader).label_TabTitle.Content as string;
            }
            set
            {
                (Header as CloseableHeader).label_TabTitle.Content = value;
            }
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            ((CloseableHeader)Header).button_close.Visibility = Visibility.Visible;
        }

        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
            ((CloseableHeader)Header).button_close.Visibility = Visibility.Hidden;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            ((CloseableHeader)Header).button_close.Visibility = Visibility.Visible;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!IsSelected)
            {
                ((CloseableHeader)Header).button_close.Visibility = Visibility.Hidden;
            }
        }

        // Button MouseEnter - When the mouse is over the button - change color to Red
        private void ButtonCloseMouseEnter(object sender, MouseEventArgs e)
        {
            ((CloseableHeader)Header).button_close.Foreground = Brushes.Red;
        }
        // Button MouseLeave - When mouse is no longer over button - change color back to black
        private void ButtonCloseMouseLeave(object sender, MouseEventArgs e)
        {
            ((CloseableHeader)Header).button_close.Foreground = Brushes.Black;
        }
        // Button Close Click - Remove the Tab - (or raise
        // an event indicating a "CloseTab" event has occurred)
        private void ButtonCloseClick(object sender, RoutedEventArgs e)
        {
            OnTabCloseClick?.Invoke(this, EventArgs.Empty);
        }
        // Label SizeChanged - When the Size of the Label changes
        // (due to setting the Title) set position of button properly
        private void LabelTabTitleSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((CloseableHeader)Header).button_close.Margin = new Thickness(
               ((CloseableHeader)Header).label_TabTitle.ActualWidth + 5, 3, 4, 0);
        }
    }
}
