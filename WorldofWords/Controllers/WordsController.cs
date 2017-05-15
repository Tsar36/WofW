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
    [WowAuthorization(Roles = "Student")]
    [RoutePrefix("api/Words")]
    public class WordsController : ApiController
    {
        private readonly IWordMapper _mapper;
        private readonly IWordService _wordService;
        private readonly IWordTranslationService _wordTranslationService;
        private readonly IWordManagingService _wordManagService;
        private readonly ITagMapper _tagMapper;
        private readonly ITagService _tagService;

        public WordsController(IWordMapper mapper, IWordService service, IWordTranslationService wordTranslationService,
            IWordManagingService wordManagService, ITagService tagService, ITagMapper tagMapper)
        {
            _mapper = mapper;
            _wordService = service;
            _wordTranslationService = wordTranslationService;
            _tagService = tagService;
            _wordManagService = wordManagService;
            _tagMapper = tagMapper;
        }

        public async Task<IHttpActionResult> Post(string searchSyn, int languageId)
        {
            if (searchSyn == null)
                throw new ArgumentNullException("word model can't be empty");
            int n = await _wordService.ExistsAsync(searchSyn, languageId);
            if (n != 0)
            {
                return Ok(n.ToString());
            }
            else
            {
                return BadRequest("Such word doesn't exist");
            }
        }

        public async Task<IHttpActionResult> Post(WordModel word)
        {
            int result = await _wordManagService.AddWord(word);
            if (result > 0)
            {
                return Ok(result);
            }
            return BadRequest("Failed to add Word.");
        }

        [HttpGet]
        public async Task<List<TagModel>> Get(string searchTag, int searchResultsCount)
        {
            return (await _tagService.GetTopBySearchTagAsync(searchTag, searchResultsCount)).ToList();
        }

        public async Task<IEnumerable<WordModel>> Get(int languageId)
        {
            return (await _wordService.GetAllWordsBySpecificLanguageAsync(languageId)).Select(l => _mapper.ToApiModel(l));
        }

        public async Task<IEnumerable<WordModel>> Get()
        {
            return (await _wordService.GetAllWordsAsync()).Select(l => _mapper.ToApiModel(l));
        }

        public async Task<List<WordModel>> Get(string searchWord, int languageId, int searchResultsCount)
        {
            return (await _wordService.GetTopBySearchWordAsync(searchWord, languageId, searchResultsCount)).Select(item => _mapper.ToApiModel(item)).ToList();
        }

        public async Task<WordModel> GetWordById(int wordId)
        {
            return _mapper.ToApiModel(await _wordService.GetWordByIdAsync(wordId));
        }

        [Route("AllSynonyms/{WordId}")]
        public async Task<List<WordValueModel>> GetAllSynonyms(int wordId)
        {
            return await _wordTranslationService.GetAllWordSynonymsById(wordId);
        }

        [Route("EditWord")]
        public async Task<IHttpActionResult> EditWordTranslation(WordTranslationEditModel word)
        {
            if (await _wordManagService.EditWord(word))
            {
                return Ok();
            }
            return BadRequest("Failed to edit Word.");
        }

        [Route("DeleteWord")]
        public async Task<IHttpActionResult> DeleteWord(int wordId)
        {
            bool result = await _wordService.DeleteWord(wordId);
            return result ? Ok() as IHttpActionResult : BadRequest() as IHttpActionResult;
        }
    }
}
