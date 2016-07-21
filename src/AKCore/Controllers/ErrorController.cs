using Microsoft.AspNetCore.Mvc;

namespace AKCore.Controllers
{
    public class ErrorController : Controller
    {
        public override NotFoundResult NotFound()
        {

            return new NotFoundResult();
        }
    }
}