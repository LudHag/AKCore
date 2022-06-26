using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AKCore.DataModel;

namespace AKCore.Models
{
    public class MailBoxModel
    {
        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

    }
}