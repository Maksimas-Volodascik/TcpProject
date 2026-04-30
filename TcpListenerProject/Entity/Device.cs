using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenerProject.Entity
{
    public class Device
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(15)]
        public string Imei { get; set; } = string.Empty;
    }
}
