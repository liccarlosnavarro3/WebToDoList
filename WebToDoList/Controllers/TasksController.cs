using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using WebToDoList.Data;
using WebToDoList.Models;

namespace WebToDoList.Controllers
{
    [Authorize] // Protege todas las acciones en este controlador para que solo los usuarios autenticados puedan acceder
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            try
            {
                var tasks = await _context.Tasks.Include(t => t.SubTasks).ToListAsync();

                return View(tasks);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {

                if (id == null)
                {
                    return NotFound();
                }

                var task = await _context.Tasks
                    .Include(t => t.SubTasks)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (task == null)
                {
                    return NotFound();
                }

                ViewBag.UserId = task.UserId;

                return View(task);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
                int userId = user != null ? user.Id : 0;

                ViewBag.UserId = userId;

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,IsCompleted,DueDate,UserId,Priority,Tags")] Models.Task task)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    //int userId = userIdClaim != null ? int.Parse(userIdClaim) : 0;

                    //task.UserId = userId;

                    _context.Add(task);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(task);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var task = await _context.Tasks.FindAsync(id);
                if (task == null)
                {
                    return NotFound();
                }

                ViewBag.UserId = task.UserId;
                return View(task);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IsCompleted,DueDate,UserId,Priority,Tags")] Models.Task task)
        {
            try
            {
                if (id != task.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(task);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TaskExists(task.Id))
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
                return View(task);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var task = await _context.Tasks
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (task == null)
                {
                    return NotFound();
                }

                ViewBag.UserId = task.UserId;

                return View(task);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }

        // CRUD para SubTasks
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSubTask([Bind("Id,Title,IsCompleted,TaskId")] SubTask subTask)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.SubTasks.Add(subTask);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = subTask.TaskId });
                }
                return View("Details", await _context.Tasks.Include(t => t.SubTasks).FirstOrDefaultAsync(m => m.Id == subTask.TaskId));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubTask(int id)
        {
            try
            {
                var subTask = await _context.SubTasks.FindAsync(id);
                if (subTask != null)
                {
                    _context.SubTasks.Remove(subTask);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Details), new { id = subTask.TaskId });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
