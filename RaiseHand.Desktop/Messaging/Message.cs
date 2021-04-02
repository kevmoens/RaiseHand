using RaiseHand.Core.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RaiseHand.Desktop.Messaging
{
    class Message : IMessage
    {
        public string Caption { get; set;}
        public string Text { get; set;}

        public async Task<string> Show()
        {
            await Task.Delay(0);
            var result = MessageBox.Show(Text, Caption, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                return "OK";
            } 
            return "";
        }
    }
}
