global using LIN.Access.Contacts;
global using LIN.Types.Cloud.Identity.Enumerations;
global using LIN.Types.Cloud.Identity.Models;
global using LIN.Types.Contacts.Models;
global using LIN.Types.Responses;
global using Microsoft.AspNetCore.Components;
using LIN.Access.Auth;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthenticationService();
builder.Services.AddContactsService();
LIN.Access.Search.Build.Init();

LIN.Contacts.Client.Online.Realtime.Build();


await builder.Build().RunAsync();
