using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaiseHand.Core.SignalR
{
    public class Connection
    {
        private bool IsHost = false;
        private string hostCode;
        private ILogger<Connection> _logger;
        private ConcurrentDictionary<string, bool> UserNames = new ConcurrentDictionary<string, bool>(System.StringComparer.InvariantCultureIgnoreCase);
        private HubConnection hubConnection;
        public Connection(ILogger<Connection> logger)
        {
            _logger = logger;
        }
        public async Task Connect(string url = "https://localhost:44362/chathub")
        {
            hubConnection = new HubConnectionBuilder()
                    .WithUrl(url)
                    .Build();

            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                _logger.LogInformation($"ReceiveMessage {user} {message}");
                ConnectEvent.Instance.Publish(payload: new Connect() { Method = "ReceiveMessage", Parms = new Dictionary<string, object>() { { "User", user }, { "Message", message } } });
            });
            hubConnection.On<string, bool, string>("ReceiveCheckHostCode", async (hostcode, ishost, username) =>
            {
                _logger.LogInformation($"ReceiveCheckHostCode {hostcode} {ishost} {username}");
                if (IsHost && hostcode.ToUpper() == hostCode.ToUpper())
                {
                    _logger.LogInformation($"ReceiveCheckHostCode HostCode Matches");
                    if (!UserNames.ContainsKey(username))
                    {
                        _logger.LogInformation($"ReceiveCheckHostCode Add username");
                        UserNames.TryAdd(username, false);
                    }
                    await hubConnection.SendAsync("SendUpdate", hostCode, UserNames.Keys.ToList());
                }
            });
            hubConnection.On<string, List<string>>("ReceiveUpdate", (hostcode, usernames) =>
            {
                _logger.LogInformation($"ReceiveUpdate {hostcode} {usernames.Count()}");
                if (hostcode.ToUpper() == hostCode.ToUpper())
                {
                    _logger.LogInformation($"ReceiveUpdate HostCode matches");
                    ConnectEvent.Instance.Publish(payload: new Connect() { Method = "ReceiveUpdate", Parms = new Dictionary<string, object>() { { "HostCode", hostCode }, { "UserNames", usernames } } });
                }
            });
            hubConnection.On<string, string>("ReceiveLeaveHost", async (hostcode, username) =>
            {
                _logger.LogInformation($"ReceiveLeaveHost {hostcode} {username}");
                if (IsHost && hostcode.ToUpper() == hostCode.ToUpper())
                {
                    _logger.LogInformation($"ReceiveLeaveHost HostCode matches and IsHost");
                    if (UserNames.ContainsKey(username))
                    {
                        _logger.LogInformation($"ReceiveLeaveHost Found UserName");
                        bool Raised;
                        UserNames.TryRemove(username, out Raised);
                    }
                    _logger.LogInformation($"ReceiveLeaveHost {hostcode} {UserNames.Count}");
                    await hubConnection.SendAsync("SendUpdate", hostCode, UserNames.Keys.ToList());
                }
            });
            hubConnection.On<string>("ReceiveCloseHost", (hostcode) =>
            {
                _logger.LogInformation($"ReceiveCloseHost {hostcode}");
                if (hostCode.ToUpper() == hostCode.ToUpper())
                {
                    _logger.LogInformation($"ReceiveCloseHost HostCode Matches");
                    ConnectEvent.Instance.Publish(payload: new Connect() { Method = "ReceiveCloseHost", Parms = new Dictionary<string, object>() { { "HostCode", hostcode } } });
                    ConnectEvent.Instance.Publish(payload: new Connect() { Method = "Set", Parms = new Dictionary<string, object> { { "HostCode", "" }, { "IsHost", false } } });
                }
            });

            hubConnection.On<string, bool, string>("ReceiveRaiseHand", async (hostcode, raised, username) =>
            {
                _logger.LogInformation($"ReceiveRaiseHand {hostcode} {username} {raised}");
                if (IsHost && hostcode.ToUpper() == hostCode.ToUpper())
                {
                    _logger.LogInformation($"ReceiveRaiseHand HostCode Matches");
                    if (UserNames.ContainsKey(username))
                    {
                        _logger.LogInformation($"ReceiveRaiseHand Update User Raised");
                        UserNames[username] = raised;
                    }
                    var RaisedHandes = UserNames.Where(u => u.Value == true).Select(u => u.Key).ToList();
                    _logger.LogInformation($"ReceiveRaiseHand {RaisedHandes.Count}");
                    await hubConnection.SendAsync("SendUpdateRaiseHands", hostCode, RaisedHandes);
                }
            });

            hubConnection.On<string, List<string>>("ReceiveUpdateRaiseHands", (hostcode, raised) =>
            {
                _logger.LogInformation($"ReceiveUpdateRaiseHands {hostcode} {raised.Count}");
                if (hostcode.ToUpper() == hostCode.ToUpper()) //!IsHost && 
                {
                    _logger.LogInformation($"ReceiveUpdateRaiseHands HostCode Matches");
                    ConnectEvent.Instance.Publish(payload: new Connect() { Method = "ReceiveUpdateRaiseHands", Parms = new Dictionary<string, object>() { { "HostCode", hostcode }, { "RaisedHands", raised } } });
                }
            });
            ConnectEvent.Instance.Subscribe(async (obj) =>
            {
                if (obj.Method.ToUpper().StartsWith("SEND"))
                {
                    _logger.LogInformation($"Send {obj.Method} {obj.Parms.Count}");
                    var Keys = obj.Parms.Keys.ToList();
                    switch (obj.Parms.Count)
                    {
                        case 0:
                            await hubConnection.SendAsync(obj.Method);
                            break;
                        case 1:
                            await hubConnection.SendAsync(obj.Method, obj.Parms[Keys[0]]);
                            break;
                        case 2:
                            await hubConnection.SendAsync(obj.Method, obj.Parms[Keys[0]], obj.Parms[Keys[1]]);
                            break;
                        case 3:
                            await hubConnection.SendAsync(obj.Method, obj.Parms[Keys[0]], obj.Parms[Keys[1]], obj.Parms[Keys[2]]);
                            break;
                    }
                }

                if (obj.Method.ToUpper().StartsWith("SET"))
                {
                    _logger.LogInformation($"SET ");
                    var hostcode = (string)obj.Parms["HostCode"];
                    var isHost = (bool)obj.Parms["IsHost"];
                    var username = string.Empty;
                    if (obj.Parms.ContainsKey("Name"))
                    {
                        username = (string)obj.Parms["Name"];
                    }
                    _logger.LogInformation($"SET {hostcode} {isHost} {username}");
                    if (hostcode == string.Empty)
                    {
                        if (IsHost)
                        {
                            _logger.LogInformation($"SET SendCloseHost");
                            await hubConnection.SendAsync("SendCloseHost", hostCode);
                        }
                        UserNames = new ConcurrentDictionary<string, bool>();
                    }
                    hostCode = hostcode;
                    IsHost = isHost;
                    if (IsHost)
                    {
                        _logger.LogInformation($"SET Add myself");
                        UserNames.TryAdd(username, false);
                    }
                }
            });
            await hubConnection.StartAsync();
        }
    }
}
