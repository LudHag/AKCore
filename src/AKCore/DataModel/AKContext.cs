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

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public DbSet<Page> Pages { get; set; }
        //public DbSet<User> Users { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<SubMenu> SubMenus { get; set; }
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
        [Required]
        [StringLength(450)]
        public string Path { get; set; }
        public string Content { get; set; }
        [Required]
        public AuthorityRank Authority { get; set; }
        public Page Parent { get; set; }
    }
    /**
    public class User 
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(450)]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Email { get; set; }
        [Required]
        public AuthorityRank Authority { get; set; }
    }
    */
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

    public enum AuthorityRank
    {
        Guest=1,
        User=2,
        Styrelse=3,
        SuperNintendo=4
    }
}