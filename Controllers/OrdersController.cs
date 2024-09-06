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
    public class OrdersController : Controller
    {
        private readonly SpecificationContext _context;

        public OrdersController(SpecificationContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var specificationContext = _context.OrderModel.Include(w => w.SpecificationModel);
            return View(await specificationContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderModel == null)
            {
                return NotFound();
            }

            var orderModel = await _context.OrderModel
                .Include(w => w.SpecificationModel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderModel == null)
            {
                return NotFound();
            }

            return View(orderModel);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["SpecificationModelId"] = new SelectList(_context.SpecificationModels, "Id", "Name");
            ViewData["MeasureUnits"] = new SelectList(_context.SpecificationModels, "Id", "MeasureUnit");
            ViewData["OrderStatus"] = new SelectList(Enum
                .GetValues(typeof(OrderStatus))
                .Cast<OrderStatus>()
                .Select(item => new { Id = (int)item, Name = item.GetEnumDescription() }),
                "Id", "Name");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SpecificationModelId,OrderDate,OrderDateDeadline,Status,ClientName,Quantity,MeasureUnit")] OrderModel order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecificationModelId"] = new SelectList(_context.SpecificationModels, "Id", "Name", order.SpecificationModelId);
            ViewData["MeasureUnits"] = new SelectList(_context.SpecificationModels, "Id", "MeasureUnit");
            ViewData["OrderStatus"] = new SelectList(Enum
                .GetValues(typeof(OrderStatus))
                .Cast<OrderStatus>()
                .Select(item => new { Id = (int)item, Name = item.GetEnumDescription() }),
                "Id", "Name");
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrderModel == null)
            {
                return NotFound();
            }

            var order = await _context.OrderModel.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["SpecificationModelId"] = new SelectList(_context.SpecificationModels, "Id", "Name", order.SpecificationModelId);
            ViewData["MeasureUnits"] = new SelectList(_context.SpecificationModels, "Id", "MeasureUnit");
            ViewData["OrderStatus"] = new SelectList(Enum
                .GetValues(typeof(OrderStatus))
                .Cast<OrderStatus>()
                .Select(item => new { Id = (int)item, Name = item.GetEnumDescription() }),
                "Id", "Name");
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SpecificationModelId,OrderDate,OrderDateDeadline,Status,ClientName,Quantity,MeasureUnit")] OrderModel order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if(order.Status.ToString() == "Выполнен")
                {
                    WarehouseRecordModel warehouseRecordModel = new WarehouseRecordModel();
                    {
                        warehouseRecordModel.SpecificationModelId = order.SpecificationModelId;
                        warehouseRecordModel.RecordType = WarehouseRecordType.Убывание;
                        warehouseRecordModel.Quantity = order.Quantity;
                    }
                    _context.Add(warehouseRecordModel);
                    await _context.SaveChangesAsync();
                }
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["SpecificationModelId"] = new SelectList(_context.SpecificationModels, "Id", "Name", order.SpecificationModelId);
            ViewData["MeasureUnits"] = new SelectList(_context.SpecificationModels, "Id", "MeasureUnit");
            ViewData["OrderStatus"] = new SelectList(Enum
                .GetValues(typeof(OrderStatus))
                .Cast<OrderStatus>()
                .Select(item => new { Id = (int)item, Name = item.GetEnumDescription() }),
                "Id", "Name");
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderModel == null)
            {
                return NotFound();
            }

            var order = await _context.OrderModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrderModel == null)
            {
                return Problem("Entity set 'SpecificationContext.Order'  is null.");
            }
            var order = await _context.OrderModel.FindAsync(id);
            if (order != null)
            {
                _context.OrderModel.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.OrderModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> Calculate(int? id = null, DateTime? EndDate = null)
        {
            ViewData["EndDate"] = EndDate?.ToString("yyyy-MM-dd") ?? "";

            EndDate = EndDate.HasValue ? EndDate.Value : DateTime.MaxValue;
            
            var orderRecords = new List<OrderModel>();
            if (id != null)
            {
                var order = await _context.OrderModel
                    .Include(og => og.SpecificationModel)
                    .FirstOrDefaultAsync(og => og.Id == id);

                if (order == null)
                {
                    // Обработка случая, когда не найдена группа заказов с указанным orderGroupId
                    return NotFound();
                }
                    var existingSpecification = order.SpecificationModel;
                    var requiredCount = order.Quantity;
                    await CalculateOrderRecordsForSpecification(existingSpecification, requiredCount, orderRecords);
           
                return View(orderRecords);
            }
            else
            {
                var orderGroups = await _context.OrderModel
                        .Include(or => or.SpecificationModel)
                            .ThenInclude(spec => spec.Childrens) // Включаем дочерние спецификации
                    .Where(og => og.Status != OrderStatus.Выполнен)
                    .Where(og => og.OrderDateDeadline <= EndDate)
                    .ToListAsync();

                var specificationCounts = new Dictionary<int, decimal>(); // Словарь для хранения общего требуемого количества спецификаций

                foreach (var orderGroup in orderGroups)
                {
                    var existingSpecificationId = orderGroup.SpecificationModelId;
                    var requiredCount = orderGroup.Quantity;

                    if (specificationCounts.ContainsKey(existingSpecificationId))
                    {
                        // Спецификация уже присутствует в словаре, добавляем требуемое количество
                        specificationCounts[existingSpecificationId] += requiredCount;
                    }
                    else
                    {
                        // Спецификация еще не добавлена в словарь, устанавливаем требуемое количество
                        specificationCounts[existingSpecificationId] = requiredCount;
                    }
                }

                foreach (var specificationCount in specificationCounts)
                {
                    var specificationId = specificationCount.Key;
                    var requiredCount = specificationCount.Value;

                    var specification = await _context.SpecificationModels
                        .Include(spec => spec.Childrens) // Включаем дочерние спецификации
                        .FirstOrDefaultAsync(spec => spec.Id == specificationId);

                    await CalculateOrderRecordsForSpecification(specification, requiredCount, orderRecords);
                }
                return View(orderRecords);
            }
            return NoContent();
        }

        private async Task CalculateOrderRecordsForSpecification(SpecificationModel SpecificationModel, decimal requiredCount, List<OrderModel> orderRecords)
        {
            var specification = _context.SpecificationModels.Include(s => s.Childrens).First(s => s.Id == SpecificationModel.Id);
            var totalCount = await GetTotalCountForSpecification(specification);
            var availableCount = totalCount >= 0 ? totalCount : 0;
            var missingCount = Math.Max(requiredCount - availableCount, 0);

            if (missingCount > 0 && !specification.Childrens.Any())
            {
                var newOrderRecord = new OrderModel
                {
                    SpecificationModelId = specification.Id,
                    SpecificationModel = specification,
                    Quantity = missingCount
                };

                orderRecords.Add(newOrderRecord);
            }
            else if (specification.Childrens != null)
            {
                foreach (var childSpecification in specification.Childrens)
                {
                    var childRequiredCount = missingCount * (childSpecification.CountForParent);
                    await CalculateOrderRecordsForSpecification(childSpecification, childRequiredCount, orderRecords);
                }
            }
        }

        private async Task<decimal> GetTotalCountForSpecification(SpecificationModel specification)
        {
            var totalCount = 0m;

            var warehouseRecords = await _context.WarehouseRecordModel
                .Where(wr => wr.SpecificationModelId == specification.Id)
                .ToListAsync();

            totalCount = warehouseRecords.Sum(wr => wr.Quantity *(wr.RecordType == WarehouseRecordType.Убывание ? -1 : 1));

            return totalCount;
        }
    }
}
