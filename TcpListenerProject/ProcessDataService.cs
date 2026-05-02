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

        public async Task<Device> GetDeviceByImeiAsync(string imei)
        {
            var device = _context.Set<Device>().FirstOrDefaultAsync(e => e.Imei.Equals(imei));

            return await device;
        }
    }
}
