using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecipeBlog.Models;

namespace RecipeBlog.Controllers
{
    public class RecipecategoriesController : Controller
    {
        private readonly ModelContext _context;

        public RecipecategoriesController(ModelContext context)
        {
            _context = context;
        }

        // GET: Recipecategories
        public async Task<IActionResult> Index()
        {
              return _context.Recipecategories != null ? 
                          View(await _context.Recipecategories.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Recipecategories'  is null.");
        }

        // GET: Recipecategories/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Recipecategories == null)
            {
                return NotFound();
            }

            var recipecategory = await _context.Recipecategories
                .FirstOrDefaultAsync(m => m.Categoryid == id);
            if (recipecategory == null)
            {
                return NotFound();
            }

            return View(recipecategory);
        }

        // GET: Recipecategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Recipecategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Categoryid,Categoryname")] Recipecategory recipecategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recipecategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(recipecategory);
        }

        // GET: Recipecategories/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Recipecategories == null)
            {
                return NotFound();
            }

            var recipecategory = await _context.Recipecategories.FindAsync(id);
            if (recipecategory == null)
            {
                return NotFound();
            }
            return View(recipecategory);
        }

        // POST: Recipecategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Categoryid,Categoryname")] Recipecategory recipecategory)
        {
            if (id != recipecategory.Categoryid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipecategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipecategoryExists(recipecategory.Categoryid))
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
            return View(recipecategory);
        }

        // GET: Recipecategories/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Recipecategories == null)
            {
                return NotFound();
            }

            var recipecategory = await _context.Recipecategories
                .FirstOrDefaultAsync(m => m.Categoryid == id);
            if (recipecategory == null)
            {
                return NotFound();
            }

            return View(recipecategory);
        }

        // POST: Recipecategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Recipecategories == null)
            {
                return Problem("Entity set 'ModelContext.Recipecategories'  is null.");
            }
            var recipecategory = await _context.Recipecategories.FindAsync(id);
            if (recipecategory != null)
            {
                _context.Recipecategories.Remove(recipecategory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipecategoryExists(decimal id)
        {
          return (_context.Recipecategories?.Any(e => e.Categoryid == id)).GetValueOrDefault();
        }
    }
}
