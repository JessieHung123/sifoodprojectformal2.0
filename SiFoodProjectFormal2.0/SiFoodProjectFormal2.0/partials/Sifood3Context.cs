using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace SiFoodProjectFormal2._0.Partials;


    public partial class SifoodContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration=new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();
                optionsBuilder.UseLazyLoadingProxies().UseSqlServer(
                    configuration.GetConnectionString("Sifood"));
            }
        }
    }

