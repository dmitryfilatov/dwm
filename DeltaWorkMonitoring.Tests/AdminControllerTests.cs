using DeltaWorkMonitoring.Controllers;
using DeltaWorkMonitoring.Models;
using DeltaWorkMonitoring.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeltaWorkMonitoring.Tests
{
    public class AdminControllerTests
    {
        [Fact]
        public void Index_ReturnsResult()
        {
            // Setup
            var controller = CreateController(out UserManager<AppUser> usrMgr);
            var userId = Guid.NewGuid();
            var userName = "User1";
            var users = new List<AppUser> { new AppUser { Id = userId.ToString(), UserName = userName } };
            usrMgr.Users.Returns(users.AsQueryable());

            // Act
            var result = TestHelper.GetViewModel<IEnumerable<AppUser>>(controller.Index()).ToArray();

            // Assert
            Assert.Equal(1, result.Length);
            Assert.Equal(userId.ToString(), result[0].Id);
            Assert.Equal(userName, result[0].UserName);
            Assert.True(TestHelper.IsAuthorized(controller, "Index", null, new string[] { "Admins" }));
        }

        [Fact]
        public void Create_NoArgument_ReceivedCall()
        {
            // Setup
            var controller = CreateController(out UserManager<AppUser> usrMgr, true);

            // Act
            controller.Create();

            // Assert
            controller.Received(1).View();
            Assert.True(TestHelper.IsAuthorized(controller, "Create", null, new string[] { "Admins" }));
        }

        [Fact]
        public void Create_CreateLogin_ReturnsSuccessResult()
        {
            // Setup
            var controller = CreateController(out UserManager<AppUser> usrMgr);
            var createLogin = new CreateLogin { Name = "User1", Email = "user1@example.com", Password = "Secret123$" };
            usrMgr.CreateAsync(createLogin.User, createLogin.Password).Returns(IdentityResult.Success);

            // Act
            IActionResult result = controller.Create(createLogin).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", (result as RedirectToActionResult).ActionName);
            Assert.True(TestHelper.IsAuthorized(controller, "Create", new Type[] { typeof(CreateLogin) }, new string[] { "Admins" }));
        }

        [Fact]
        public void Create_CreateLogin_ReturnsFailedResult()
        {
            // Setup
            var controller = CreateController(out UserManager<AppUser> usrMgr);
            var createLogin = new CreateLogin { Name = "User1", Email = "user1@example.com", Password = "Secret123$" };
            var errorMessage = "Error 1";
            usrMgr.CreateAsync(createLogin.User, createLogin.Password).Returns(TestHelper.GetIdentityFailedResult(errorMessage));

            // Act
            IActionResult result = controller.Create(createLogin).Result;

            // Assert
            Assert.Equal(1, controller.ModelState.ErrorCount);
            Assert.Equal(errorMessage, controller.ModelState[""].Errors[0].ErrorMessage);
        }

        private AdminController CreateController(out UserManager<AppUser> usrMgr, bool mock = false)
        {
            usrMgr = TestHelper.GetUserManager();
            var userValidator = Substitute.For<IUserValidator<AppUser>>();
            var passwordValidator = Substitute.For<IPasswordValidator<AppUser>>();
            var passwordHasher = Substitute.For<IPasswordHasher<AppUser>>();
            var repo = Substitute.For<ITaskRepository>();
            return mock ?
                Substitute.For<AdminController>(usrMgr, userValidator, passwordValidator, passwordHasher, repo)
                : new AdminController(usrMgr, userValidator, passwordValidator, passwordHasher, repo);
        }
    }
}
