using System.Collections.Generic;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;
using WorldOfWords.API.Models;

namespace WorldOfWords.Domain.Services
{
    public interface IWordService
    {
        Task<List<Word>> GetAllWordsAsync();
        Task<List<Word>> GetAllWordsBySpecificLanguageAsync(int languageId);
        Task<List<Word>> GetTopBySearchWordAsync(string searchWord, int languageId, int count);
        Task<Word> GetWordByIdAsync(int id);
        Task<int> AddAsync(WordModel word);
        Task<int> ExistsAsync(string value, int languageId);
        Task<bool> UpdateAsync(WordModel word);
        Task<Word> GetFirstByWordAsync(string searchWord);
        int Exists(string value, int languageId);
        int Add(WordModel word);
        Word GetWordById(int id);
        List<Word> GetAllByTags(List<string> tags, int languageId);
        Task<bool> DeleteWord(int wordId);
    }
}