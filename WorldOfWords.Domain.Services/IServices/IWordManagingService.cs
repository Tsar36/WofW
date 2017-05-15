using System.Threading.Tasks;
using WorldOfWords.API.Models;

namespace WorldOfWords.Domain.Services
{
    public interface IWordManagingService
    {
        Task<bool> EditWord(WordTranslationEditModel model);
        Task<int> AddWord(WordModel model);
    }
}
