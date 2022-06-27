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
            var items = _db.MailBoxItems.OrderByDescending(x=>x.Created).Where(x=>x.Archived == archived).ToList();

            return Ok(items);
        }


        [HttpPost("{id}/Archive")]
        [Authorize(Roles = AkRoles.SuperNintendo)]
        public ActionResult Archive(int id)
        {
            if (id < 0)
            {
                return Json(new { success = false });
            }
            var item = _db.MailBoxItems.FirstOrDefault(x => x.Id == id);
            if (item == null) return Json(new { success = false });
            item.Archived = !item.Archived;
            _db.SaveChanges();
            return Json(new { success = true });
        }
        [Authorize(Roles = AkRoles.SuperNintendo)]
        [HttpDelete("{id}")]
        public ActionResult Remove(int id)
        {
            if (id < 0)
            {
                return Json(new { success = false });
            }
            var item = _db.MailBoxItems.FirstOrDefault(x => x.Id == id);
            if (item == null) return Json(new { success = false });
            _db.MailBoxItems.Remove(item);
            _db.SaveChanges();
            return Json(new { success = true });
        }
    }
}
