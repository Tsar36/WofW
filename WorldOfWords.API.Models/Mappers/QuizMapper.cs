using System;
using System.Collections.Generic;
using System.Linq;
using WorldOfWords.Domain.Models;
using WorldOfWords.API.Models.Mappers;
using WorldOfWords.API.Models.IMappers;

namespace WorldOfWords.API.Models
{
    public class QuizMapper : IQuizMapper
    {
        public QuizModel Map(Quiz quiz)
        {
            QuizModel quizModel = new QuizModel();
            quizModel.Id = quiz.Id;
            quizModel.Name = quiz.Name;

            return quizModel;
        }
    }
}
