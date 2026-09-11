using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenerProject.Entity
{
    public class LogEntry
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public Guid TraceId { get; set; }

        [MaxLength(20)]
        public string Imei { get; set; } = string.Empty;
        public DateTimeOffset ReceivedDate { get; set; }

        [MaxLength(2000)]
        public string Message { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
    }
}
