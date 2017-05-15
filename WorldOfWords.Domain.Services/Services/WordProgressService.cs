using System;
using System.Collections.Generic;
using System.Linq;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Factory;

namespace WorldOfWords.Domain.Services
{
    public class WordProgressService : IWordProgressService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public WordProgressService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public bool IsStudentWord(WordProgress wordProgress)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return uow
                        .WordProgressRepository.GetAll()
                        .Single(wp => wp.WordSuiteId == wordProgress.WordSuiteId &&
                                      wp.WordTranslationId == wordProgress.WordTranslationId).IsStudentWord;
            }
        }

        public void CopyProgressesForUsersInGroup(List<User> users, int groupId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var group = uow.GroupRepository.GetById(groupId);
                if (group == null)
                {
                    throw new ArgumentException("Group with id you are requesting does not exist");
                }
                var originalWordsuites = group.Course.WordSuites.Where(w => w.PrototypeId == null);
                var progressesToAdd = new List<WordProgress>();
                foreach (var user in users)
                {
                    foreach (var wordsuite in originalWordsuites)
                    {
                        var copiedWordSuite = group.Course.WordSuites.FirstOrDefault(w => w.PrototypeId == wordsuite.Id && w.OwnerId == user.Id);
                        progressesToAdd.AddRange(copiedWordSuite.PrototypeWordSuite.WordProgresses.Select(wp => new WordProgress
                        {
                            WordSuiteId = copiedWordSuite.Id,
                            WordTranslationId = wp.WordTranslationId,
                            Progress = 0
                        }));
                    }
                }
                uow.WordProgressRepository.Add(progressesToAdd);
                uow.Save();
            }
        }

        public bool AddRange(List<WordProgress> wordProgressRange)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                foreach (var wordProgress in wordProgressRange)
                {
                    uow.WordProgressRepository.AddOrUpdate(wordProgress);
                }
                uow.Save();
                return true;
            }
        }

        public bool RemoveRange(List<WordProgress> wordProgressRange)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                wordProgressRange.ForEach(wp =>
                    uow
                    .WordProgressRepository
                    .Delete(uow
                            .WordProgressRepository.GetAll()
                            .Single(dbWp => dbWp.WordSuiteId == wp.WordSuiteId
                                            && dbWp.WordTranslationId == wp.WordTranslationId)));
                uow.Save();
            }
            return true;
        }

        public bool RemoveByStudent(WordProgress wordProgress)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                if (IsStudentWord(wordProgress))
                {
                    uow
                    .WordProgressRepository
                    .Delete(uow
                        .WordProgressRepository.GetAll()
                        .Single(wp => wp.WordSuiteId == wordProgress.WordSuiteId &&
                                      wp.WordTranslationId == wordProgress.WordTranslationId));
                    uow.Save();
                    return true;
                }
                return false;
            }
        }

        public bool IncrementProgress(int wordSuiteId, int wordTranslationId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var wordProgress = uow.WordProgressRepository.GetAll().First(x => (x.WordSuiteId == wordSuiteId
                            && x.WordTranslationId == wordTranslationId));
                ++(wordProgress.Progress);
                uow.WordProgressRepository.AddOrUpdate(wordProgress);
                uow.Save();
                return true;
            }
        }

        public bool IncrementProgressOfOriginalWord(int wordSuiteId, string originalWord)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var wordProgress = uow.WordProgressRepository.GetAll().First(x => (x.WordSuiteId == wordSuiteId
                            && x.WordTranslation.OriginalWord.Value.ToLower() == originalWord.ToLower()));
                ++(wordProgress.Progress);
                uow.WordProgressRepository.AddOrUpdate(wordProgress);
                uow.Save();
                return true;
            }
        }

        public void RemoveProgressesForEnrollment(int enrollmentId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                Enrollment enrollment = uow.EnrollmentRepository.GetById(enrollmentId);
                if (enrollment == null)
                {
                    throw new ArgumentException("Enrollment with id you are requesting does not exist");
                }
                List<int> originalWordsuitesIds = enrollment.Group.Course.WordSuites.Where(w => w.PrototypeId == null).Select(w => w.Id).ToList();
                var wordsuitesToDeleteFrom = enrollment.Group.Course.WordSuites
                    .Where(w => w.OwnerId == enrollment.User.Id && w.PrototypeId != null
                        && originalWordsuitesIds.Contains((int)w.PrototypeId)).ToList();
                uow.WordProgressRepository.Delete(wordsuitesToDeleteFrom.SelectMany(ws => ws.WordProgresses));
                uow.Save();
            }
        }
    }
}
