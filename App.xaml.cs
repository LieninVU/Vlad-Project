using System.Windows;
using System.Windows.Media;
using ForVlad.Properties;

namespace ForVlad
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ApplySavedTheme();
        }

        private static void ApplySavedTheme()
        {
            string savedTheme = Settings.Default.UiTheme;
            bool isDark = savedTheme == "Dark" || savedTheme == "Тёмная";

            var resources = Current.Resources;

            if (isDark)
            {
                resources["WindowBackground"] = new SolidColorBrush(Color.FromRgb(44, 62, 80));
                resources["PanelBackground"] = new SolidColorBrush(Color.FromRgb(52, 73, 94));
                resources["TextForeground"] = new SolidColorBrush(Color.FromRgb(236, 240, 241));
                resources["DataGridRowBackground"] = new SolidColorBrush(Color.FromRgb(44, 62, 80));
                resources["DataGridAlternatingRowBackground"] = new SolidColorBrush(Color.FromRgb(52, 73, 94));
                resources["HeaderBackground"] = new SolidColorBrush(Color.FromRgb(41, 128, 185));
            }
            else
            {
                resources["WindowBackground"] = new SolidColorBrush(Color.FromRgb(236, 240, 241));
                resources["PanelBackground"] = new SolidColorBrush(Color.FromRgb(248, 249, 250));
                resources["TextForeground"] = new SolidColorBrush(Color.FromRgb(44, 62, 80));
                resources["DataGridRowBackground"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                resources["DataGridAlternatingRowBackground"] = new SolidColorBrush(Color.FromRgb(248, 249, 250));
                resources["HeaderBackground"] = new SolidColorBrush(Color.FromRgb(52, 152, 219));
            }
        }
    }
}