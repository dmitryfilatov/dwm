using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DeltaWorkMonitoring.Infrastructure
{
    public static class LocationClaimsProvider
    {
        public static Task<ClaimsPrincipal> AddClaims(ClaimsTransformationContext context)
        {
            ClaimsPrincipal principal = context.Principal;
            if (principal != null
                    && !principal.HasClaim(c => c.Type == ClaimTypes.PostalCode))
            {
                ClaimsIdentity identity = principal.Identity as ClaimsIdentity;
                if (identity != null && identity.IsAuthenticated
                        && identity.Name != null)
                {
                    if (identity.Name.ToLower() == "dmitry")
                    {
                        identity.AddClaims(new Claim[] {
                            CreateClaim(ClaimTypes.PostalCode, "344079"),
                            CreateClaim(ClaimTypes.StateOrProvince, "RO")
                        });
                    }
                    else
                    {
                        identity.AddClaims(new Claim[] {
                            CreateClaim(ClaimTypes.PostalCode, "121500"),
                            CreateClaim(ClaimTypes.StateOrProvince, "MO")
                        });
                    }
                }
            }
            return Task.FromResult(principal);
        }

        private static Claim CreateClaim(string type, string value) =>
            new Claim(type, value, ClaimValueTypes.String, "RemoteClaims");
    }
}
