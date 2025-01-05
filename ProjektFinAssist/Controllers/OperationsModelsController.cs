using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjektFinAssist.Data;
using ProjektFinAssist.Models;

namespace ProjektFinAssist.Controllers
{
    [Authorize]
    public class OperationsModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OperationsModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OperationsModels
        public async Task<IActionResult> Index()
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(await _context.OperationsModel
                .Include(s => s.IdentityUser)
                .Where(s => s.UserID == userID)
                .ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetExpenseData()
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var expenseData = await _context.OperationsModel
                .Where(o => o.UserID == userID && o.Type == Models.Type.Expense)
                .GroupBy(o => o.Category)
                .Select(g => new
                {
                    Category = g.Key.ToString(),
                    TotalAmount = g.Sum(o => o.Amount)
                })
                .ToListAsync();

            return Json(expenseData);
        }

        public async Task<IActionResult> UserPanel()
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var expenseSumsByCategory = await _context.OperationsModel
                .Where(o => o.UserID == userID && o.Type == Models.Type.Expense)
                .GroupBy(o => o.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(o => o.Amount)
                })
                .ToListAsync();

            ViewBag.ExpenseSumsByCategory = expenseSumsByCategory;

            return View();
        }
        public async Task<IActionResult> History(string categoryFilter, string typeFilter, DateTime? startDate, DateTime? endDate)
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var operations = _context.OperationsModel
                .Where(o => o.UserID == userID);

            if (!string.IsNullOrEmpty(categoryFilter) && Enum.TryParse(categoryFilter, out Category categoryEnum))
            {
                operations = operations.Where(o => o.Category == categoryEnum);
            }

            if (!string.IsNullOrEmpty(typeFilter) && Enum.TryParse(typeFilter, out Models.Type typeEnum))
            {
                operations = operations.Where(o => o.Type == typeEnum);
            }

            if (startDate.HasValue)
            {
                operations = operations.Where(o => o.Date >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                operations = operations.Where(o => o.Date <= endDate.Value);
            }

            var filteredOperations = await operations.ToListAsync();

            ViewBag.CategoryList = Enum.GetValues(typeof(Category))
                .Cast<Category>()
                .Select(c => c.ToString())
                .ToList();

            ViewBag.TypeList = Enum.GetValues(typeof(Models.Type))
                .Cast<Models.Type>()
                .Select(t => t.ToString())
                .ToList();

            return View(filteredOperations);
        }

        // GET: OperationsModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var operation = await _context.OperationsModel.FindAsync(id);
            if (operation == null || operation.UserID != userID)
            {
                return NotFound();
            }
            return View(operation);
        }

        // GET: OperationsModels/Create
        public IActionResult Create()
        {
            ViewBag.CategoryList = Enum.GetValues(typeof(Models.Category))
                               .Cast<Models.Category>()
                               .Select(c => new SelectListItem
                               {
                                   Value = c.ToString(),
                                   Text = c.ToString()
                               })
                               .ToList();

            ViewBag.TypeList = Enum.GetValues(typeof(Models.Type))
                                   .Cast<Models.Type>()
                                   .Select(t => new SelectListItem
                                   {
                                       Value = t.ToString(),
                                       Text = t.ToString()
                                   })
                                   .ToList();

            return View();
        }

        // POST: OperationsModels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Category,Type,Name,Amount,Date,UserID")] OperationsModel operationsModel)
        {
            if (ModelState.IsValid)
            {
                operationsModel.UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(operationsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(operationsModel);
        }

        // GET: OperationsModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var operation = await _context.OperationsModel.FindAsync(id);
            if (operation == null || operation.UserID != userID)
            {
                return NotFound();
            }

            ViewBag.CategoryList = Enum.GetValues(typeof(Models.Category))
                               .Cast<Models.Category>()
                               .Select(c => new SelectListItem
                               {
                                   Value = c.ToString(),
                                   Text = c.ToString(),
                                   Selected = c == operation.Category
                               })
                               .ToList();

            ViewBag.TypeList = Enum.GetValues(typeof(Models.Type))
                                   .Cast<Models.Type>()
                                   .Select(t => new SelectListItem
                                   {
                                       Value = t.ToString(),
                                       Text = t.ToString(),
                                       Selected = t == operation.Type
                                   })
                                   .ToList();

            return View(operation);
        }

        // POST: OperationsModels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Category,Type,Name,Amount,Date,UserID")] OperationsModel operationsModel)
        {
            if (id != operationsModel.ID)
            {
                return NotFound();
            }

            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (operationsModel.UserID != userID)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(operationsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OperationsModelExists(operationsModel.ID))
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
            return View(operationsModel);
        }

        // GET: OperationsModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var operation = await _context.OperationsModel.FindAsync(id);
            if (operation == null || operation.UserID != userID)
            {
                return NotFound();
            }
            return View(operation);
        }

        // POST: OperationsModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var operationsModel = await _context.OperationsModel.FindAsync(id);
            if (operationsModel != null && operationsModel.UserID == userID)
            {
                _context.OperationsModel.Remove(operationsModel);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool OperationsModelExists(int id)
        {
            return _context.OperationsModel.Any(e => e.ID == id);
        }
    }
}