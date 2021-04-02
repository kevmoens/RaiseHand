using Blazored.Modal;
using Blazored.Modal.Services;
using RaiseHand.Core.Messaging;
using RaiseHand.Web.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaiseHand.Web.Messaging
{
    public class Message : IMessage
    {
        
        public string Caption { get; set;}
        public string Text { get; set; }
        public IModalService Modal { get; set; }
        public Message(IModalService modal)
        {
            Modal = modal;
        }
        public async Task<string> Show()
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(DisplayMessage.Message), Text);
            var DisplayMessageModal =  Modal.Show<DisplayMessage>(Caption, parameters);
            var result = await DisplayMessageModal.Result;
            if (result.Cancelled)
            {
                return "";                
            } else
            {
                return "OK";
            }
        }
    }
}
