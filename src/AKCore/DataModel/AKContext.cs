using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AKCore.DataModel
{
    public class AKContext : IdentityDbContext<AkUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var path = Directory.GetCurrentDirectory();
            var connection = $"Filename={Path.Combine(path, "akdb.db")}";
            optionsBuilder.UseSqlite(connection);
        }

        public DbSet<Page> Pages { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<SubMenu> SubMenus { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<SignUp> SignUps { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Recruit> Recruits { get; set; }
    }

    public class Page
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(450)]
        public string Slug { get; set; }
        public string WidgetsJson { get; set; }
        [Required]
        public bool LoggedIn { get; set; }
        public bool LoggedOut { get; set; }
        public bool BalettOnly { get; set; }
    }
  
    public class Menu
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(450)]
        public string Name { get; set; }
        public Page Link { get; set; }
        public List<SubMenu> Children { get; set; }
        public int PosIndex { get; set; }
        public bool LoggedIn { get; set; }
    }

    public class SubMenu
    {
        [Key]
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
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public DateTime Created { get; set; }
        public DateTime Released { get; set; }
        public List<Track> Tracks { get; set; }
    }

    public class Track
    {
        [Key]
        public int Id { get; set; }
        public int Number { get; set; }
        public string FileName { get; set; }
        public DateTime Created { get; set; }
    }

    public class Recruit
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }//ta bort
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Instrument { get; set; }
        public string Other { get; set; }
        public DateTime Created { get; set; }
        public bool Archived { get; set; }
    }

    public class Event
    {
        [Key]
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
        public DateTime Halan { get; set; }
        public DateTime There { get; set; }
        public DateTime Starts { get; set; }
        public bool Stand { get; set; }
        public List<SignUp> SignUps { get; set; }

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
        public int Id { get; set; }
        [Required]
        public string Person { get; set; }
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

}