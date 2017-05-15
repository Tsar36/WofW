using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Services;
using System.Threading.Tasks;

using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using System.Text;
using WorldOfWords.Domain.Services.IServices;
using System.Data.Entity;

namespace WorldofWords.Controllers
{
    [WowAuthorization(AllRoles = new[] { "Teacher", "Admin" })]
    [RoutePrefix("api/WordTranslation")]
    public class WordTranslationController : BaseController
    {
        private const int TranslationLanguageId = 4;

        private readonly IWordTranslationMapper wordTranslationMapper;
        private readonly IWordTranslationService wordTranslationService;
        private readonly IWordService wordService;
        private readonly IWordManagingService editService;
        private readonly ITagService tagService;
        private readonly ITagMapper tagMapper;

        public WordTranslationController(IWordTranslationService wordTranslationService,
                                         IWordTranslationMapper wordTranslationMapper,
                                         IWordManagingService editService,
                                         IWordService wordService,
                                         ITagService tagService,
                                         ITagMapper tagMapper)
        {
            this.wordTranslationService = wordTranslationService;
            this.wordTranslationMapper = wordTranslationMapper;
            this.editService = editService;
            this.wordService = wordService;
            this.tagService = tagService;
            this.tagMapper = tagMapper;
        }

        [Route("ImportWordTranslations")]
        public async Task<List<WordTranslationModel>> Post(List<WordTranslationImportModel> wordTranslations)
        {
            List<WordTranslationModel> wordTranslationsToReturn = new List<WordTranslationModel>();

            foreach (WordTranslationImportModel wordTranslation in wordTranslations)
            {
                wordTranslation.OriginalWordId = await wordService.ExistsAsync(wordTranslation.OriginalWord, wordTranslation.LanguageId);

                if (wordTranslation.OriginalWordId == 0)
                {
                    WordModel modelForAdd = new WordModel()
                    {
                        Value = wordTranslation.OriginalWord,
                        LanguageId = wordTranslation.LanguageId,
                        Transcription = wordTranslation.Transcription,
                        Description = wordTranslation.Description,
                        Comment = wordTranslation.Comment,
                        PartOfSpeechId = wordTranslation.PartOfSpeechId
                    };
                    foreach (var i in wordTranslation.Tags)
                    {
                        modelForAdd.Tags.Add(new TagModel()
                        {
                            Id = null,
                            Value = i.Value
                        });
                    }
                    wordTranslation.OriginalWordId = await editService.AddWord(modelForAdd);
                }

                wordTranslation.TranslationWordId = await wordService.ExistsAsync(wordTranslation.TranslationWord, TranslationLanguageId);

                if (wordTranslation.TranslationWordId == 0)
                {
                    wordTranslation.TranslationWordId = await wordService.AddAsync(new WordModel()
                        {
                            Value = wordTranslation.TranslationWord,
                            LanguageId = TranslationLanguageId
                        });
                }

                wordTranslation.OwnerId = UserId;
                int wordTranslationId = wordTranslationService.Add(wordTranslationMapper.Map(wordTranslation));

                wordTranslationsToReturn.Add(new WordTranslationModel()
                {
                    Id = wordTranslationId,
                    OriginalWord = wordTranslation.OriginalWord,
                    TranslationWord = wordTranslation.TranslationWord
                });
            }

            return wordTranslationsToReturn;
        }

        [Route("CreateWordTranslation")]
        public async Task<IHttpActionResult> Post(WordTranslationImportModel wordtranslation)
        {
            if (wordtranslation == null)
                throw new ArgumentNullException("word translation model can't be empty");
            if (wordTranslationService.Exists(wordtranslation.OriginalWordId, wordtranslation.TranslationWordId) == 0)
            {
                int res = await wordTranslationService.AddAsync(wordtranslation);
                return Ok(res.ToString());
            }
            else
            {
                return BadRequest("Such wordtranslation exists");
            }
        }

        [Route("SearchWordTranslations")]
        public List<WordTranslationModel> Post(SearchWordTranslationModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", "Search model can't be null");
            }
            else
            {
                return wordTranslationMapper
                   .MapRange(wordTranslationService.GetTopBySearchWord(model.SearchWord, model.LanguageId));
            }            
        }

        public List<WordTranslationModel> Get(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID can't be negative or 0", "id");
            }

            return wordTranslationMapper.MapRange(wordTranslationService.GetByWordSuiteID(id));
        }

        public async Task<IHttpActionResult> Delete(int orId, int trId)
        {
            if (orId <= 0 && trId <= 0)
            {
                throw new ArgumentNullException("id", "ID can't be negative or 0");
            }

            return await wordTranslationService.DeleteAsync(orId, trId) ? Ok() as IHttpActionResult : BadRequest() as IHttpActionResult;
        }
        


    }
}