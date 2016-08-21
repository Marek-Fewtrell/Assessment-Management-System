using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assessment_Management_System.Data;
using Assessment_Management_System.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace Assessment_Management_System.Controllers
{
    [Authorize]
    public class SubmissionsController : Controller
    {
        private readonly UserManager<ApplicationUser> _manager;
        private readonly ApplicationDbContext _context;
        private IHostingEnvironment _environment;

        public SubmissionsController(ApplicationDbContext context, IHostingEnvironment environment, UserManager<ApplicationUser> manager)
        {
            _context = context;
            _environment = environment;
            _manager = manager;
        }

        // GET: Submissions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Submission.Include(s => s.Assessment).ToListAsync());
        }

        // GET: Submissions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submission = await _context.Submission.Include(i => i.Assessment).SingleOrDefaultAsync(m => m.ID == id);
            if (submission == null)
            {
                return NotFound();
            }

            return View(submission);
        }

        // GET: Submissions/Create
        public async Task<IActionResult> Create(int? id)
        {
            var user = await _manager.GetUserAsync(HttpContext.User);
            if (_context.Submission.Any(e => e.AssessmentID == id && e.ApplicationUser == user))
            {
                var result = await _context.Submission.FirstOrDefaultAsync(s => s.ApplicationUser == user && s.AssessmentID == id);
                return RedirectToAction("Edit", "Submissions", new { id = result.ID });
            }

            ViewData["AssessmentID"] = new SelectList(_context.Assessment, "ID", "Title", id);
            return View();
        }

        // POST: Submissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssessmentID")]Submission submission, IFormFile file) /*[Bind("ID,fileName,submittedOn")]*/
        {
            if (ModelState.IsValid)
            {
                var user = await _manager.GetUserAsync(HttpContext.User);
                //var submissionExists = _context.Submission.Any(s => s.AssessmentID == submission.AssessmentID && s.ApplicationUser == user);
                var submissionExists = await _context.Submission.SingleOrDefaultAsync(s => s.AssessmentID == submission.AssessmentID && s.ApplicationUser == user);
                if (submissionExists != null)
                {
                    return RedirectToAction("Edit", new { id = submissionExists.ID });
                }
                var guid = Guid.NewGuid().ToString();
                if (file.Length > 0)
                {
                    var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                    var path = Path.Combine(uploads, guid);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    return View(submission);
                }

                submission.fileName = file.FileName;
                submission.storageFileName = guid;
                submission.submittedOn = DateTime.Now;

                //var user = await _manager.GetUserAsync(HttpContext.User);
                submission.ApplicationUser = user;

                _context.Add(submission);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["AssessmentID"] = new SelectList(_context.Assessment, "ID", "Title", submission.AssessmentID);
            return View(submission);
        }

        // GET: Submissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submission = await _context.Submission.SingleOrDefaultAsync(m => m.ID == id);
            if (submission == null)
            {
                return NotFound();
            }
            ViewData["AssessmentID"] = new SelectList(_context.Assessment, "ID", "Title", id);
            return View(submission);
        }

        // POST: Submissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,fileName,AssessmentID,storageFileName,studentID")] Submission submission, IFormFile file)
        {
            if (id != submission.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    submission.submittedOn = DateTime.Now;

                    if (file != null)
                    {
                        var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                        var path = Path.Combine(uploads, submission.storageFileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }

                    _context.Update(submission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmissionExists(submission.ID))
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
            ViewData["AssessmentID"] = new SelectList(_context.Assessment, "ID", "Title", id);
            return View(submission);
        }

        // GET: Submissions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submission = await _context.Submission.SingleOrDefaultAsync(m => m.ID == id);
            if (submission == null)
            {
                return NotFound();
            }

            return View(submission);
        }

        // POST: Submissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var submission = await _context.Submission.SingleOrDefaultAsync(m => m.ID == id);

            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            var path = Path.Combine(uploads, submission.storageFileName);
            System.IO.File.Delete(path);

            _context.Submission.Remove(submission);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool SubmissionExists(int id)
        {
            return _context.Submission.Any(e => e.ID == id);
        }
    }
}