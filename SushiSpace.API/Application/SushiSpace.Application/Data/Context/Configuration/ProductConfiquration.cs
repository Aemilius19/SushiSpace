using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SushiSpace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Application.Data.Context.Configuration
{
    public class ProductConfiquration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x=>x.ImgUrl).HasMaxLength(50);
            builder.Property(x=>x.Name).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Price).IsRequired();
        }
    }
}
