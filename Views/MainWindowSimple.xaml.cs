using System;
using System.Windows;

namespace ForVlad.Views
{
    public partial class MainWindowSimple : Window
    {
        public MainWindowSimple()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при инициализации окна: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}