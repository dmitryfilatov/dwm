using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeltaWorkMonitoring.Models.ViewModels
{
    public class Login
    {
        [Required]
        [UIHint("email")]
        public string Email { get; set; }

        [Required]
        [UIHint("password")]
        public string Password { get; set; }
    }

    public class CreateLogin
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [UIHint("email")]
        public string Email { get; set; }
        [Required]
        [UIHint("password")]
        public string Password { get; set; }

        private AppUser _appUser;
        public AppUser User
        {
            get
            {
                if (_appUser == null)
                {
                    _appUser = new AppUser { UserName = Name, Email = Email };
                }
                return _appUser;
            }
        }
    }

    public class CreateRole
    {
        [Required]
        public string Name { get; set; }
        
        private IdentityRole _role;
        public IdentityRole Role
        {
            get
            {
                if (_role == null)
                {
                    _role = new IdentityRole(Name);
                }
                return _role;
            }
        }
    }

    public class RoleEdit
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<AppUser> Members { get; set; }
        public IEnumerable<AppUser> NonMembers { get; set; }
    }

    public class RoleModification
    {
        [Required]
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public string[] IdsToAdd { get; set; }
        public string[] IdsToDelete { get; set; }
    }
}
