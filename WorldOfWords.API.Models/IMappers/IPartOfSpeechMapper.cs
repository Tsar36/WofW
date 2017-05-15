using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models.IMappers
{
    public interface IPartOfSpeechMapper
    {
        PartsOfSpeech ToDomainModel(PartOfSpeechModel apiModel);
        PartOfSpeechModel ToApiModel(PartsOfSpeech domainModel);
    }
}
