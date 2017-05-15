using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;
using WorldOfWords.API.Models.IMappers;
using System.Linq;
using System;
using System.Text;


namespace WorldOfWords.API.Models.Mappers
{
    public class PictureMapper : IPictureMapper
    {
        public Picture ToDomainModel(PictureModelFromUser pictureModel)
        {
            return new Picture
            {
                Content = pictureModel.Content,
                WordId = pictureModel.WordId,
            };
        }

        public PictureModelToUser ToApiModel(Picture picture)
        {
            char[] contentChars = new char[picture.Content.Length];
            for (int i = 0; i < contentChars.Length; i++)
            {
                contentChars[i] = (char)picture.Content[i];
            }

            return new PictureModelToUser
            {
                Content = new string(contentChars)
            };
        }
    }
}
