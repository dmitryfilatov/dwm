using DeltaWorkMonitoring.Infrastructure;
using DeltaWorkMonitoring.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeltaWorkMonitoring.Tests
{
    public class RoleUsersTagHelperTests
    {
        [Fact]
        public async Task ProcessAsync_ReturnsContent()
        {
            // Setup
            var usrMgr = TestHelper.GetUserManager();
            var roleMgr = TestHelper.GetRoleManager();

            var user1 = new AppUser { Id = Guid.NewGuid().ToString(), UserName = "User1" };
            var user2 = new AppUser { Id = Guid.NewGuid().ToString(), UserName = "User2" };
            var users = new List<AppUser> { user1, user2 };

            var identityRole = new IdentityRole { Name = "Members" };

            usrMgr.Users.Returns(users.AsQueryable());
            roleMgr.FindByIdAsync(identityRole.Name).Returns(identityRole);
            usrMgr.IsInRoleAsync(user1, identityRole.Name).Returns(true);
            usrMgr.IsInRoleAsync(user2, identityRole.Name).Returns(true);

            var tagHelper = new RoleUsersTagHelper(usrMgr, roleMgr) { Role = identityRole.Name };

            TagHelperContext ctx = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "");
            var content = Substitute.For<TagHelperContent>();
            TagHelperOutput output = new TagHelperOutput("div",
                new TagHelperAttributeList(),
                (cache, encoder) => Task.FromResult(content));

            // Act
            await tagHelper.ProcessAsync(ctx, output);

            // Assert
            var res = output.Content.GetContent();
            Assert.Equal($"{user1.UserName}, {user2.UserName}", res);
        }
    }
}
