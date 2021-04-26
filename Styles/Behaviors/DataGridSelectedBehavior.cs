using Prism.Commands;
using System.Windows;
using System.Windows.Controls;

namespace Styles.Behaviors
{
    public class DataGridSelectedBehavior
    {
        public static DependencyProperty CommandProperty =
                 DependencyProperty.RegisterAttached("Command",
                                                     typeof(DelegateCommand<object>),
                                                     typeof(DataGridSelectedBehavior),
                                                     new UIPropertyMetadata(CommandChanged));

        public static DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter",
                                                typeof(object),
                                                typeof(DataGridSelectedBehavior),
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
                    dg.SelectedCellsChanged += DgSelectedCellsChanged;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    dg.SelectedCellsChanged -= DgSelectedCellsChanged;
                }
            }
        }

        private static void DgSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGrid control = sender as DataGrid;
            DelegateCommand<object> command = (DelegateCommand<object>)control.GetValue(CommandProperty);
            object commandParameter = control.GetValue(CommandParameterProperty);
            command.Execute(commandParameter);
        }
    }
}
