using System;
using System.Collections.Generic;
using System.Web.Http;
using WorldOfWords.API.Models;
using WorldOfWords.Domain.Services;
using System.Linq;
using System.Web.Http.Results;
using WorldOfWords.Domain.Services.IServices;
using WorldOfWords.Domain.Models;
using System.Threading.Tasks;


namespace WorldofWords.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/TagController")]
    public class TagController : BaseController
    {
        private readonly ITagMapper _tagMapper;
        private readonly ITagService _tagService;
        private readonly IWordMapper _wordMapper;
        private readonly IWordService _wordService;
        public TagController(ITagService tagService, ITagMapper tagMapper, IWordMapper wordMapper, IWordService wordService)
        {
            _tagService = tagService;
            _tagMapper = tagMapper;
            _wordMapper = wordMapper;
            _wordService = wordService;
        }
        

    }
}
