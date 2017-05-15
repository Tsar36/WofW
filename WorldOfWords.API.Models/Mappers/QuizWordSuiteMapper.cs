using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public class QuizWordSuiteMapper: IQuizWordSuiteMapper
    {
        IQuizWordTranslationMapper _mapper;

        public QuizWordSuiteMapper(IQuizWordTranslationMapper mapper)
        {
            this._mapper = mapper;
        }
        public TrainingWordSuiteModel Map(WordSuite WordSuite)
        {
            TrainingWordSuiteModel wordSuiteModel = new TrainingWordSuiteModel();
            wordSuiteModel.Id = WordSuite.Id;
            wordSuiteModel.OwnerId = WordSuite.OwnerId;
            wordSuiteModel.QuizResponseTime = WordSuite.QuizResponseTime;
            wordSuiteModel.QuizStartTime = WordSuite.QuizStartTime;
            wordSuiteModel.Name = WordSuite.Name;
            wordSuiteModel.WordTranslations = new List<WordTranslationModel>();
            wordSuiteModel.WordTranslations.AddRange(WordSuite.WordProgresses.Select(x => _mapper.Map(x.WordTranslation)).ToList());
            return wordSuiteModel;
        }

        public List<TrainingWordSuiteModel> Map(List<WordSuite> WordSuites)
        {
            return WordSuites.Select(x => Map(x)).ToList(); ;
        }
    }
}
