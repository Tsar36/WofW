using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using System.Threading.Tasks;
using WorldOfWords.API.Models.IMappers;
using System;
using Microsoft.AspNet.Identity;
using WorldOfWords.API.Models;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Services.IServices;
using WorldOfWords.Validation;
using System.Text;

// please delete all sync methods if nessesary and leave only async methods...
namespace WorldOfWords.Domain.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ITagMapper tagMapper;

        public TagService(IUnitOfWorkFactory factory, ITagMapper tagMapper)
        {
            this._unitOfWorkFactory = factory;
            this.tagMapper = tagMapper;
        }
        
        public int Exists(string name)
        {
            using (var _uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var tag = _uow.TagRepository.GetAll().FirstOrDefault(t => t.Name == name);
                return tag != null ? tag.Id : 0;
            }
        }
        
        public async Task<List<TagModel>> GetTopBySearchTagAsync(string searchTag, int count)
        {
            List<TagModel> tagModels;
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var tags = await uow.TagRepository.GetAll()
                    .Where(t => t.Name.Contains(searchTag))
                    .OrderBy(t => t.Name.IndexOf(searchTag))
                    .ThenBy(t => t.Name)
                    .Take(count)
                    .ToListAsync();
                tagModels = tags.Select(t => tagMapper.Map(t)).ToList();
            } 
            return tagModels;
        }

        public async Task<Tag> GetTagByIdAsync(int id)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return await uow.TagRepository.GetByIdAsync(id);
            }
        }

        public async Task<Tag> GetFirstTagByNameAsync(string tagName)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                return await uow.TagRepository.GetAll()
                            .Where(t => t.Name == tagName)
                            .FirstOrDefaultAsync();
            }
        }

        public async Task<int> AddAsync(TagModel tagModel)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                int existResult = Exists(tagModel.Value);
                if (existResult > 0)
                {
                    return -1;
                }
                var tag = tagMapper.Map(tagModel);
                uow.TagRepository.Add(tag);
                await uow.SaveAsync();
                return tag.Id;
            }
        }

        public async Task AddToWordAsync(int wordId, int tagId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var word = await uow.WordRepository.GetByIdAsync(wordId);
                var tag = await uow.TagRepository.GetByIdAsync(tagId);
                word.Tags.Add(tag);
                await uow.SaveAsync();
            }
        }

        public async Task AddRangeToWordAsync(int wordId, List<int> tags)
        {
            foreach (var tag in tags)
            {
                await AddToWordAsync(wordId, tag);                
            }
        }

        public async Task<bool> RemoveTagFromWordAsync(int wordId, int tagId)
        {
            using (var uow = _unitOfWorkFactory.GetUnitOfWork())
            {
                var word = await uow.WordRepository.GetByIdAsync(wordId);
                var tag = await uow.TagRepository.GetByIdAsync(tagId);
                word.Tags.Remove(tag);
                return await uow.SaveAsync() > 0;
            }

        }
        
        public async Task<bool> RemoveIdRangeAsync(int wordId, List<int> tagIds)
        {
            bool result = true;
            foreach (var tag in tagIds)
            {
                result = result && await RemoveTagFromWordAsync(wordId, tag);
            }
            return result;
        }


    }
}
