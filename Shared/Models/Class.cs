using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CourseworkPastPaperApplication2.Shared;

// Class representing the Class table
// Names of properties indicate the field they represent except from for the collections which are representing any one-to-many/many-to-many links or properties not using the { get; set; } syntax as those are simply used for easier functionality in the class. 
public class Class : DbTable
{
    public Guid TeacherId { get; set; }

    public string Name { get; set; } = null!;

    public virtual Teacher TeacherNavigation { get; set; } = null!;

    public virtual ICollection<Student> Students { get; init; } = new List<Student>();

    public virtual ICollection<Assignment> Assignments { get; init; } = new List<Assignment>();
}
