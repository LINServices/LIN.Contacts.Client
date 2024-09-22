global using LIN.Types.Cloud.Identity.Enumerations;
global using LIN.Types.Cloud.Identity.Models;
global using LIN.Types.Enumerations;
global using LIN.Types.Responses;
global using Microsoft.JSInterop;
global using LIN.Types.Contacts.Models;
global using Microsoft.AspNetCore.Components;
global using LIN.Access.Contacts;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LIN.Access.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthenticationService();
builder.Services.AddContactsService();
LIN.Access.Search.Build.Init();

LIN.Contacts.Client.Online.Realtime.Build();


await builder.Build().RunAsync();
