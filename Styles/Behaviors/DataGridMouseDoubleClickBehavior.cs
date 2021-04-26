using Prism.Commands;
using System.Windows;
using System.Windows.Controls;

namespace Styles.Behaviors
{
    public class DataGridMouseDoubleClickBehavior
    {
        public static DependencyProperty CommandProperty =
               DependencyProperty.RegisterAttached("Command",
                                                   typeof(DelegateCommand<object>),
                                                   typeof(DataGridMouseDoubleClickBehavior),
                                                   new UIPropertyMetadata(CommandChanged));

        public static DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter",
                                                typeof(object),
                                                typeof(DataGridMouseDoubleClickBehavior),
                                                new UIPropertyMetadata(null));

        public static void SetCommand(DependencyObject target, DelegateCommand<object> value) => target.SetValue(CommandProperty, value);
        public static void SetCommandParameter(DependencyObject target, object value) => target.SetValue(CommandParameterProperty, value);
        public static object GetCommandParameter(DependencyObject target) => target.GetValue(CommandParameterProperty);
        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is DataGrid dg)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    dg.MouseDoubleClick += OnMouseDoubleClick;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    dg.MouseDoubleClick -= OnMouseDoubleClick;
                }
            }
        }
        private static void OnMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            DataGrid control = sender as DataGrid;
            DelegateCommand<object> command = (DelegateCommand<object>)control.GetValue(CommandProperty);
            object commandParameter = control.GetValue(CommandParameterProperty);
            command.Execute(commandParameter);
        }
    }
}
