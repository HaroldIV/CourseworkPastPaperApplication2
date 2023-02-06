using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CourseworkPastPaperApplication2.Shared;

public partial class Teacher
{
    public string Name { get; set; } = null!;

    public long Password { get; set; }

    [JsonIgnore]
    public virtual ICollection<Class> Classes { get; } = new List<Class>();
}
