using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AKCore.DataModel
{
    public class AKContext : IdentityDbContext<AkUser>
    {
        public AKContext(DbContextOptions<AKContext> options)
          :base(options)
        {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AkUser>()
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<Page> Pages { get; set; }
        public DbSet<Revision> Revisions { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<SubMenu> SubMenus { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<SignUp> SignUps { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Recruit> Recruits { get; set; }
        public DbSet<Hire> Hires { get; set; }
        public DbSet<LogItem> Log { get; set; }
    }

    public class Page
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(450)]
        public string Slug { get; set; }
        public string MetaDescription { get; set; }
        public string WidgetsJson { get; set; }
        [Required]
        public bool LoggedIn { get; set; }
        public bool LoggedOut { get; set; }
        public bool BalettOnly { get; set; }
        public DateTime LastModified { get; set; }
        public List<Revision> Revisions { get; set; }
    }

    public class Revision
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(450)]
        public string Slug { get; set; }
        public string WidgetsJson { get; set; }
        public bool LoggedIn { get; set; }
        public bool LoggedOut { get; set; }
        public bool BalettOnly { get; set; }
        public DateTime Modified { get; set; }
        public AkUser ModifiedBy { get; set; }
    }

    public class Menu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(450)]
        public string Name { get; set; }
        public Page Link { get; set; }
        public List<SubMenu> Children { get; set; }
        public int PosIndex { get; set; }
        public bool LoggedIn { get; set; }
        public bool Balett { get; set; }
    }

    public class SubMenu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(450)]
        public string Name { get; set; }
        public Page Link { get; set; }
        public int SubPosIndex { get; set; }
    }
    public class Media
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Tag { get; set; }
        public DateTime Created { get; set; }
        public string GetExtension()
        {
            return Name.Split('.').Last();
        }
    }

    public class Album
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Year { get; set; }
        public string Category { get; set; }
        public DateTime Created { get; set; }
        public DateTime Released { get; set; }
        public List<Track> Tracks { get; set; }
    }

    public class Track
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Number { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }

        public string GetDisplayName()
        {
            if (!string.IsNullOrWhiteSpace(Name)) return Name;
            var parts = FileName.Split('.');
            return parts[^2].Replace('_', ' ');
        }
    }

    public class Recruit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Instrument { get; set; }
        public string Other { get; set; }
        public DateTime Created { get; set; }
        public bool Archived { get; set; }
    }

    public class Hire
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public string Other { get; set; }
        public DateTime Created { get; set; }
        public bool Archived { get; set; }
    }

    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(450)]
        public string Type { get; set; }
        [Required]
        [StringLength(450)]
        public string Name { get; set; }
        public string Place { get; set; }
        public string Description { get; set; }
        public string InternalDescription { get; set; }
        public string Fika { get; set; }
        public DateTime Day { get; set; }
        public TimeSpan HalanTime { get; set; }
        public TimeSpan ThereTime { get; set; }
        public TimeSpan StartsTime { get; set; }
        public string Stand { get; set; }
        public List<SignUp> SignUps { get; set; }
        public bool Secret { get; set; }

        public bool HasNoDescription()
        {
            return string.IsNullOrWhiteSpace(Description) && string.IsNullOrWhiteSpace(InternalDescription);
        }

        public int CanCome()
        {
            return SignUps?.Count(x => x.Where != AkSignupType.CantCome) ?? 0;
        }
        public int CantCome()
        {
            return SignUps?.Count(x => x.Where == AkSignupType.CantCome) ?? 0;
        }
    }

    public class SignUp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Person { get; set; }
        [Required]
        public string PersonId { get; set; }
        public string PersonName { get; set; }
        public string Where { get; set; }
        public bool Car { get; set; }
        public bool Instrument { get; set; }
        public string InstrumentName { get; set; }
        public string OtherInstruments { get; set; }
        public string Comment { get; set; }
        public DateTime SignupTime { get; set; }

        public string GetInfo()
        {
            var info = Where;
            if (Instrument)
            {
                info += ", har instrument";
            }
            if (Car)
            {
                info += ", har bil";
            }
            return info;
        }
    }
    public class LogItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }
        public DateTime Modified { get; set; }
        public AkUser ModifiedBy { get; set; }
    }
}