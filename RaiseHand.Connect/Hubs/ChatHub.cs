using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaiseHand.Connect.Hubs
{
    public class ChatHub : Hub
    {
        //ILogger<ChatHub> _logger;
        //public ChatHub(ILogger<ChatHub> logger)
        //{
        //    _logger = logger;
        //}
        /// <summary>
        /// Host starts app and picks Host.  Is then given a Host Code.  Then clicks start
        ///     Validates Name is not empty
        ///     Navigates to meeting page
        ///     (PubSub Set HostCode)
        /// User picks Join and types in Host Code then clicks start
        ///     Validates Name is not empty
        ///     sends message to Host
        ///         SendCheckHostCode(HostCode, UserName)
        ///         ReceiveCheckHostCode        
        ///     Host sends back message within 2 seconds
        ///         SendUpdate(HostCode, UserNames)
        ///         ReceiveUpdate
        ///     If user doesn't receive message within 2 seconds it will be considered invalid
        ///     If valid navigate to meeting page        
        ///         Other users join in meeting
        ///             Message goes to host            
        ///             Message goes to all users the list of all User Names
        ///     Client Leaves
        ///         SendLeaveHost(HostCode, UserName)
        ///         ReceiveLeaveHost
        ///         SendUpdate (HostCode, UsersNames)
        ///         ReceiveUpdate
        /// Host goes back to main screen
        ///     Message goes to all users the session is over
        ///         SendCloseHost(HostCode)
        ///         ReceiveCloseHost
        /// Use Clicks on RaiseHand (Raise, Lower)
        ///     SendRaiseHand(HostCode, User, Raised)
        ///     ReceiveRaiseHand
        ///     
        /// </summary>

        public async Task SendMessage(string user, string message)
        {
            NLog.LogManager.GetCurrentClassLogger().Info($"SendMessage -> ReceiveMessage {user} {message}");
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task SendCheckHostCode(string HostCode, bool IsHost, string UserName)
        {
            NLog.LogManager.GetCurrentClassLogger().Info($"SendCheckHostCode -> ReceiveCheckHostCode {HostCode} {IsHost} {UserName}");
            await Clients.All.SendAsync("ReceiveCheckHostCode", HostCode, IsHost, UserName);
        }
        public async Task SendUpdate(string HostCode, List<string> UserNames)
        {
            NLog.LogManager.GetCurrentClassLogger().Info($"SendUpdate -> ReceiveUpdate {HostCode} {UserNames.Count}");
            await Clients.All.SendAsync("ReceiveUpdate", HostCode, UserNames);
        }
        public async Task SendLeaveHost(string HostCode, string UserName)
        {
            NLog.LogManager.GetCurrentClassLogger().Info($"SendLeaveHost -> ReceiveLeaveHost {HostCode} {UserName}");
            await Clients.All.SendAsync("ReceiveLeaveHost", HostCode, UserName);
        }
        public async Task SendCloseHost(string HostCode)
        {
            NLog.LogManager.GetCurrentClassLogger().Info($"SendCloseHost -> ReceiveCloseHost {HostCode}");
            await Clients.All.SendAsync("ReceiveCloseHost", HostCode);
        }
        public async Task SendRaiseHand(string HostCode, bool Raised , string UserName)
        {
            NLog.LogManager.GetCurrentClassLogger().Info($"SendRaiseHand -> ReceiveRaiseHand {HostCode} {Raised} {UserName}");
            await Clients.All.SendAsync("ReceiveRaiseHand", HostCode, Raised, UserName);
        }
        public async Task SendUpdateRaiseHands(string HostCode, List<string> RaisedHands)
        {
            NLog.LogManager.GetCurrentClassLogger().Info($"SendUpdateRaiseHands -> ReceiveUpdateRaiseHands {HostCode} {RaisedHands.Count}");
            await Clients.All.SendAsync("ReceiveUpdateRaiseHands", HostCode, RaisedHands);
        }
    }
}
