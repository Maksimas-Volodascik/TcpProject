using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpListenerProject.Entity;

namespace TcpListenerProject
{
    public class DataContext : DbContext
    {
        public DbSet<RawRecord> RawRecords { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options){}
    }
}
