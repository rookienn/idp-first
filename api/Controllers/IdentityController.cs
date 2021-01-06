using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Authorize]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        [Route("api/identity")]
        public object GetUserClaims()
        {
            return User.Claims.Select(r => new { r.Type, r.Value });
        }
    }
}
