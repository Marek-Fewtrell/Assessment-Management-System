using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assessment_Management_System.Data;
using Assessment_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Assessment_Management_System.Controllers
{
    [Authorize]
    public class AssessmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _manager;

        public AssessmentsController(ApplicationDbContext context, UserManager<ApplicationUser> manager)
        {
            _context = context;
            _manager = manager;
        }

        // GET: Assessments
        public async Task<IActionResult> Index()
        {
            var user = await _manager.GetUserAsync(HttpContext.User);
            IQueryable<Assessment> applicationDbContext;
            
            //if teacher
            if (await _manager.IsInRoleAsync(user, "teacher"))
            {
                //applicationDbContext = _context.Assessment.Include(a => a.ApplicationUser).Where(a => a.teacherID == user.Id);
                applicationDbContext = _context.Assessment.Include(a => a.ApplicationUser);
            }
            else
            {
                //if student
                applicationDbContext = _context.Assessment.Include(a => a.ApplicationUser);
            }

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Assessments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assessment = await _context.Assessment.Include(a=> a.ApplicationUser).SingleOrDefaultAsync(m => m.ID == id);
            if (assessment == null)
            {
                return NotFound();
            }

            return View(assessment);
        }

        // GET: Assessments/Create
        [Authorize(Roles = "admin, teacher")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Assessments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> Create([Bind("ID,Description,DueDate,Title")] Assessment assessment)
        {
            if (ModelState.IsValid)
            {
                var user = await _manager.GetUserAsync(HttpContext.User);
                assessment.teacherID = user.Id;
                _context.Add(assessment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(assessment);
        }

        // GET: Assessments/Edit/5
        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assessment = await _context.Assessment.SingleOrDefaultAsync(m => m.ID == id);
            if (assessment == null)
            {
                return NotFound();
            }
            return View(assessment);
        }

        // POST: Assessments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Description,DueDate,Title")] Assessment assessment)
        {
            if (id != assessment.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assessment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssessmentExists(assessment.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(assessment);
        }

        // GET: Assessments/Delete/5
        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assessment = await _context.Assessment.SingleOrDefaultAsync(m => m.ID == id);
            if (assessment == null)
            {
                return NotFound();
            }

            return View(assessment);
        }

        // POST: Assessments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assessment = await _context.Assessment.SingleOrDefaultAsync(m => m.ID == id);
            _context.Assessment.Remove(assessment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool AssessmentExists(int id)
        {
            return _context.Assessment.Any(e => e.ID == id);
        }
    }
}
