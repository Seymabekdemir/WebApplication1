using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UygulamaDbContext _context;

        public HomeController(ILogger<HomeController> logger, UygulamaDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Anasayfa
        public IActionResult Index()
        {
            return View();
        }

        // Kişi Ekleme Sayfası (GET)
        public IActionResult kişiekle()
        {
            return View();
        }

        // Kişi Ekleme İşlemi (POST)
        [HttpPost]
        public IActionResult kişiekle(Kisi yeniKisi)
        {
            if (ModelState.IsValid)
            {
                _context.Kisiler.Add(yeniKisi);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(yeniKisi);
        }

        // Kişi Silme Sayfası (GET)
        [HttpGet]
        [HttpGet]
        public IActionResult KisiSil()
        {
            return View();
        }

        [HttpPost]
        public IActionResult KisiSil(int id)
        {
            var kisi = _context.Kisiler.FirstOrDefault(k => k.Id == id);
            if (kisi == null)
            {
                ViewBag.Mesaj = "Bu ID'ye sahip kişi bulunamadı.";
                return View();
            }

            _context.Kisiler.Remove(kisi);
            _context.SaveChanges();
            ViewBag.Mesaj = "Kişi başarıyla silindi.";
            return View();
        }


        // Kişi Güncelleme Sayfası (GET)
        [HttpGet]
        public IActionResult kişibilgilerinigüncelle()
        {
            return View();
        }

        // ID ile Kişi Getirme (POST)
        [HttpPost]
        public IActionResult KisiGetir(int id)
        {
            var kisi = _context.Kisiler.FirstOrDefault(k => k.Id == id);
            if (kisi == null)
            {
                ViewBag.Mesaj = "Bu ID'ye sahip kişi bulunamadı.";
                return View("kişibilgilerinigüncelle");
            }

            return View("kişibilgilerinigüncelle", kisi);
        }

        // Kişi Güncelleme İşlemi (POST)
        [HttpPost]
        public IActionResult kişibilgilerinigüncelle(Kisi guncellenmisKisi)
        {
            var kisi = _context.Kisiler.FirstOrDefault(k => k.Id == guncellenmisKisi.Id);
            if (kisi == null)
            {
                ViewBag.Mesaj = "Bu ID'ye sahip kişi bulunamadı.";
                return View("kişibilgilerinigüncelle");
            }

            kisi.Ad = guncellenmisKisi.Ad;
            kisi.Soyad = guncellenmisKisi.Soyad;
            kisi.Email = guncellenmisKisi.Email;
            kisi.Departman = guncellenmisKisi.Departman;
            kisi.DogumTarihi = guncellenmisKisi.DogumTarihi;
            kisi.IsTanimi = guncellenmisKisi.IsTanimi;

            _context.SaveChanges();

            ViewBag.Mesaj = "Kişi başarıyla güncellendi.";
            return View("kişibilgilerinigüncelle", kisi);
        }

        // Kişi Görüntüleme Sayfası (GET)
        [HttpGet]
        public IActionResult KisiGoruntule()
        {
            ViewBag.Kisiler = _context.Kisiler.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult KisiGoruntule(int id)
        {
            var kisi = _context.Kisiler.FirstOrDefault(k => k.Id == id);
            ViewBag.Kisiler = _context.Kisiler.ToList();

            if (kisi == null)
            {
                ViewBag.Hata = "Bu ID'ye sahip kişi bulunamadı.";
                return View();
            }

            return View(kisi);
        }

        // Hata Sayfası
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
