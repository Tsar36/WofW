using System.Collections.Generic;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface IQuizMapper
    {
        QuizModel Map(Quiz quiz);
    }
}
