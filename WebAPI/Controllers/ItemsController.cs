using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
	public class ItemsController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
