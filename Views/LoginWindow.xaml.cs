using System;
using System.Windows;
using System.Windows.Controls;
using ForVlad.ViewModels;

namespace ForVlad.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            var viewModel = new LoginViewModel();
            DataContext = viewModel;

            viewModel.LoginSuccess += OnLoginSuccess;
            viewModel.LoginFailed += OnLoginFailed;
        }
        
        private void OnLoginSuccess(object sender, EventArgs e)
        {
            // Открываем главное окно
            var mainWindow = new MainWindow();
            mainWindow.Show();
            
            // Закрываем окно входа
            Close();
        }
        
        private void OnLoginFailed(object sender, string errorMessage)
        {
            // Ошибка уже обработана в ViewModel
        }
        
        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            VisiblePasswordBox.Text = PasswordBox.Password;
            PasswordBox.Visibility = Visibility.Collapsed;
            VisiblePasswordBox.Visibility = Visibility.Visible;
        }
        
        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = VisiblePasswordBox.Text;
            VisiblePasswordBox.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Visible;
        }
    }
}
