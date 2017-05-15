using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WorldOfWords.API.Models.IMappers;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services.IServices;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using System.Data.Entity;

namespace WorldOfWords.Domain.Services.Services
{
    /// <summary>
    /// Responsible for obtaining and manipulating the list of languages.
    /// </summary>
    public class LanguageService : ILanguageService
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly ILanguageMapper languageMapper;
        private readonly IPartOfSpeechMapper partOfSpeechMapper;

        public LanguageService(IUnitOfWorkFactory factory, ILanguageMapper mapper, IPartOfSpeechMapper parts)
        {
            unitOfWorkFactory = factory;
            languageMapper = mapper;
            partOfSpeechMapper = parts;
        }

        /// <summary>
        /// Adds a new language to the database.
        /// </summary>
        /// <param name="language">The language to be added to the database.</param>
        /// <returns>The id of a new database record, or -1, if such language already exists.</returns>
        public async Task<int> AddAsync(LanguageModel model)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                Language language = languageMapper.ToDomainModel(model);

                if (uow.LanguageRepository.GetAll().Any(l => l.Name == language.Name))
                {
                    return -1;
                }

                uow.LanguageRepository.Add(language);

                await uow.SaveAsync();

                return language.Id;
            }
        }

        /// <summary>
        /// Removes language from the database.
        /// </summary>
        /// <param name="id"> ID of the language to be removed from the database.</param>
        /// <returns>True, if language successfully deleted, false, if no language with such ID</returns>
        public async Task<bool> RemoveAsync(int id)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                var language = uow.LanguageRepository.GetById(id);

                if (language != null)
                {
                    if (language.Courses.Count == 0)
                    {
                        if (language.WordSuites.Count != 0)
                        {
                            var wordSuites = uow.WordSuiteRepository.GetAll().Where(ws => ws.LanguageId == id);
                            var wordProgresses = uow.WordProgressRepository.GetAll().Where(wp => wordSuites.Select(ws => ws.Id).Contains(wp.WordSuiteId));
                            uow.WordProgressRepository.Delete(wordProgresses);
                            uow.WordSuiteRepository.Delete(wordSuites);
                        }
                        if (language.Words.Count != 0)
                        {
                            var wordTranslations = uow.WordTranslationRepository.GetAll().Where(wt => wt.OriginalWord.LanguageId == id);
                            uow.WordTranslationRepository.Delete(wordTranslations);

                            var words = uow.WordRepository.GetAll().Where(w => w.LanguageId == id);
                            uow.WordRepository.Delete(words);
                        }

                        uow.LanguageRepository.Delete(language);
                        await uow.SaveAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns a list of all languages in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<LanguageModel>> GetLanguagesAsync()
        {
            List<Language> lang;

            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                lang = await uow.LanguageRepository.GetAll().ToListAsync();
            }

            return lang.Select(l => languageMapper.ToApiModel(l));
        }

        /// <summary>
        /// Returns a list of all parts of speech by language id.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PartOfSpeechModel>> GetAllPartsOfSpeechByLanguageIdAsync(int languageId)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                List<PartsOfSpeech> Parts = await (from parts in uow.PartsOfSpeechRepository.GetAll() where parts.LanguageId == languageId select parts).ToListAsync();
                return Parts.Select(p => partOfSpeechMapper.ToApiModel(p));
            }
        }

        public IEnumerable<LanguageModel> GetWorldLanguages()
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures).Select(c => languageMapper.CuntureInfoToLanguageModel(c));
        }

        public async Task<LanguageModel> GetlanguageByIDAsync(int id)
        {
            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                return languageMapper.ToApiModel(await uow.LanguageRepository.GetByIdAsync(id));
            }
        }
    }
}