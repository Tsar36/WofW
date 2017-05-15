using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface ITrainingWordSuiteMapper
    {
        TrainingWordSuiteModel Map(WordSuite WordSuite);
        List<TrainingWordSuiteModel> Map(List<WordSuite> WordSuites);
    }
}
