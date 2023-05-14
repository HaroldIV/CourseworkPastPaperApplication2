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
    // Attribute and implemented interface to indicate that this can be serialised. 
    [Serializable]
    // This inherits from the IEnumerable interface which makes an IEnumerable and so able to be natively looped through and manipulated with LINQ queries. 
    // This is a form of polymorphism without the limitations of polymorphism. 
    // However, this also creates an issue in that the default JsonSerializer class will recognise that this is a collection and try to serialise it as such. 
    // I have created two options for countering this:
    // A lightweight but slightly less performant option that is useful for single conversions: the NonEnumerableResultsTable class, more on that by the definition. 
    // The other option is to use the custom JSON serialisation converters implemented below, this requires additional memory and management, however, and only improves on performance by a minute amount and so is not the preferred method used in the codebase despite looking nicer to use. 

    // This class is used to represent the results for an assignment, it contains a string[][] containing all the scores and total marks for each question. 
    // These values could be calculated from the Question[] and Student[], however, this table is designed to be used for quickly rendering the tables and so pre-calculating these values makes the page more responsive. 
    public class ResultsTable : IEnumerable<IEnumerable<string>>, ISerializable, IResultsTable
    {
        // Instantiates a new ResultsTable object with empty values. 
        public ResultsTable()
        {
            Results = Array.Empty<string[]>();
            Questions = Array.Empty<Question>();
            Students = Array.Empty<Student>();
        }

        // Constructor that takes in results, questions, and students. 
        public ResultsTable(string[][] results, Question[] questions, Student[] students)
        {
            Results = results;
            Questions = questions;
            Students = students;
        }

        // Property that represents the string[][]. 
        public string[][] Results { get; set; }
        // Property that represents the Question[]
        public Question[] Questions { get; set; }
        // Property that represents the Student[]
        public Student[] Students { get; set; }

        // Count of rows of the table. 
        // JsonIgnore indicates that this property should not be serialised, this is done since it cannot be assigned to and is calulated from the Question[]. 
        [JsonIgnore]
        public int Rows => Questions.Length + 1;
        
        // Count of columns of the table. 
        // JsonIgnore indicates that this property should not be serialised, this is done since it cannot be assigned to and is calulated from the Student[]. 
        [JsonIgnore]
        public int Columns => Students.Length + 1;

        // Implicit converter between ResultsTable and string[][]. 
        // This operator will convert a table to a 2D string array. 
        public static implicit operator string[][](ResultsTable table)
        {
            // Initialises the array. 
            string[][] arr = new string[table.Rows - 1][];

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = new string[table.Columns - 1];
            }

            // Assigns the values of the array with values from the table. 
            for (int i = 0; i < table.Rows; i++)
            {
                for (int j = 0; j < table.Columns; j++)
                {
                    arr[i][j] = table[i, j];
                }
            }

            return arr;
        }

        // Conversion method from the IResultsTable backing interface, simply maps the properties. 
        public static ResultsTable FromIResultsTable(IResultsTable table)
        {
            return new ResultsTable { Questions = table.Questions, Results = table.Results, Students = table.Students };
        }

        // Indexer property, allows the table to be used as such: table[3, 7] to access values in the table, as though it were a 2D array. 
        public string this[int i, int j]
        {
            // Gets a value by pattern matching
            // Top left value is empty as indicated by the (0, 0) case. 
            // Rest of top row are students as indicated by the (0, int studentIndex) case. 
            // studentIndex matches the value of j. 
            // Left-most column are questions as indicated by the (int questionIndex, 0) case. 
            // (_, _) case simply matches anything that does not match any other case and so, of course, by elimination, must be a result. 
            get => (i, j) switch
            {
                (0, 0) => string.Empty,
                (0, int studentIndex) => Students[studentIndex - 1].Name,
                (int questionIndex, 0) => Questions[questionIndex - 1].FileName,
                (_, _) => Results[i - 1][j - 1]
            };

            // Same process as above but assigning instead of returning. 
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

        // Representation of this as an IEnumerable
        // Yields each row. 
        public IEnumerator<IEnumerable<string>> GetEnumerator()
        {
            for (int i = 0; i < this.Rows; i++)
            {
                yield return Enumerable.Range(0, this.Columns).Select(j => this[i, j]);
            }
        }

        // Required for the IEnumerable interface, links back to the GetEnumerator method defined above. 
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Used for de/serialisation, adds each property to the serialisation info such that it knows what to serialise. 
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Type resultsType = typeof(ResultsTable);

            resultsType.GetProperties().ForEach(property => info.AddValue(property.Name, property.GetValue(this)));
        }

        // Used for deserialisation, constructs a new instance using serialisation info and a streaming context. 
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected ResultsTable(SerializationInfo info, StreamingContext context)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Type resultsType = typeof(ResultsTable);

            // Gets all parameters to be used. 
            object?[] parameters = resultsType.GetProperties().Select(property => info.GetValue(property.Name, property.PropertyType)).ToArray();

            // Gets a constructor to call with the parameters given then calls it to instantiate the object. 
            ConstructorInfo? constructorInfo = resultsType.GetConstructor(resultsType.GetProperties().Select(property => property.PropertyType).ToArray());

            if (constructorInfo is null)
            {
                throw new SerializationException($"No constructor was found for given properties on {nameof(ResultsTable)}");
            }

            constructorInfo.Invoke(this, parameters);
        }
    }

    // Backing interface for the ResultsTable classes. 
    public interface IResultsTable
    {
        public Question[] Questions { get; set; }
        public Student[] Students { get; set; }
        public string[][] Results { get; set; }

        [JsonIgnore]
        public string this[int i, int j] { get; set; }
    }

    // Used for serialisation and deserialisation process. 
    // Can be implicitly converter to and from with the ResultsTable class, allowing for syntax such as `ResultsTable table = Deserialise<NonEnumerableResultsTable>()`, for example (this is not actual code, there is no Deserialise method with that signature in this codebase). 
    public class NonEnumerableResultsTable : IResultsTable
    {
        // Private instance of ResultsTable to store all the data. 
        private ResultsTable table = new ResultsTable();

        // All properties defined here have accessor methods that simply link back to the ResultsTable class. 
        public string this[int i, int j] { get => table[i, j] ; set => table[i, j] = value; }

        public Student[] Students { get => table.Students; set => table.Students = value; }
        public Question[] Questions
        {
            get => table.Questions;
            set => table.Questions = value;
        }

        public string[][] Results { get => table.Results; set => table.Results = value; }

        // Implicit conversion method between NonEnumerableResultsTable and ResultsTable. 
        public static implicit operator ResultsTable(NonEnumerableResultsTable nonEnumerableResultsTable)
        {
            // Returns the table field. 
            return nonEnumerableResultsTable.table;
        }

        // Implicit conversion method between ResultsTable and NonEnumerableResultsTable. 
        public static implicit operator NonEnumerableResultsTable(ResultsTable resultsTable)
        {
            // Instantiates a new NonEnumerableResultsTable with the table field set to the table being converted. 
            var nonEnumerableTable = new NonEnumerableResultsTable
            {
                table = resultsTable
            };

            return nonEnumerableTable;
        }
    }

    // ResultsTableInitialisationComponents is a class containing all parts needed to build a new ResultsTable, just used to store and transport data. 
    public class ResultsTableInitialisationComponents
    {
        public Student[] Students { get; set; } = null!;
        public Guid AssignmentId { get; set; }
        public string Name { get; set; } = null!;
        public Question[] Questions { get; set; } = null!;
    }

    // Custom ResultsTable converter for JSON. 
    public class ResultsTableJsonConverter : JsonConverter<ResultsTable>
    {   
        // Dictionary matching property names to their information. 
        private static readonly Dictionary<string, PropertyInfo> propertyNameToInfoDictionary;

        // Initialises the propertyNameToInfoDictionary with the types of ResultsTable. 
        static ResultsTableJsonConverter()
        {
            propertyNameToInfoDictionary = typeof(ResultsTable)
                .GetProperties()
                .Where(property => !property.GetIndexParameters().Any())
                .Where(property => !Attribute.IsDefined(property, typeof(JsonIgnoreAttribute)))
                .ToDictionary(property => property.Name);
        }

        // Reads in a new ResultsTable instance from a Utf8JsonReader instance and options. 
        /// Consider stopping Question from fully serialising to save on memory and improve speed. 
        public override ResultsTable? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Initialises a blank object. 
            ResultsTable tableToBeCreated = new ResultsTable();

            // Reads until file empty. 
            while (reader.Read())
            {
                // Checks if read a property name. 
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    // If read, gets the corresponding value from the dictionary and deserialises it from JSON then sets it as a property on tableToBeCreated. 
                    if (propertyNameToInfoDictionary.TryGetValue(reader.GetString() ?? string.Empty, out PropertyInfo? info))
                    {
                        reader.Read();
                        
                        object? value = JsonSerializer.Deserialize(ref reader, info.PropertyType, options);

                        info.SetValue(tableToBeCreated, value);
                    }

                    continue;
                }

                // If the object's JSON form has ended, stops searching. 
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break; 
                }
            }

            return tableToBeCreated;
        }

        // Writes the JSON format of a ResultsTable to a Utf8JsonWriter. 
        public override void Write(Utf8JsonWriter writer, ResultsTable value, JsonSerializerOptions options)
        {
            // Starts the JSON string. 
            writer.WriteStartObject();

            // Loops through properties and serialises each one then writes it. 
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

    // Custom JSON converter to ignore a property on write if a predicate is matched. 
    public class IgnorePropertyIfConverter<T> : JsonConverter<T>
    {
        // The specified prediate. 
        private readonly Func<PropertyInfo, bool> propertyPredicate;

        // Constructor to initialise with a predicate. 
        public IgnorePropertyIfConverter(Func<PropertyInfo, bool> propertyPredicate)
        {
            this.propertyPredicate = propertyPredicate;
        }

        // Since this only deals with Writing, uses the default reading operation. 
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<T>(ref reader, options) ?? throw new SerializationException("Could not serialize");
        }

        // Writes the JSON format of the object being serialised to a Utf8JsonWriter. 
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            // Starts the JSON string. 
            writer.WriteStartObject();

            // Loops through properties that match the predicate and serialises each one then writes it. 
            foreach (var property in typeof(T).GetProperties().Where(p => propertyPredicate(p)))
            {
                writer.WritePropertyName(property.Name);
                JsonSerializer.Serialize(writer, property.GetValue(value), options);
            }

            writer.WriteEndObject();
        }
    }


}
