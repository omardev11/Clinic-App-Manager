using ClinicAppManagerDataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ClinicAppManagerDataLayer
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public  DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentStatus> AppointmentStatuses { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<PrescribedMedication> PrescribedMedications { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Data> Data { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach ( var relatinships in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relatinships.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<Payment>()
                        .Property(p => p.AmountPaid)
                        .HasPrecision(18, 2);
            modelBuilder.Entity<Person>().Property(x => x.Age).HasComputedColumnSql("DATEDIFF(YEAR, DateOfBirth, GETDATE())");
            modelBuilder.Entity<Person>().Property(x => x.FullName).HasComputedColumnSql("FirstName + ' ' + LastName", stored: true);
        }


    }
}
