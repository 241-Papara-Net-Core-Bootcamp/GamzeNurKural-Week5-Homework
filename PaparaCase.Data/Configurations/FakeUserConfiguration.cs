using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaparaCase.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaparaCase.Data.Configurations
{
    public class FakeUserConfiguration : IEntityTypeConfiguration<FakeUser>
    {
        public void Configure(EntityTypeBuilder<FakeUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId);
            builder.Property(x => x.Body);
            builder.Property(x => x.Title);
            builder.ToTable("FakeUsers");
        }
    }
}
