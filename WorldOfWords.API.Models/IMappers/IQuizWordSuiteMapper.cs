using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface IQuizWordSuiteMapper
    {
        TrainingWordSuiteModel Map(WordSuite WordSuite);
        List<TrainingWordSuiteModel> Map(List<WordSuite> WordSuites);
    }
}
