using AKCore.DataModel;
using System;
using System.Collections.Generic;
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
        public bool LoggedIn { get; set; }
        public bool LoggedOut { get; set; }
        public bool BalettOnly { get; set; }
        public string Template { get; set; }
        public int PageId { get; set; }
        public Revision SelectedRevision { get; set; }
        public DateTime LastModified { get; set; }
        public IEnumerable<Album> Albums { get; set; }
        public IList<Widget> Widgets { get; set; }
        public IEnumerable<RevisionViewModel> Revisions { get; set; }
    }
    public class RevisionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public IList<Widget> Widgets { get; set; }
        public bool LoggedIn { get; set; }
        public bool LoggedOut { get; set; }
        public bool BalettOnly { get; set; }
        public string Modified { get; set; }
        public string ModifiedBy { get; set; }
    }
}