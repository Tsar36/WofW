using System.Collections.Generic;
using System.Threading.Tasks;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.Domain.Services.IServices
{
    public interface ILanguageService
    {
        Task<int> AddAsync(LanguageModel model);
        Task<bool> RemoveAsync(int id);
        Task<IEnumerable<LanguageModel>> GetLanguagesAsync();
        IEnumerable<LanguageModel> GetWorldLanguages();
        Task<LanguageModel> GetlanguageByIDAsync(int id);
        Task<IEnumerable<PartOfSpeechModel>> GetAllPartsOfSpeechByLanguageIdAsync(int languageId);
    }
}