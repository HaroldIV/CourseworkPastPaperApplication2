using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CourseworkPastPaperApplication2.Shared;

public class Student : User
{
    public Student()
    {

    }

    public Student(Guid id, string name, byte[] password) : base(id, name, password)
    {
    }

    public virtual ICollection<PaperResult> PaperResults { get; } = new List<PaperResult>();

    [JsonIgnore]
    public virtual ICollection<Class> CurrentClasses { get; } = new List<Class>();
}
