using Microsoft.AspNetCore.Components;
using RaiseHand.Core.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaiseHand.Web.Threading
{
    public class RHDispatcher : IRHDispatcher
    {
        public void Invoke(Object sender, Action action)
        {
            action.Invoke();
            if (sender is IRHComponentBase)
            {
                ((IRHComponentBase)sender).OnStateChanged();
            }
        }
    }
}
