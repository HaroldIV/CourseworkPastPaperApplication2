using CourseworkPastPaperApplication2.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CourseworkPastPaperApplication2.Shared;

public class Teacher : User
{
    public Teacher()
    {
    }

    public Teacher(Guid id, string name, byte[] password) : base(id, name, password)
    {
    }

    [JsonIgnore]
    public virtual ICollection<Class> Classes { get; } = new List<Class>();
}
