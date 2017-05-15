using System.Collections.Generic;
using System.Linq;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public class TrainingWordTranslationMapper: ITrainingWordTranslationMapper
    {
        public WordTranslationModel Map(WordTranslation WordTranslation)
        {
            WordTranslationModel wordTranslationModel = new WordTranslationModel();
            wordTranslationModel.Id = WordTranslation.Id;
            wordTranslationModel.OriginalWord = WordTranslation.OriginalWord.Value;
            wordTranslationModel.TranslationWord = WordTranslation.TranslationWord.Value;
            wordTranslationModel.OriginalWordId = WordTranslation.OriginalWord.Id;
            return wordTranslationModel;
        }

        public List<WordTranslationModel> Map(List<WordTranslation> WordTranslations)
        {
            return WordTranslations.Select(x => Map(x)).ToList();
        }
    }
}
