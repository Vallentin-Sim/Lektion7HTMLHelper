using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lektion7HTMLHelper.Controllers;

public class Exercise1Controller : Controller
{
    private readonly List<SelectListItem> _countries;

    public Exercise1Controller()
    {
        _countries = new List<SelectListItem>
        {
            new SelectListItem { Text = "Denmark", Value = "DK" },
            new SelectListItem { Text = "Sweden", Value = "SE" },
            new SelectListItem { Text = "Norway", Value = "NO" },
            new SelectListItem { Text = "Germany", Value = "DE" },
            new SelectListItem { Text = "United Kingdom", Value = "UK" },
            new SelectListItem { Text = "United States", Value = "US" },
            new SelectListItem { Text = "France", Value = "FR" },
            new SelectListItem { Text = "Spain", Value = "ES" },
            new SelectListItem { Text = "Italy", Value = "IT" },
            new SelectListItem { Text = "Canada", Value = "CA" },
            new SelectListItem { Text = "Japan", Value = "JP" },
            new SelectListItem { Text = "South Korea", Value = "KR" },
            new SelectListItem { Text = "China", Value = "CN" },
            new SelectListItem { Text = "India", Value = "IN" },
            new SelectListItem { Text = "Thailand", Value = "TH" },
            new SelectListItem { Text = "Singapore", Value = "SG" },
            new SelectListItem { Text = "Vietnam", Value = "VN" }
        };
    }

    public IActionResult Index()
    {
        return View(_countries);
    }
}
