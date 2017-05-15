using System.Collections.Generic;
using System.Web.Http;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Services;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Transactions;

namespace WorldofWords.Controllers
{
    [RoutePrefix("api/Record")]
    public class RecordController : ApiController
    {
        private readonly IRecordService _recordService;
        private readonly IWordService _wordService;

        public RecordController(IRecordService recordService, IWordService wordService)
        {
            _recordService = recordService;
            _wordService = wordService;
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post(RecordModel recordToAdd)
        {
            if (await _recordService.AddAsync(recordToAdd))
            {
                return Ok();
            }
            return BadRequest("Failed to add record.");
        }

        public async Task<RecordModel> GetById(int recordId)
        {
            return await _recordService.GetRecordModelByIdAsync(recordId);
        }

        public async Task<RecordModel> GetByWordId(int wordId)
        {
            return await _recordService.GetRecordModelByWordIdAsync(wordId);
        }

        [Route("IsThereRecord/{searchId}")]
        public async Task<bool> GetRecordExists(int searchId)
        {
            return await _recordService.IsThereRecord(searchId);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeleteRecord(int wordId)
        {
            bool result = await _recordService.DeleteRecordAsync(wordId);
            if(result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}