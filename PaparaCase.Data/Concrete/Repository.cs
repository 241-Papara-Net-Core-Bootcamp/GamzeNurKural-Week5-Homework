using Hangfire;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PaparaCase.Core.Enums;
using PaparaCase.Core.Interfaces;
using PaparaCase.Data.Abstract;
using PaparaCase.Data.Context;
using PaparaCase.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PaparaCase.Data.Concrete
{
    public class Repository<T> : IRepository<T> where T : class
    {
        PaparaCaseDbContext Context { get; }
        private readonly Func<CacheTech, ICacheService> _cacheService;
        private readonly static CacheTech cacheTech = CacheTech.Memory;
        private readonly string cacheKey = $"{typeof(T)}";
        public Repository(PaparaCaseDbContext context, Func<CacheTech, ICacheService> cacheService)
        {
            Context = context;
            _cacheService = cacheService;
        }
        public async Task<FakeUser> Get()
        {
            using var client = new HttpClient();

            Random rnd = new Random();

            var id = rnd.Next(0, 100);

            var responseTask = await client.GetAsync($"https://jsonplaceholder.typicode.com/posts/{id}");

            var responseStr = await responseTask.Content.ReadAsStringAsync();

            var fakeUser = JsonConvert.DeserializeObject<FakeUser>(responseStr);
            fakeUser.Id = null;
            return fakeUser;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var result = _cacheService(cacheTech).TryGet(cacheKey, out IEnumerable<T> cachedList);
            if (!result)
            {
                cachedList = await Context.Set<T>().ToListAsync();
                _cacheService(cacheTech).Set(cacheKey, cachedList);
            }
            return cachedList;
        }

        public async Task RefreshCache()
        {
            _cacheService(cacheTech).Remove(cacheKey);
            var cachedList = await Context.Set<T>().ToListAsync();
            _cacheService(cacheTech).Set(cacheKey, cachedList);
        }

        public async Task Create(T entity)
        {
            await Context.AddAsync(entity);
            var result = await Context.SaveChangesAsync();
            BackgroundJob.Enqueue(() => RefreshCache());
        }
    }
}
