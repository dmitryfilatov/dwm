using DeltaWorkMonitoring.Controllers;
using DeltaWorkMonitoring.Infrastructure;
using DeltaWorkMonitoring.Models;
using DeltaWorkMonitoring.Models.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace DeltaWorkMonitoring.Tests
{
    public class TaskControllerTests
    {
        [Fact]
        public void List_PageNumber_ReturnsResult()
        {
            var repo = Substitute.For<ITaskRepository>();
            var currUserId = Guid.NewGuid();
            var period = TaskPeriod.Quarter;
            var tasks = GetTestTasks(6, period).ToArray();
            repo.GetTasks(currUserId, period).Returns(tasks);
            var controller = CreateController(repo, currUserId);
            controller.PageSize = 3;
            controller.TaskPeriod = period;

            TaskListViewModel result = TestHelper.GetViewModel<TaskListViewModel>(controller.List(TaskStatus.None, 2));

            WorkTask[] resultTasks = result.Tasks.ToArray();
            Assert.True(resultTasks.Length == 3);
            Assert.Equal(tasks[3].Name, resultTasks[0].Name);
            Assert.Equal(tasks[4].Name, resultTasks[1].Name);
            PagingInfo pageInfo = result.PagingInfo;
            Assert.Equal(2, pageInfo.CurrentPage);
            Assert.Equal(3, pageInfo.ItemsPerPage);
            Assert.Equal(6, pageInfo.TotalItems);
            Assert.Equal(2, pageInfo.TotalPages);
        }
                
        [Fact]
        public void Edit_TaskId_ReturnsResult()
        {
            var repo = Substitute.For<ITaskRepository>();
            var currUserId = Guid.NewGuid();
            var period = TaskPeriod.Quarter;
            var tasks = GetTestTasks(4, period).ToArray();
            repo.GetTasks(currUserId, period).Returns(tasks);
            var controller = CreateController(repo, currUserId);
            controller.TaskPeriod = period;

            WorkTask t1 = TestHelper.GetViewModel<WorkTask>(controller.Edit(tasks[1].Id));
            WorkTask t2 = TestHelper.GetViewModel<WorkTask>(controller.Edit(tasks[2].Id));
            WorkTask t3 = TestHelper.GetViewModel<WorkTask>(controller.Edit(Guid.NewGuid()));

            Assert.Equal(tasks[1].Id, t1.Id);
            Assert.Equal(tasks[2].Id, t2.Id);
            //Assert.Null(t3); - ???
        }

        [Fact]
        public void Edit_Task_SuccessSaveChanges()
        {
            var repo = Substitute.For<ITaskRepository>();
            var tempData = Substitute.For<ITempDataDictionary>();
            var currUserId = Guid.NewGuid();
            var taskName = "Task";
            var task = new WorkTask { Name = taskName };
            var controller = CreateController(repo, currUserId, tempData);
            controller.Url = GetUrlHelper();

            IActionResult result = controller.Edit(task);

            repo.Received().SaveTask(task, currUserId);
            Assert.Equal($"'{taskName}' has been saved", controller.TempData["message"]);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", (result as RedirectToActionResult).ActionName);
        }

        [Fact]
        public void Edit_Task_ErrorSaveChanges()
        {
            var repo = Substitute.For<ITaskRepository>();
            var tempData = Substitute.For<ITempDataDictionary>();
            var currUserId = Guid.NewGuid();
            var task = new WorkTask { Name = "Task" };
            var controller = CreateController(repo, currUserId, tempData);
            controller.ModelState.AddModelError("error", "error");

            IActionResult result = controller.Edit(task);

            repo.DidNotReceive().SaveTask(Arg.Any<WorkTask>(), currUserId);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Delete_TaskId_SuccessDelete()
        {
            var repo = Substitute.For<ITaskRepository>();
            var tempData = Substitute.For<ITempDataDictionary>();
            var period = TaskPeriod.Quarter;
            var currUserId = Guid.NewGuid();
            var tasks = GetTestTasks(3, TaskPeriod.Quarter).ToArray();
            var task = tasks[2];
            repo.GetTasks(currUserId, period).Returns(tasks);
            repo.DeleteTask(task.Id).Returns(task);
            var controller = CreateController(repo, currUserId, tempData);
            controller.Url = GetUrlHelper();
            controller.TaskPeriod = period;

            IActionResult result = controller.Delete(task.Id);

            repo.Received().DeleteTask(task.Id);
            Assert.Equal($"'{task.Name}' was deleted", controller.TempData["message"]);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", (result as RedirectToActionResult).ActionName);
        }

        private TaskController CreateController(ITaskRepository repo, Guid currUserId, ITempDataDictionary tempData = null)
        {
            var usrMgr = TestHelper.GetUserManager();
            var httpContext = Substitute.For<HttpContext>();
            var currPrincipal = new ClaimsPrincipal();
            httpContext.User.Returns(currPrincipal);
            usrMgr.GetUserId(currPrincipal).Returns(currUserId.ToString());
            if(tempData == null)
                tempData = Substitute.For<ITempDataDictionary>();
            var controller = new TaskController(repo, usrMgr) { TempData = tempData };
            controller.ControllerContext.HttpContext = httpContext;
            return controller;
        }

        private IUrlHelper GetUrlHelper()
        {
            var urlHelper = Substitute.For<IUrlHelper>();
            var urlHelperFactory = Substitute.For<IUrlHelperFactory>();
            urlHelperFactory.GetUrlHelper(Arg.Any<ActionContext>())
                .Returns(urlHelper);
            return urlHelper;
        }

        private IEnumerable<WorkTask> GetTestTasks(int count, TaskPeriod period)
        {
            var dt = DateTime.Now;
            return Enumerable.Range(0, count).Select(i => new WorkTask { Id = Guid.NewGuid(), Name = $"Task{i}", Created = dt.GetRandomDateTime(period) });
        }

        //[Fact]
        //public void Test()
        //{
        //    ILoggerFactory loggerFactory = new LoggerFactory()
        //        .AddConsole()
        //        .AddDebug();
        //}
    }
}
