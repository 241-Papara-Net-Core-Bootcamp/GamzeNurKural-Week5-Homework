using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PaparaCase.Data.Abstract;
using PaparaCase.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaparaCase.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FakeUserController : Controller
    {
        private readonly IRepository<FakeUser> _repo;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;
        public FakeUserController(IRepository<FakeUser> repo, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            this._repo = repo;
            this._backgroundJobClient = backgroundJobClient;
            this._recurringJobManager = recurringJobManager;
        }
        [HttpGet("Get")]
        public void Get()
        {
            var result = _repo.Get();
            _backgroundJobClient.Enqueue(() => Create(result.Result));
        }

        [HttpPost("Create")]
        public async Task<FakeUser> Create(FakeUser fakeUser)
        {
            await _repo.Create(fakeUser);
            return fakeUser;
        }

        [HttpGet("GetAll")]
        public IEnumerable<FakeUser> GetAll()
        {
            return _repo.GetAll().Result;
        }

        /// <summary>
        /// If you want to set recurring job use this method
        /// </summary>
        [HttpGet("SetRecurringJob")]
        public void SetJob()
        {
            _recurringJobManager.AddOrUpdate("1", () => Get(), Cron.Minutely());
        }
    }
}
