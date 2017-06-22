using DeltaWorkMonitoring.Controllers;
using DeltaWorkMonitoring.Models;
using DeltaWorkMonitoring.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeltaWorkMonitoring.Tests
{
    public class RoleAdminControllerTests
    {
        [Fact]
        public void Index_ReturnsResult()
        {
            // Setup
            var controller = CreateController(out RoleManager<IdentityRole> roleMgr);
            var identityRole1 = new IdentityRole { Name = "Admins" };
            var identityRole2 = new IdentityRole { Name = "Members" };
            var identityRoles = new List<IdentityRole> { identityRole1, identityRole2 };
            roleMgr.Roles.Returns(identityRoles.AsQueryable());

            // Act
            var result = TestHelper.GetViewModel<IEnumerable<IdentityRole>>(controller.Index()).ToArray();

            // Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(identityRole1.Name, result[0].Name);
            Assert.Equal(identityRole2.Name, result[1].Name);
        }

        [Fact]
        public void Create_NoArgument_ReceivedCall()
        {
            // Setup
            var controller = CreateController(out RoleManager<IdentityRole> roleMgr, true);

            // Act
            controller.Create();

            // Assert
            controller.Received(1).View();
        }

        [Fact]
        public void Create_CreateRole_ReturnsSuccessResult()
        {
            // Setup
            var controller = CreateController(out RoleManager<IdentityRole> roleMgr);
            var createRole = new CreateRole { Name = "Members" };
            roleMgr.CreateAsync(createRole.Role).Returns(IdentityResult.Success);

            // Act
            IActionResult result = controller.Create(createRole).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", (result as RedirectToActionResult).ActionName);
        }

        [Fact]
        public void Create_CreateRole_ReturnsFailedResult()
        {
            // Setup
            var controller = CreateController(out RoleManager<IdentityRole> roleMgr);
            var createRole = new CreateRole { Name = "Members" };
            var errorMessage = "Error 1";
            roleMgr.CreateAsync(createRole.Role).Returns(IdentityResult.Failed(new[] { new IdentityError { Description = errorMessage } }));

            // Act
            IActionResult result = controller.Create(createRole).Result;

            // Assert
            Assert.Equal(1, controller.ModelState.ErrorCount);
            Assert.Equal(errorMessage, controller.ModelState[""].Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Edit_RoleId_ReturnsResult()
        {
            // Setup
            var usrMgr = TestHelper.GetUserManager();
            var roleMgr = TestHelper.GetRoleManager();
            var controller = new RoleAdminController(roleMgr, usrMgr);
            var identityRole = new IdentityRole("Members")
            {
                Id = Guid.NewGuid().ToString()
            };
            roleMgr.FindByIdAsync(identityRole.Id).Returns(identityRole);
            var user1 = new AppUser { Id = Guid.NewGuid().ToString(), UserName = "User1" };
            var user2 = new AppUser { Id = Guid.NewGuid().ToString(), UserName = "User2" };
            var user3 = new AppUser { Id = Guid.NewGuid().ToString(), UserName = "User3" };
            var users = new List<AppUser> { user1, user2, user3 };
            usrMgr.Users.Returns(users.AsQueryable());
            usrMgr.IsInRoleAsync(user1, identityRole.Name).Returns(true);

            // Act
            RoleEdit res = TestHelper.GetViewModel<RoleEdit>(await controller.Edit(identityRole.Id));

            // Assert
            Assert.Equal(identityRole.Id, res.Role.Id);
            Assert.Equal(user1.UserName, res.Members.ElementAt(0).UserName);
            Assert.Equal(user2.UserName, res.NonMembers.ElementAt(0).UserName);
            Assert.Equal(user3.UserName, res.NonMembers.ElementAt(1).UserName);
        }

        [Fact]
        public async Task Edit_AddUserToRole_SuccessSaveChanges()
        {
            // Setup
            var usrMgr = TestHelper.GetUserManager();
            var roleMgr = TestHelper.GetRoleManager();
            var controller = new RoleAdminController(roleMgr, usrMgr);
            RoleModification model = new RoleModification { RoleName = "Members" };
            var user1 = new AppUser { Id = Guid.NewGuid().ToString(), UserName = "User1" };
            model.IdsToAdd = new string[] { user1.Id };
            usrMgr.FindByIdAsync(user1.Id).Returns(user1);
            usrMgr.AddToRoleAsync(user1, model.RoleName).Returns(IdentityResult.Success);

            // Act
            IActionResult result = await controller.Edit(model);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", (result as RedirectToActionResult).ActionName);
            await usrMgr.Received().FindByIdAsync(user1.Id);
            await usrMgr.Received().AddToRoleAsync(user1, model.RoleName);
        }

        [Fact]
        public async Task Edit_AddUserToRole_FailedSaveChanges()
        {
            // Setup
            var usrMgr = TestHelper.GetUserManager();
            var roleMgr = TestHelper.GetRoleManager();
            var controller = new RoleAdminController(roleMgr, usrMgr);
            RoleModification model = new RoleModification { RoleName = "Members" };
            var user1 = new AppUser { Id = Guid.NewGuid().ToString(), UserName = "User1" };
            model.IdsToAdd = new string[] { user1.Id };
            usrMgr.FindByIdAsync(user1.Id).Returns(user1);
            var errorMessage = "Error 1";
            usrMgr.AddToRoleAsync(user1, model.RoleName).Returns(TestHelper.GetIdentityFailedResult(errorMessage));

            // Act
            IActionResult result = await controller.Edit(model);

            // Assert
            Assert.Equal(1, controller.ModelState.ErrorCount);
            Assert.Equal(errorMessage, controller.ModelState[""].Errors[0].ErrorMessage);
            await usrMgr.Received().FindByIdAsync(user1.Id);
            await usrMgr.Received().AddToRoleAsync(user1, model.RoleName);
        }

        [Fact]
        public async Task Edit_RemoveUserFromRole_SuccessSaveChanges()
        {
            // Setup
            var usrMgr = TestHelper.GetUserManager();
            var roleMgr = TestHelper.GetRoleManager();
            var controller = new RoleAdminController(roleMgr, usrMgr);
            RoleModification model = new RoleModification { RoleName = "Members" };
            var user1 = new AppUser { Id = Guid.NewGuid().ToString(), UserName = "User1" };
            model.IdsToDelete = new string[] { user1.Id };
            usrMgr.FindByIdAsync(user1.Id).Returns(user1);
            usrMgr.RemoveFromRoleAsync(user1, model.RoleName).Returns(IdentityResult.Success);

            // Act
            IActionResult result = await controller.Edit(model);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", (result as RedirectToActionResult).ActionName);
            await usrMgr.Received().FindByIdAsync(user1.Id);
            await usrMgr.Received().RemoveFromRoleAsync(user1, model.RoleName);
        }

        private RoleAdminController CreateController(out RoleManager<IdentityRole> roleMgr, bool mock = false)
        {
            var usrMgr = TestHelper.GetUserManager();
            roleMgr = TestHelper.GetRoleManager();
            return mock ?
                Substitute.For<RoleAdminController>(roleMgr, usrMgr)
                : new RoleAdminController(roleMgr, usrMgr);
        }
    }
}
