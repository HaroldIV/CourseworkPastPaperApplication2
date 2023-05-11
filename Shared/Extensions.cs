using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CourseworkPastPaperApplication2.Shared
{
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

        public static IEnumerable<T> GetRow<T>(this IEnumerable<IEnumerable<T>> values, int rowIndex)
        {
            return values.Skip(rowIndex).First();
        }

        public static IEnumerable<T> GetColumn<T>(this T[][] array, int columnIndex)
        {
            int columnLength = array.Length;
            for (int i = 0; i < columnLength; i++)
            {
                yield return array[i][columnIndex];
            }
        }

        public static IEnumerable<T> GetColumn<T>(this IEnumerable<IEnumerable<T>> values, int columnIndex)
        {
            foreach (IEnumerable<T> enumerable in values)
            {
                if (enumerable.FirstOrDefault() is T value)
                {
                    yield return value;
                }
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

        public static string Parse(this string value) => value;

        public static T Deserialize<T>(this string jsonString) where T : new()
        {
            return (T)jsonString.Deserialize(typeof(T));
        }

        public static object Deserialize(this string jsonString, Type type)
        {
            var document = JsonDocument.Parse(jsonString);
            
            if (type.GetMethod("Parse", new Type[] { typeof(string) }) is MethodInfo info)
            {
                try
                {
                    return info.Invoke(null, new object[] { jsonString })!;
                }
                catch
                {

                }
            }

            if (type.IsPrimitive || type == typeof(string) || type == typeof(byte[]) || type == typeof(Guid))
            {
                return document.RootElement.Deserialize(type)!;
            }
            
            if (type.IsArray)
            {
                System.Collections.ArrayList values = new System.Collections.ArrayList();

                foreach (var item in document.RootElement.EnumerateArray())
                {
                    values.Add(item.GetRawText().Deserialize(type.GetElementType()!));
                }
                Array array = Array.CreateInstance(type.GetElementType()!, values.Count);
                values.CopyTo(array);
                return array;
            }

            object value = type.GetConstructor(Type.EmptyTypes)?.Invoke(null) ?? throw new SerializationException($"Must contain a blank constructor: {type.FullName}");

            foreach (var property in type.GetProperties().Where(property => property.CanRead && property.CanWrite && property.GetCustomAttribute(typeof(JsonIgnoreAttribute)) is null).Where(property => !property.GetIndexParameters().Any()))
            {
                try
                {
                    property.SetValue(value, document.RootElement.TryGetProperty(JsonNamingPolicy.CamelCase.ConvertName(property.Name), out JsonElement element) ? element.GetRawText().Deserialize(property.PropertyType) : null);
                }
                catch (Exception e)
                {
                    property.SetValue(value, null);
                    Console.WriteLine(e.Message);
                }
            }

            return value;
        }

        public static void AddRange<T>(this ICollection<T> values, IEnumerable<T> valuesToAdd) => valuesToAdd.ForEach(values.Add);
    }
}
