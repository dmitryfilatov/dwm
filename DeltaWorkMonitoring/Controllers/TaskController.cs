using DeltaWorkMonitoring.Models;
using DeltaWorkMonitoring.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeltaWorkMonitoring.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private ITaskRepository repository;
        private UserManager<AppUser> userManager;
        public int PageSize = 6;
        public TaskPeriod TaskPeriod = TaskPeriod.Quarter;

        public TaskController(ITaskRepository repo, UserManager<AppUser> usrMgr)
        {
            repository = repo;
            userManager = usrMgr;
        }

        public ViewResult List(TaskStatus status, int page = 1)
        {
            var tasks = repository.GetTasks(GetCurrentUserId(), TaskPeriod);

            return View(new TaskListViewModel
            {
                Tasks = tasks
                    .Where(t => status == TaskStatus.None || t.Status == status)
                    .OrderByDescending(t => t.Started)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize),

                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = status == TaskStatus.None ?
                        tasks.Count() :
                        tasks.Where(e =>
                            e.Status == status).Count()
                },
                CurrentStatus = status
            });
        }

        public ViewResult Edit(Guid taskId)
        {
            ViewBag.Projects = repository.Projects;
            var task = repository.GetTasks(GetCurrentUserId(), TaskPeriod).FirstOrDefault(t => t.Id == taskId);
            return View(task);
        }

        [HttpPost]
        public IActionResult Edit(WorkTask task)
        {
            if (ModelState.IsValid)
            {
                repository.SaveTask(task, GetCurrentUserId());
                TempData["message"] = $"'{task.Name}' has been saved";
                return RedirectToAction("List");
            }
            else
            {
                ViewBag.Projects = repository.Projects;
                return View(task);
            }
        }

        public ViewResult Create()
        {
            ViewBag.Projects = repository.Projects;
            return View("Edit", new WorkTask());
        }

        [HttpPost]
        public IActionResult Delete(Guid taskId)
        {
            WorkTask deletedTask = repository.DeleteTask(taskId);
            if (deletedTask != null)
            {
                TempData["message"] = $"'{deletedTask.Name}' was deleted";
            }
            return RedirectToAction("List");
        }

        private Guid GetCurrentUserId()
        {
            var userId = userManager.GetUserId(HttpContext.User);
            return new Guid(userId);
        }
    }
}
