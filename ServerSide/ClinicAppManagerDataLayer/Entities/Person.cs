using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.Entities
{
    [Index("FullName")]
    [Index("DateOfBirth")]
    public class Person
    {
        [Key]
        public int PersonId { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string LastName { get; set; }
        [MaxLength(100)]

        public string FullName { get; set; }
        public int Age { get ; set; }
        [MaxLength(100)]
        public string Address { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public char Gender {  get; set; }
    }
}
