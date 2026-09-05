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
        public Guid Id { get; set; }

        public Guid TraceId { get; set; }

        [MaxLength(20)]
        public string? Imei { get; set; }
        public DateTimeOffset ReceivedDate { get; set; }

        [MaxLength(2000)]
        public string Message { get; set; } = string.Empty;
        public Severity Severity { get; set; }
    }

    public enum Severity
    {
        Info = 2,
        Warning = 3,
        Error = 4
    }
}
