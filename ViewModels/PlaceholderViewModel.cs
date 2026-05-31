namespace ForVlad.ViewModels
{
    public class PlaceholderViewModel : ViewModelBase
    {
        private string _title;
        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }
        
        private string _message;
        public string Message
        {
            get => _message;
            set => SetField(ref _message, value);
        }
        
        private string _icon;
        public string Icon
        {
            get => _icon;
            set => SetField(ref _icon, value);
        }
        
        public PlaceholderViewModel(string title, string message, string icon = "🚧")
        {
            Title = title;
            Message = message;
            Icon = icon;
        }
    }
}
