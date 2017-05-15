using System;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Contracts;

namespace WorldOfWords.Infrastructure.Data.EF.UnitOfWork
{
    public interface IWorldOfWordsUow : IDisposable
    {
        IRepository<Enrollment> EnrollmentRepository { get; }
        IRepository<Group> GroupRepository { get; }
        IRepository<Role> RoleRepository { get; }
        IRepository<Word> WordRepository { get; }
        IRepository<Language> LanguageRepository { get; }
        IRepository<Course> CourseRepository { get; }
        IRepository<Tag> TagRepository { get; }
        IRepository<Ticket> TicketRepository { get; }
        IRepository<Picture> PictureRepository { get; }
        IRepository<Record> RecordRepository { get; }
        IRepository<PartsOfSpeech> PartsOfSpeechRepository { get; }
        IRepository<Quiz> QuizRepository { get; }
        IWordTranslationRepository WordTranslationRepository { get; }
        IWordSuiteRepository WordSuiteRepository { get; }
        IWordProgressRepository WordProgressRepository { get; }
        IIncomingUserRepository IncomingUserRepository { get; }
        IUserRepository UserRepository { get; }

        void Save();
        Task<int> SaveAsync();
        Task<bool> SaveAsyncBool();
    }
}
