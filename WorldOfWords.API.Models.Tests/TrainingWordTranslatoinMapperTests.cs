using NUnit.Framework;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models.Tests
{
    [TestFixture]
    class TrainingWordTranslatoinMapperTests
    {
        [Test]
        public void Map_WordTranslationAndTrainingWordTranslationModelAreEqual()
        {
            var initial = new WordTranslation()
            {
                OriginalWord = new Word
                {
                    Value = "Sunday"
                },
                TranslationWord = new Word
                {
                    Value = "неділя"
                }
            };
            var expected = new WordTranslationModel()
            {
                OriginalWord = "Sunday",
                TranslationWord = "неділя"
            };
            var actual = (new TrainingWordTranslationMapper()).Map(initial);
            Assert.AreEqual(expected.OriginalWord,actual.OriginalWord);
            Assert.AreEqual(expected.TranslationWord, actual.TranslationWord);
        }
    }
}
