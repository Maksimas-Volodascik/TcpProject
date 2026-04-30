using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenerProject.Entity
{
    public class RawRecord
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [MaxLength(1500)]
        public string RawData { get; set; } = string.Empty;
        public DateTimeOffset ReceivedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset ExpiresAt { get; set; } = DateTimeOffset.UtcNow.AddDays(7);
        public Guid DeviceId { get; set; }
    }
}
