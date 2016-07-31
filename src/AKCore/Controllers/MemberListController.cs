using System.Linq;
using AKCore.DataModel;
using AKCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AKCore.Controllers
{
    [Route("MemberList")]
    [Authorize]
    public class MemberListController : Controller
    {
        private readonly UserManager<AkUser> _userManager;

        public MemberListController(
            UserManager<AkUser> userManager
            )
        {
            _userManager = userManager;
        }

        public ActionResult Index(MemberListModel model)
        {
            ViewBag.Title = "Adressregister";
            PopulateModel(model);

            return View(model);
        }

        private void PopulateModel(MemberListModel model)
        {
            var userQuery = _userManager.Users.Where(x=>x.Instrument!=null);
            if (model.SearchPhrase != null)
            {
                userQuery = userQuery.Where(x => x.FirstName.Contains(model.SearchPhrase)
                                                 || x.LastName.Contains(model.SearchPhrase)
                                                 || x.Instrument.Contains(model.SearchPhrase)
                                                 || x.City.Contains(model.SearchPhrase)
                                                 || x.SlavPoster.Contains(model.SearchPhrase));
            }
            if (model.Instrument != null)
            {
                userQuery = userQuery.Where(x => x.Instrument == model.Instrument);
            }
            model.Users = userQuery.OrderBy(x=>x.FirstName).GroupBy(x=>x.Instrument).ToList();
            
        }
    }
}