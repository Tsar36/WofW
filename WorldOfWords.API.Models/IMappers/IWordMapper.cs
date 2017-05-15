using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public interface IWordMapper
    {
        Word ToDomainModel(WordModel apiModel);
        WordModel ToApiModel(Word domainModel);
        //Word EditToDomainModel(WordTranslationEditModel apiModel);
        //WordTranslationEditModel ToApiEditModel(Word domainModel);
        WordValueModel ToValueModel(Word domainModel);
        Word ToDomainModel(WordValueModel apiModel);
    }
}
