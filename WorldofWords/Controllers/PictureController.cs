using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WorldOfWords.Domain.Services.IServices;
using WorldOfWords.Domain.Models;
using WorldOfWords.API.Models.Models;
using System.Threading.Tasks;
using System.Transactions;

namespace WorldofWords.Controllers
{
    [WowAuthorization(AllRoles = new[] { "Student", "Teacher", "Admin" })]
    [RoutePrefix("api/Picture")]
    public class PictureController : ApiController
    {
        private readonly IPictureService _pictureService;

        public PictureController(IPictureService service)
        {
            _pictureService = service;
        }

        [HttpGet]
        public async Task<PictureModelToUser> GetPictureByWordId(int wordId)
        {
            return await _pictureService.GetPictureByWordIdAsync(wordId);
        }

        public async Task<IHttpActionResult> Post(PictureModelFromUser picture)
        {
            if (await _pictureService.TryToAddPictureAsync(picture))
            {
                return Ok();
            }
            return BadRequest("Such picture already exists");
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeletePictureByWordId(int wordId)
        {
            if (await _pictureService.TryToDeletePictureAsync(wordId))
            {
                return Ok();
            }
            return BadRequest("There is no such picture");
        }
    }

}
