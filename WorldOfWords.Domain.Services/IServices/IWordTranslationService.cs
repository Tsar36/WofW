using System.Collections.Generic;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;
using WorldOfWords.API.Models;

namespace WorldOfWords.Domain.Services
{
    public interface IWordTranslationService
    {
        List<WordTranslation> GetTopBySearchWord(string searchWord, int languageId);
        //List<WordTranslation> GetAllByTags(List<string> tags, int languageId);
        List<WordTranslation> GetByWordSuiteID(int id);
        int Exists(int originalWordId, int translationWordId);
        int Add(WordTranslation wordTranslation);
        Task<WordTranslation> GetByIdAsync(int id);
        Task<WordTranslation> GetByIdWithWordsAsync(int id);
        Task<int> AddAsync(WordTranslationImportModel wordTranslation);
        Task AddIdRangeAsync(int originalWordId, List<int> wordTranslationIds, int ownerId);
        Task<bool> RemoveIdRangeAsync(int originalWordId, List<int> wordTranslationIds);
        Task<List<WordTranslationImportModel>> GetWordsFromIntervalAsync(int startOfInterval, int endOfInterval, int originalLangId, int translationLangId, int[] selectedPartsOfSpeech);
        Task<List<WordTranslationImportModel>> GetWordsWithSearchValueAsync(string searchValue, int startOfInterval, int endOfInterval, int originalLangId, int translationLangId, int[] selectedPartsOfSpeech);
        Task<int> GetAmountOfWordTranslationsByLanguageAsync(int originalLangId, int translationLangId, int[] selectedPartsOfSpeech);
        Task<int> GetAmountOfWordsBySearchValuesAsync(string searchValue, int originalLangId, int translationLangId, int[] selectedPartsOfSpeech);
        Task<WordTranslation> GetWordTranslationByIdAsync(int id);
        Task<WordTranslationFullModel> GetWordFullInformationAsync(string word, int originalLangId, int translationLangId);
        Task<WordTranslationFullStringsModel> GetWordFullInformationStringsAsync(string word, int originalLangId, int translationLangId);
        Task<List<WordValueModel>> GetAllWordSynonymsById(int id);
        Task<bool> DeleteAsync(int orId, int trId);
        Task<int> GetAmountOfTranslations(int word, int translLangId);
        Task<int> GetAmountOfTagsBySearchValuesAsync(string searchValue, int originalLangId, int translationLangId);
        Task<List<WordTranslationImportModel>> GetWordsWithTagAsync(int startOfInterval,int endOfInterval,int originalLangId,string searchValue,int translationLangId);
    }
}