using System.Collections.Generic;
using System.Threading.Tasks;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Domain.Services
{
    public interface IWordSuiteService
    {
        Task<List<WordSuite>> GetTeacherWordSuitesAsync(int id);
        Task<List<WordSuite>> GetWordSuitesByOwnerAndLanguageIdAsync(int userId, int languageId);
        Task<WordSuite> GetByIDAsync(int id);
        Task<WordSuite> GetWithNotStudiedWordsAsync(int id);
        Task SetTimeAsync(int id);
        Task CopyWordsuitesForUsersByGroupAsync(List<User> users, int groupId);
        //mfomitc
        Task<bool> CopyWordsuitesForTeacherByIdAsync(int teacherId, int wordSuiteId);
        Task<bool> CopyWordsuitesForTeacherListByIdAsync(List<int> teacherIds, int wordSuiteId);
        void RemoveWordSuitesForEnrollment(int enrollmentId);
        double GetWordSuiteProgress(int id);
        Task<List<WordTranslationFreq>> GetTranslationsFreqAddedToWordSuiteByUserAsync(int wordSuiteId);
        int Add(WordSuite wordSuite);
        bool Update(WordSuite wordSuite);
        bool Delete(int id);
        bool UpdateProhibitedQuizzes(int wordSuiteId, IEnumerable<int> quizzesId);
        IEnumerable<Quiz> GetAllQuizzes();
        bool DoesContainsAtLeastOneRecord(int wordSuiteId);
        bool DoesContainsAtLeastOnePicture(int wordSuiteId);
    }
}
