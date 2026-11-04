using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("AspNetUser")]
        public string AspNetUserId { get; set; }

        public AppUser AspNetUser { get; set; }

        [MaxLength(50)]
        public string UserName { get; set; }
        public bool IsActive { get; set; } 

        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public Person Person { get; set; }  
    }
}
