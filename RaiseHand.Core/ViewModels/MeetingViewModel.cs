using Prism.Commands;
using Prism.Events;
using RaiseHand.Core.Navigation;
using RaiseHand.Core.SignalR;
using RaiseHand.Core.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace RaiseHand.Core.ViewModels
{
    public class MeetingViewModel : INotifyPropertyChanged, INavigationView
    {
        private object lockobj = new object();
        private SubscriptionToken token;
        private IServiceProvider _serviceProvider;
        private NavigationState _navigationState;
        private INavigation _navigation;
        private int _NavigationIndex = -1;
        public int NavigationIndex { get { return _NavigationIndex; } set { _NavigationIndex = value; OnPropertyChanged(); } }
        private IRHDispatcher _dispatcher;
        public Object Dispatcher { get; set; }
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _IsHost;
        public bool IsHost { get { return _IsHost; } set { _IsHost = value; OnPropertyChanged(); } }
        private string _HostCode;
        public string HostCode { get { return _HostCode; } set { _HostCode = value; OnPropertyChanged(); } }
        private string _Name;
        public string Name { get { return _Name; } set { _Name = value; OnPropertyChanged(); } }
        private bool _IsMyHandRaised;
        public bool IsMyHandRaised { get { return _IsMyHandRaised; } set { _IsMyHandRaised = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _UserNames = new ObservableCollection<string>();
        public ObservableCollection<string> UserNames { get { return _UserNames; } set { _UserNames = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _RaisedNames = new ObservableCollection<string>();
        public ObservableCollection<string> RaisedNames { get { return _RaisedNames; } set { _RaisedNames = value; OnPropertyChanged(); } }
        public ICommand LoadedCommand { get; set; }
        public ICommand LeaveCommand { get; set; }
        public ICommand RaiseHandCommand { get; set; }
        public MeetingViewModel(IRHDispatcher dispatcher, IServiceProvider serviceProvider, NavigationState navigationState, INavigation navigation)
        {
            _dispatcher = dispatcher;
            _serviceProvider = serviceProvider;
            _navigationState = navigationState;
            _navigation = navigation;
            token = ConnectEvent.Instance.Subscribe((c) =>
            {
            lock (lockobj) //Synchronous
            {
                if (c.Parms.ContainsKey("HostCode") && c.Parms["HostCode"] != null && ((string)c.Parms["HostCode"]).ToUpper() == HostCode)
                {
                    switch (c.Method)
                    {
                        case "ReceiveUpdate":
                                _dispatcher.Invoke(Dispatcher, () => {
                                    var newUserNames = (List<string>)c.Parms["UserNames"];
                                    foreach (var newUser in newUserNames)
                                    {
                                        if (!UserNames.Contains(newUser))
                                        {
                                            UserNames.Add(newUser);
                                        }
                                    }
                                    var removeUsers = new List<string>();
                                    foreach (var existUser in UserNames)
                                    {
                                        if (!newUserNames.Contains(existUser))
                                        {
                                            removeUsers.Add(existUser);
                                        }
                                    }
                                    foreach (var user in removeUsers)
                                    {
                                        UserNames.Remove(user);
                                    }
                                });
                                break;
                            case "ReceiveUpdateRaiseHands":
                                if (c.Parms.ContainsKey("HostCode"))
                                {
                                    _dispatcher.Invoke(Dispatcher, () => {
                                        var RaisedHands = (List<string>)c.Parms["RaisedHands"];
                                        RaisedNames.Clear();
                                        foreach (var name in RaisedHands)
                                        {
                                            RaisedNames.Add(name);
                                        }
                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RaisedNames"));
                                    });
                                }
                                break;
                            case "ReceiveCloseHost":
                                if (!IsHost)
                                {
                                    var leaveparms = new Dictionary<string, object>();
                                    leaveparms.Add("HostCode", HostCode);
                                    leaveparms.Add("Name", Name);
                                    leaveparms.Add("IsHost", IsHost);
                                    ConnectEvent.Instance.Publish(new Connect() { Method = "Set", Parms = leaveparms });
                                    _navigation.GoBack();
                                }
                                break;
                        }
                    }
                }
            });
            LoadedCommand = new DelegateCommand(OnLoaded);
            LeaveCommand = new DelegateCommand(OnLeave);
            RaiseHandCommand = new DelegateCommand<string>(OnRaiseHand);
        }
        public void OnLoaded()
        {
            if (NavigationIndex > -1) 
            {
                var parms = _navigationState.State[NavigationIndex];

                if (parms.ContainsKey("HostCode"))
                {
                    HostCode = (string)parms["HostCode"];
                }
                if (parms.ContainsKey("Name"))
                {
                    Name = (string)parms["Name"];
                }
                if (parms.ContainsKey("IsHost"))
                {
                    IsHost = (bool)parms["IsHost"];
                }
                if (parms.ContainsKey("UserNames"))
                {
                    UserNames = new ObservableCollection<string>((List<string>)parms["UserNames"]);
                } else
                {
                    UserNames = new ObservableCollection<string>();
                    UserNames.Add(Name);
                }
            }
        }
        public void OnLeave()
        {
            if (IsHost)
            {
                var parms = new Dictionary<string, object>();
                parms.Add("HostCode", HostCode);
                ConnectEvent.Instance.Publish(new Connect() { Method = "SendCloseHost", Parms = parms });
            } else
            {
                var parms = new Dictionary<string, object>();
                parms.Add("HostCode", HostCode);
                parms.Add("UserName", Name);
                ConnectEvent.Instance.Publish(new Connect() { Method = "SendLeaveHost", Parms = parms });
            }

            var leaveparms = new Dictionary<string, object>();
            leaveparms.Add("HostCode", HostCode);
            leaveparms.Add("Name", Name);            
            leaveparms.Add("IsHost", IsHost);
            ConnectEvent.Instance.Publish(new Connect() { Method = "Set", Parms = leaveparms });

            _navigation.GoBack();

        }
        public void OnRaiseHand(string name)
        {
            if (name == Name)
            {
                IsMyHandRaised = !IsMyHandRaised;
                var parms = new Dictionary<string, object>();
                parms.Add("HostCode", HostCode);
                parms.Add("Raised", IsMyHandRaised);
                parms.Add("UserName", Name);
                ConnectEvent.Instance.Publish(new Connect() { Method = "SendRaiseHand", Parms = parms });
            }
        }

    }
}
