using System.ComponentModel.DataAnnotations;
namespace AKCore.Models
{
    public class HireModel
    {
        [Display(Name = "Namn")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Epost")]
        [Required]
        public string Email { get; set; }

        [Display(Name = "Telefonnummer")]
        public string Tel { get; set; }

        [Display(Name = "Var och när vill ni boka oss")]
        public string Other { get; set; }
    }
}