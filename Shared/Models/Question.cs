using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CourseworkPastPaperApplication2.Shared;

// Class representing the Question table
// Names of properties indicate the field they represent except from for the collections which are representing any one-to-many/many-to-many links or properties not using the { get; set; } syntax as those are simply used for easier functionality in the class. 
public class Question : DbTable
{
    public string FileName { get; set; } = null!;

    public int Marks { get; set; }

    public byte[] Data { get; set; } = null!;

    public string ReadData { get; set; } = null!;

    public ExamBoard? ExamBoard { get; set; } = null!;

    public Level? Level { get; set; } = null!;
}