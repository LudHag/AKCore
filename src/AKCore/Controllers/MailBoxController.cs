using System;
using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using AKCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AKCore.Controllers
{
    [Route("MailBox")]
    public class MailBoxController : Controller
    {
        private readonly AKContext _db;
        private readonly MailBoxService _mailBoxService;

        public MailBoxController(AKContext db, MailBoxService mailBoxService)
        {
            _db = db;
            _mailBoxService = mailBoxService;
        }

        [HttpPost]
        [Authorize]
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
                Json(new { success = true, message = "Ditt meddelande är skickat och kommer kunna ses av styrelsen" });
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
        public async Task<ActionResult> Archive(int id)
        {
            var result = await _mailBoxService.ArchiveAsync(id, User.Identity.Name);
            return Json(new { success = result.Success });
        }

        [Authorize(Roles = AkRoles.SuperNintendo)]
        [HttpPost("{id}/Delete")]
        public async Task<ActionResult> Remove(int id)
        {
            var result = await _mailBoxService.RemoveAsync(id, User.Identity.Name);
            return Json(new { success = result.Success });
        }
    }
}
