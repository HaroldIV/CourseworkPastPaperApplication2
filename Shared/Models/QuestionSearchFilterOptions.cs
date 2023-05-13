using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CourseworkPastPaperApplication2.Shared
{
    // Class containing a list of levels and of exam boards. 
    public class RadioOptions
    {
        public Level[] Levels { get; set; }
        public ExamBoard[] ExamBoards { get; set; }

        public RadioOptions(Level[] Levels, ExamBoard[] ExamBoards)
        {
            this.Levels = Levels;
            this.ExamBoards = ExamBoards;
        }
    }

    // Level class that represents the Level table
    // TypeConverter attribute that indicates that the type can be converted using the FilterOptionConverter<Level> class. 
    [TypeConverter(typeof(FilterOptionConverter<Level>))]
    // Inherits from the IFilterOption interface
    public class Level : IFilterOption
    {
        // Uses a short Id as global uniqueness and count of possibilities are both far less important as there is no security risk for levels nor risk of using all 65536 possibilities. 
        public short Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Overrides the ToString method from the object class to return the JSON representation of the object. 
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    // Level class that represents the ExamBoard table
    // TypeConverter attribute that indicates that the type can be converted using the FilterOptionConverter<ExamBoard> class. 
    [TypeConverter(typeof(FilterOptionConverter<ExamBoard>))]
    // Inherits from the IFilterOption interface
    public class ExamBoard : IFilterOption
    {
        // Uses a short Id as global uniqueness and count of possibilities are both far less important as there is no security risk for levels nor risk of using all 65536 possibilities. 
        public short Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Overrides the ToString method from the object class to return the JSON representation of the object. 
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    // Backing interface for the Filter Options. 
    public interface IFilterOption
    {
        public short Id { get; set; }
        public string Name { get; set; }
    }

    // Type converter for filter options that allows for converting from and to the string format of a filter option. 
    public class FilterOptionConverter<T> : TypeConverter where T : IFilterOption, new()
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            /// May need to assign Id also, assuming not for now. 
            return JsonSerializer.Deserialize<T>((string)value);
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (value is null)
            {
                return value;
            }

            return JsonSerializer.Serialize(value);
        }
    }
}
