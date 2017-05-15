using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface IWordTranslationMapper
    {
        WordTranslationImportModel MapToImportModel(WordTranslation wordTranslation);
        WordTranslationModel Map(WordTranslation wordTranslation);
        List<WordTranslationModel> MapRange(List<WordTranslation> wordTranslations);
        WordTranslation Map(WordTranslationImportModel wordTranslation);
        WordTranslation Map(WordTranslationImportModel wordTranslation, List<Tag> tags);
    }
}
