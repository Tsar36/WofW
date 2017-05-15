using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldOfWords.Domain.Services.IServices;
using WorldOfWords.API.Models.Models;
using WorldOfWords.API.Models.IMappers;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using System.Data.Entity;

namespace WorldOfWords.Domain.Services.Services
{
    public class PictureService : IPictureService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IPictureMapper _pictureMapper;

        public PictureService(IUnitOfWorkFactory unitOfWorkfactory, IPictureMapper pictureMapper)
        {
            _unitOfWorkFactory = unitOfWorkfactory;
            _pictureMapper = pictureMapper;
        }

        public async Task<PictureModelToUser> GetPictureByWordIdAsync(int wordId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                Picture pic = await uow.PictureRepository.GetAll().FirstOrDefaultAsync(p => p.WordId == wordId);
                return pic != null ? _pictureMapper.ToApiModel(pic) : null;
            }
        }

        public async Task<bool> WordHasPicture(int wordId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var pic = await uow.PictureRepository.GetAll().FirstOrDefaultAsync(p => p.WordId == wordId);
                return pic != null;
            }
        }

        public async Task<bool> TryToAddPictureAsync(PictureModelFromUser picture)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                if (await WordHasPicture(picture.WordId))
                {
                    return false;
                }
                var pictureToAdd = _pictureMapper.ToDomainModel(picture);
                uow.PictureRepository.Add(pictureToAdd);
                return await uow.SaveAsyncBool();
            }
        }

        public async Task<bool> UpdatePictureAsync(PictureModelFromUser picture)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var oldPicture = uow.PictureRepository.GetById(picture.WordId);
                if (oldPicture == null)
                {
                    return false;
                }
                oldPicture.Content = picture.Content;
                oldPicture.WordId = picture.WordId;

                uow.PictureRepository.Update(oldPicture);
                return await uow.SaveAsyncBool();
            }
        }

        public async Task<bool> TryToDeletePictureAsync(int wordId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                if (await WordHasPicture(wordId))
                {
                    var entity = await uow.PictureRepository.GetAll().FirstAsync(p => p.WordId == wordId);
                    uow.PictureRepository.Delete(entity);
                    return await uow.SaveAsyncBool();
                }
                else
                {
                    return false; 
                }
            }
        }

    }
}
