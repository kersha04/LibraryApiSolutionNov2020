using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPI.Services
{
    public class RedisOnCallLookup : ILookupOnCallDevelopers
    {
        private readonly IDistributedCache _cache;

        public RedisOnCallLookup(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<string> GetOnCallDeveloperAsync()
        {
            //first, ask the cache if it has a good version
            var emailFromCache = await _cache.GetAsync("oncall");
            string email;

            //if so, return the cached version
            if(emailFromCache != null)
            {
                var decodedString = Encoding.UTF8.GetString(emailFromCache);
                email = decodedString;
            }
            else // if not, do the work
            {
                //wait 1-3 seconds
                await Task.Delay(new Random().Next(1000, 3000));
                email = $"bob-{DateTime.Now.Millisecond}@aol.com";

                var encodedEmail = Encoding.UTF8.GetBytes(email);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(15)
                };

                await _cache.SetAsync("oncall", encodedEmail, options);
            }

            return email;
        }
    }
}
