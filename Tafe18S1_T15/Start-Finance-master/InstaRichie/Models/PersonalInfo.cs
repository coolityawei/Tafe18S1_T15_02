using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;
namespace StartFinance.Models
{
    public class PersonalInfo
    {
        //•	Personal Info (e.g. PersonalID, FirstName, LastName, DOB, Gender, Email Address, MobilePhone) 

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        public String FirstName { get; set; }
        public String LastName { get; set; }

        [NotNull]
        public String DateOfBirth { get; set; }
        public String Gender { get; set; }

        [Unique]
        public String Email { get; set; }
        public String PhoneNumber { get; set; }
    }
}
