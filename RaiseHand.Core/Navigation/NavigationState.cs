using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaiseHand.Core.Navigation
{
    public class NavigationState
    {
        public Dictionary<int, Dictionary<string, object>> State { get; set; }
        public int Index { get; set; }
        public NavigationState()
        {
            Index = -1;
            State = new Dictionary<int, Dictionary<string, object>>();
        }
        public int Navigate(Dictionary<string, object> parms)
        {
            Index += 1;
            State.Add(Index, parms);
            return Index;
        }
        public Dictionary<string, object> Retrieve(int Index)
        {
            return Retrieve(Index, false);
        }
        public Dictionary<string, object> Retrieve(int Index, bool ClearCache)
        {
            var returnValue = State[Index];
            if (ClearCache)
            {
                State.Remove(Index);
            }
            return returnValue;
        }
    }
}
