using System;
using System.Collections.Generic;
using System.Linq;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public class WordTranslationMapper: IWordTranslationMapper
    {
        public WordTranslationModel Map(WordTranslation wordTranslation)
        {
            if (wordTranslation == null)
            {
                throw new ArgumentNullException("wordTranslation");
            }
            return new WordTranslationModel()
            {
                Id = wordTranslation.Id,
                OriginalWord = wordTranslation.OriginalWord.Value,
                TranslationWord = wordTranslation.TranslationWord.Value
            };
        }

        public List<WordTranslationModel> MapRange (List<WordTranslation> wordTranslations)
        {
            if (wordTranslations == null)
            {
                throw new ArgumentNullException("wordTranslations");
            }
            return wordTranslations.Select(w => Map(w)).ToList();
        }

        public WordTranslation Map(WordTranslationImportModel wordTranslation)
        {
            return new WordTranslation()
            {
                OriginalWordId = wordTranslation.OriginalWordId,
                TranslationWordId = wordTranslation.TranslationWordId,
                OwnerId = wordTranslation.OwnerId
            };
        }

        public WordTranslation Map (WordTranslationImportModel wordTranslation, List<Tag> tags)
        {
            return new WordTranslation()
            {
                OriginalWordId = wordTranslation.OriginalWordId,
                TranslationWordId = wordTranslation.TranslationWordId,
                OwnerId = wordTranslation.OwnerId,
            };
        }

        public WordTranslationImportModel MapToImportModel(WordTranslation wordTranslation)
        {
            return new WordTranslationImportModel()
            {
                OriginalWord = wordTranslation.OriginalWord.Value,
                OriginalWordId = wordTranslation.OriginalWordId,
                Transcription = wordTranslation.OriginalWord.Transcription,
                Description = wordTranslation.OriginalWord.Description,
                TranslationWord = wordTranslation.TranslationWord.Value,
                TranslationWordId = wordTranslation.TranslationWordId,
                LanguageId = wordTranslation.OriginalWord.LanguageId,
                PartOfSpeechId = wordTranslation.OriginalWord.PartOfSpeechId,
                Comment = wordTranslation.OriginalWord.Comment
            };
        }
    }
}
