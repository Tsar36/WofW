using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WorldOfWords.Domain.Models;
using WorldOfWords.API.Models;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using System.Threading.Tasks;

namespace WorldOfWords.Domain.Services
{
    public class WordSuiteService : IWordSuiteService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public WordSuiteService(IUnitOfWorkFactory unitOfWorkFactory, IQuizMapper quizMapper)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<WordSuite> GetByIDAsync(int id)
        {
            WordSuite wordSuite;
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                wordSuite = await uow
                            .WordSuiteRepository.GetAll()
                            .Include(ws => ws.Language)
                            .Include(ws => ws.WordProgresses.Select(wp => wp.WordTranslation).Select(wt => wt.OriginalWord))
                            .Include(ws => ws.WordProgresses.Select(wp => wp.WordTranslation).Select(wt => wt.TranslationWord))
                            .Include(ws => ws.ProhibitedQuizzes)
                            .FirstAsync(x => x.Id == id);
            }
            return wordSuite;
        }



        public async Task SetTimeAsync(int id)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var suite = await uow
                    .WordSuiteRepository.GetByIdAsync(id);
                suite.QuizStartTime = DateTime.Now;
                await uow.SaveAsync();
            }
        }

        public void RemoveWordSuitesForEnrollment(int enrollmentId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                Enrollment enrollment = uow.EnrollmentRepository.GetById(enrollmentId);
                if (enrollment == null)
                {
                    throw new ArgumentException("Enrollment with id you are requesting does not exist");
                }
                List<int> originalWordsuitesIds = enrollment.Group.Course.WordSuites.Where(w => w.PrototypeId == null).Select(w => w.Id).ToList();
                uow.WordSuiteRepository.Delete(enrollment.Group.Course.WordSuites
                    .Where(w => w.OwnerId == enrollment.User.Id && w.PrototypeId != null
                        && originalWordsuitesIds.Contains((int)w.PrototypeId)));
                uow.Save();
            }
        }

        public async Task<WordSuite> GetWithNotStudiedWordsAsync(int id)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var suite = await uow.WordSuiteRepository.GetAll()
                    .Include(ws => ws.WordProgresses.Select(wp => wp.WordTranslation).Select(wt => wt.OriginalWord))
                    .Include(ws => ws.WordProgresses.Select(wp => wp.WordTranslation).Select(wt => wt.TranslationWord))
                    .Include(ws => ws.ProhibitedQuizzes)
                    .FirstOrDefaultAsync(ws => ws.Id == id);
                if (suite != null)
                {
                    var n = suite.Threshold;
                    suite.WordProgresses = suite.WordProgresses.Where(x => (x.Progress < n)).ToList();
                }
                return suite;
            }
        }

        public double GetWordSuiteProgress(int id)
        {
            double progress = 0;
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var suite = uow
                    .WordSuiteRepository.GetAll()
                    .Include(ws => ws.WordProgresses)
                    .Include(ws => ws.ProhibitedQuizzes)
                    .FirstOrDefault(ws => ws.Id == id);
                if (suite != null)
                {
                    double allProgress = suite.Threshold * suite.WordProgresses.Count;
                    var userProgress = (int)suite.WordProgresses.Select(x => x.Progress).Sum();
                    progress = userProgress / allProgress;
                }
                return Math.Round(progress * 100);
            }
        }

        public async Task CopyWordsuitesForUsersByGroupAsync(List<User> users, int groupId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                Group group = await uow.GroupRepository.GetByIdAsync(groupId);
                if (group == null)
                {
                    throw new ArgumentException("Group with id you are requesting does not exist");
                }
                List<WordSuite> wordsuitesToCopy = group.Course.WordSuites.Where(w => w.PrototypeId == null).ToList();
                List<WordSuite> wordsuitesToAdd = new List<WordSuite>();
                foreach (var user in users)
                {
                    wordsuitesToAdd.AddRange(wordsuitesToCopy.Select(w => new WordSuite
                    {
                        Name = w.Name,
                        LanguageId = w.LanguageId,
                        Threshold = w.Threshold,
                        QuizResponseTime = w.QuizResponseTime,
                        QuizStartTime = null,
                        OwnerId = user.Id,
                        PrototypeId = w.Id,
                        Courses = new[] { group.Course }
                    }));
                }
                uow.WordSuiteRepository.Add(wordsuitesToAdd);
                await uow.SaveAsync();
            }
        }

        // mfomitc
        public async Task<bool> CopyWordsuitesForTeacherByIdAsync(int teacherId, int wordSuiteId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                WordSuite wordSuite =               await uow.WordSuiteRepository.GetByIdAsync(wordSuiteId);
                        if (wordSuite == null)
                        {
                            throw new ArgumentException("Word Suite with id you are requesting does not exist");
                        }   
                User teacherWhoShare =              await uow.UserRepository.GetByIdAsync(wordSuite.OwnerId);
                List<WordProgress> wordProgresses = await uow.WordProgressRepository.GetAll().Where(t => t.WordSuiteId == wordSuiteId).ToListAsync();             
                WordSuite wordSuiteToCopy = new WordSuite
                        {
                            Name = wordSuite.Name + "_(Shared_by_" + teacherWhoShare.Name + ")",
                            LanguageId = wordSuite.LanguageId,
                            Threshold = wordSuite.Threshold,
                            QuizResponseTime = wordSuite.QuizResponseTime,
                            QuizStartTime = wordSuite.QuizStartTime,
                            OwnerId = teacherId,
                            PrototypeId = null
                        };
                List<WordTranslation> wordTranslationsToCopy = new List<WordTranslation>();
                        foreach (var wordProgress in wordProgresses)
                        {
                            wordTranslationsToCopy.Add(wordProgress.WordTranslation);
                        }                
                List<WordProgress> WordProgressList = new List<WordProgress>();
                        foreach (var wordTranslation in wordTranslationsToCopy)
                        {
                            WordProgress wordProgress = new WordProgress { WordSuite = wordSuiteToCopy, WordTranslation = wordTranslation};
                            WordProgressList.Add(wordProgress);
                        }                
                uow.WordSuiteRepository.Add(wordSuiteToCopy);
                uow.WordProgressRepository.Add(WordProgressList);
                await uow.SaveAsync();
                return true;
            }                            
        }

        public async Task<bool> CopyWordsuitesForTeacherListByIdAsync(List<int> teacherIds, int wordSuiteId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                WordSuite wordSuite = await uow.WordSuiteRepository.GetByIdAsync(wordSuiteId);
                if (wordSuite == null)
                {
                    throw new ArgumentException("Word Suite with id you are requesting does not exist");
                }
                User teacherWhoShare = await uow.UserRepository.GetByIdAsync(wordSuite.OwnerId);
                List<WordProgress> wordProgresses = await uow.WordProgressRepository.GetAll().Where(t => t.WordSuiteId == wordSuiteId).ToListAsync();
                
                List<WordSuite> wordSuitesToCopy = new List<WordSuite>();
                foreach (int teacherId in teacherIds)
                {
                    WordSuite wordSuiteToCopy = new WordSuite
                    {
                        Name = wordSuite.Name + "_(Shared_by_" + teacherWhoShare.Name + ")",
                        LanguageId = wordSuite.LanguageId,
                        Threshold = wordSuite.Threshold,
                        QuizResponseTime = wordSuite.QuizResponseTime,
                        QuizStartTime = wordSuite.QuizStartTime,

                        OwnerId = teacherId,
                        PrototypeId = null
                    };
                    wordSuitesToCopy.Add(wordSuiteToCopy);
                };
                List<WordTranslation> wordTranslationsToCopy = new List<WordTranslation>();
                foreach (var wordProgress in wordProgresses)
                {
                    wordTranslationsToCopy.Add(wordProgress.WordTranslation);
                }
                List<WordProgress> WordProgressList = new List<WordProgress>();
                foreach (var wordTranslation in wordTranslationsToCopy)
                {
                    foreach (var wordSuiteToCopy in wordSuitesToCopy)
                    {
                        WordProgress wordProgress = new WordProgress { WordSuite = wordSuiteToCopy, WordTranslation = wordTranslation };
                        WordProgressList.Add(wordProgress);
                    }
                }
                uow.WordSuiteRepository.Add(wordSuitesToCopy);
                uow.WordProgressRepository.Add(WordProgressList);
                await uow.SaveAsync();
                return true;
            }
        }

        public async Task<List<WordSuite>> GetTeacherWordSuitesAsync(int id)
        {
            List<WordSuite> wordSuites;
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                wordSuites = await uow.WordSuiteRepository.GetAll()
                    .Include(ws => ws.Language)
                    .Include(ws => ws.TranslationLanguage)
                    .Include(ws => ws.ProhibitedQuizzes)
                    .Where(ws => ws.OwnerId == id && ws.PrototypeId == null)
                    .ToListAsync();
            }
            return wordSuites;
        }

        public async Task<List<WordSuite>> GetWordSuitesByOwnerAndLanguageIdAsync(int userId, int languageId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return await uow.WordSuiteRepository.GetAll().
                    Include(ws => ws.ProhibitedQuizzes).
                     Where(ws => ws.OwnerId == userId && ws.PrototypeId == null && ws.LanguageId == languageId).
                     ToListAsync();
            }
        }

        public async Task<List<WordTranslationFreq>> GetTranslationsFreqAddedToWordSuiteByUserAsync(int baseWordSuiteId)
        {
            List<WordTranslationFreq> result;
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                try
                {
                    result = await uow.WordProgressRepository.GetAll()
                        .Where(x => x.WordSuite.PrototypeId == baseWordSuiteId && x.IsStudentWord)
                        .GroupBy(x => x.WordTranslationId)
                        .Select(x =>
                            new WordTranslationFreq
                            {
                                Id = x.FirstOrDefault().WordTranslation.Id,
                                OriginalWord = x.FirstOrDefault().WordTranslation.OriginalWord.Value,
                                TranslationWord = x.FirstOrDefault().WordTranslation.TranslationWord.Value,
                                Freq = x.Count()
                            }
                        )
                        .OrderByDescending(x => x.Freq)
                        .ToListAsync();
                }
                catch (NullReferenceException)
                {
                    result = new List<WordTranslationFreq>();
                }
            }
            return result;
        }

        public int Add(WordSuite wordSuite)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                uow.WordSuiteRepository.AddOrUpdate(wordSuite);
                uow.Save();
                return wordSuite.Id;
            }
        }

        public bool Update(WordSuite wordSuite)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var oldWordSuite = uow.WordSuiteRepository.GetById(wordSuite.Id);
                oldWordSuite.Name = wordSuite.Name;
                oldWordSuite.Threshold = wordSuite.Threshold;
                oldWordSuite.QuizResponseTime = wordSuite.QuizResponseTime;      
                uow.WordSuiteRepository.Update(oldWordSuite);
                uow.Save();
            }
            return true;
        }

        public bool Delete(int id)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                uow.WordProgressRepository.Delete(uow.WordProgressRepository.GetAll().Where(wp => wp.WordSuiteId == id));
                uow.WordSuiteRepository.Delete(id);
                uow.Save();
                return true;
            }
        }

        public bool UpdateProhibitedQuizzes(int wordSuiteId, IEnumerable<int> prohibitedQuizzesId)
        {
            if (prohibitedQuizzesId == null)
            {
                throw new ArgumentException("prohibitedQuizzesId is null");
            }

            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                // get all prohibitedQuizzes
                List<Quiz> quizzes = new List<Quiz>();
                foreach (int id in prohibitedQuizzesId)
                {
                    quizzes.Add(uow.QuizRepository.GetById(id));
                }

                // get all child word suites
                var childWordSuites = uow.WordSuiteRepository.GetAll().Where(w => w.PrototypeId == wordSuiteId).ToList();

                // main wordSuite
                var wordSuite = uow.WordSuiteRepository.GetById(wordSuiteId);

                // clear quizzes
                wordSuite.ProhibitedQuizzes.Clear();

                foreach (WordSuite ws in childWordSuites)
                {
                    ws.ProhibitedQuizzes.Clear();
                }

                // restoring quizzes
                foreach(Quiz quiz in quizzes)
                {
                    foreach (WordSuite ws in childWordSuites)
                    {
                        ws.ProhibitedQuizzes.Add(quiz);
                    }

                    wordSuite.ProhibitedQuizzes.Add(quiz);
                }

                uow.Save();
                return true;
            }
        }

        public IEnumerable<Quiz> GetAllQuizzes()
        {
            using(var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return uow.QuizRepository.GetAll()
                    .Include(q => q.ProhibitedWordSuits)
                    .ToList();
            }
        }

        public bool DoesContainsAtLeastOnePicture(int wordSuiteId)
        {
            using(var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var originalWordsId = uow.WordSuiteRepository.GetById(wordSuiteId)
                    .WordProgresses
                    .Select(w => w.WordTranslation.OriginalWordId);
                return uow.PictureRepository.GetAll()
                    .Where(p => originalWordsId
                        .Contains(p.WordId))
                    .Count() > 0;
            }
        }

        public bool DoesContainsAtLeastOneRecord(int wordSuiteId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var originalWordsId = uow.WordSuiteRepository.GetById(wordSuiteId)
                    .WordProgresses
                    .Select(w => w.WordTranslation.OriginalWordId);
                return uow.RecordRepository.GetAll()
                    .Where(p => originalWordsId
                        .Contains(p.WordId))
                    .Count() > 0;
            }
        }
    }
}
