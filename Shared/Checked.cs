using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkPastPaperApplication2.Shared
{
    // Simple class used for Checkboxes to represent both the value and the state of the checkbox. 
    public class Checked<T>
    {
        public T Value { get; init; } = default!;
        public bool IsChecked { get; set; } = false;

        // Implicit converters between the value, two-way. 
        public static implicit operator Checked<T>(T value) => new Checked<T> { Value = value, IsChecked = false };
        public static implicit operator T(Checked<T> value) => value.Value;
    }
}