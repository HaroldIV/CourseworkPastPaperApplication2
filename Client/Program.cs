using CourseworkPastPaperApplication2.Client;
using CourseworkPastPaperApplication2.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored;
using Blazored.LocalStorage;
using System;
using Microsoft.AspNetCore.Components.Forms;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace CourseworkPastPaperApplication2.Client
{
    public class Program
    {
        // Async main to avoid blocking the browser page thread. 
        public static async Task Main(string[] args)
        {
            // Sets up the app. 
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            // Adds an HttpClient per user. 
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            // Adds local storage services
            builder.Services.AddBlazoredLocalStorage();
            
            WebAssemblyHost app = builder.Build();

            // Runs the app asynchronously to avoid blocking the browser page thread. 
            await app.RunAsync();
        }
    }
}