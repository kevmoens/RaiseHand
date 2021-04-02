using Blazored.Modal;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RaiseHand.Core.Messaging;
using RaiseHand.Core.Navigation;
using RaiseHand.Core.SignalR;
using RaiseHand.Core.Threading;
using RaiseHand.Core.ViewModels;
using RaiseHand.Web.Messaging;
using RaiseHand.Web.Navigation;
using RaiseHand.Web.Threading;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RaiseHand.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddBlazoredModal();
            builder.Services.AddTransient<IRHDispatcher, RHDispatcher>();
            builder.Services.AddTransient<IMessage, Message>();
            builder.Services.AddTransient<MainWindowViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<HostViewModel>();
            builder.Services.AddTransient<JoinViewModel>();
            builder.Services.AddTransient<MeetingViewModel>();
            builder.Services.AddTransient<INavigation, Navigation.Navigation>();
            builder.Services.AddSingleton<NavigationState>();
            builder.Services.AddSingleton<Connection>();

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
