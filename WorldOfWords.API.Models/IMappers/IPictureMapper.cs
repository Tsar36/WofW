using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models.IMappers
{
    public interface IPictureMapper
    {
        Picture ToDomainModel(PictureModelFromUser pictureModel);
        PictureModelToUser ToApiModel(Picture picture);
    }
}
