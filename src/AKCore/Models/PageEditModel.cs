using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AKCore.DataModel;

namespace AKCore.Models
{
    public class PageEditModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(450)]
        public string Slug { get; set; }
        public string WidgetsJson { get; set; }
        [DisplayName("Kräver inloggning")]
        public bool LoggedIn { get; set; }
        [DisplayName("Enbart utloggad")]
        public bool LoggedOut { get; set; }
        [DisplayName("Enbart balett")]
        public bool BalettOnly { get; set; }
        public string Template { get; set; }
        public DateTime LastModified { get; set; }
        public IList<Album> Albums { get; set; }
        public IList<Widget> Widgets { get; set; }
    }
}