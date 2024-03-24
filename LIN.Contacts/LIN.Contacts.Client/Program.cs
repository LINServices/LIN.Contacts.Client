global using LIN.Types.Cloud.Identity.Enumerations;
global using LIN.Types.Cloud.Identity.Models;
global using LIN.Types.Enumerations;
global using LIN.Types.Responses;
global using Microsoft.JSInterop;
global using LIN.Types.Contacts.Models;
global using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

LIN.Access.Auth.Build.Init();
LIN.Access.Search.Build.Init();
LIN.Access.Contacts.Build.Init();


await builder.Build().RunAsync();
