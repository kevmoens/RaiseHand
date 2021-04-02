using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RaiseHand.Core.Messaging
{
    public interface IMessage
    {
        string Caption { get; set; }
        string Text { get; set; }
        Task<string> Show();
    }
}
