using System;
using System.Collections.Generic;
using System.Text;

namespace RaiseHand.Core.Navigation
{
    public interface INavigation
    {
        void Navigate(string Path);
        void Navigate(string Path, Dictionary<string, object> parms);
        void GoBack();
    }
}
