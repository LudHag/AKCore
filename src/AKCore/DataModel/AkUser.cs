using AKCore.DataModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AKCore.DataModel
{
    public class AkUser : IdentityUser
    {
        public string GoogleId { get; set; }
    }
    
}

