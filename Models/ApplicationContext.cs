using MedBrat.Areas.Account.Models;
using MedBrat.Areas.Appointment.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MedBrat.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Patient> Patients { get; set; } = null!;
        public DbSet<Doctor> Doctors { get; set; } = null!;
        public DbSet<Curator> Curators { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<Clinic> Clinics { get; set; } = null!;
        public DbSet<Schedule> Schedules { get; set; } = null!;
        public DbSet<MedRecord> MedRecords { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Role adminRole = new Role { Id = 1, Name = "admin" };
            Role moderatorRole = new Role { Id = 2, Name = "moderator" };
            Role curatorRole = new Role { Id = 3, Name = "curator" };
            Role doctorRole = new Role { Id = 4, Name = "doctor" };
            Role patientRole = new Role { Id = 5, Name = "patient" };

            City city1 = new City { Id = 1, Name = "city1" };
            City city2 = new City { Id = 2, Name = "city2" };

            Clinic clinic1 = new Clinic { Id = 1, Name = "clinic1", Address = "address1", CityId = city1.Id };
            Clinic clinic2 = new Clinic { Id = 2, Name = "clinic2", Address = "address2", CityId = city1.Id };
            Clinic clinic3 = new Clinic { Id = 3, Name = "clinic3", Address = "address3", CityId = city2.Id };
            Clinic clinic4 = new Clinic { Id = 4, Name = "clinic4", Address = "address4", CityId = city2.Id };

            var adminUser = new User()
            {
                Id = 1,
                Name = "admin",
                Email = null,
                Password = "admin",
                Polis = "admin",
                DateOfBirth = new DateTime(2000, 1, 1),
                Sex = "m",
                RoleId = adminRole.Id
            };
            var moders = new List<User>();
            for (var i = 2; i <= 4; i++)
            {
                var n = i - 2;
                var moderator = new User()
                {
                    Id = i,
                    Name = "moderator" + n,
                    Email = null,
                    Password = "mod" + n,
                    Polis = "mod" + n,
                    DateOfBirth = new DateTime(2000, 1, i),
                    Sex = "m",
                    RoleId = moderatorRole.Id
                };
                moders.Add(moderator);
            }
            var curators = new List<Curator>();
            for (var i = 5; i <= 8; i++)
            {
                var n = i - 5;
                var curator = new Curator()
                {
                    Id = i,
                    Name = "curator" + n,
                    Email = null,
                    Password = "cur" + n,
                    Polis = "cur" + n,
                    DateOfBirth = new DateTime(2000, 1, i),
                    Sex = "w",
                    RoleId = curatorRole.Id,
                    ClinicId = n + 1
                };
                curators.Add(curator);
            }
            var doctors = new List<Doctor>();
            var schedules = new List<Schedule>();
            for (var i = 9; i <= 18; i++)
            {
                var n = i - 9;
                var weekSch = new Dictionary<int, List<TimeSpan>>();
                for (var j = 1; j <= 5; j++)
                {
                    var daySch = new List<TimeSpan>();
                    for (var k = 1; k <= 10; k++)
                    {
                        daySch.Add(TimeSpan.FromHours(j % 2 + 14) + TimeSpan.FromMinutes(15 * k));
                    }
                    weekSch[j] = daySch;
                }
                var schedule = new Schedule()
                {
                    Id = i-8,
                    DoctorId = i,
                    WeekSchedule = weekSch
                };
                schedules.Add(schedule);
                var doctor = new Doctor()
                {
                    Id = i,
                    Name = "doctor" + n,
                    Email = null,
                    Password = "doc" + n,
                    Polis = "doc" + n,
                    DateOfBirth = new DateTime(1970, 1, i),
                    Sex = "m",
                    RoleId = doctorRole.Id,
                    Specialization = "spec" + n,
                    ClinicId = (n % 4) + 1
                };
                doctors.Add(doctor);
            }
            var patients = new List<Patient>();
            for (var i = 19; i <= 28; i++)
            {
                var n = i - 19;
                var patient = new Patient()
                {
                    Id = i,
                    Name = "patient" + n,
                    Email = null,
                    Password = "pat" + n,
                    Polis = "pat" + n,
                    DateOfBirth = new DateTime(2000, 1, i),
                    Sex = "m",
                    RoleId = patientRole.Id
                };
                patients.Add(patient);
            }

            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, moderatorRole, curatorRole, doctorRole, patientRole });
            modelBuilder.Entity<City>().HasData(new City[] { city1, city2 });
            modelBuilder.Entity<Clinic>().HasData(new Clinic[] { clinic1, clinic2, clinic3, clinic4 });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });
            modelBuilder.Entity<User>().HasData(moders);
            modelBuilder.Entity<Curator>().HasData(curators);
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Doctors)
                .WithMany(d => d.Patients)
                .UsingEntity<MedRecord>(
                    j => j
                        .HasOne(pt => pt.Doctor)
                        .WithMany(t => t.MedRecords)
                        .HasForeignKey(pt => pt.DoctorId),
                    j => j
                        .HasOne(pt => pt.Patient)
                        .WithMany(p => p.MedRecords)
                        .HasForeignKey(pt => pt.PatientId),
                    j =>
                    {
                        j.HasKey(t => new { t.PatientId, t.DoctorId });
                        j.ToTable("MedRecords");
                        j.Property(r => r.Visits)
                        .HasConversion(
                            w => JsonConvert.SerializeObject(w),
                            w => JsonConvert.DeserializeObject<List<Visit>>(w));
                            });
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Doctors)
                .WithMany(d => d.Patients)
                .UsingEntity<MedTicket>(
                    j => j
                        .HasOne(pt => pt.Doctor)
                        .WithMany(t => t.MedTickets)
                        .HasForeignKey(pt => pt.DoctorId),
                    j => j
                        .HasOne(pt => pt.Patient)
                        .WithMany(p => p.MedTickets)
                        .HasForeignKey(pt => pt.PatientId),
                    j =>
                    {
                        j.HasKey(t => t.Id);
                        j.ToTable("MedTickets");
                    });
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Schedule)
                .WithOne(s => s.Doctor)
                .HasForeignKey<Schedule>(s => s.DoctorId);
            modelBuilder.Entity<Doctor>().HasData(doctors);
            modelBuilder.Entity<Patient>().HasData(patients);
            modelBuilder.Entity<Schedule>()
                .Property(s => s.WeekSchedule)
                .HasConversion(
                    w => JsonConvert.SerializeObject(w),
                    w => JsonConvert.DeserializeObject<Dictionary<int, List<TimeSpan>>>(w));
            
            modelBuilder.Entity<Schedule>().HasData(schedules);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
