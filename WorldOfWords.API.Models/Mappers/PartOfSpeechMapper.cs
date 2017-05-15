using System.Collections.Generic;
using WorldOfWords.Domain.Models;
using WorldOfWords.API.Models.Models;
using WorldOfWords.API.Models.IMappers;
using System;

namespace WorldOfWords.API.Models.Mappers
{
    public class PartOfSpeechMapper : IPartOfSpeechMapper
    {
        public PartsOfSpeech ToDomainModel(PartOfSpeechModel apiModel)
        {
            if (apiModel != null)
                return new PartsOfSpeech()
                {
                    Id = apiModel.Id,
                    ShortName = apiModel.ShortName,
                    Name = apiModel.Name,
                    LanguageId = apiModel.LanguageId
                };
            else
                throw new NullReferenceException("Cannot map empty object");
        }

        public PartOfSpeechModel ToApiModel(PartsOfSpeech domainModel)
        {
            if (domainModel != null)
                return new PartOfSpeechModel()
                {
                    Id = domainModel.Id,
                    ShortName = domainModel.ShortName,
                    Name = domainModel.Name,
                    LanguageId = domainModel.LanguageId
                };
            else
                throw new NullReferenceException("Cannot map empty object");
        }
    }
}
