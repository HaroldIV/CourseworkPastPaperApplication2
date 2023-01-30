using System;
using System.Collections.Generic;

namespace CourseworkPastPaperApplication2.Shared;

public partial class Teacher
{
    public long Password { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Class> Classes { get; } = new List<Class>();
}
