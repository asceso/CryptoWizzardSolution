using Prism.Commands;
using System.Windows;

namespace Styles.Behaviors
{
    public class LoadedBehavior
    {
        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
                                                typeof(DelegateCommand),
                                                typeof(LoadedBehavior),
                                                new UIPropertyMetadata(CommandChanged));
        public static void SetCommand(DependencyObject target, DelegateCommand value) => target.SetValue(CommandProperty, value);
        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is FrameworkElement fe)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    fe.Loaded += OnLoaded;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    fe.Loaded -= OnLoaded;
                }
            }
        }
        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement control = sender as FrameworkElement;
            DelegateCommand command = (DelegateCommand)control.GetValue(CommandProperty);
            command.Execute();
        }
    }
}
