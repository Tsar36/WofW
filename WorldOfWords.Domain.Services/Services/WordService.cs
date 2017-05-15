using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using System;
using System.Text;
using WorldOfWords.Domain.Services.IServices;
using System.Data.Entity;
using WorldOfWords.API.Models.IMappers;
using WorldOfWords.API.Models.Models;
using WorldOfWords.API.Models;


namespace WorldOfWords.Domain.Services.Services
{
    public class WordService : IWordService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IWordMapper wordMapper;

        public WordService(IUnitOfWorkFactory unitOfWorkFactory, IWordMapper wordMapper)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            this.wordMapper = wordMapper;
        }

        public List<Word> GetAllByTags(List<string> tags, int languageId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var words = uow.WordRepository.GetAll()
                        .Where(w => w.LanguageId == languageId &&
                                    w.Tags.Select(t => t.Name).Contains(tags[0]));

                if (tags.Count > 1)
                {
                    foreach (string tag in tags)
                    {
                        words = words.Where(wt => wt.Tags.Select(t => t.Name).Contains(tag));
                    }
                }

                return words.ToList();
            }
        }

        public async Task<List<Word>> GetTopBySearchWordAsync(string searchWord, int languageId, int count)
        {
            List<Word> words;
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                words = await uow.WordRepository.GetAll()
                    .Where(w => w.Value.Contains(searchWord) &&
                                w.LanguageId == languageId)
                    .OrderBy(w => w.Value.IndexOf(searchWord))
                    .ThenBy(w => w.Value)
                    .Take(count)
                    .ToListAsync();
            }            return words;
            }

        public async Task<List<Word>> GetAllWordsAsync()
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return await uow.WordRepository.GetAll().ToListAsync();
            }
        }

        public async Task<List<Word>> GetAllWordsBySpecificLanguageAsync(int languageId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return await (from words in uow.WordRepository.GetAll() where words.LanguageId == languageId select words).ToListAsync();
            }
        }

        public async Task<int> ExistsAsync(string value, int languageId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var word = await uow.WordRepository.GetAll().FirstOrDefaultAsync(w => w.Value == value && w.LanguageId == languageId);

                if (word != null)
                {
                    return word.Id;
                }
                else
                {
                    return 0;
                }
            }
        }

        public async Task<int> AddAsync(WordModel word)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                int existResult = await ExistsAsync(word.Value, word.LanguageId);
                if (existResult > 0)
                {
                    return -1;
                }
                var wordToAdd = wordMapper.ToDomainModel(word);
                uow.WordRepository.Add(wordToAdd);
                await uow.SaveAsync();
                return wordToAdd.Id;
            }
        }

        public async Task<Word> GetWordByIdAsync(int id)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return await uow.WordRepository.GetByIdAsync(id);
            }
        }

        public Word GetWordById(int id)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return uow.WordRepository.GetById(id);
            }
        }

        public async Task<bool> UpdateAsync(WordModel word)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var oldWord = uow.WordRepository.GetById(word.Id);
                oldWord.Description = word.Description;
                oldWord.Transcription = word.Transcription;
                oldWord.PartOfSpeechId = word.PartOfSpeechId;
                oldWord.Comment = word.Comment;

                uow.WordRepository.Update(oldWord);
                await uow.SaveAsync();
            }
            return true;
        }

        public int Add(WordModel word)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                int existResult = Exists(word.Value, word.LanguageId);
                if (existResult > 0)
                {
                    return -1;
                }
                var wordToAdd = wordMapper.ToDomainModel(word);
                uow.WordRepository.Add(wordToAdd);
                uow.Save();
                return wordToAdd.Id;
            }
        }

        public int Exists(string value, int languageId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var word = uow.WordRepository.GetAll().FirstOrDefault(w => w.Value == value && w.LanguageId == languageId);

                if (word != null)
                {
                    return word.Id;
                }
                else
                {
                    return 0;
                }
            }
        }
        public async Task<Word> GetFirstByWordAsync(string searchWord)
        {
            Word word;
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                word = await uow.WordRepository.GetAll().FirstOrDefaultAsync(w => w.Value == searchWord);
            }
            return word;
        }

        public async Task<bool> DeleteWord(int wordId)
        {
            using(var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                bool doesWordHasDependency = await DoesWordHasDependency(wordId);
                if (!doesWordHasDependency)
                {
                    uow.WordRepository.Delete(wordId);
                    return await uow.SaveAsync() > 0;
                }
                else
                {
                    return false;
                }
            }
        }

        private async Task<bool> DoesWordHasDependency(int wordId)
        {
            using(var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var result = await uow.WordTranslationRepository.GetAll().Where(wt => wt.OriginalWordId == wordId || wt.TranslationWordId == wordId)
                    .Include(wt => wt.WordProgresses).Select(wt => wt.WordProgresses).FirstAsync();

                if(result.Count != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
        }
    }
}