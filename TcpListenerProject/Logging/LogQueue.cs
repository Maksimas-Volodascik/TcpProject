using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using TcpListenerProject.Entity;

namespace TcpListenerProject.Logging
{
    public class LogQueue
    {
        private readonly Channel<LogEntry> _channel = Channel.CreateBounded<LogEntry>(
            new BoundedChannelOptions(10_000)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true
            });

        public ChannelReader<LogEntry> Reader => _channel.Reader;

        public bool TryEnqueue(LogEntry log) => _channel.Writer.TryWrite(log);

    }
}
