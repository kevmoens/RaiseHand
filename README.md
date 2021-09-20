# RaiseHand
Allowed multiple people on a conference call to easily take turns talking.

I wanted to prove that a WebAssembly Blazor app and WPF can share code.
In this example I used the MVVM pattern and SignalR to show when a person wants to talk while on a conference call.
The WPF app is using Prism for navigation and the MVVM pattern.
The Blazor app I used MatBlazor for the material design look and feel and BlazoredModel for message boxes.

RaiseHand.Connect is the SignalR project.
RaiseHand.Core is the shared project.

Notice that I abstracted:
<ul>
<li>Messaging</li>
<li>Navigation</li>
<li>UI Notification</li>
</ul>

As each of these features are handled differently between Blazor and WPF.

I used the IServiceProvider for dependeny injection.

I hosted this internally at work while working at home during COVID-19.  
