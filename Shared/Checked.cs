using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkPastPaperApplication2.Shared
{
    public class Checked<T>
    {
        public T Value { get; init; } = default!;
        public bool IsChecked { get; set; } = false;

        public static implicit operator Checked<T>(T value) => new Checked<T> { Value = value, IsChecked = false };
        public static implicit operator T(Checked<T> value) => value.Value;
    }
}