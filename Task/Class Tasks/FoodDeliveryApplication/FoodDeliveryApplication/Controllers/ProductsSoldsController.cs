using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FoodDeliveryApplication.Data;
using FoodDeliveryApplication.Models;

namespace FoodDeliveryApplication.Controllers
{
    public class ProductsSoldsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsSoldsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ProductsSolds
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.ProductsSold.Include(p => p.Sale);
            return View(await appDbContext.ToListAsync());
        }

        // GET: ProductsSolds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productsSold = await _context.ProductsSold
                .Include(p => p.Sale)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productsSold == null)
            {
                return NotFound();
            }

            return View(productsSold);
        }

        // GET: ProductsSolds/Create
        public IActionResult Create()
        {
            ViewData["SaleId"] = new SelectList(_context.Sales, "SaleId", "SaleId");
            return View();
        }

        // POST: ProductsSolds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,SaleId,Qty,TotalProductAmount,Status")] ProductsSold productsSold)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productsSold);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SaleId"] = new SelectList(_context.Sales, "SaleId", "SaleId", productsSold.SaleId);
            return View(productsSold);
        }

        // GET: ProductsSolds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productsSold = await _context.ProductsSold.FindAsync(id);
            if (productsSold == null)
            {
                return NotFound();
            }
            ViewData["SaleId"] = new SelectList(_context.Sales, "SaleId", "SaleId", productsSold.SaleId);
            return View(productsSold);
        }

        // POST: ProductsSolds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,SaleId,Qty,TotalProductAmount,Status")] ProductsSold productsSold)
        {
            if (id != productsSold.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productsSold);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsSoldExists(productsSold.Id))
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
            ViewData["SaleId"] = new SelectList(_context.Sales, "SaleId", "SaleId", productsSold.SaleId);
            return View(productsSold);
        }

        // GET: ProductsSolds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productsSold = await _context.ProductsSold
                .Include(p => p.Sale)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productsSold == null)
            {
                return NotFound();
            }

            return View(productsSold);
        }

        // POST: ProductsSolds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productsSold = await _context.ProductsSold.FindAsync(id);
            if (productsSold != null)
            {
                _context.ProductsSold.Remove(productsSold);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsSoldExists(int id)
        {
            return _context.ProductsSold.Any(e => e.Id == id);
        }
    }
}
