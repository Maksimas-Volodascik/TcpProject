using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpListenerProject.Entity;

namespace TcpListenerProject.Processing
{
    public interface IProcessDataService
    {
        Task<Device?> GetDeviceByImeiAsync(string imei);
        Task<string?> SaveRawRecordAsync(string imei, string rawMessage, string? parsedMessage);
    }
}
