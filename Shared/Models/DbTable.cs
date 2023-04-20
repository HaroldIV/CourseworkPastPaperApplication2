using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkPastPaperApplication2.Shared
{
    public abstract class DbTable
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object? obj) => obj switch
        {
            DbTable table => table == this,
            _ => base.Equals(obj)
        };

        public static bool operator ==(DbTable left, DbTable right) => left.Id == right.Id;

        public static bool operator !=(DbTable left, DbTable right) => left.Id != right.Id;
    }
}
