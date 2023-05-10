using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CourseworkPastPaperApplication2;

namespace CourseworkPastPaperApplication2.Shared
{
    [Serializable]
    public class ResultsTable : IEnumerable<IEnumerable<string>>, ISerializable
    {
        public ResultsTable()
        {
            Results = Array.Empty<string[]>();
            Questions = Array.Empty<Question>();
            Students = Array.Empty<Student>();
        }

        public ResultsTable(string[][] results, Question[] questions, Student[] students)
        {
            Results = results;
            Questions = questions;
            Students = students;
        }

        public string[][] Results { get; set; }
        public Question[] Questions { get; set; }
        public Student[] Students { get; set; }

        [JsonIgnore]
        public int Rows => Questions.Length + 1;
        [JsonIgnore]
        public int Columns => Students.Length + 1;

        public static implicit operator string[][](ResultsTable table)
        {
            string[][] arr = new string[table.Rows][];

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = new string[table.Columns];
            }

            for (int i = 0; i < table.Rows; i++)
            {
                for (int j = 0; j < table.Columns; j++)
                {
                    arr[i][j] = table[i, j];
                }
            }

            return arr;
        }

        public static ResultsTable FromIResultsTable(IResultsTable table)
        {
            return new ResultsTable { Questions = table.Questions, Results = table.Results, Students = table.Students };
        }

        public string this[int i, int j]
        {
            get => (i, j) switch
            {
                (0, 0) => string.Empty,
                (0, int studentIndex) => Students[studentIndex - 1].Name,
                (int questionIndex, 0) => Questions[questionIndex - 1].FileName,
                (_, _) => Results[i - 1][j - 1]
            };

            set
            {
                switch ((i, j))
                {
                    case (0, 0):
                        break;
                    case (0, int questionIndex):
                        Questions[questionIndex - 1].FileName = value;
                        break;
                    case (int studentIndex, 0):
                        Students[studentIndex - 1].Name = value;
                        break;
                    case (_, _):
                        Results[i - 1][j - 1] = value;
                        break;
                }
            }
        }

        public IEnumerator<IEnumerable<string>> GetEnumerator()
        {
            for (int i = 0; i < this.Rows; i++)
            {
                yield return Enumerable.Range(0, this.Columns).Select(j => this[i, j]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Type resultsType = typeof(ResultsTable);

            resultsType.GetProperties().ForEach(property => info.AddValue(property.Name, property.GetValue(this)));
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected ResultsTable(SerializationInfo info, StreamingContext context)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Type resultsType = typeof(ResultsTable);

            object?[] parameters = resultsType.GetProperties().Select(property => info.GetValue(property.Name, property.PropertyType)).ToArray();

            ConstructorInfo? constructorInfo = resultsType.GetConstructor(resultsType.GetProperties().Select(property => property.PropertyType).ToArray());

            if (constructorInfo is null)
            {
                throw new SerializationException($"No constructor was found for given properties on {nameof(ResultsTable)}");
            }

            constructorInfo.Invoke(this, parameters);
        }
    }

    public interface IResultsTable
    {
        Question[] Questions { get; set; }
        Student[] Students { get; set; }
        string[][] Results { get; set; }

        [JsonIgnore]
        public string this[int i, int j] { get; set; }
    }

    public class NonEnumerableResultsTable : IResultsTable
    {
        private ResultsTable table = new ResultsTable();

        public string this[int i, int j] { get => table[i, j] ; set => table[i, j] = value; }

        public Student[] Students { get => table.Students; set => table.Students = value; }
        public Question[] Questions
        {
            get => table.Questions;
            set => table.Questions = value;
        }

        public string[][] Results { get => table.Results; set => table.Results = value; }

        public static implicit operator ResultsTable(NonEnumerableResultsTable nonEnumerableResultsTable)
        {
            return nonEnumerableResultsTable.table;
        }

        public static implicit operator NonEnumerableResultsTable(ResultsTable resultsTable)
        {
            var nonEnumerableTable = new NonEnumerableResultsTable
            {
                table = resultsTable
            };

            return nonEnumerableTable;
        }

        public static NonEnumerableResultsTable Deserialize(string jsonString)
        {
            using var document = JsonDocument.Parse(jsonString);

            NonEnumerableResultsTable? table = document.Deserialize<NonEnumerableResultsTable>();

            var root = document.RootElement;

            var a = document.RootElement.GetProperty("students").Deserialize<Student[]>() ?? throw new SerializationException("AAAAAAAAAAAHHHHHHHHHH!");

            table.Students = document.RootElement.GetProperty("students").Deserialize<Student[]>() ?? throw new SerializationException("AAAAAAAAAAAHHHHHHHHHH!");

            return table;
        }
    }

    public class ResultsTableJsonConverter : JsonConverter<ResultsTable>
    {   
        private static readonly Dictionary<string, PropertyInfo> propertyNameToInfoDictionary;

        static ResultsTableJsonConverter()
        {
            propertyNameToInfoDictionary = typeof(ResultsTable)
                .GetProperties()
                .Where(property => !property.GetIndexParameters().Any())
                .Where(property => !Attribute.IsDefined(property, typeof(JsonIgnoreAttribute)))
                .ToDictionary(property => property.Name);
        }

        /// Consider stopping Question from fully serialising to save on memory and improve speed. 
        public override ResultsTable? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            ResultsTable tableToBeCreated = new ResultsTable();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    if (propertyNameToInfoDictionary.TryGetValue(reader.GetString() ?? string.Empty, out PropertyInfo? info))
                    {
                        reader.Read();
                        if (info.PropertyType == typeof(Student[]))
                        {
                            Console.WriteLine("A!");
                        }
                        object? value = JsonSerializer.Deserialize(ref reader, info.PropertyType, options);


                        info.SetValue(tableToBeCreated, value);
                    }

                    continue;
                }

                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break; 
                }
            }

            return tableToBeCreated;
        }

        public override void Write(Utf8JsonWriter writer, ResultsTable value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (PropertyInfo propertyInfo in propertyNameToInfoDictionary.Values)
            {
                object property = propertyInfo.GetValue(value) ?? throw new SerializationException($"Could not serialize in {propertyInfo} for {value}");

                writer.WritePropertyName(propertyInfo.Name);

                var newOptions = new JsonSerializerOptions(options);

                newOptions.Converters.Add(new IgnorePropertyIfConverter<Question>(property => property.Name == "Data"));

                JsonSerializer.Serialize(writer, property, propertyInfo.PropertyType, newOptions);
            }

            writer.WriteEndObject();
        }
    }

    public class IgnorePropertyIfConverter<T> : JsonConverter<T>
    {
        private readonly Func<PropertyInfo, bool> propertyPredicate;

        public IgnorePropertyIfConverter(Func<PropertyInfo, bool> propertyPredicate)
        {
            this.propertyPredicate = propertyPredicate;
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<T>(ref reader, options) ?? throw new SerializationException("Could not serialize");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var property in typeof(T).GetProperties().Where(p => !propertyPredicate(p)))
            {
                writer.WritePropertyName(property.Name);
                JsonSerializer.Serialize(writer, property.GetValue(value), options);
            }

            writer.WriteEndObject();
        }
    }


}
