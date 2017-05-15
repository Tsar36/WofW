using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Domain.Services
{
    public interface IWordProgressService
    {
        void CopyProgressesForUsersInGroup(List<User> users, int groupId);
        bool AddRange(List<WordProgress> wordProgressRange);
        bool RemoveRange(List<WordProgress> wordProgressRange);
        bool IncrementProgress(int wordSuiteId, int wordTranslationId);
        bool IncrementProgressOfOriginalWord(int wordSuiteId, string originalWord);
        void RemoveProgressesForEnrollment(int enrollmentId);
        bool RemoveByStudent(WordProgress wordProgress);
        bool IsStudentWord(WordProgress wordProgress);
    }
}
