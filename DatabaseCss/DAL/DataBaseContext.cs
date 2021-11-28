using System;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace DatabaseCss.DAL
{
    public class DataBaseContext : DbContext
    {
        public DbSet<CssFile> CssFiles { get; set; }

        public DataBaseContext(DbContextOptions<DataBaseContext> options)
            : base(options)
        {
        }
    }
    
    public class CssFile
    {
        public int Id { get; set; }
        
        public string Css { get; set; }
        
        public string Name { get; set; }
        
        public DateTimeOffset LastUpdateDate { get; set; }
    }

}