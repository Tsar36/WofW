using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Services;
using WorldOfWords.Domain.Services.IServices;
using WorldOfWords.API.Models.Models;
using WorldofWords.Hubs;
using Microsoft.AspNet.SignalR;

namespace WorldofWords.Controllers
{
    [RoutePrefix("api/wordsuite")]
    [WowAuthorization(Roles = "Teacher")]
    public class WordSuiteController : BaseController
    {
        private readonly IWordSuiteService wordSuiteService;
        private readonly IWordSuiteMapper wordSuiteMapper;
        private readonly IWordProgressMapper wordProgressMapper;
        private readonly ITrainingWordSuiteMapper trainingWordSuiteMapper;
        private readonly IWordProgressService wordProgressService;
        private readonly IUserForListingMapper _userListMapper;
        private readonly IUserService _userservice;
        private readonly IQuizMapper _quizMapper;

        public WordSuiteController(
            IWordSuiteService wordSuiteService,
            IWordSuiteMapper wordSuiteMapper,
            ITrainingWordSuiteMapper trainingWordSuiteMapper,
            IWordProgressService wordProgressService,
            IWordProgressMapper wordProgressMapper,
            IUserService userservice, 
            IUserForListingMapper userListMapper,
            IQuizMapper quizMapper)
        {
            this.wordSuiteMapper = wordSuiteMapper;
            this.wordSuiteService = wordSuiteService;
            this.trainingWordSuiteMapper = trainingWordSuiteMapper;
            this.wordProgressMapper = wordProgressMapper;
            this.wordProgressService = wordProgressService;
            this._userservice = userservice;
            this._userListMapper = userListMapper;
            this._quizMapper = quizMapper;
        }

        public WordSuiteController() { }

        [WowAuthorization(Roles = "Teacher")]
        [Route("CreateWordSuite")]
        public IHttpActionResult Post(WordSuiteModel wordSuite)
        {
            if (wordSuite == null)
            {
                throw new ArgumentNullException("wordSuite", "WordSuite can't be null");
            }

            int wordSuiteId = wordSuiteService.Add(wordSuiteMapper.Map(wordSuite));

            if (wordSuiteId <= 0)
            {
                return BadRequest("Failed to add WordSuite");
            }

            if(wordSuite.ProhibitedQuizzesId.Count > 0)
            {
                if(!wordSuiteService.UpdateProhibitedQuizzes(wordSuiteId,wordSuite.ProhibitedQuizzesId))
                {
                    return BadRequest("Failed to add prohibited quizzes");
                }
            }
            if (wordSuite.WordTranslationsId.Count > 0)
            {
                if (!wordProgressService.AddRange(wordProgressMapper.MapRange(wordSuiteId, wordSuite.WordTranslationsId)))
                {
                    return BadRequest("Failed to add WordTranslations");
                }
            }

            return Ok();
        }

        [WowAuthorization(Roles = "Teacher")]
        //mfomitc
        [Route("ShareWordSuiteList")]
        public async Task<IHttpActionResult> ShareWordSuiteList(int teacherId, int wordSuiteId)
        {
            if (wordSuiteId <= 0 && teacherId <=0)
            {
                throw new ArgumentNullException("wordSuite and teacher", "WordSuite and teacher ID can't be negative!");
            }
            return await wordSuiteService.CopyWordsuitesForTeacherByIdAsync(teacherId, wordSuiteId) ?
                Ok() as IHttpActionResult : BadRequest() as IHttpActionResult;
        }

        [WowAuthorization(Roles = "Teacher")]
        //mfomitc
        [Route("ShareWordSuite")]
        public async Task<IHttpActionResult> ShareWordSuite(WordSuiteToShareModelTest model)
        {
            if (model.WordSuiteId <= 0 && model.teachersId == null)
            {
                throw new ArgumentNullException("wordSuite and teacher", "WordSuite and teacher ID can't be negative!");
            }
            bool sharedSuites = await wordSuiteService.CopyWordsuitesForTeacherListByIdAsync(model.teachersId, model.WordSuiteId);
            return sharedSuites ? Ok() as IHttpActionResult : BadRequest() as IHttpActionResult;
        }

        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult Delete(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException("id", "ID can't be negative or 0");
            }

            return wordSuiteService.Delete(id) ? Ok() as IHttpActionResult : BadRequest() as IHttpActionResult;
        }

        [WowAuthorization(Roles = "Teacher")]
        [Route("EditWordSuite")]
        public IHttpActionResult Post(WordSuiteEditModel wordSuite)
        {
            if (wordSuite == null)
            {
                throw new ArgumentNullException("wordSuite", "WordSuite can't be null");
            }

            if (wordSuite.IsBasicInfoChanged)
            {
                if (!wordSuiteService.Update(wordSuiteMapper.Map(wordSuite)))
                {
                    return BadRequest("Failed to edit WordSuite");
                }
            }

            if (wordSuite.AreProhibitedQuizzesChanged)
            {
                if(!wordSuiteService.UpdateProhibitedQuizzes(wordSuite.Id, wordSuite.ProhibitedQuizzesId))
                {
                    return BadRequest("Failed to change ProhibitedQuizzes in wordSuite");
                }
            }

            if (wordSuite.WordTranslationsToDeleteIdRange != null)
            {
                if (wordSuite.WordTranslationsToDeleteIdRange.Count > 0)
                {
                    if (!wordProgressService.RemoveRange(wordProgressMapper.MapRange(wordSuite.Id, wordSuite.WordTranslationsToDeleteIdRange)))
                    {
                        return BadRequest("Failed to remove WordTranslations");
                    }
                }
            }

            if (wordSuite.WordTranslationsToAddIdRange != null)
            {
                if (wordSuite.WordTranslationsToAddIdRange.Count > 0)
                {
                    if (!wordProgressService.AddRange(wordProgressMapper.MapRange(wordSuite.Id, wordSuite.WordTranslationsToAddIdRange)))
                    {
                        return BadRequest("Failed to add WordTranslations");
                    }
                }
            }

            return Ok();
        }

        [Route("GetTeacherWordSuites")]
        public async Task<List<WordSuiteEditModel>> GetTeacherWordSuites(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException("id", "ID can't be negative or 0");
            }
            return wordSuiteMapper.MapRangeForEdit(await wordSuiteService.GetTeacherWordSuitesAsync(id));
        }

        [Route("GetWordSuitesByLanguageId")]
        public async Task<List<CourseWordSuiteModel>> GetWordSuites(int languageId)
        {
            if (languageId <= 0)
            {
                throw new ArgumentNullException("languageID", "Language ID can't be negative or 0");
            }

            return wordSuiteMapper.Map(await wordSuiteService.GetWordSuitesByOwnerAndLanguageIdAsync(UserId, languageId));
        }

        [Route("GetWordSuiteByID")]
        public async Task<WordSuiteEditModel> GetByID(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException("id", "ID can't be negative or 0");
            }

            return wordSuiteMapper.MapForEdit(await wordSuiteService.GetByIDAsync(id));
        }



        [Route("AddWordProgresses")]
        public IHttpActionResult AddWordProgresses(List<WordProgressModel> wordProgresses)
        {
            if (wordProgresses == null)
                throw new ArgumentNullException("wordProgresses", "WordProgresses can't be null");

            return wordProgressService.AddRange(wordProgressMapper.MapRange(wordProgresses))
                ? Ok() as IHttpActionResult
                : BadRequest() as IHttpActionResult;
        }

        [Route("RemoveWordProgresses")]
        public IHttpActionResult RemoveWordProgress(WordProgressModel wordProgress)
        {
            if (wordProgress == null)
                throw new ArgumentNullException("wordProgress", "WordProgress can't be null");

            return wordProgressService.RemoveByStudent(wordProgressMapper.Map(wordProgress))
                ? Ok() as IHttpActionResult
                : BadRequest() as IHttpActionResult;
        }

        [Route("allQuizzes")]
        public IEnumerable<QuizModel> GetAllQuizzes()
        {
            return wordSuiteService.GetAllQuizzes().Select( ws => _quizMapper.Map(ws)).ToList();
        }
    }
}
