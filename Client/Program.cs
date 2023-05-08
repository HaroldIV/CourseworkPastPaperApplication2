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
        public static IEnumerable<(int, T)> WithIndex<T>(this IEnumerable<T> values, int indexToStartAt = 0) => values.Skip(indexToStartAt).Select((value, i) => (i + indexToStartAt, value));

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

        public static void ForEach<T>(this IEnumerable<T> values, Action<T, int> action)
        {
            foreach (var (index, value) in values.WithIndex())
            {
                action.Invoke(value, index);
            }
        }

        public static async Task ForEach<T>(this IEnumerable<T> values, Func<T, Task> action)
        {
            foreach (T value in values)
            {
                await action.Invoke(value);
            }
        }

        public static async Task ForEach<T>(this IEnumerable<T> values, Func<T, int, Task> action)
        {
            foreach (var (index, value) in values.WithIndex())
            {
                await action.Invoke(value, index);
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

        public static IEnumerable<T> GetRow<T>(this T[][] array, int rowIndex)
        {
            int rowLength = array[0].Length;
            for (int i = 0; i < rowLength; i++)
            {
                yield return array[rowIndex][i];
            }
        }

        public static IEnumerable<T> GetColumn<T>(this T[][] array, int columnIndex)
        {
            int columnLength = array.Length;
            for (int i = 0; i < columnLength; i++)
            {
                yield return array[i][columnIndex];
            }
        }

        public static IEnumerable<IEnumerable<T>> SkipRow<T>(this T[][] array, int rowIndex)
        {
            int rowCount = array.Length;
            int columnCount = array[0].Length;

            for (int i = 0; i < rowCount; i++)
            {
                if (i == rowIndex)
                    continue;

                yield return buildRow(array, columnCount, i);
            }

            static IEnumerable<T> buildRow(T[][] array, int columnCount, int i)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    yield return array[i][j];
                }
            }
        }

        public static IEnumerable<IEnumerable<T>> SkipColumn<T>(this T[][] array, int columnIndex)
        {
            int rowCount = array.Length;
            int columnCount = array[0].Length;

            for (int i = 0; i < rowCount; i++)
            {
                if (i == columnIndex)
                    continue;

                yield return buildRow(array, columnCount, i);
            }

            static IEnumerable<T> buildRow(T[][] array, int columnCount, int i)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    yield return array[j][i];
                }
            }
        }

        public static IEnumerable<T> IndexedPartition<T>(this IEnumerable<T> values, int partitionIndex, out IEnumerable<T> restOfValues)
        {
            using var enumerator = values.GetEnumerator();

            var upTo = GetUpToIndex(enumerator, partitionIndex).ToArray();

            restOfValues = enumerator.ToEnumerable().ToArray();

            return upTo;
        }

        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> values)
        {
            while (values.MoveNext())
            {
                yield return values.Current;
            }
        }

        private static IEnumerable<T> GetUpToIndex<T>(IEnumerator<T> enumerator, int partitionIndex)
        {
            while (partitionIndex-- >= 0)
            {
                bool sequenceContinues = enumerator.MoveNext();
                
                if (!sequenceContinues)
                {
                    throw new ArgumentOutOfRangeException(nameof(partitionIndex));
                }

                yield return enumerator.Current;
            }
        }
    }
}