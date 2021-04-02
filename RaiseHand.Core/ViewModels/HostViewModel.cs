using Microsoft.Extensions.DependencyInjection;
using Prism.Commands;
using RaiseHand.Core.Messaging;
using RaiseHand.Core.Navigation;
using RaiseHand.Core.SignalR;
using RaiseHand.Core.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace RaiseHand.Core.ViewModels
{
    public class HostViewModel : INotifyPropertyChanged, INavigationView
    {
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

        private string _HostCode;
        public string HostCode { get { return _HostCode; } set { _HostCode = value; OnPropertyChanged(); } }
        private string _Name;
        public string Name { get { return _Name; } set { _Name = value; OnPropertyChanged(); } }
        public ICommand LoadedCommand { get; set; }
        public ICommand StartCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public HostViewModel(IRHDispatcher dispatcher, IServiceProvider serviceProvider, NavigationState navigationState, INavigation navigation)
        {
            _dispatcher = dispatcher;
            _serviceProvider = serviceProvider;
            _navigationState = navigationState;
            _navigation = navigation;
            GetNewHostCode();
            ConnectEvent.Instance.Subscribe((obj) =>
            {
                switch (obj.Method)
                {
                    case "ReceiveMessage":
                        
                        break;
                }
            });
            LoadedCommand = new DelegateCommand(OnLoaded);
            StartCommand = new DelegateCommand(OnStart);
            BackCommand = new DelegateCommand(OnBack);
        }

        private void GetNewHostCode()
        {
            var rnd = new Random();
            var data = "2346789BCDFGHJKLMNPQRTWXYZ";
            for (var i = 0; i < 5; i++)
            {
                var p = rnd.Next(0, 25);
                HostCode += data.Substring(p, 1);
            }
        }
        
        public void OnLoaded()
        {

        }

        public async void OnStart() 
        {
            if (String.IsNullOrWhiteSpace(Name))
            {
                var msg = _serviceProvider.GetService<IMessage>();
                msg.Caption = "Raise Hand";
                msg.Text = "Name is required.";
                await msg.Show();
                return;
            }
            
            var parms = new Dictionary<string, object>();
            parms.Add("HostCode", HostCode);
            parms.Add("IsHost", true);
            parms.Add("Name", Name);
            ConnectEvent.Instance.Publish(new Connect() { Method = "SET", Parms = parms });
            _navigation.Navigate("Meeting", parms);
        }
        public void OnBack()
        {
            _navigation.GoBack();
        }
    }
}
