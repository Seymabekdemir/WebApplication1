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

        // Kişi Ekleme Sayfası
        [HttpGet]
        public IActionResult KisiEkle() => View();

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

        // Kişi Silme Sayfası
        [HttpGet]
        public IActionResult KisiSil() => View();

        [HttpPost]
        public IActionResult KisiSil(int id)
        {
            var kisi = _context.Kisiler.FirstOrDefault(k => k.Id == id);
            if (kisi == null) return NotFound("Kişi bulunamadı.");

            _context.Kisiler.Remove(kisi);
            _context.SaveChanges();
            return Content("Kişi başarıyla silindi.");
        }

        // ✅ EKLENEN METOT — Canlı Arama Sonuçları (Silme için)
        [HttpGet]
        public IActionResult KisiAramaSonuclariSil(string query)
        {
            var kisiler = string.IsNullOrEmpty(query)
                ? new List<Kisi>()
                : _context.Kisiler
                    .Where(k => k.Ad.Contains(query) || k.Soyad.Contains(query))
                    .OrderBy(k => k.Ad)
                    .Take(10)
                    .ToList();

            return PartialView("_KisiListesiSil", kisiler);
        }

        // Güncelleme Sayfası (ID’siz)
        [HttpGet]
        public IActionResult KisiBilgileriniGuncelle() => View();

        // Canlı Arama (Güncelleme için)
        [HttpGet]
        public IActionResult KisiArama(string query)
        {
            var kisiler = string.IsNullOrEmpty(query)
                ? new List<Kisi>()
                : _context.Kisiler
                    .Where(k => k.Ad.Contains(query) || k.Soyad.Contains(query))
                    .OrderBy(k => k.Ad)
                    .Take(10)
                    .ToList();

            return PartialView("_KisiListesiGuncelle", kisiler);
        }

        // JSON kişi bilgisi
        [HttpGet]
        public IActionResult GetKisi(int id)
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

        // Güncelleme işlemi
        [HttpPost]
        public IActionResult KisiGuncelle(Kisi model)
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

            return Ok("Başarıyla güncellendi!");
        }

        // Görüntüleme sayfası
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

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_KisiListesi", kisiler.ToList());
            }

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
