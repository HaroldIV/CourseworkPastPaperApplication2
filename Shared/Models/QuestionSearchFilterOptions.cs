using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkPastPaperApplication2.Shared
{
    public record RadioOptions(Level[] Levels, ExamBoard[] ExamBoards);

    [TypeConverter(typeof(FilterOptionConverter<Level>))]
    public class Level : IFilterOption
    {
        public short Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [TypeConverter(typeof(FilterOptionConverter<ExamBoard>))]
    public class ExamBoard : IFilterOption
    {
        public short Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public interface IFilterOption
    {
        public short Id { get; set; }
        public string Name { get; set; }
    }

    public class FilterOptionConverter<T> : TypeConverter where T : IFilterOption, new()
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            /// May need to assign Id also, assuming not for now. 
            return new T { Name = (string)value };
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (value is null)
            {
                return value;
            }

            return ((T)value).Name;
        }
    }
}
