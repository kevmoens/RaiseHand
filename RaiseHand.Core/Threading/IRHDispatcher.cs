using System;
using System.Collections.Generic;
using System.Text;

namespace RaiseHand.Core.Threading
{
    public interface IRHDispatcher
    {
        void Invoke(Object sender, Action action);
    }
}
