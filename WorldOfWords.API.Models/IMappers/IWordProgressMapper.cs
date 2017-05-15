using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface IWordProgressMapper
    {
        WordProgress Map(WordProgressModel wordProgressApiModel);
        List<WordProgress> MapRange(List<WordProgressModel> wordProgressApiModelRange);
        WordProgress Map(int wordSuiteId, int wordTranslationId);
        List<WordProgress> MapRange(int wordSuiteId, List<int> wordTranslationsId);
    }
}
