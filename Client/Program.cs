using CourseworkPastPaperApplication2.Client;
using CourseworkPastPaperApplication2.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored;
using Blazored.LocalStorage;
using Syncfusion.Blazor;
using System;
using Microsoft.AspNetCore.Components.Forms;
using System.Runtime.CompilerServices;

namespace CourseworkPastPaperApplication2.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSyncfusionBlazor();
            builder.Services.AddBlazoredLocalStorage();
            
            WebAssemblyHost app = builder.Build();

            await app.RunAsync();
        }
    }
}