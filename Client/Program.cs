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

    public static class Extensions
    {
        public static IEnumerable<(int, T)> WithIndex<T>(this IEnumerable<T> values) => values.Select((value, i) => (i, value));

        public static async Task<string> ToDataUrlAsync(this IBrowserFile image)
        {
            /// Could be optimised to avoid unnecessary allocations
            using var reader = new MemoryStream();

            await image.OpenReadStream().CopyToAsync(reader);

            string base64Image = Convert.ToBase64String(reader.ToArray());

            return $"""data:{image.ContentType};base64,{base64Image}""";
        }

        public static string ToDataUrl(this Question question)
        {
            string base64Image = Convert.ToBase64String(question.Data);

            return $"""data:image/{Path.GetExtension(question.FileName.AsSpan()).Slice(1)};base64,{base64Image}""";
        }

        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (T value in values)
            {
                action.Invoke(value);
            }
        }

        public static T GetOrAdd<T>(this ICollection<T> values, T value)
        {
            if (values.Contains(value))
            {
                return value;
            }

            values.Add(value);

            return value;
        }
    }
}