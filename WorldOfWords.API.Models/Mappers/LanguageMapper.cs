using System.Globalization;
using WorldOfWords.API.Models.IMappers;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models
{
    public class LanguageMapper : ILanguageMapper
    {
        public Language ToDomainModel(LanguageModel apiModel)
        {
            Language result = new Language
            {
                Name = apiModel.Name,
                ShortName = apiModel.ShortName
            };

            if (apiModel.Id != null)
            {
                result.Id = (int)apiModel.Id;
            }

            return result;
        }

        public LanguageModel ToApiModel(Language domainModel)
        {
            return new LanguageModel
            {
                Id = domainModel.Id,
                Name = domainModel.Name,
                ShortName = domainModel.ShortName
            };
        }

        public LanguageModel CuntureInfoToLanguageModel(CultureInfo info)
        {
            return new LanguageModel {
                Name = info.EnglishName,
                ShortName = info.Name
            };
        }
    }
}
