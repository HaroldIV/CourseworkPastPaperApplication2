using CourseworkPastPaperApplication2.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CourseworkPastPaperApplication2.Shared;

// Class representing the Teacher table
// Names of properties indicate the field they represent except from for the collections which are representing any one-to-many/many-to-many links or properties not using the { get; set; } syntax as those are simply used for easier functionality in the class. 
// This class inherits from the User class which defines certain properties such as the Name, Password, and PasswordAsHex (hexadecimal representation of the passwrod). 
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
