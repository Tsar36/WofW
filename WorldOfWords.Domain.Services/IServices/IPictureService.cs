using System.Collections.Generic;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;
using WorldOfWords.API.Models.Models;

namespace WorldOfWords.Domain.Services.IServices
{
    public interface IPictureService
    {
        Task<PictureModelToUser> GetPictureByWordIdAsync(int wordId);
        Task<bool> WordHasPicture(int wordId);
        Task<bool> TryToAddPictureAsync(PictureModelFromUser picture);
        Task<bool> UpdatePictureAsync(PictureModelFromUser picture);
        Task<bool> TryToDeletePictureAsync(int pictureId);
    }
}
