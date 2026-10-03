using Microsoft.AspNetCore.Mvc;
using Lektion7HTMLHelper.Models;

namespace Lektion7HTMLHelper.Controllers;

public class Exercise1Controller : Controller
{
    private readonly List<CountryItem> _countries;

    public Exercise1Controller()
    {
        _countries = new List<CountryItem>
        {
            new CountryItem { Name = "Denmark", Code = "DK", Capital = "Copenhagen", Sentence = "Hej! Velkommen til Danmark, håber du har en god dag." },
            new CountryItem { Name = "Sweden", Code = "SE", Capital = "Stockholm", Sentence = "Hej! Välkommen till Sverige, hoppas du får en fin dag." },
            new CountryItem { Name = "Norway", Code = "NO", Capital = "Oslo", Sentence = "Hei! Velkommen til Norge, håper du har en flott dag." },
            new CountryItem { Name = "Germany", Code = "DE", Capital = "Berlin", Sentence = "Hallo! Willkommen in Deutschland, ich hoffe du hast einen schönen Tag." },
            new CountryItem { Name = "United Kingdom", Code = "UK", Capital = "London", Sentence = "Hello! Welcome to the United Kingdom, hope you have a great day." },
            new CountryItem { Name = "United States", Code = "US", Capital = "Washington, D.C.", Sentence = "Hey there! Welcome to the United States, have a wonderful day." },
            new CountryItem { Name = "France", Code = "FR", Capital = "Paris", Sentence = "Bonjour! Bienvenue en France, passez une excellente journée." },
            new CountryItem { Name = "Spain", Code = "ES", Capital = "Madrid", Sentence = "¡Hola! Bienvenido a España, que tengas un excelente día." },
            new CountryItem { Name = "Italy", Code = "IT", Capital = "Rome", Sentence = "Ciao! Benvenuto in Italia, ti auguro una splendida giornata." },
            new CountryItem { Name = "Canada", Code = "CA", Capital = "Ottawa", Sentence = "Hello! Welcome to Canada / Bonjour! Bienvenue au Canada." },
            new CountryItem { Name = "Japan", Code = "JP", Capital = "Tokyo", Sentence = "こんにちは！日本へようこそ、良い一日をお過ごしください。" },
            new CountryItem { Name = "South Korea", Code = "KR", Capital = "Seoul", Sentence = "안녕하세요! 한국에 오신 것을 환영합니다." },
            new CountryItem { Name = "China", Code = "CN", Capital = "Beijing", Sentence = "你好！欢迎来到中国，祝你度过愉快的一天。" },
            new CountryItem { Name = "India", Code = "IN", Capital = "New Delhi", Sentence = "नमस्ते! भारत में आपका स्वागत है।" },
            new CountryItem { Name = "Thailand", Code = "TH", Capital = "Bangkok", Sentence = "สวัสดี! ยินดีต้อนรับสู่ประเทศไทย" },
            new CountryItem { Name = "Singapore", Code = "SG", Capital = "Singapore", Sentence = "Welcome to Singapore! 欢迎来到新加坡！" },
            new CountryItem { Name = "Vietnam", Code = "VN", Capital = "Hanoi", Sentence = "Xin chào! Chào mừng bạn đến với Việt Nam." }
        };
    }

    public IActionResult Index()
    {
        return View(_countries);
    }
}
