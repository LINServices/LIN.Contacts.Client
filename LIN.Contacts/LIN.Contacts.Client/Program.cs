global using LIN.Types.Auth.Enumerations;
global using LIN.Types.Auth.Models;
global using LIN.Types.Enumerations;
global using LIN.Types.Responses;
global using Microsoft.JSInterop;
global using LIN.Types.Contacts.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddBlazoredLocalStorage(config =>
      config.JsonSerializerOptions.WriteIndented = true);

await builder.Build().RunAsync();
