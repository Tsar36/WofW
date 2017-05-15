using System;
using System.Collections.Generic;
using WorldOfWords.Domain.Models;
using System.Linq;

namespace WorldOfWords.API.Models.Mappers
{
    public class QuizWordTranslationMapper : IQuizWordTranslationMapper
    {
        public WordTranslationModel Map(WordTranslation WordTranslation)
        {
            WordTranslationModel wordTranslationModel = new WordTranslationModel();
            wordTranslationModel.Id = WordTranslation.Id;
            wordTranslationModel.OriginalWordDesc = WordTranslation.OriginalWord.Description;
            wordTranslationModel.TranslationWord = WordTranslation.TranslationWord.Value;
            wordTranslationModel.OriginalWord = WordTranslation.OriginalWord.Value;
            wordTranslationModel.OriginalWordId = WordTranslation.OriginalWordId;
            return wordTranslationModel;
        }

        public List<WordTranslationModel> Map(List<WordTranslation> WordTranslations)
        {
            return WordTranslations.Select(x => Map(x)).ToList();
        }
    }
}
