using CourseworkPastPaperApplication2.Client;
using CourseworkPastPaperApplication2.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored;
using Blazored.LocalStorage;
using Syncfusion.Blazor;
using System;
using Microsoft.AspNetCore.Components.Forms;

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

    public static class Extensions
    {
        public static IEnumerable<(int, T)> WithIndex<T>(this IEnumerable<T> values) => values.Select((value, i) => (i, value));

        public static async Task<string> ImageToDataUrlAsync(this IBrowserFile image)
        {
            /// Could be optimised to avoid unnecessary allocations
            using var reader = new MemoryStream();

            await image.OpenReadStream().CopyToAsync(reader);

            string base64Image = Convert.ToBase64String(reader.ToArray());

            return $"""data:{image.ContentType};base64,{base64Image}""";
        }
    }
}