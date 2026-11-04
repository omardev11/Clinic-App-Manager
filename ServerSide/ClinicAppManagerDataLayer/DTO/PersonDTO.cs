using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.DTO
{
    public class PersonDTO
    {
        public int? PersonId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public int? Age { get; set; }
        public string? Address { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public char? Gender { get; set; }
    }
}
