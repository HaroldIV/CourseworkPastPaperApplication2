using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CourseworkPastPaperApplication2.Shared;

// Class representing the PaperResult table
// Names of properties indicate the field they represent except from for the collections which are representing any one-to-many/many-to-many links or properties not using the { get; set; } syntax as those are simply used for easier functionality in the class. 
public class PaperResult : DbTable
{
    public int Score { get; set; }

    public Guid AssignmentId { get; set; }

    public Guid StudentId { get; set; }

    public Guid QuestionId { get; set; }

    [JsonIgnore]
    public virtual Assignment Assignment { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;

    [JsonIgnore]
    public virtual Student Student { get; set; } = null!;
}
