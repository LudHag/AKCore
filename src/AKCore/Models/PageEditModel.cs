using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AKCore.Models
{
    public class PageEditModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(450)]
        public string Slug { get; set; }
        public string Content { get; set; }
        [DisplayName("Kräver inloggning")]
        public bool LoggedIn { get; set; }
        public string Template { get; set; }
    }
}