using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace KaLEDoscope
{
    public class DropDownButtonBehavior : Behavior<Button>
    {
        private long _attachedCount;
        private bool _isContextMenuOpen;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(AssociatedObject_Click), true);
        }

        void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button source && source.ContextMenu != null && !_isContextMenuOpen)
            {
                source.ContextMenu.AddHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenu_Closed), true);
                Interlocked.Increment(ref _attachedCount);
                source.ContextMenu.PlacementTarget = source;
                source.ContextMenu.Placement = PlacementMode.Bottom;
                source.ContextMenu.IsOpen = true;
                _isContextMenuOpen = true;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(Button.ClickEvent, new RoutedEventHandler(AssociatedObject_Click));
        }

        void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            _isContextMenuOpen = false;
            if (sender is ContextMenu contextMenu)
            {
                contextMenu.RemoveHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenu_Closed));
                Interlocked.Decrement(ref _attachedCount);
            }
        }
    }
}
