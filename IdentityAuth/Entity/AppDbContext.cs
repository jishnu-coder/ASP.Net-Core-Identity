using IdentityAuth.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAuth.Entity
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
                
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<Customer>().HasKey(m => m.Id);
            builder.Entity<Customer>().Property(m => m.Id).UseIdentityColumn();
            builder.Entity<Customer>().HasData(
                new Customer() { Id=1,Name="Sam",Gender="Male"},
                 new Customer() {Id=2, Name = "Alexa", Gender = "Female" },
                  new Customer() {Id=3, Name = "Krish", Gender = "Male" },
                   new Customer() { Id=4,Name = "Zaya", Gender = "Female" }
                );
            foreach(var foreignkey in builder.Model.GetEntityTypes().SelectMany(e=>e.GetForeignKeys()))
            {
                foreignkey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
        public DbSet<Customer> customers { get; set; }
    }
}
