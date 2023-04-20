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

        public static byte[] SQLStringToUtf8(string password)
        {
            return Encoding.UTF8.GetBytes(password);
        }
    }
}
