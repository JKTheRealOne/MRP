using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class WarehouseRecordModelsController : Controller
    {
        private readonly SpecificationContext _context;

        public WarehouseRecordModelsController(SpecificationContext context)
        {
            _context = context;
        }

        // GET: WarehouseRecordModels
        public async Task<IActionResult> Index()
        {
            var specificationContext = _context.WarehouseRecordModel.Include(w => w.SpecificationModel);
            return View(await specificationContext.ToListAsync());
        }

        // GET: WarehouseRecordModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.WarehouseRecordModel == null)
            {
                return NotFound();
            }

            var warehouseRecordModel = await _context.WarehouseRecordModel
                .Include(w => w.SpecificationModel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (warehouseRecordModel == null)
            {
                return NotFound();
            }

            return View(warehouseRecordModel);
        }

        // GET: WarehouseRecordModels/Create
        public IActionResult Create()
        {
            ViewData["SpecificationModelId"] = new SelectList(_context.SpecificationModels, "Id", "Name");
            ViewData["MeasureUnits"] = new SelectList(_context.SpecificationModels, "Id", "MeasureUnit");
            ViewData["RecordTypes"] = new SelectList(Enum
                .GetValues(typeof(WarehouseRecordType))
                .Cast<WarehouseRecordType>()
                .Select(item => new { Id = (int)item, Name = item.GetEnumDescription() }),
                "Id", "Name");
            return View();
        }

        // POST: WarehouseRecordModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SpecificationModelId,RecordType,Quantity,MeasureUnit")] WarehouseRecordModel warehouseRecordModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(warehouseRecordModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecificationModelId"] = new SelectList(_context.SpecificationModels, "Id", "Name", warehouseRecordModel.SpecificationModelId);
            ViewData["MeasureUnits"] = new SelectList(_context.SpecificationModels, "Id", "MeasureUnit");
            ViewData["RecordTypes"] = new SelectList(Enum
                .GetValues(typeof(WarehouseRecordType))
                .Cast<WarehouseRecordType>()
                .Select(item => new { Id = (int)item, Name = item.GetEnumDescription() }),
                "Id", "Name");
            return View(warehouseRecordModel);
        }

        // GET: WarehouseRecordModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.WarehouseRecordModel == null)
            {
                return NotFound();
            }

            var warehouseRecordModel = await _context.WarehouseRecordModel.FindAsync(id);
            if (warehouseRecordModel == null)
            {
                return NotFound();
            }
            ViewData["SpecificationModelId"] = new SelectList(_context.SpecificationModels, "Id", "Name", warehouseRecordModel.SpecificationModelId);
            ViewData["RecordTypes"] = new SelectList(Enum
                .GetValues(typeof(WarehouseRecordType))
                .Cast<WarehouseRecordType>()
                .Select(item => new { Id = (int)item, Name = item.GetEnumDescription() }),
                "Id", "Name");
            return View(warehouseRecordModel);
        }

        // POST: WarehouseRecordModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SpecificationModelId,RecordType,Quantity,MeasureUnit")] WarehouseRecordModel warehouseRecordModel)
        {
            if (id != warehouseRecordModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(warehouseRecordModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WarehouseRecordModelExists(warehouseRecordModel.Id))
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
            ViewData["SpecificationModelId"] = new SelectList(_context.SpecificationModels, "Id", "Name", warehouseRecordModel.SpecificationModelId);
            ViewData["RecordTypes"] = new SelectList(Enum
                .GetValues(typeof(WarehouseRecordType))
                .Cast<WarehouseRecordType>()
                .Select(item => new { Id = (int)item, Name = item.GetEnumDescription() }),
                "Id", "Name");
            return View(warehouseRecordModel);
        }

        // GET: WarehouseRecordModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.WarehouseRecordModel == null)
            {
                return NotFound();
            }

            var warehouseRecordModel = await _context.WarehouseRecordModel
                .Include(w => w.SpecificationModel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (warehouseRecordModel == null)
            {
                return NotFound();
            }

            return View(warehouseRecordModel);
        }

        // POST: WarehouseRecordModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.WarehouseRecordModel == null)
            {
                return Problem("Entity set 'SpecificationContext.WarehouseRecordModel'  is null.");
            }
            var warehouseRecordModel = await _context.WarehouseRecordModel.FindAsync(id);
            if (warehouseRecordModel != null)
            {
                _context.WarehouseRecordModel.Remove(warehouseRecordModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Calculate()
        {
            var storedList = _context.WarehouseRecordModel.GroupBy(wr => wr.SpecificationModelId).Select(g => new WarehouseRecordModel()
            {
                SpecificationModel = _context.SpecificationModels.FirstOrDefault(s => s.Id == g.Key),
                Quantity = (decimal)g.Sum(wr => (double)wr.Quantity * (wr.RecordType == WarehouseRecordType.Убывание ? -1 : 1))
            });
            return View(storedList);
        }
        private bool WarehouseRecordModelExists(int id)
        {
          return (_context.WarehouseRecordModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
