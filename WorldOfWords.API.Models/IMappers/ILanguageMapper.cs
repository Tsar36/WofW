using System.Globalization;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models.IMappers
{
    public interface ILanguageMapper
    {
        Language ToDomainModel(LanguageModel apiModel);
        LanguageModel ToApiModel(Language domainModel);
        LanguageModel CuntureInfoToLanguageModel(CultureInfo info);
    }
}
