using Prism.Commands;
using RaiseHand.Core.Navigation;
using RaiseHand.Core.SignalR;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RaiseHand.Core.ViewModels
{
    public class MainWindowViewModel
    {
        private INavigation _navigation;

        public ICommand LoadedCommand { get; set; }
        private Connection _connection;
        public MainWindowViewModel(INavigation navigation, Connection connection)
        {
            _navigation = navigation;
            _connection = connection;
            LoadedCommand = new DelegateCommand(async () => { await OnLoaded(); });
            
        }
        public async Task OnLoaded()
        {
            string url = "https://localhost:44362/chathub";
            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("ConnectURL"))
            {
                url = System.Configuration.ConfigurationManager.AppSettings["ConnectURL"];
            }
            await _connection.Connect(url);
            _navigation.Navigate("Home");
        }
    }
}
