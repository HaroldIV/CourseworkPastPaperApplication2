using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CourseworkPastPaperApplication2.Shared;

// Class representing the Assignment table
// Names of properties indicate the field they represent except from for the collections which are representing any one-to-many/many-to-many links or properties not using the { get; set; } syntax as those are simply used for easier functionality in the class. 
public class Assignment : DbTable
{
    public DateOnly Set { get; set; }

    public DateOnly Due { get; set; }

    public string Name { get; set; } = null!;

    [JsonInclude]
    public int TotalMarks => Questions.Sum(question => question.Marks);

    [JsonInclude]
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    [JsonIgnore]
    public virtual Class Class { get; set; } = null!;
}