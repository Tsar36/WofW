using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface IQuizWordTranslationMapper
    {
        WordTranslationModel Map(WordTranslation WordTranslation);
        List<WordTranslationModel> Map(List<WordTranslation> WordTranslations);
    }
}
