using Microsoft.Extensions.DependencyInjection;
using Prism.Commands;
using Prism.Events;
using RaiseHand.Core.Messaging;
using RaiseHand.Core.Navigation;
using RaiseHand.Core.SignalR;
using RaiseHand.Core.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RaiseHand.Core.ViewModels
{
    public class JoinViewModel : INotifyPropertyChanged, INavigationView
    {
        private SubscriptionToken token;
        private IServiceProvider _serviceProvider;
        private NavigationState _navigationState;
        private int _NavigationIndex = -1;
        public int NavigationIndex { get { return _NavigationIndex; } set { _NavigationIndex = value; OnPropertyChanged(); } }
        private INavigation _navigation;
        private IRHDispatcher _dispatcher;
        public Object Dispatcher { get; set; }
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _IsEnabled = true;
        public bool IsEnabled { get { return _IsEnabled; } set { _IsEnabled = value; OnPropertyChanged(); } }
        private string _HostCode;
        public string HostCode { get { return _HostCode; } set { _HostCode = value; OnPropertyChanged(); } }
        private string _Name;
        public string Name { get { return _Name; } set { _Name = value; OnPropertyChanged(); } }
        public ICommand LoadedCommand { get; set; }
        public ICommand StartCommand { get; set; }
        public ICommand BackCommand { get; set; }

        public JoinViewModel(IRHDispatcher dispatcher, IServiceProvider serviceProvider, NavigationState navigationState, INavigation navigation)
        {
            _dispatcher = dispatcher;
            _serviceProvider = serviceProvider;
            _navigationState = navigationState;
            _navigation = navigation;

            LoadedCommand = new DelegateCommand(OnLoaded);
            StartCommand = new DelegateCommand(OnStart);
            BackCommand = new DelegateCommand(OnBack);
            
        }
        public void OnLoaded()
        {
            token = ConnectEvent.Instance.Subscribe((c) =>
            {
                if (c.Method == "ReceiveUpdate" && !IsEnabled)
                {
                    if (c.Parms.ContainsKey("HostCode") && c.Parms["HostCode"].ToString().ToUpper() == HostCode.ToUpper())
                    {
                        var parms = new Dictionary<string, object>();
                        parms.Add("HostCode", HostCode.ToUpper());
                        parms.Add("IsHost", false);
                        parms.Add("Name", Name);
                        if (c.Parms.ContainsKey("UserNames"))
                        {
                            parms.Add("UserNames", c.Parms["UserNames"]);
                        }
                        var i = _navigationState.Navigate(parms);
                        _navigation.Navigate($"Meeting", parms);
                        token.Dispose();
                        token = null;
                    }
                }
            });
        }

        public async void OnStart()
        {

            if (String.IsNullOrWhiteSpace(HostCode))
            {
                var msg = _serviceProvider.GetService<IMessage>();
                msg.Caption = "Raise Hand";
                msg.Text = "Code is required.";
                await msg.Show();
                return;
            }

            if (String.IsNullOrWhiteSpace(Name))
            {
                var msg = _serviceProvider.GetService<IMessage>();
                msg.Caption = "Raise Hand";
                msg.Text = "Name is required.";
                await msg.Show();
                return;
            }

            IsEnabled = false;
            var parms = new Dictionary<string, object>();
            parms.Add("HostCode", HostCode.ToUpper());
            parms.Add("IsHost", false); 
            parms.Add("Name", Name);           
            ConnectEvent.Instance.Publish(new Connect() { Method = "SET", Parms = parms });
            ConnectEvent.Instance.Publish(new Connect() { Method = "SendCheckHostCode", Parms = parms });
            await Task.Delay(2000);
            IsEnabled = true;
            if (token != null) {
                var msg = _serviceProvider.GetService<IMessage>();
                msg.Caption = "Raise Hand";
                msg.Text = "Invalid Host Code";
                await msg.Show();
            }
        }
        public void OnBack()
        {
            _navigation.GoBack();
            if (token != null)
            {
                token.Dispose();
            }
            token = null;
        }
    }
}
