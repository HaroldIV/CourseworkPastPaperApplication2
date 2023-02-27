using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkPastPaperApplication2.Shared
{
    public abstract class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public byte[] Password { get; set; } = null!;

        public string PasswordAsHex { set => Password = Encoding.UTF8.GetBytes(value); }

        public static byte[] SQLStringToUtf8(string password)
        {
            return Encoding.UTF8.GetBytes(password);
        }
    }
}
