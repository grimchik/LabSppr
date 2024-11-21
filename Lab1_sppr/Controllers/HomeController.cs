using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab1_sppr.Controllers
{
    public class ListDemo
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class HomeController : Controller
    {
        // GET: Home
        public IActionResult Index()
        {
            ViewData["TitleText"] = "Лабораторная работа №2";

            var listItems = new List<ListDemo>
            {
                new ListDemo { Id = 1, Name = "Элемент 1" },
                new ListDemo { Id = 2, Name = "Элемент 2" },
                new ListDemo { Id = 3, Name = "Элемент 3" }
            };

            ViewBag.ListItems = new SelectList(listItems, "Id", "Name");

            return View();
        }

        [HttpPost]
        public IActionResult Index(int selectedItem)
        {
            ViewData["SelectedItem"] = selectedItem;

            ViewData["TitleText"] = "Лабораторная работа №2";

            var listItems = new List<ListDemo>
            {
                new ListDemo { Id = 1, Name = "Элемент 1" },
                new ListDemo { Id = 2, Name = "Элемент 2" },
                new ListDemo { Id = 3, Name = "Элемент 3" }
            };

            ViewBag.ListItems = new SelectList(listItems, "Id", "Name");

            return View();
        }
    }
}
