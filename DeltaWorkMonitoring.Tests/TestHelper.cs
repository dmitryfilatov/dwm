using DeltaWorkMonitoring.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DeltaWorkMonitoring.Tests
{
    public static class TestHelper
    {
        public static UserManager<AppUser> GetUserManager()
        {
            return Substitute.For<UserManager<AppUser>>(
                Substitute.For<IUserStore<AppUser>>(),
                Substitute.For<IOptions<IdentityOptions>>(),
                Substitute.For<IPasswordHasher<AppUser>>(),
                Substitute.For<IEnumerable<IUserValidator<AppUser>>>(),
                Substitute.For<IEnumerable<IPasswordValidator<AppUser>>>(),
                Substitute.For<ILookupNormalizer>(),
                Substitute.For<IdentityErrorDescriber>(),
                Substitute.For<IServiceProvider>(),
                Substitute.For<ILogger<UserManager<AppUser>>>());
        }

        public static RoleManager<IdentityRole> GetRoleManager()
        {
            return Substitute.For<RoleManager<IdentityRole>>(
                Substitute.For<IRoleStore<IdentityRole>>(),
                Substitute.For<IEnumerable<IRoleValidator<IdentityRole>>>(),
                Substitute.For<ILookupNormalizer>(),
                Substitute.For<IdentityErrorDescriber>(),
                Substitute.For<ILogger<RoleManager<IdentityRole>>>(),
                Substitute.For<IHttpContextAccessor>());
        }

        public static T GetViewModel<T>(IActionResult result) where T : class
        {
            return (result as ViewResult)?.ViewData.Model as T;
        }

        public static IdentityResult GetIdentityFailedResult(string errorMessage)
        {
            return IdentityResult.Failed(new[] { new IdentityError { Description = errorMessage } });
        }

        /// <summary>
        /// Check to see if a method allows anonymous access -
        /// 1. A method is anonymous if it is decorated with the AllowAnonymousAttribute attribute.
        /// 2. Or, a method is anonymous if neither the method nor controller are decorated with the AuthorizeAttribute attribute.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="methodName"></param>
        /// <param name="methodTypes">Optional</param>
        /// <returns>true is method is anonymous</returns>
        public static bool IsAnonymous(Controller controller, string methodName, Type[] methodTypes)
        {
            return GetMethodAttribute<AllowAnonymousAttribute>(controller, methodName, methodTypes) != null ||
                (GetControllerAttribute<AuthorizeAttribute>(controller) == null &&
                    GetMethodAttribute<AuthorizeAttribute>(controller, methodName, methodTypes) == null);
        }

        /// <summary>
        /// Check to see if a method requires authorization -
        /// 1. A method is authorized if it is decorated with the Authorize attribute.
        /// 2. Or, a method is authorized if the controller is decorated with the AuthorizeAttribute attribute, and
        /// the method is not decorated with the AllowAnonymousAttribute attribute.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="methodName"></param>
        /// <param name="methodTypes">Optional</param>
        /// <returns></returns>
        public static bool IsAuthorized(Controller controller, string methodName, Type[] methodTypes)
        {
            return GetMethodAttribute<AuthorizeAttribute>(controller, methodName, methodTypes) != null ||
                (GetControllerAttribute<AuthorizeAttribute>(controller) != null &&
                    GetMethodAttribute<AllowAnonymousAttribute>(controller, methodName, methodTypes) == null);
        }

        /// <summary>
        /// Check to see if a method requires authorization for the roles and users specified
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="methodName"></param>
        /// <param name="methodTypes">Optional</param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static bool IsAuthorized(Controller controller, string methodName, Type[] methodTypes, string[] roles)
        {
            if (roles == null)
                return IsAuthorized(controller, methodName, methodTypes);

            if (!IsAuthorized(controller, methodName, methodTypes))
                return false;

            AuthorizeAttribute controllerAttribute = GetControllerAttribute<AuthorizeAttribute>(controller);
            AuthorizeAttribute methodAttribute = GetMethodAttribute<AuthorizeAttribute>(controller, methodName, methodTypes);

            // Check to see if all roles are authorized
            if (roles != null)
            {
                foreach (string role in roles)
                {
                    string lowerRole = role.ToLower();

                    bool roleIsAuthorized =
                        (controllerAttribute != null ?
                            controllerAttribute.Roles.ToLower().Split(',').Any(r => r == lowerRole) : false) ||
                        (methodAttribute != null ?
                            methodAttribute.Roles.ToLower().Split(',').Any(r => r == lowerRole) : false);

                    if (!roleIsAuthorized)
                        return false;
                }
            }

            return true;
        }

        private static T GetControllerAttribute<T>(Controller controller) where T : Attribute
        {
            Type type = controller.GetType();
            object[] attributes = type.GetTypeInfo().GetCustomAttributes<T>().ToArray();
            T attribute = attributes.Count() == 0 ? null : (T)attributes[0];
            return attribute;
        }

        private static T GetMethodAttribute<T>(Controller controller, string methodName, Type[] methodTypes) where T : Attribute
        {
            Type type = controller.GetType();
            if (methodTypes == null)
            {
                methodTypes = new Type[0];
            }
            MethodInfo method = type.GetMethod(methodName, methodTypes);
            object[] attributes = method.GetCustomAttributes(typeof(T), true).ToArray();
            T attribute = attributes.Count() == 0 ? null : (T)attributes[0];
            return attribute;
        }
    }
}
