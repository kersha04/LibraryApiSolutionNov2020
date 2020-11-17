using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Controllers
{
    public class CacheController : ControllerBase
    {

        private readonly ILookupOnCallDevelopers _onCallLookup;

        public CacheController(ILookupOnCallDevelopers onCallLookup)
        {
            _onCallLookup = onCallLookup;
        }

        [HttpGet("/oncall")]
        public async Task<ActionResult> GetOnCallDeveloper()
        {
            //first as the cache (redis) if it has a good version of it.
            //if so, return it

            //if it doesn't, call the Teams/Sharepoint api, get the new version, add it to cache
            //with an expiration, and then return it
            var developerEmail = await _onCallLookup.GetOnCallDeveloperAsync();

            return Ok(new
            {
                email = developerEmail
            });
        }

        [HttpGet("/cache/time")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 15/*seconds*/)]
        public ActionResult GetTheTime()
        {
            return Ok(new
            {
                CreatedAt = DateTime.Now.ToLongTimeString()
            });
        }
    }
}
