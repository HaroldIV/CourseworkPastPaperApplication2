using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CourseworkPastPaperApplication2.Shared
{
    // Abstract base class for the Student and Teacher classes, defining the Name, Password, and PasswordAsHex properties of the class. 
    public abstract class User : DbTable
    {
        public User()
        {

        }

        public User(Guid id, string name, byte[] password)
        {
            Id = id;
            Name = name;
            Password = password;
        }

        [JsonConstructor]
        public User(Guid id, string name, string password)
        {
            Id = id;
            Name = name;
            PasswordAsHex = password;
        }

        public string Name { get; set; } = null!;
        public byte[] Password { get; set; } = null!;

        [JsonIgnore]
        public string PasswordAsHex { get => Encoding.UTF8.GetString(Password); set => Password = Encoding.UTF8.GetBytes(value); }

        // SQL defined function that converts a password as a string to its byte format
        // This can be used in SQL queries. 
        public static byte[] SQLStringToUtf8(string password)
        {
            return Encoding.UTF8.GetBytes(password);
        }
    }
}
