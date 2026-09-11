using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpListenerProject.Entity;

namespace TcpListenerProject.Logging
{
    public class QueueSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly LogQueue _queue;
        public QueueSink(LogQueue queue, IFormatProvider? formatProvider = null)
        {
            _formatProvider = formatProvider;
            _queue = queue;
        }

        public void Emit(LogEvent logEvent)
        {
            Guid correlationId = Guid.CreateVersion7();
            if (logEvent.Properties.TryGetValue("CorrelationId", out var value))
            {
                if(value is ScalarValue scalar)
                {
                    if (scalar.Value is Guid guid)
                    {
                        correlationId = guid;
                    }
                }
            }

            _queue.TryEnqueue(new LogEntry
            {
                TraceId = correlationId,
                ReceivedDate = logEvent.Timestamp.ToUniversalTime(),
                Message = logEvent.RenderMessage(_formatProvider),
                Severity = logEvent.Level.ToString(),
            });
        }
    }
}
