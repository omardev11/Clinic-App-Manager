using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerDataLayer.Entities
{
    public class Data
    {
        public int Id { get; set; }
        public string RefreshToken { get; set; }
       public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? RevokedDate { get; set; }

        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
    }
}
