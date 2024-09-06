using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class SpecificationModelsController : Controller
    {
        public  JsonResult GetChildren(int id)
        {
            var model = _context.SpecificationModels
                .Include(b => b.Childrens)
                .AsNoTracking()
                .FirstOrDefault(b => b.Id == id);
            var childrens = model.Childrens.ToList();
            var bomModels = new List<SpecificationModel> { model };
            while (childrens.Any())
            {
                var newChildrens = new List<SpecificationModel>();
                foreach (var childId in childrens.Select(c => c.Id))
                {
                    var child =  _context.SpecificationModels
                        .Include(b => b.Parent)
                        .Include(b => b.Childrens)
                        .FirstOrDefault(b => b.Id == childId);
                    var allCount = child.CountForParent;
                    var parent = child.Parent;
                    while (parent != null)
                    {
                        allCount *= parent.CountForParent;
                        parent = parent.Parent;
                    }

                    child.AllCount = allCount;
                    bomModels.Add(child);
                    newChildrens.AddRange(child.Childrens);
                }
                childrens = newChildrens;

            }
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };

            //string tylerJson = JsonSerializer.Serialize(tyler, options);

            return Json(bomModels,options);
        }
        private readonly SpecificationContext _context;

        public SpecificationModelsController(SpecificationContext context)
        {
            _context = context;
        }

        // GET: SpecificationModels
        public async Task<IActionResult> Index()
        {
            var specifications = _context.SpecificationModels.Include(b => b.Parent);
              return View(await specifications.ToListAsync());
        }

        // GET: SpecificationModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SpecificationModels == null)
            {
                return NotFound();
            }

            var specificationModel = await _context.SpecificationModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specificationModel == null)
            {
                return NotFound();
            }

            return View(specificationModel);
        }

        // GET: SpecificationModels/Create
        public IActionResult Create()
        {
            ViewData["ParentId"] = new SelectList(_context.SpecificationModels, "Id", "Name");
            return View();
        }

        // POST: SpecificationModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParentId,Name,Description,CountForParent,MeasureUnit")] SpecificationModel specificationModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(specificationModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentId"] = new SelectList(_context.SpecificationModels, "Id", "Name");
            return View(specificationModel);
        }

        // GET: SpecificationModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SpecificationModels == null)
            {
                return NotFound();
            }

            var specificationModel = await _context.SpecificationModels.FindAsync(id);
            if (specificationModel == null)
            {
                return NotFound();
            }
            ViewData["ParentId"] = new SelectList(_context.SpecificationModels.Where(s => s.Id != id), "Id", "Name");
            return View(specificationModel);
        }

        // POST: SpecificationModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ParentId,Name,Description,CountForParent,MeasureUnit")] SpecificationModel specificationModel)
        {
            if (id != specificationModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specificationModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecificationModelExists(specificationModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentId"] = new SelectList(_context.SpecificationModels.Where(s => s.Id != id), "Id", "Name");
            return View(specificationModel);
        }

        // GET: SpecificationModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SpecificationModels == null)
            {
                return NotFound();
            }

            var specificationModel = await _context.SpecificationModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specificationModel == null)
            {
                return NotFound();
            }

            return View(specificationModel);
        }

        // POST: SpecificationModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SpecificationModels == null)
            {
                return Problem("Entity set 'SpecificationContext.SpecificationModels'  is null.");
            }
            var specificationModel = await _context.SpecificationModels.Include(a => a.Childrens).FirstOrDefaultAsync(a => a.Id == id);
            if (specificationModel != null)
            {
                //if(specificationModel.Childrens.Any())
                //{
                //    foreach (var item in specificationModel.Childrens)
                //    {
                //        item.ParentId = null;
                //    }
                //}
                _context.SpecificationModels.Remove(specificationModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpecificationModelExists(int id)
        {
          return _context.SpecificationModels.Any(e => e.Id == id);
        }
    }
}
