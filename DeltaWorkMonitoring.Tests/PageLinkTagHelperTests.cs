using DeltaWorkMonitoring.Controllers;
using DeltaWorkMonitoring.Infrastructure;
using DeltaWorkMonitoring.Models;
using DeltaWorkMonitoring.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeltaWorkMonitoring.Tests
{
    public class PageLinkTagHelperTests
    {
        [Fact]
        public void Process_PageInfo_ReturnsContent()
        {
            var urlHelper = Substitute.For<IUrlHelper>();
            urlHelper.Action(Arg.Any<UrlActionContext>())
                .Returns("Test/Page1", 
                         "Test/Page2", 
                         "Test/Page3");
            var urlHelperFactory = Substitute.For<IUrlHelperFactory>();
            urlHelperFactory.GetUrlHelper(Arg.Any<ActionContext>())
                .Returns(urlHelper);
            PageLinkTagHelper tagHelper = new PageLinkTagHelper(urlHelperFactory)
            {
                PageModel = new PagingInfo
                {
                    CurrentPage = 2,
                    TotalItems = 28,
                    ItemsPerPage = 10
                },
                PageAction = "Test"
            };
            TagHelperContext ctx = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(), "");
            var content = Substitute.For<TagHelperContent>();
            TagHelperOutput output = new TagHelperOutput("div",
                new TagHelperAttributeList(),
                (cache, encoder) => Task.FromResult(content));

            tagHelper.Process(ctx, output);

            var res = output.Content.GetContent();
            Assert.Equal(@"<a href=""Test/Page1"">1</a>"
                + @"<a href=""Test/Page2"">2</a>"
                + @"<a href=""Test/Page3"">3</a>",
                 res);
        }
    }
}
