using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WorldOfWords.API.Models.IMappers;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Services.IServices;

namespace WorldofWords.Controllers
{
   
    /// <summary>
    /// Responsible for obtaining and manipulating a list of languages.
    /// </summary>
    [AllowAnonymous]
    [RoutePrefix("api/language")]
    public class LanguageController : ApiController
    {
        private readonly ILanguageService languageService;

        public LanguageController(ILanguageService service)
        {
            languageService = service;
        }

        public async Task<LanguageModel> GetByID(int id)
        {
            return await languageService.GetlanguageByIDAsync(id);
        }

        /// <summary>
        /// Returns a list of all languages.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<LanguageModel>> Get()
        {
            return await languageService.GetLanguagesAsync();
        }

        /// <summary>
        /// Returns list of world languages
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("worldLanguages")]
        public IEnumerable<LanguageModel> GetWorldLanguages()
        {
            return languageService.GetWorldLanguages();
        }

        [HttpGet]
        [Route("partsOfSpeech")]
        public async Task<IEnumerable<PartOfSpeechModel>> GetPartsOfSpeech(int id)
        {
            return await languageService.GetAllPartsOfSpeechByLanguageIdAsync(id);
        }

        /// <summary>
        /// Adds a new language to the database.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<int> Post(LanguageModel model)
        {
            return await languageService.AddAsync(model);
        }

        /// <summary>
        /// Removes language from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException("id", "Language ID can't be negative or 0");
            }

            return await languageService.RemoveAsync(id) ? Ok() as IHttpActionResult : BadRequest() as IHttpActionResult;
        }
    }
}