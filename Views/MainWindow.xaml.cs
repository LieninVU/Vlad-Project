using System;
using System.Windows;
using ForVlad.Data;
using ForVlad.ViewModels;

namespace ForVlad.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                DataContext = new MainViewModel(DataServiceProvider.Create());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка подключения к базе данных",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }
    }
}