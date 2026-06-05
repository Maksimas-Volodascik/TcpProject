using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpListenerProject.Entity;

namespace TcpListenerProject
{
    public class ProcessDataService : IProcessDataService
    {
        private readonly DataContext _context;
        public ProcessDataService(DataContext context)
        {
            _context = context;
        }

        public async Task<Device?> GetDeviceByImeiAsync(string imei)
        {
            if (string.IsNullOrWhiteSpace(imei))
            {
                throw new ArgumentException("IMEI cannot be null, empty, or whitespace.");
            }
            
            var device = await _context.Set<Device>().FirstOrDefaultAsync(e => e.Imei == imei);

            if (device == null)
            {
                throw new ArgumentException("Device is not registered on the database");
            }

            return device;
        }

        public async Task<string?> SaveRawRecordAsync(string imei, string rawMessage, string? parsedMessage)
        {
            if (string.IsNullOrWhiteSpace(imei))
                return null;

            if (string.IsNullOrWhiteSpace(rawMessage))
                return null;

            if (string.IsNullOrEmpty(parsedMessage))
                parsedMessage = "{}";

            Device? device = await GetDeviceByImeiAsync(imei);
            if (device is null)
                return null;

            var record = new RawRecord
            {
                RawData = rawMessage,
                ParsedData = parsedMessage,
                ReceivedAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.AddYears(1),   // expires 1 year from now
                DeviceId = device.Id
            };

            try
            {
                _context.Set<RawRecord>().Add(record);
                await _context.SaveChangesAsync();
                return rawMessage; 
            }
            catch
            {
                return null;   
            }
        }
    }
}
