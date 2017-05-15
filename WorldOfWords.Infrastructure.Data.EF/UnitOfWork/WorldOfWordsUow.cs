using System;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Contracts;
using WorldOfWords.Infrastructure.Data.EF.Repositories;

namespace WorldOfWords.Infrastructure.Data.EF.UnitOfWork
{
    public class WorldOfWordsUow : IWorldOfWordsUow
    {
        private readonly WorldOfWordsDatabaseContext _context = new WorldOfWordsDatabaseContext();
        private bool _disposed;

        private IRepository<Ticket> _ticketRepository;
        private IRepository<Role> _roleRepository;
        private IRepository<Word> _wordRepostiory;
        private IRepository<Language> _languageRepository;
        private IRepository<Group> _groupRepository;
        private IRepository<Enrollment> _enrollmentRepository;
        private IRepository<Course> _courseRepository;
        private IRepository<Tag> _tagRepository;
        private IRepository<Picture> _pictureRepository;
        private IRepository<Quiz> _quizRepository;
        private IRepository<Record> _recordRepository;
        private IRepository<PartsOfSpeech> _partsOfSpeechRepository; 
        private IWordTranslationRepository _wordTranslationRepository;
        private IWordSuiteRepository _wordSuiteRepository;
        private IWordProgressRepository _wordProgressRepository;
        private IIncomingUserRepository _incomingUserRepository;
        private IUserRepository _userRepository;


        public IRepository<Enrollment> EnrollmentRepository
        {
            get
            {
                return _enrollmentRepository ?? (_enrollmentRepository = new EfRepository<Enrollment>(_context)); 
            }
        }
        public IRepository<Group> GroupRepository
        {
            get
            {
                return _groupRepository ?? (_groupRepository = new EfRepository<Group>(_context));
            }
        }
        public IRepository<Role> RoleRepository
        {
            get
            {
                return _roleRepository ?? (_roleRepository = new EfRepository<Role>(_context));
            }
        }
        public IRepository<Word> WordRepository
        {
            get
            {
                return _wordRepostiory ?? (_wordRepostiory = new EfRepository<Word>(_context));
            }
        }
        public IRepository<Language> LanguageRepository
        {
            get
            {
                return _languageRepository ?? (_languageRepository = new EfRepository<Language>(_context));
            }
        }
        public IRepository<Course> CourseRepository
        {
            get
            {
                return _courseRepository ?? (_courseRepository = new EfRepository<Course>(_context));
            }
        }
        public IRepository<Tag> TagRepository
        {
            get
            {
                return _tagRepository ?? (_tagRepository = new EfRepository<Tag>(_context));
            }
        }
        public IRepository<Record> RecordRepository
        {
            get
            {
                return _recordRepository ?? (_recordRepository = new EfRepository<Record>(_context));
            }
        }
        public IWordTranslationRepository WordTranslationRepository
        {
            get
            {
                return _wordTranslationRepository ??
                       (_wordTranslationRepository = new WordTranslationRepository(_context));
            }
        }
        public IWordSuiteRepository WordSuiteRepository
        {
            get
            {
                return _wordSuiteRepository ?? (_wordSuiteRepository = new WordSuiteRepository(_context));
            }
        }
        public IWordProgressRepository WordProgressRepository
        {
            get
            {
                return _wordProgressRepository ?? (_wordProgressRepository = new WordProgressRepository(_context));
            }
        }
        public IIncomingUserRepository IncomingUserRepository
        {
            get
            {
                return _incomingUserRepository ?? (_incomingUserRepository = new IncomingUserRepository(_context));
            }
        }
        public IUserRepository UserRepository
        {
            get
            {
                return _userRepository ?? (_userRepository = new UserRepository(_context));
            }
        }
        public IRepository<Ticket> TicketRepository
        {
            get
            {
                return _ticketRepository ?? (_ticketRepository = new EfRepository<Ticket>(_context));
            }
        }
        public IRepository<Picture> PictureRepository
        {
            get
            {
                return _pictureRepository ?? (_pictureRepository = new EfRepository<Picture>(_context));
            }
        }

        public IRepository<PartsOfSpeech> PartsOfSpeechRepository
        {
            get
            {
                return _partsOfSpeechRepository ?? (_partsOfSpeechRepository = new EfRepository<PartsOfSpeech>(_context));
            }
        }

        public IRepository<Quiz> QuizRepository
        {
            get
            {
                return _quizRepository ?? (_quizRepository = new EfRepository<Quiz>(_context));
            }
        }


        public void Save()
        {
            _context.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        public async Task<bool> SaveAsyncBool()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
