using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using WorldOfWords.API.Models;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;
using WorldOfWords.Infrastructure.Data.EF;
using System.Web.Http;
using WorldOfWords.Domain.Services.MessagesAndConsts;

namespace WorldOfWords.Domain.Services
{
    public class WordTranslationService : IWordTranslationService
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IWordTranslationMapper wordTranslationMapper;
        private readonly IWordMapper wordMapper;
        private readonly ITagMapper tagMapper;

        public WordTranslationService(IUnitOfWorkFactory unitOfWorkFactory, IWordTranslationMapper wordTranslationMapper,
                                        IWordMapper wordMapper, ITagMapper tagMapper)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.wordTranslationMapper = wordTranslationMapper;
            this.wordMapper = wordMapper;
            this.tagMapper = tagMapper;
        }



        public List<WordTranslation> GetTopBySearchWord(string searchWord, int languageId)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                return uow.WordTranslationRepository.GetAll()
                     .Where(w => w.OriginalWord.LanguageId == languageId && w.TranslationWord.LanguageId != languageId &&
                           (w.OriginalWord.Value.Contains(searchWord) || w.TranslationWord.Value.Contains(searchWord)))
                     .OrderBy(w => w.OriginalWord.Value.IndexOf(searchWord))
                     .ThenBy(w => w.OriginalWord.Value)
                     .Take(10)
                     .Include(w => w.OriginalWord)
                     .Include(w => w.TranslationWord)
                     .ToList();
            }
        }

        public List<WordTranslation> GetByWordSuiteID(int id)
        {
            List<WordTranslation> wordTranslations;
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                var wordTranslationIds = uow.WordProgressRepository.GetAll()
                        .Where(wp => wp.WordSuiteId == id)
                    .Select(wp => wp.WordTranslationId);
                wordTranslations = uow.WordTranslationRepository.GetAll()
                    .Where(wt => wordTranslationIds.Contains(wt.Id))
                    .Include(wt => wt.OriginalWord)
                    .Include(wt => wt.TranslationWord)
                    .ToList();

            }
            return wordTranslations;
        }

        public int Exists(int originalWordId, int translationWordId)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                var wordTranslation = uow.WordTranslationRepository.GetAll()
                    .FirstOrDefault(wt => wt.OriginalWordId == originalWordId &&
                                          wt.TranslationWordId == translationWordId);

                if (wordTranslation != null)
                {
                    return wordTranslation.Id;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int Add(WordTranslation wordTranslation)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {

                uow.WordTranslationRepository.AddOrUpdate(wordTranslation);
                uow.Save();
                return wordTranslation.Id;
            }
        }

        public async Task<WordTranslation> GetByIdAsync(int id)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                return await uow.WordTranslationRepository.GetByIdAsync(id);
            }
        }

        public async Task<WordTranslation> GetByIdWithWordsAsync(int id)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                return await uow.WordTranslationRepository.GetAll()
                    .Where(wt => wt.Id == id)
                    .Include(wt => wt.OriginalWord)
                    .Include(wt => wt.TranslationWord)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<int> AddAsync(WordTranslationImportModel wordTranslation)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                WordTranslation wordToAdd = wordTranslationMapper.Map(wordTranslation);
                uow.WordTranslationRepository.AddOrUpdate(wordToAdd);
                await uow.SaveAsync();
                return wordToAdd.Id;
            }
        }

        public async Task<List<WordTranslationImportModel>> GetWordsFromIntervalAsync(int startOfInterval, int endOfInterval, int originalLangId, int translationLangId, int[] selectedPartsOfSpeechId)
        {
            List<WordTranslationImportModel> wordTranslationModels;
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                if (startOfInterval >= uow.WordTranslationRepository.GetAll().Count() || startOfInterval < 0
                    || startOfInterval > endOfInterval || endOfInterval > uow.WordTranslationRepository.GetAll().Count())
                    throw new ArgumentException("Start of interval is bigger than end");

                List<WordTranslation> wordTranslations = await uow.WordTranslationRepository.GetAll()
                    .Where(item => item.OriginalWord.LanguageId == originalLangId
                        && item.TranslationWord.LanguageId == translationLangId
                        && ((selectedPartsOfSpeechId.Contains(ConstContainer.WithoutPartOfSpeechId) && item.OriginalWord.PartOfSpeechId == null)
                              || (item.OriginalWord.PartOfSpeechId != null && selectedPartsOfSpeechId.Contains((int)item.OriginalWord.PartOfSpeechId))))
                    .GroupBy(item => item.OriginalWord)
                    .Select(x => x.FirstOrDefault())
                    .OrderBy(item => item.OriginalWord.Value)
                    .Skip(startOfInterval)
                    .Take(endOfInterval - startOfInterval)
                    .Include(w => w.OriginalWord)
                    .Include(w => w.TranslationWord)
                    .ToListAsync();
                wordTranslationModels = wordTranslations
                    .Select(item => wordTranslationMapper.MapToImportModel(item))
                    .ToList();
            }
            return wordTranslationModels;
        }

        public async Task<List<WordTranslationImportModel>> GetWordsWithSearchValueAsync(string searchValue, int startOfInterval, int endOfInterval, int originalLangId, int translationLangId, int[] selectedPartsOfSpeechId)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                if (startOfInterval >= uow.WordTranslationRepository.GetAll().Count() || startOfInterval < 0
                    || startOfInterval > endOfInterval || endOfInterval > uow.WordTranslationRepository.GetAll().Count())
                    throw new ArgumentException("Start of interval is bigger than end");

                List<WordTranslation> wordTranslations = await uow.WordTranslationRepository.GetAll()
                    .Where(w => w.OriginalWord.LanguageId == originalLangId && w.TranslationWord.LanguageId == translationLangId
                        && (w.OriginalWord.Value.Contains(searchValue) || w.TranslationWord.Value.Contains(searchValue))
                        && ((selectedPartsOfSpeechId.Contains(ConstContainer.WithoutPartOfSpeechId) && w.OriginalWord.PartOfSpeechId == null)
                              || (w.OriginalWord.PartOfSpeechId != null && selectedPartsOfSpeechId.Contains((int)w.OriginalWord.PartOfSpeechId))))
                    .GroupBy(item => item.OriginalWord)
                    .Select(x => x.FirstOrDefault())
                    .OrderBy(w => w.OriginalWord.Value)
                    .Skip(startOfInterval)
                    .Take(endOfInterval - startOfInterval)
                    .Include(w => w.OriginalWord)
                    .Include(w => w.TranslationWord)
                    .ToListAsync();
                return wordTranslations
                        .Select(item => wordTranslationMapper.MapToImportModel(item))
                        .ToList();
            }
        }

        public async Task<int> GetAmountOfWordTranslationsByLanguageAsync(int originalLangId, int translationLangId, int[] selectedPartsOfSpeechId)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                return await uow.WordTranslationRepository.GetAll()
                    .Where(item => item.OriginalWord.LanguageId == originalLangId
                         && item.TranslationWord.LanguageId == translationLangId
                         && ((selectedPartsOfSpeechId.Contains(ConstContainer.WithoutPartOfSpeechId) && item.OriginalWord.PartOfSpeechId == null)
                              || (item.OriginalWord.PartOfSpeechId != null
                              && selectedPartsOfSpeechId.Contains((int)item.OriginalWord.PartOfSpeechId))))
                    .GroupBy(item => item.OriginalWord)
                    .CountAsync();
            }
        }

        public async Task<int> GetAmountOfWordsBySearchValuesAsync(string searchValue, int originalLangId, int translationLangId, int[] selectedPartsOfSpeechId)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                return await uow.WordTranslationRepository.GetAll()
                    .Where(w => w.OriginalWord.LanguageId == originalLangId && w.TranslationWord.LanguageId == translationLangId
                        && (w.OriginalWord.Value.Contains(searchValue) || w.TranslationWord.Value.Contains(searchValue))
                        && ((selectedPartsOfSpeechId.Contains(ConstContainer.WithoutPartOfSpeechId) && w.OriginalWord.PartOfSpeechId == null)
                              || (w.OriginalWord.PartOfSpeechId != null && selectedPartsOfSpeechId.Contains((int)w.OriginalWord.PartOfSpeechId))))
                    .Distinct()
                    .CountAsync();
            }
        }

        public async Task<WordTranslation> GetWordTranslationByIdAsync(int id)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                return await uow.WordTranslationRepository.GetByIdAsync(id);
            }
        }

        public async Task<WordTranslationFullModel> GetWordFullInformationAsync(string word, int originalLangId, int translationLangId)
        {
            WordTranslationFullModel infoWord = new WordTranslationFullModel
            {
                OriginalWord = word
            };

            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                Word origWord = await uow.WordRepository.GetAll()
                            .Where(w => w.Value == word && w.LanguageId == originalLangId)
                            .FirstOrDefaultAsync();
                infoWord.Id = origWord.Id;
                infoWord.Description = origWord.Description;
                infoWord.Transcription = origWord.Transcription;
                infoWord.PartOfSpeechId = origWord.PartOfSpeechId;
                infoWord.Comment = origWord.Comment;

                if (await GetAmountOfTranslations(origWord.Id, translationLangId) > 0)
                {
                    List<Word> translations = await uow.WordTranslationRepository.GetAll()
                        .Where(x => x.OriginalWord.Value == word && x.OriginalWord.LanguageId == originalLangId
                            && x.TranslationWord.LanguageId == translationLangId)
                        .Select(x => x.TranslationWord)
                        .ToListAsync();
                    infoWord.Translations = translations.Select(x => wordMapper.ToValueModel(x)).ToList();
                }
                if (await GetAmountOfTranslations(origWord.Id, originalLangId) > 0)
                {
                    List<Word> synonims = await uow.WordTranslationRepository.GetAll()
                        .Where(x => x.OriginalWord.Value == word && x.OriginalWord.LanguageId == originalLangId
                            && x.TranslationWord.LanguageId == originalLangId)
                        .Select(x => x.TranslationWord)
                        .ToListAsync();
                    infoWord.Synonims = synonims.Select(x => wordMapper.ToValueModel(x)).ToList();
                }
                if (origWord.Tags.Count > 0)
                {
                    List<Tag> tags = origWord.Tags.ToList();
                    infoWord.Tags = tags.Select(x => tagMapper.Map(x)).ToList();
                }
            }
            return infoWord;
        }

        public async Task<WordTranslationFullStringsModel> GetWordFullInformationStringsAsync(string word, int originalLangId, int translationLangId)
        {
            WordTranslationFullStringsModel infoWord = new WordTranslationFullStringsModel
            {
                OriginalWord = word
            };

            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                WordTranslation wordTr = await uow.WordTranslationRepository.GetAll()
                        .Where(x => x.OriginalWord.Value == word && x.OriginalWord.LanguageId == originalLangId
                            && x.TranslationWord.LanguageId == translationLangId)
                        .FirstOrDefaultAsync();

                infoWord.Description = wordTr.OriginalWord.Description;
                infoWord.Transcription = wordTr.OriginalWord.Transcription;
                infoWord.PartOfSpeech = wordTr.OriginalWord.PartOfSpeech.Name;
                infoWord.Comment = wordTr.OriginalWord.Comment;

                List<string> translations = uow.WordTranslationRepository.GetAll()
                    .Where(x => x.OriginalWord.Value == word && x.OriginalWord.LanguageId == originalLangId
                        && x.TranslationWord.LanguageId == translationLangId)
                    .Select(x => x.TranslationWord.Value)
                    .ToList();
                infoWord.Translations = translations.Count > 0 ? translations : null;

                List<string> synonyms = uow.WordTranslationRepository.GetAll()
                    .Where(x => x.OriginalWord.Value == word && x.OriginalWord.LanguageId == originalLangId
                        && x.TranslationWord.LanguageId == originalLangId)
                    .Select(x => x.TranslationWord.Value)
                    .ToList();
                infoWord.Synonims = synonyms.Count > 0 ? synonyms : null;

                List<string> tags = wordTr.OriginalWord.Tags.Select(x => x.Name).ToList();
                infoWord.Tags = tags.Count > 0 ? tags : null;
            }
            return infoWord;
        }

        private IEnumerable<Word> GetSynonyms(IQueryable<WordTranslation> translation, int wordId)
        {
            var baseQuery = translation.Where(t => t.OriginalWord.LanguageId == t.TranslationWord.LanguageId);
            var filter = new HashSet<int> { wordId };
            var query = (from t in baseQuery where filter.Contains(t.OriginalWordId) && !filter.Contains(t.TranslationWordId) select t.TranslationWord)
                .Union(from t in baseQuery where filter.Contains(t.TranslationWordId) && !filter.Contains(t.OriginalWordId) select t.OriginalWord);
            var processed = new HashSet<int> { wordId };
            var next = new List<int>();
            while (true)
            {
                foreach (var item in query)
                {
                    if (processed.Add(item.Id))
                    {
                        yield return item;
                        next.Add(item.Id);
                    }
                }
                if (next.Count == 0) break;
                filter.Clear();
                foreach (var item in next)
                    filter.Add(item);
                next.Clear();
            }
        }

        public async Task<List<WordValueModel>> GetAllWordSynonymsById(int id)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                IEnumerable<Word> synonyms = null;

                await Task.Run(() =>
                {
                    synonyms = GetSynonyms(uow.WordTranslationRepository.GetAll(), id);
                });

                return synonyms.Select(w => wordMapper.ToValueModel(w)).ToList();
            }
        }

        private async Task<bool> DoesWordHasDependency(int orId, int trId)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                var result = await uow.WordTranslationRepository.GetAll().Where(wt => wt.OriginalWordId == orId && wt.TranslationWordId == trId)
                    .Include(wt => wt.WordProgresses).Select(wt => wt.WordProgresses).FirstAsync();

                if (result.Count != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public async Task<bool> DeleteAsync(int orId, int trId)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                bool doesWordHasDependency = await DoesWordHasDependency(orId, trId);
                if (!doesWordHasDependency)
                {
                    uow.WordTranslationRepository.Delete(orId, trId);
                }
                return await uow.SaveAsync() > 0;
            }
        }

        public async Task AddIdRangeAsync(int originalWordId, List<int> wordTranslationIds, int ownerId)
        {
            foreach (var wordId in wordTranslationIds)
            {
                if (Exists(originalWordId, wordId) == 0)
                {
                    await AddAsync(new WordTranslationImportModel
                    {
                        OriginalWordId = originalWordId,
                        TranslationWordId = wordId,
                        OwnerId = ownerId
                    });
                }
            }
        }

        public async Task<bool> RemoveIdRangeAsync(int originalWordId, List<int> wordTranslationIds)
        {
            bool result = true;
            foreach (var wordId in wordTranslationIds)
            {
                result = result && await DeleteAsync(originalWordId, wordId);
            }
            return result;
        }

        public async Task<int> GetAmountOfTranslations(int wordId, int translLangId)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                return await uow.WordTranslationRepository.GetAll()
                    .Where(x => x.OriginalWord.Id == wordId && x.TranslationWord.LanguageId == translLangId)
                    .CountAsync();
            }


        }

        public async Task<int> GetAmountOfTagsBySearchValuesAsync(string searchValue, int originalLangId, int translationLangId)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                return await uow.WordTranslationRepository.GetAll()
                    .Where(w => w.OriginalWord.LanguageId == originalLangId && w.TranslationWord.LanguageId == translationLangId)
                    .Include(w => w.OriginalWord.Tags)
                    .Where(w => w.OriginalWord.Tags.Any(t => t.Name.Contains(searchValue)))
                    .Select(w => w.OriginalWord)
                    .Distinct()
                    .CountAsync();
            }
        }

        public async Task<List<WordTranslationImportModel>> GetWordsWithTagAsync(int startOfInterval, int endOfInterval, int originalLangId, string searchValue, int translationLangId)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                if (startOfInterval >= uow.WordTranslationRepository.GetAll().Count() || startOfInterval < 0
                    || startOfInterval > endOfInterval || endOfInterval > uow.WordTranslationRepository.GetAll().Count())
                    throw new ArgumentException("Start of interval is bigger than end");

                List<WordTranslation> wordTranslations = await uow.WordTranslationRepository.GetAll()
                    .Where(w => w.OriginalWord.LanguageId == originalLangId && w.TranslationWord.LanguageId == translationLangId)
                    .Include(w => w.OriginalWord.Tags)
                    .Where(w => w.OriginalWord.Tags.Any(t => t.Name.Contains(searchValue)))
                    .GroupBy(item => item.OriginalWord)
                    .Select(x => x.FirstOrDefault())
                    .OrderBy(w => w.OriginalWord.Value)
                    .Skip(startOfInterval)
                    .Take(endOfInterval - startOfInterval)
                    .Include(w => w.OriginalWord)
                    .Include(w => w.TranslationWord)
                    .ToListAsync();
                return wordTranslations
                        .Select(item => wordTranslationMapper.MapToImportModel(item))
                        .ToList();
            }
        }
    }
}
