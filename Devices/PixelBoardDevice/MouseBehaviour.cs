using System.Windows;
using System.Windows.Input;

namespace PixelBoardDevice
{
    public static class MouseBehaviour
    {
        public static readonly DependencyProperty MouseMoveCommandProperty =
            DependencyProperty.RegisterAttached("MouseMoveCommand", typeof(ICommand),
            typeof(MouseBehaviour), new FrameworkPropertyMetadata(
            new PropertyChangedCallback(MouseMoveCommandChanged)));

        private static void MouseMoveCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;

            element.MouseMove += new MouseEventHandler(Element_MouseMove);
        }

        private static void Element_MouseMove(object sender, MouseEventArgs e)
        {
            var element = (FrameworkElement)sender;
            ICommand command = GetMouseMoveCommand(element);
            command.Execute(e);
        }

        public static ICommand GetMouseMoveCommand(FrameworkElement element)
        {
            return (ICommand)element.GetValue(MouseMoveCommandProperty);
        }

        public static void SetMouseMoveCommand(UIElement element, ICommand value)
        {
            element.SetValue(MouseMoveCommandProperty, value);
        }

        public static readonly DependencyProperty MouseUpCommandProperty =
            DependencyProperty.RegisterAttached("MouseUpCommand", typeof(ICommand),
            typeof(MouseBehaviour), new FrameworkPropertyMetadata(
            new PropertyChangedCallback(MouseUpCommandChanged)));

        private static void MouseUpCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;

            element.MouseLeftButtonUp += new MouseButtonEventHandler(Element_MouseUp);
        }

        private static void Element_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = (FrameworkElement)sender;
            ICommand command = GetMouseUpCommand(element);
            command.Execute(e);
        }

        public static ICommand GetMouseUpCommand(FrameworkElement element)
        {
            return (ICommand)element.GetValue(MouseUpCommandProperty);
        }

        public static void SetMouseUpCommand(UIElement element, ICommand value)
        {
            element.SetValue(MouseUpCommandProperty, value);
        }

        public static readonly DependencyProperty MouseDownCommandProperty =
            DependencyProperty.RegisterAttached("MouseDownCommand", typeof(ICommand),
            typeof(MouseBehaviour), new FrameworkPropertyMetadata(
            new PropertyChangedCallback(MouseDownCommandChanged)));

        private static void MouseDownCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;

            element.MouseLeftButtonDown += new MouseButtonEventHandler(Element_MouseDown);
        }

        private static void Element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = (FrameworkElement)sender;
            ICommand command = GetMouseDownCommand(element);
            command.Execute(e);
        }

        public static ICommand GetMouseDownCommand(FrameworkElement element)
        {
            return (ICommand)element.GetValue(MouseDownCommandProperty);
        }

        public static void SetMouseDownCommand(UIElement element, ICommand value)
        {
            element.SetValue(MouseDownCommandProperty, value);
        }

        public static readonly DependencyProperty MouseLeaveCommandProperty =
            DependencyProperty.RegisterAttached("MouseLeaveCommand", typeof(ICommand),
            typeof(MouseBehaviour), new FrameworkPropertyMetadata(
            new PropertyChangedCallback(MouseLeaveCommandChanged)));

        private static void MouseLeaveCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;

            element.MouseLeave += new MouseEventHandler(Element_MouseLeave);
        }

        private static void Element_MouseLeave(object sender, MouseEventArgs e)
        {
            var element = (FrameworkElement)sender;
            ICommand command = GetMouseLeaveCommand(element);
            command.Execute(e);
        }

        public static ICommand GetMouseLeaveCommand(FrameworkElement element)
        {
            return (ICommand)element.GetValue(MouseLeaveCommandProperty);
        }

        public static void SetMouseLeaveCommand(UIElement element, ICommand value)
        {
            element.SetValue(MouseLeaveCommandProperty, value);
        }

        public static readonly DependencyProperty MouseWheelCommandProperty =
            DependencyProperty.RegisterAttached("MouseWheelCommand", typeof(ICommand),
            typeof(MouseBehaviour), new FrameworkPropertyMetadata(
            new PropertyChangedCallback(MouseWheelCommandChanged)));

        private static void MouseWheelCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;

            element.MouseWheel += new MouseWheelEventHandler(Element_MouseWheel);
        }

        private static void Element_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var element = (FrameworkElement)sender;
            ICommand command = GetMouseWheelCommand(element);
            command.Execute(e);

        }

        public static ICommand GetMouseWheelCommand(FrameworkElement element)
        {
            return (ICommand)element.GetValue(MouseWheelCommandProperty);
        }

        public static void SetMouseWheelCommand(UIElement element, ICommand value)
        {
            element.SetValue(MouseWheelCommandProperty, value);
        }
    }
}
