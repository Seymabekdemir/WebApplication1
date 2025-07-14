using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;



namespace WebApplication1.Models

{
    public class UygulamaDbContext : DbContext
    {
        public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Kisi> Kisiler { get; set; }

    }
}