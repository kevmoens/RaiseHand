using RaiseHand.Core.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaiseHand.Desktop.Threading
{
    public class RHDispatcher : IRHDispatcher
    {
        public void Invoke(Object sender, Action action)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(action);
        }
    }
}
