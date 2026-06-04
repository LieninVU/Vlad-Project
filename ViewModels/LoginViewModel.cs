using System;
using System.ComponentModel;
using System.Windows.Input;
using ForVlad.Services;

namespace ForVlad.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authService;
        
        private string _username;
        public string Username
        {
            get => _username;
            set => SetField(ref _username, value);
        }
        
        private string _password;
        public string Password
        {
            get => _password;
            set => SetField(ref _password, value);
        }
        
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetField(ref _errorMessage, value);
        }
        
        private bool _hasError;
        public bool HasError
        {
            get => _hasError;
            set => SetField(ref _hasError, value);
        }
        
        private bool _isLoggingIn;
        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            set => SetField(ref _isLoggingIn, value);
        }
        
        public ICommand LoginCommand { get; }
        public ICommand LoginSuccessCommand { get; }
        
        public event EventHandler LoginSuccess;
        public event EventHandler<string> LoginFailed;
        
        public LoginViewModel()
        {
            _authService = new AuthenticationService();
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            LoginSuccessCommand = new RelayCommand(ExecuteLoginSuccess);
        }
        
        private bool CanExecuteLogin(object obj) => !IsLoggingIn && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        
        private void ExecuteLogin(object obj)
        {
            if (IsLoggingIn) return;
            
            IsLoggingIn = true;
            HasError = false;
            ErrorMessage = string.Empty;
            
            try
            {
                bool isAuthenticated = _authService.Authenticate(Username, Password);
                
                if (isAuthenticated)
                {
                    LoginSuccess?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Неверный логин или пароль";
                    LoginFailed?.Invoke(this, "Неверный логин или пароль");
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = "Ошибка аутентификации: " + ex.Message;
                LoginFailed?.Invoke(this, ex.Message);
            }
            finally
            {
                IsLoggingIn = false;
            }
        }
        
        private void ExecuteLoginSuccess(object obj)
        {
            // Дополнительная логика при успешном входе
        }
    }
}
