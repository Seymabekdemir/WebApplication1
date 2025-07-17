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
        [HttpGet]
        public IActionResult KisiEkle()
        {
            return View();
        }

        // Kişi Ekleme İşlemi (POST)
        [HttpPost]
        public IActionResult KisiEkle(Kisi yeniKisi)
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

        // Yeni Kişi Görüntüleme Sayfası (GET + canlı arama)
        [HttpGet]
        public IActionResult KisiGoruntule(string searchTerm)
        {
            var kisiler = from k in _context.Kisiler select k;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                kisiler = kisiler.Where(k =>
                    k.Ad.ToLower().Contains(searchTerm) ||
                    k.Soyad.ToLower().Contains(searchTerm));
            }

            // Canlı arama için AJAX isteği ise sadece partial liste döner
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_KisiListesi", kisiler.ToList());
            }

            // Normal sayfa açılışı için full listeyi gönder
            return View(kisiler.ToList());
        }

        // Hata Sayfası
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}