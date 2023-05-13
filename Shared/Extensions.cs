using FluentValidation.Results;
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
    // C# has a feature called extension methods which allow for a class to be provided with additional methods based on their public properties. 
    // This class is a static class used as a library of all those extensions used across this application. 
    public static class Extensions
    {
        // Returns an enumerable with its index attached as a tuple, inspired by the usefulness of Python's enumerate function. 
        // The indexToStartAt parameter indicates to start enumerating from a given index and also have a matching index returned. 
        public static IEnumerable<(int, T)> WithIndex<T>(this IEnumerable<T> values, int indexToStartAt = 0) => values.Skip(indexToStartAt).Select((value, i) => (i + indexToStartAt, value));

        // Converts an IBrowserFile to a Data URL to be displayed in the browser. 
        public static async Task<string> ToDataUrlAsync(this IBrowserFile image)
        {
            // Creates a memory stream and copies the image to that stream. 
            /// Could be optimised to avoid unnecessary allocations
            using var reader = new MemoryStream();

            await image.OpenReadStream().CopyToAsync(reader);

            // Converts the image to base64 then performs the necessary concatenations to convert it into a valid data URL. 
            string base64Image = Convert.ToBase64String(reader.ToArray());

            return $"""data:{image.ContentType};base64,{base64Image}""";
        }

        // Converts an image to a data URL via its Data property and FileName property. 
        public static string ToDataUrl(this Question question)
        {
            // Converts the image data to base 64. 
            string base64Image = Convert.ToBase64String(question.Data);

            // Gets the extension then cuts off the '.' at the start of the extension. 
            // Performs the necessary concatenations to create a Data URL. 
            return $"""data:image/{Path.GetExtension(question.FileName.AsSpan()).Slice(1)};base64,{base64Image}""";
        }

        // Enumerates through values and applies an action to each one as a more terse and function-based alternative to the standard foreach (var item in values) syntax. 
        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            // Loops through each value and applies the given subroutine to each value. 
            foreach (T value in values)
            {
                action.Invoke(value);
            }
        }

        // Same as ForEach except also provides an index. 
        public static void ForEach<T>(this IEnumerable<T> values, Action<T, int> action)
        {
            foreach (var (index, value) in values.WithIndex())
            {
                action.Invoke(value, index);
            }
        }

        // ForEach for asynchronous subroutines. 
        public static async Task ForEach<T>(this IEnumerable<T> values, Func<T, Task> action)
        {
            foreach (T value in values)
            {
                await action.Invoke(value);
            }
        }

        // Async ForEach with index also. 
        public static async Task ForEach<T>(this IEnumerable<T> values, Func<T, int, Task> action)
        {
            foreach (var (index, value) in values.WithIndex())
            {
                await action.Invoke(value, index);
            }
        }

        // Gets the row at index rowIndex, useful for extracting a row from an array of arrays etc. 
        public static IEnumerable<T> GetRow<T>(this IEnumerable<IEnumerable<T>> values, int rowIndex)
        {
            return values.Skip(rowIndex).First();
        }

        
        // Gets the column at columnIndex, useful for extracting a column from an array of arrays etc. 
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
        
        // Gets all rows but the one indicated by rowIndex
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

            // Local function used to build a row
            // Simply enumerates through and returns all values of the row as indicated by the index, i. 
            static IEnumerable<T> buildRow(T[][] array, int columnCount, int i)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    yield return array[i][j];
                }
            }
        }

        // Gets all columns but the one indicated by columnIndex
        public static IEnumerable<IEnumerable<T>> SkipColumn<T>(this T[][] array, int columnIndex)
        {
            int rowCount = array.Length;
            int columnCount = array[0].Length;

            for (int i = 0; i < rowCount; i++)
            {
                if (i == columnIndex)
                    continue;

                yield return buildColumn(array, columnCount, i);
            }

            // Local function used to build a column
            // Simply enumerates through and returns all values of the column as indicated by the index, i. 
            static IEnumerable<T> buildColumn(T[][] array, int columnCount, int i)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    yield return array[j][i];
                }
            }
        }

        // Splits a collection of values into two separate enumerables with one up to a given index and the other from the given index onwards.
        // The second group of values is returned via the out parameters, restOfValues. 
        public static IEnumerable<T> IndexedPartition<T>(this IEnumerable<T> values, int partitionIndex, out IEnumerable<T> restOfValues)
        {
            // The `using` keyword specifies that this variable is to be disposed of when it goes out of scope, similar to Python's context manager (`with`) syntax. 
            using var enumerator = values.GetEnumerator();

            // Function to get all values up to the partitionIndex
            var upTo = GetUpToIndex(enumerator, partitionIndex);

            // Gets all the rest of the values. 
            restOfValues = enumerator.ToEnumerable();

            return upTo;
        }

        // Converts an IEnumerator to an IEnumerable. 
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> values)
        {
            // Continues looping until there are no more values. 
            while (values.MoveNext())
            {
                // Yields each value. 
                yield return values.Current;
            }
        }

        // Gets all values from an IEnumerator until partitionIndex. 
        private static IEnumerable<T> GetUpToIndex<T>(IEnumerator<T> enumerator, int partitionIndex)
        {
            // post-decrements partitionIndex once per loop and checks if it is greater or equal to 0. 
            // In layman's terms, this counts down until partitionIndex = 0. 
            while (partitionIndex-- >= 0)
            {
                // Ensures there are still values to enumerate through (that partitionIndex is not greater than the length of the collection of values)
                bool sequenceContinues = enumerator.MoveNext();

                // Throws if the aforementioned error occurs. 
                if (!sequenceContinues)
                {
                    throw new ArgumentOutOfRangeException(nameof(partitionIndex));
                }

                yield return enumerator.Current;
            }
        }
        
        // Custom JSON deserialisation functions. 
        public static T Deserialize<T>(this string jsonString) where T : new()
        {
            return (T)jsonString.Deserialize(typeof(T));
        }

        public static object Deserialize(this string jsonString, Type type)
        {
            // Parses the jsonString into a new JsonDocument. 
            var document = JsonDocument.Parse(jsonString);
            
            // Checks if value can be parsed then tries to parse it. 
            if (type.GetMethod("Parse", new Type[] { typeof(string) }) is MethodInfo info)
            {
                try
                {
                    return info.Invoke(null, new object[] { jsonString })!;
                }
                // Empty catch to skip to next part if parsing fails. 
                catch
                {

                }
            }

            // Checks if is JSON primitive type and deserialises based on that. 
            if (type.IsPrimitive || type == typeof(string) || type == typeof(byte[]) || type == typeof(Guid))
            {
                return document.RootElement.Deserialize(type)!;
            }
            
            // If an array, loops through the JSON values, deserialises them, and adds them to a list then returns that list. 
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

            // Ensures that the type has a blank constructor to instantiate with. 
            object value = type.GetConstructor(Type.EmptyTypes)?.Invoke(null) ?? throw new SerializationException($"Must contain a blank constructor: {type.FullName}");

            // Loops through each property using reflection (only the properties that can be used and have not been specified to not be serialised with the JsonIgnoreAttribute also where it is not an indexer as those should not be deserialised or serialised. 
            foreach (var property in type.GetProperties().Where(property => property.CanRead && property.CanWrite && property.GetCustomAttribute(typeof(JsonIgnoreAttribute)) is null).Where(property => !property.GetIndexParameters().Any()))
            {
                // Tries to set a value onto the new object or sets it to null (error only thrown when JSON value is null)
                try
                {
                    property.SetValue(value, document.RootElement.TryGetProperty(JsonNamingPolicy.CamelCase.ConvertName(property.Name), out JsonElement element) ? element.GetRawText().Deserialize(property.PropertyType) : null);
                }
                catch (Exception e)
                {
                    property.SetValue(value, null);
#if DEBUG
                    Console.WriteLine(e.Message);
#endif
                }
            }

            return value;
        }

        // Function to add multiple values in one go. 
        public static void AddRange<T>(this ICollection<T> values, IEnumerable<T> valuesToAdd) => valuesToAdd.ForEach(values.Add);

        // Function to convert a list of validation errors to a dictionary, mapping the name of the property for which the validation error occurred on to the array of validation errors. 
        public static Dictionary<string, string[]> ToDictionary(this List<ValidationFailure> errors)
        {
            var result = new Dictionary<string, List<string>>();

            // Loops through each error and adds it to the dictionary
            // Also adds the property name if not already present in the dictionary
            foreach (ValidationFailure error in errors)
            {
                if (result.TryAdd(error.PropertyName, new List<string>() { error.ErrorMessage }))
                {
                    continue;
                }

                result[error.PropertyName].Add(error.ErrorMessage);
            }

            // Converts each list to an array, to match with what is required by certain third-party services. 
            return result.ToDictionary(
                keyValuePair => keyValuePair.Key,
                keyValuePair => keyValuePair.Value.ToArray()
                );
        }
    }
}
