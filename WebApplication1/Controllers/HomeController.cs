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

        // GÜNCELLEME SAYFASI (GET)
        [HttpGet]
        public IActionResult KisiBilgileriniGuncelle(int id)
        {
            var kisi = _context.Kisiler.FirstOrDefault(k => k.Id == id);
            if (kisi == null) return NotFound();

            // View içine TEK bir kişi modeli gönderiyoruz, partial değil normal view
            return View("KisiBilgileriniGuncelle", kisi);
        }

        // GÜNCELLEME İŞLEMİ (POST)
        [HttpPost]
        // Arama sonucu partial view döner
        [HttpGet]
        public IActionResult KisiAramaSonuclari(string query)
        {
            if (string.IsNullOrEmpty(query))
                return PartialView("_KisiListesiGuncelle", new List<Kisi>());

            var kisiler = _context.Kisiler
                .Where(k => k.Ad.Contains(query) || k.Soyad.Contains(query))
                .OrderBy(k => k.Ad)
                .Take(10)
                .ToList();

            return PartialView("_KisiListesiGuncelle", kisiler);
        }

        // Seçilen kişinin detayını JSON olarak döner
        [HttpGet]
        public IActionResult GetKisiById(int id)
        {
            var kisi = _context.Kisiler.FirstOrDefault(k => k.Id == id);
            if (kisi == null)
                return NotFound();

            return Json(new
            {
                id = kisi.Id,
                ad = kisi.Ad,
                soyad = kisi.Soyad,
                email = kisi.Email,
                departman = kisi.Departman,
                dogumTarihi = kisi.DogumTarihi.ToString("yyyy-MM-dd"),
                isTanimi = kisi.IsTanimi
            });
        }

        // Güncelleme işlemi POST
        [HttpPost]
        public IActionResult KisiBilgileriniGuncelle(Kisi model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Geçersiz veri");

            var kisi = _context.Kisiler.FirstOrDefault(k => k.Id == model.Id);
            if (kisi == null)
                return NotFound();

            kisi.Ad = model.Ad;
            kisi.Soyad = model.Soyad;
            kisi.Email = model.Email;
            kisi.Departman = model.Departman;
            kisi.DogumTarihi = model.DogumTarihi;
            kisi.IsTanimi = model.IsTanimi;

            _context.SaveChanges();

            return Ok("Başarıyla güncellendi");
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
