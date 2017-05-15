using System.Collections.Generic;
using System.Web.Http;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Services;
using System.Linq;
using System;
using WorldOfWords.Domain.Services.MessagesAndConsts;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;

namespace WorldofWords.Controllers
{
    [WowAuthorization(Roles = "Student")]
    [RoutePrefix("api/TrainingWordSuite")]
    public class TrainingWordSuiteController : BaseController
    {
        private readonly IQuizWordSuiteMapper _quizMapper;
        private readonly ITrainingWordSuiteMapper _trainingMapper;
        private readonly IWordProgressService _progressService;
        private readonly IWordSuiteService _service;
        private readonly IWordProgressMapper _progressMapper;
        private readonly IWordTranslationService _wordTranslationService;
        private readonly IWordService _wordService;

        public TrainingWordSuiteController(IQuizWordSuiteMapper quizMapper, ITrainingWordSuiteMapper trainingMapper, IWordSuiteService service,
            IWordProgressService progressService, IWordProgressMapper progressMapper, IWordTranslationService wordTranslationService,
            IWordService wordService)
        {
            _quizMapper = quizMapper;
            _trainingMapper = trainingMapper;
            _service = service;
            _progressService = progressService;
            _progressMapper = progressMapper;
            _wordTranslationService = wordTranslationService;
            _wordService = wordService;
        }

        private static void Shuffle<T>(IList<T> list)
        {
            Random random = new Random();
            var currentNode = list.Count;
            while (currentNode > 1)
            {
                currentNode--;
                var randomNode = random.Next(currentNode + 1);
                T value = list[randomNode];
                list[randomNode] = list[currentNode];
                list[currentNode] = value;
            }
        }

        [Route("AllWords")]
        public async Task<TrainingWordSuiteModel> GetWordSuiteWithAllWords(int id)
        {
            var wordSuite = _trainingMapper.Map(await _service.GetByIDAsync(id));
            foreach (WordTranslationModel word in wordSuite.WordTranslations)
            {
                if (_progressService.IsStudentWord(_progressMapper.Map(wordSuite.Id, word.Id)))
                {
                    word.IsStudentWord = true;
                }
                else
                {
                    word.IsStudentWord = false;
                }
            }
            return wordSuite;
        }
        [Route("ExtensionTranslationsFreq")]
        public async Task<List<WordTranslationFreq>> GetTranslationsFreqAddedToWordSuiteByUser(int baseWordSuiteId)
        {
            return await _service.GetTranslationsFreqAddedToWordSuiteByUserAsync(baseWordSuiteId);
        }

        [Route("NotStudiedWords")]
        public async Task<TrainingWordSuiteModel> GetWordSuiteWithNotStudiedWords(int id)
        {
            return _trainingMapper.Map(await _service.GetWithNotStudiedWordsAsync(id));
        }

        [Route("Task")]
        public async Task<IHttpActionResult> GetTask(int id)
        {
            var suite = _quizMapper.Map(await _service.GetWithNotStudiedWordsAsync(id));
            if (suite.OwnerId == UserId)
            {
                if (suite.QuizStartTime == null)
                {
                    suite.QuizStartTime = new DateTime();
                }
                if (((DateTime)suite.QuizStartTime).AddDays(1) < DateTime.Now)
                {
                    await _service.SetTimeAsync(suite.Id);
                    Shuffle(suite.WordTranslations);
                    return Ok(suite);
                }
                return BadRequest(MessagesContainer.OneQuizPerDay);
            }
            return BadRequest(MessagesContainer.NotYourQuiz);
        }

        [HttpPost]
        [Route("CheckTask")]
        public async Task<IHttpActionResult> CheckTask(TrainingWordSuiteModel answer)
        {
            TrainingWordSuiteModel wordSuite = _trainingMapper.Map(await _service.GetWithNotStudiedWordsAsync(answer.Id));
            DateTime endTime = wordSuite.QuizStartTime.Value.
                AddSeconds(wordSuite.QuizResponseTime * wordSuite.WordTranslations.Count + 20);
            if (endTime > DateTime.Now)
            {
                Check(answer, wordSuite);
                return Ok(answer);
            }
            return BadRequest(MessagesContainer.TimeCheating);
        }

        private void Check(TrainingWordSuiteModel answer, TrainingWordSuiteModel wordSuite)
        {
            foreach (var word in answer.WordTranslations)
            {
                word.OriginalWord = wordSuite.WordTranslations.First(x => x.Id == word.Id).OriginalWord;
                if (word.TranslationWord.ToLower() == word.OriginalWord.ToLower())
                {
                    word.Result = true;
                    _progressService.IncrementProgress(answer.Id, word.Id);
                }
            }
        }

        [Route("SynonymTask")]
        public async Task<IHttpActionResult> GetSynonymTask(int id)
        {
            var suite = await _service.GetWithNotStudiedWordsAsync(id);
            if (suite.OwnerId == UserId)
            {
                if (suite.QuizStartTime == null)
                {
                    suite.QuizStartTime = new DateTime();
                }
                if (((DateTime)suite.QuizStartTime).AddDays(1) < DateTime.Now)
                {
                    TrainingSynonymTask task = new TrainingSynonymTask();
                    task.WordSuiteId = suite.Id;
                    task.WordSuiteName = suite.Name;
                    task.QuizResponseTime = suite.QuizResponseTime;
                    task.Synonyms = new List<List<WordValueModel>>();

                    foreach (var wordProgress in suite.WordProgresses)
                    {
                        var synonyms = await _wordTranslationService.GetAllWordSynonymsById(
                            wordProgress.WordTranslation.OriginalWordId);
                        if (synonyms.Count != 0)
                            task.Synonyms.Add(synonyms);
                    }

                    return Ok(task);
                }
                return BadRequest(MessagesContainer.OneQuizPerDay);
            }
            return BadRequest(MessagesContainer.NotYourQuiz);
        }

        [HttpPost]
        [Route("CheckSynonymTask")]
        public async Task<IHttpActionResult> CheckSynonymTask(TrainingSynonymTask task)
        {
            if (task == null) return BadRequest("Invalid parameter");

            // Only for demo
            //await _service.SetTimeAsync(task.WordSuiteId);

            if (task.Answer.Count == 0 || task.Synonyms.Count == 0 || task.Answer.Count != task.Synonyms.Count)
                return BadRequest("Invalid model");

            task.Result = new List<bool>();
            for (int i = 0; i < task.Synonyms.Count; i++)
            {
                if (task.Synonyms[i].Count == 0) return BadRequest("Invalid model");

                var synonyms = await _wordTranslationService.GetAllWordSynonymsById(task.Synonyms[i][0].Id ?? default(int));

                var result = synonyms.Exists(x => x.Value.ToLower() == task.Answer[i].ToLower());
                task.Result.Add(result);

                if (result) _progressService.IncrementProgressOfOriginalWord(task.WordSuiteId, task.Answer[i]);
            }

            return Ok(task);
        }

        [Route("MixTask")]
        public async Task<IHttpActionResult> GetMixTask(int id)
        {
            var suite = await _service.GetWithNotStudiedWordsAsync(id);
            if (suite.OwnerId == UserId)
            {
                if (suite.QuizStartTime == null)
                {
                    suite.QuizStartTime = new DateTime();
                }
                if (((DateTime)suite.QuizStartTime).AddDays(1) < DateTime.Now)
                {
                    TrainingMixTask task = new TrainingMixTask();
                    task.WordSuiteId = suite.Id;
                    task.WordSuiteName = suite.Name;
                    task.QuizResponseTime = suite.QuizResponseTime;
                    task.Questions = new List<MixTaskQuestionModel>();

                    foreach (var wordProgress in suite.WordProgresses)
                    {
                        var synonyms = await _wordTranslationService.GetAllWordSynonymsById(
                            wordProgress.WordTranslation.OriginalWordId);
                        
                        if (synonyms.Count != 0)
                        {
                            var taskQuestion = new MixTaskQuestionModel()
                            {
                                Synonyms = synonyms
                            };
                            task.Questions.Add(taskQuestion);
                        }

                        var wordTranslation = wordProgress.WordTranslation;

                        if (wordTranslation.OriginalWord.Description != null)
                        {
                            var taskQuestion = new MixTaskQuestionModel()
                            {
                                Description = new WordValueModel
                                {
                                    Id = wordTranslation.OriginalWordId,
                                    Value = wordTranslation.OriginalWord.Description
                                }
                            };
                            task.Questions.Add(taskQuestion);
                        }

                        task.Questions.Add(new MixTaskQuestionModel()
                        {
                            Translation = new WordValueModel
                            {
                                Id = wordTranslation.Id,
                                Value = wordTranslation.TranslationWord.Value
                            }
                        });
                    }

                    Shuffle(task.Questions);

                    return Ok(task);
                }
                return BadRequest(MessagesContainer.OneQuizPerDay);
            }
            return BadRequest(MessagesContainer.NotYourQuiz);
        }

        [HttpPost]
        [Route("CheckMixTask")]
        public async Task<IHttpActionResult> CheckMixTask(TrainingMixTask task)
        {
            if (task == null) return BadRequest("Invalid parameter");

            //Only for demo
            //await _service.SetTimeAsync(task.WordSuiteId);

            if (task.Answer.Count == 0 || task.Questions.Count == 0 || task.Answer.Count != task.Questions.Count)
                return BadRequest("Invalid model");

            task.Result = new List<bool>();
            for (int i = 0; i < task.Questions.Count; i++)
            {
                if (task.Questions[i].Synonyms != null)
                {
                    if (task.Questions[i].Synonyms.Count == 0) return BadRequest("Invalid model");

                    var synonyms = await _wordTranslationService.GetAllWordSynonymsById(task.Questions[i].Synonyms[0].Id ?? default(int));

                    var result = synonyms.Exists(x => x.Value.ToLower() == task.Answer[i].ToLower());
                    task.Result.Add(result);

                    if (result) _progressService.IncrementProgressOfOriginalWord(task.WordSuiteId, task.Answer[i]);
                }
                else if (task.Questions[i].Translation != null)
                {
                    var wordTranslation = await _wordTranslationService.GetByIdWithWordsAsync(task.Questions[i].Translation.Id ?? default(int));
                    if (wordTranslation != null)
                        task.Result.Add(wordTranslation.OriginalWord.Value.ToLower() == task.Answer[i].ToLower());
                    else
                        return InternalServerError();

                }
                else if (task.Questions[i].Description != null)
                {
                    var word = await _wordService.GetWordByIdAsync(task.Questions[i].Description.Id ?? default(int));
                    if (word != null)
                        task.Result.Add(word.Value.ToLower() == task.Answer[i].ToLower());
                    else return InternalServerError();
                }
                else
                {
                    return BadRequest("Invalid model");
                }
            }

            return Ok(task);
        }

        [Route("DescriptionTask")]
        public async Task<IHttpActionResult> GetDescriptionTask(int id)
        {
            var _suite = _quizMapper.Map(await _service.GetWithNotStudiedWordsAsync(id));
            TrainingWordSuiteModel suite = new TrainingWordSuiteModel();
            suite.Id = _suite.Id;
            suite.OwnerId = _suite.OwnerId;
            suite.QuizResponseTime = _suite.QuizResponseTime;
            suite.QuizStartTime = _suite.QuizStartTime;
            suite.Name = _suite.Name;
            suite.WordTranslations = new List<WordTranslationModel>();
            foreach (var t in _suite.WordTranslations)
            {
                if (t.OriginalWordDesc != null)
                {
                    suite.WordTranslations.Add(t);
                }

            }
            if (suite.OwnerId == UserId)
            {
                if (suite.QuizStartTime == null)
                {
                    suite.QuizStartTime = new DateTime();
                }
                if (((DateTime)suite.QuizStartTime).AddDays(1) < DateTime.Now)
                {
                    await _service.SetTimeAsync(suite.Id);
                    Shuffle(suite.WordTranslations);
                    return Ok(suite);
                }
                return BadRequest(MessagesContainer.OneQuizPerDay);
            }
            return BadRequest(MessagesContainer.NotYourQuiz);
        }
        [HttpGet]
        [Route("DoesContainAtLeastOneRecord")]
        public bool DoesContainsAtLeastOneRecord(int wordSuiteId)
        {
            return _service.DoesContainsAtLeastOneRecord(wordSuiteId);
        }
        [HttpGet]
        [Route("DoesContainAtLeastOnePicture")]
        public bool DoesContainsAtLeastOnePicture(int wordSuiteId)
        {
            return _service.DoesContainsAtLeastOnePicture(wordSuiteId);
        }

    }
}
