using Microsoft.EntityFrameworkCore;
using PaparaCase.Data.Configurations;
using PaparaCase.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaparaCase.Data.Context
{
    public class PaparaCaseDbContext : DbContext
    {
        public PaparaCaseDbContext(DbContextOptions<PaparaCaseDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FakeUserConfiguration());
        }

        DbSet<FakeUser> FakeUsers { get; set; }
    }
}
