using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

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
        public DateTime Created { get; set; }
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
        public string Description { get; set; }
        public DateTime Day { get; set; }
        public DateTime Halan { get; set; }
        public DateTime There { get; set; }
        public DateTime Starts { get; set; }
        public bool Stand { get; set; }
    }

    public class SignUp
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public AkUser Person { get; set; }
        [Required]
        public Event Event { get; set; }
        public string Where { get; set; }
        public bool Car { get; set; }
        public bool Instrument { get; set; }
        public string Comment { get; set; }
    }

}