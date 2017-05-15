using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Threading.Tasks;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Services;

namespace WorldofWords.Controllers
{
    [WowAuthorization(AllRoles = new[] { "Teacher", "Student", "Admin" })]
    [RoutePrefix("api/GlobalDictionary")]
    public class GlobalDictionaryController : ApiController
    {
        private readonly IWordTranslationService wordTranslationService;

        public GlobalDictionaryController(IWordTranslationService wordTranslationService)
        {
            this.wordTranslationService = wordTranslationService;
        }

        public async Task<List<WordTranslationImportModel>> Get([FromUri]int start, [FromUri]int end, [FromUri]int originalLangId, [FromUri]int translationLangId, [FromUri]int[] partsOfSpeechId)
        {
            return await wordTranslationService.GetWordsFromIntervalAsync(start, end, originalLangId, translationLangId, partsOfSpeechId);
        }

        public async Task<List<WordTranslationImportModel>> GetBySearchValue([FromUri]string searchValue, [FromUri]int startOfInterval, [FromUri]int endOfInterval,
            [FromUri]int originalLangId, [FromUri]int translationLangId, [FromUri]int[] partsOfSpeechId)
        {
            return await wordTranslationService.GetWordsWithSearchValueAsync(searchValue, startOfInterval, endOfInterval, originalLangId, translationLangId, partsOfSpeechId);
        }


        public async Task<int> GetAmountOfWordsBySpecificLanguage([FromUri]int originalLangId, [FromUri]int translationLangId, [FromUri]int[] partsOfSpeechId)
        {
            return await wordTranslationService.GetAmountOfWordTranslationsByLanguageAsync(originalLangId, translationLangId, partsOfSpeechId);
        }

        public async Task<int> GetAmountOfWordsBySearchValue([FromUri]string searchValue, [FromUri]int originalLangId, [FromUri]int translationLangId, [FromUri]int[] partsOfSpeechId)
        {
            return await wordTranslationService.GetAmountOfWordsBySearchValuesAsync(searchValue, originalLangId, translationLangId, partsOfSpeechId);
        }

        [HttpGet]
        public async Task<WordTranslationFullModel> GetFullWord(string word, int originalLangId, int translationLangId)
        {
            return await wordTranslationService.GetWordFullInformationAsync(word, originalLangId, translationLangId);
        }

        [HttpGet]
        public async Task<WordTranslationFullStringsModel> GetFullWordStrings(string wordValue, int originalLangId, int translationLangId)
        {
            var model = await wordTranslationService.GetWordFullInformationStringsAsync(wordValue, originalLangId, translationLangId);
            return model;
        }

        
        [Route("GetAmountByTags")]
        public async Task<int> GetAmountOfTags(string searchValue, int originalLangId, int translationLangId)
        {
             
           return await wordTranslationService.GetAmountOfTagsBySearchValuesAsync(searchValue, originalLangId, translationLangId);
          
        }

        [Route("GetWordsByTag")]
        public async Task<List<WordTranslationImportModel>> GetWordsByTagValue(int startOfInterval, int endOfInterval, int originalLangId, string searchValue, int translationLangId)
        {
            return await wordTranslationService.GetWordsWithTagAsync(startOfInterval, endOfInterval, originalLangId, searchValue, translationLangId);
            
        }
    }
}
