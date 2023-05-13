using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkPastPaperApplication2.Shared
{
    // Abstract base class from which all classes which represent table (except for Level and ExamBoard since they are smaller tables). 
    public abstract class DbTable
    {
        // Each has its own ID. 
        public Guid Id { get; set; } = Guid.NewGuid();

        // The hash code of each object is only the ID's hashcode to allow different instances of the same class representing the same record within the database to work with hash sets and other hashing operations since the ID is guaranteed constant for each record. 
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        // Overides the equality functions to work solely off of their IDs to avoid reference checking equality comparisons. . 
        public override bool Equals(object? obj) => obj switch
        {
            DbTable table => table == this,
            _ => base.Equals(obj)
        };

        public static bool operator ==(DbTable left, DbTable right) => left.Id == right.Id;

        public static bool operator !=(DbTable left, DbTable right) => left.Id != right.Id;
    }
}
