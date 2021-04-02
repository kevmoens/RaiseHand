using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Prism.Commands;
using RaiseHand.Core.Messaging;
using RaiseHand.Core.Navigation;
using RaiseHand.Core.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RaiseHand.Core.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private INavigation _navigation;
        public ICommand SendMessageCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ICommand HostCommand { get; set; }
        public ICommand JoinCommand { get; set; }
        private IRHDispatcher _dispatcher;
        public Object Dispatcher { get; set; }
        public HomeViewModel(IRHDispatcher dispatcher, INavigation navigation)
        {
            _navigation = navigation;
            _dispatcher = dispatcher;
            LoadedCommand = new DelegateCommand(OnLoaded);
            HostCommand = new DelegateCommand(OnHost);
            JoinCommand = new DelegateCommand(OnJoin);
        }
        public void OnLoaded()
        {
        }
        public void OnHost()
        {
            _navigation.Navigate("Host");
        }
        public void OnJoin()
        {
            _navigation.Navigate("Join");
        }
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}