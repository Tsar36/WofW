using System;
using System.Collections.Generic;
using System.Linq;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public class WordSuiteMapper : IWordSuiteMapper
    {
        public CourseWordSuiteModel Map(WordSuite wordSuite)
        {
            CourseWordSuiteModel model = new CourseWordSuiteModel();

            model.Id = wordSuite.Id;
            model.Name = wordSuite.Name;
            model.ProhibitedQuizzesId = wordSuite.ProhibitedQuizzes.Select(q => q.Id).ToList();
            return model;
        }


        public WordSuite Map(CourseWordSuiteModel wordSuite)
        {
            WordSuite model = new WordSuite();
            model.Id = wordSuite.Id;
            model.Name = wordSuite.Name;
            return model;

        }

        public List<CourseWordSuiteModel> Map(List<WordSuite> wordSuites)
        {
            return wordSuites.Select(x => Map(x)).ToList();
        }

        public WordSuite Map(WordSuiteModel wordSuite)
        {
            if (wordSuite == null)
            {
                throw new ArgumentNullException("wordSuite");
            }
            return new WordSuite()
            {
                Name = wordSuite.Name,
                LanguageId = wordSuite.LanguageId,
                OwnerId = wordSuite.OwnerId,
                PrototypeId = wordSuite.PrototypeId,
                Threshold = wordSuite.Threshold,
                QuizResponseTime = wordSuite.QuizResponseTime,
                TranslationLanguageId = wordSuite.TranslationLanguageId
            };
        }

        public WordSuite Map(WordSuiteEditModel wordSuite)
        {
            if (wordSuite == null)
            {
                throw new ArgumentNullException("wordSuite");
            }
            return new WordSuite()
            {
                Id = wordSuite.Id,
                Name = wordSuite.Name,
                LanguageId = wordSuite.LanguageId,
                Threshold = wordSuite.Threshold,
                QuizResponseTime = wordSuite.QuizResponseTime,
                TranslationLanguageId = wordSuite.TranslationLanguageId
            };
        }


        //mfomitc
        public WordSuite Map(WordSuiteShareModel wordSuite)
        {
            if (wordSuite == null)
            {
                throw new ArgumentNullException("wordSuite");
            }
            return new WordSuite()
            {
                Name = wordSuite.Name,
                LanguageId = wordSuite.LanguageId,
                Threshold = wordSuite.Threshold,
                QuizResponseTime = wordSuite.QuizResponseTime
            };
        }


        public WordSuiteEditModel MapForEdit(WordSuite wordSuite)
        {
            if (wordSuite == null)
            {
                throw new ArgumentNullException("wordSuite");
            }
            WordSuiteEditModel model = new WordSuiteEditModel();

            model.Id = wordSuite.Id;
            model.Name = wordSuite.Name;
            model.Language = wordSuite.Language.Name;
            model.LanguageId = wordSuite.LanguageId;
            model.TranslationLanguageId = wordSuite.TranslationLanguageId;
            if (wordSuite.TranslationLanguage != null)
                model.TranslationLanguageName = wordSuite.TranslationLanguage.Name;
            else
                model.TranslationLanguageName = "";
            model.PrototypeId = wordSuite.PrototypeId;
            model.Threshold = wordSuite.Threshold;
            model.QuizResponseTime = wordSuite.QuizResponseTime;
            model.ProhibitedQuizzesId = wordSuite.ProhibitedQuizzes.Select(q => q.Id).ToList();
            return model;
        }

        public List<WordSuiteEditModel> MapRangeForEdit(List<WordSuite> wordSuites)
        {
            if (wordSuites == null)
            {
                throw new ArgumentNullException("wordSuites");
            }
            return wordSuites.Select(ws => MapForEdit(ws)).ToList();
        }
    }
}
