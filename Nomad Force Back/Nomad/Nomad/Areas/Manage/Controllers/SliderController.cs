using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Nomad.DAL;
using Nomad.Models;
using Nomad.Utilies.Extension;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nomad.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Sliders.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Slider slider)
        {
            slider.Image = await slider.Photo.SaveFileAsync(Path.Combine(_env.WebRootPath, "assets", "image"));
            await _context.AddAsync(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            Slider slider = _context.Sliders.Find(id);
            if (slider == null) return NotFound();
            if (System.IO.File.Exists(Path.Combine(_env.WebRootPath, "assets", "image", slider.Image)))
            {
                System.IO.File.Delete(Path.Combine(_env.WebRootPath, "assets", "image", slider.Image));
            }
            _context.Sliders.Remove(slider);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            Slider slider = _context.Sliders.FirstOrDefault(c => c.Id == id);
            if (slider == null) return NotFound();
            return View(slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Slider slider)
        {
            if (ModelState.IsValid)
            {
                var s = await _context.Sliders.FindAsync(slider.Id);
                s.FullName = slider.FullName;
                s.Profession = slider.Profession;
                if (slider.Photo != null)
                {
                    if (slider.Image != null)
                    {
                        string filePath = Path.Combine(_env.WebRootPath, "assets", "image", slider.Image);
                        System.IO.File.Delete(filePath);
                    }
                    s.Image = ProcessUploadedFile(slider);
                }
                _context.Update(s);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        private string ProcessUploadedFile(Slider slider)
        {
            string uniqueFileName = null;

            if (slider.Photo != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "assets", "imgs");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + slider.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    slider.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
