using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAuth.ViewModel
{
    public class UserClaimsView
    {
        public UserClaimsView()
        {
            userClaims = new List<UserClaim>();
        }
        public string UserId { get; set; }
        public List<UserClaim> userClaims { get; set; }
    }
}
