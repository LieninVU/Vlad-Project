using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ForVlad.Behaviors
{
    /// <summary>
    /// Масштабирование содержимого окна под его текущий размер и соотношение сторон.
    /// </summary>
    public static class WindowScalingBehavior
    {
        private const string ScalingViewboxTag = "WindowScalingViewbox";

        private const double DefaultBaseWidth = 1100;
        private const double DefaultBaseHeight = 700;

        #region IsEnabled

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(WindowScalingBehavior),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject obj) => (bool)obj.GetValue(IsEnabledProperty);

        public static void SetIsEnabled(DependencyObject obj, bool value) => obj.SetValue(IsEnabledProperty, value);

        #endregion

        #region BaseWidth

        public static readonly DependencyProperty BaseWidthProperty =
            DependencyProperty.RegisterAttached(
                "BaseWidth",
                typeof(double),
                typeof(WindowScalingBehavior),
                new PropertyMetadata(DefaultBaseWidth));

        public static double GetBaseWidth(DependencyObject obj) => (double)obj.GetValue(BaseWidthProperty);

        public static void SetBaseWidth(DependencyObject obj, double value) => obj.SetValue(BaseWidthProperty, value);

        #endregion

        #region BaseHeight

        public static readonly DependencyProperty BaseHeightProperty =
            DependencyProperty.RegisterAttached(
                "BaseHeight",
                typeof(double),
                typeof(WindowScalingBehavior),
                new PropertyMetadata(DefaultBaseHeight));

        public static double GetBaseHeight(DependencyObject obj) => (double)obj.GetValue(BaseHeightProperty);

        public static void SetBaseHeight(DependencyObject obj, double value) => obj.SetValue(BaseHeightProperty, value);

        #endregion

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window == null)
                return;

            if ((bool)e.NewValue)
            {
                if (window.IsLoaded)
                    EnsureScalingViewbox(window);
                else
                    window.Loaded += Window_Loaded;
            }
            else
            {
                window.Loaded -= Window_Loaded;
            }
        }

        private static void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var window = (Window)sender;
            window.Loaded -= Window_Loaded;
            EnsureScalingViewbox(window);
        }

        private static void EnsureScalingViewbox(Window window)
        {
            var existingViewbox = window.Content as Viewbox;
            if (existingViewbox != null && ScalingViewboxTag.Equals(existingViewbox.Tag))
                return;

            var content = window.Content as FrameworkElement;
            if (content == null)
                return;

            window.Content = null;

            var baseWidth = GetBaseWidth(window);
            var baseHeight = GetBaseHeight(window);

            content.Width = baseWidth;
            content.Height = baseHeight;

            // Fill — растягивает по обеим осям, заполняя окно без полос по краям
            window.Content = new Viewbox
            {
                Tag = ScalingViewboxTag,
                Stretch = Stretch.Fill,
                StretchDirection = StretchDirection.Both,
                Child = content
            };
        }
    }
}
