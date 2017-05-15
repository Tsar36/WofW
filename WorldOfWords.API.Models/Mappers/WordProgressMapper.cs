using System;
using System.Collections.Generic;
using System.Linq;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public class WordProgressMapper: IWordProgressMapper
    {
        public WordProgress Map(WordProgressModel wordProgress)
        {
            if (wordProgress == null)
            {
                throw new ArgumentNullException("wordProgress");
            }
            return new WordProgress()
            {
                WordSuiteId = wordProgress.WordSuiteId,
                WordTranslationId = wordProgress.WordTranslationId,
                Progress = 0,
                IsStudentWord = wordProgress.IsStudentWord
            };
        }

        public List<WordProgress> MapRange(List<WordProgressModel> wordProgresses)
        {
            if (wordProgresses == null)
            {
                throw new ArgumentNullException("wordProgresses");
            }
            return wordProgresses.Select(wp => Map(wp)).ToList();
        }

        public WordProgress Map(int wordSuiteId, int wordTranslationId)
        {
            return new WordProgress()
            {
                WordSuiteId = wordSuiteId,
                WordTranslationId = wordTranslationId,
                Progress = 0
            };
        }

        public List<WordProgress> MapRange(int wordSuiteId, List<int> wordTranslationsId)
        {
            return wordTranslationsId.Select(wordTranslationId => Map(wordSuiteId, wordTranslationId)).ToList();
        }
    }
}
