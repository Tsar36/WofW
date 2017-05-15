using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface ITrainingWordTranslationMapper
    {
        WordTranslationModel Map(WordTranslation WordTranslation);
        List<WordTranslationModel> Map(List<WordTranslation> WordTranslations);
    }
}
