using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface IWordSuiteMapper
    {
        WordSuite Map(WordSuiteModel WordSuite);
        WordSuite Map(WordSuiteEditModel wordSuite);
        //mfomitc
        WordSuite Map(WordSuiteShareModel WordSuite);
        CourseWordSuiteModel Map(WordSuite WordSuite);
        WordSuite Map(CourseWordSuiteModel WordSuite);
        WordSuiteEditModel MapForEdit(WordSuite WordSuite);
        List<CourseWordSuiteModel> Map(List<WordSuite> WordSuites);
        List<WordSuiteEditModel> MapRangeForEdit(List<WordSuite> WordSuites);
    }
}
