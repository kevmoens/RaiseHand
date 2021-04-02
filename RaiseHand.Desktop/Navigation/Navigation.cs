using Prism.Regions;
using RaiseHand.Core.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RaiseHand.Desktop.Navigation
{
    class Navigation : INavigation
    {
        IRegionManager _regionManager;
        NavigationState _navigationState;
        public Navigation(IRegionManager regionManager, NavigationState navigationState)
        {
            _regionManager = regionManager;
            _navigationState = navigationState;
        }
        public void Navigate(string Path)
        {
            _regionManager.RequestNavigate("Main", Path);
        }
        public void Navigate(string Path, Dictionary<string, object> parms)
        {
            var Index = _navigationState.Navigate(parms);
            var prms = new NavigationParameters();
            prms.Add(Index.ToString(), parms);
            var callback = new Action<NavigationResult>(nav => 
                {
                    var vws = nav.Context.NavigationService.Region.Views;
                    var view = (FrameworkElement)vws.ElementAt(nav.Context.NavigationService.Region.Views.Count() - 1);
                    if (view.DataContext is INavigationView)
                    {
                        ((INavigationView)view.DataContext).NavigationIndex = Index;
                    }
                });
            _regionManager.RequestNavigate("Main", Path,callback);
        }
        public void GoBack()
        {            
            _regionManager.Regions["Main"].NavigationService.Journal.GoBack();
        }
    }
}
