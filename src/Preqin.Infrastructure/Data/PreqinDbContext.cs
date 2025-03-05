using Microsoft.EntityFrameworkCore;
using Preqin.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Preqin.Infrastructure.Data
{
    public class PreqinDbContext : DbContext
    {
        public PreqinDbContext(DbContextOptions<PreqinDbContext> options) : base(options) { }

        public DbSet<Investor> Investors { get; set; }
        public DbSet<Commitment> Commitments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Investor>().HasKey(i => i.Id);
            modelBuilder.Entity<Commitment>()
            .HasOne(c => c.Investor)
            .WithMany(i => i.Commitments)
            .HasForeignKey(c => c.InvestorId);
            modelBuilder.Entity<Investor>().HasMany(i => i.Commitments).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
