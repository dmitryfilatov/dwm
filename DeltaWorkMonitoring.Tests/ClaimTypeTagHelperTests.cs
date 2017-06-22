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
    public class ClaimTypeTagHelperTests
    {
        [Theory]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationinstant", "AuthenticationInstant")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod", "AuthenticationMethod")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/cookiepath", "CookiePath")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/denyonlyprimarysid", "DenyOnlyPrimarySid")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/denyonlyprimarygroupsid", "DenyOnlyPrimaryGroupSid")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/denyonlywindowsdevicegroup", "DenyOnlyWindowsDeviceGroup")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/dsa", "Dsa")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/expiration", "Expiration")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/expired", "Expired")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/groupsid", "GroupSid")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/ispersistent", "IsPersistent")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/primarygroupsid", "PrimaryGroupSid")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid", "PrimarySid")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Role")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/serialnumber", "SerialNumber")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata", "UserData")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/version", "Version")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname", "WindowsAccountName")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsdeviceclaim", "WindowsDeviceClaim")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsdevicegroup", "WindowsDeviceGroup")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsuserclaim", "WindowsUserClaim")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsfqbnversion", "WindowsFqbnVersion")]
        [InlineData("http://schemas.microsoft.com/ws/2008/06/identity/claims/windowssubauthority", "WindowsSubAuthority")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/anonymous", "Anonymous")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication", "Authentication")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authorizationdecision", "AuthorizationDecision")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/country", "Country")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/dateofbirth", "DateOfBirth")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/dns", "Dns")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/denyonlysid", "DenyOnlySid")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "Email")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/gender", "Gender")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", "GivenName")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/hash", "Hash")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/homephone", "HomePhone")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/locality", "Locality")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/mobilephone", "MobilePhone")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "Name")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "NameIdentifier")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/otherphone", "OtherPhone")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/postalcode", "PostalCode")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/rsa", "Rsa")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid", "Sid")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/spn", "Spn")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/stateorprovince", "StateOrProvince")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/streetaddress", "StreetAddress")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", "Surname")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/system", "System")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/thumbprint", "Thumbprint")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "Upn")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/uri", "Uri")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/webpage", "Webpage")]
        [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/x500distinguishedname", "X500DistinguishedName")]
        [InlineData("http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor", "Actor")]
        public void Process_ReturnsContent(string claimType, string claimTypeShort)
        {
            // Setup
            var tagHelper = new ClaimTypeTagHelper() { ClaimType = claimType};

            TagHelperContext ctx = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "");
            var content = Substitute.For<TagHelperContent>();
            TagHelperOutput output = new TagHelperOutput("div",
                new TagHelperAttributeList(),
                (cache, encoder) => Task.FromResult(content));

            // Act
            tagHelper.ProcessAsync(ctx, output);

            // Assert
            var res = output.Content.GetContent();
            Assert.Equal(claimTypeShort, res);
        }
    }
}
