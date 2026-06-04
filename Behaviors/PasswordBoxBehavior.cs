using System.Windows;
using System.Windows.Controls;

namespace ForVlad.Behaviors
{
    /// <summary>
    /// Attached properties for PasswordBox to enable two-way binding
    /// </summary>
    public static class PasswordBoxBehavior
    {
        private static bool _isUpdating = false;

        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached(
                "BoundPassword",
                typeof(string),
                typeof(PasswordBoxBehavior),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, BoundPasswordPropertyChanged));

        public static string GetBoundPassword(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(BoundPasswordProperty);
        }

        public static void SetBoundPassword(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(BoundPasswordProperty, value);
        }

        private static void BoundPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is PasswordBox passwordBox && !_isUpdating)
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                
                string newValue = e.NewValue as string ?? string.Empty;
                
                // Only update if values are different
                if (passwordBox.Password != newValue)
                {
                    passwordBox.Password = newValue;
                }
                
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                _isUpdating = true;
                SetBoundPassword(passwordBox, passwordBox.Password);
                _isUpdating = false;
            }
        }
    }
}
