using Microsoft.AspNetCore.Components;
using RaiseHand.Core.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaiseHand.Web.Navigation
{
    public class Navigation : INavigation
    {
        NavigationManager _navigationManager;
        NavigationState _navigationState;
        public Navigation(NavigationManager navigationManager, NavigationState navigationState)
        {
            _navigationManager = navigationManager;
            _navigationState = navigationState;
        }

        public void GoBack()
        {
            _navigationManager.NavigateTo($"/Home");
        }

        public void Navigate(string Path)
        {
            _navigationManager.NavigateTo($"/{Path}/1");
        }
        public void Navigate(string Path, Dictionary<string, object> parms)
        {
            var Index = _navigationState.Navigate(parms);
            _navigationManager.NavigateTo($"/{Path}/{Index}");
        }
    }
}
