using ClinicAppManagerDataLayer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.DTO
{
    public class UserDTO
    {
        public int? UserId { get; set; }
        public string? AspNetUserId { get; set; }
        public string? UserName { get; set; }
        public bool? IsActive { get; set; }
        public int? PersonId { get; set; }
        public PersonDTO Person { get; set; } = new PersonDTO();
    }

}
