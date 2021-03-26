using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PersonalFinancialSystem.Services;

namespace PersonalFinancialSystem.Controllers
{
    [ApiController]
    [Route("api/lancamentos")]
    public class EntryController : ControllerBase
    {
        private readonly IEntryService _service;

        public EntryController(IEntryService service)
        {
            _service = service;
        }
        [HttpGet()]
        public ActionResult Get()
        {
            return Ok("Silence is golden!");
        }
        [HttpGet("{period}")]
        public ActionResult<EntryOutput> Get(string period)
        {
            period += "-01";
            try
            {
                var dateObject = DateTime.ParseExact(period, "yyyy-MM-dd", null);
                return Ok(_service.Get(dateObject));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        public ActionResult Post([FromBody] IList<Entry> postData)
        {
            try
            {
                _service.Post(postData);
                return Ok($"Inseridos {postData.Count()} lançamentos.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}