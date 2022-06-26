using System;
using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace AKCore.Controllers
{
    [Route("MailBox")]
    public class MailBoxController : Controller
    {
        private readonly AKContext _db;
        public MailBoxController(AKContext db)
        {
            _db = db;
        }

        [HttpPost]
        [Authorize(Roles = AkRoles.Medlem)]
        public ActionResult SaveMailBox(MailBoxModel model)
        {

            var mailBoxItem = new MailBoxItem
            {
               Archived = false,
               Created = DateTime.Now,
               Message = model.Message,
               Subject = model.Subject,
            };
            _db.MailBoxItems.Add(mailBoxItem);

            _db.SaveChanges();
            return
                Json(new { success = true, message = "Dit meddelande är skickat och kommer kunna ses av styrelsen" });
        }

        [HttpGet("GetItems")]
        [Authorize(Roles = AkRoles.SuperNintendo)]
        public ActionResult GetMailboxItems(bool archived = false)
        {
            var items = _db.MailBoxItems.Where(x=>x.Archived == archived).ToList();

            return Ok(items);
        }

    }
}
