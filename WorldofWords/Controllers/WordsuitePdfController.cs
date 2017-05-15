using System.Threading.Tasks;
using System.Web.Http;
using WorldofWords.Utils;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services;

namespace WorldofWords.Controllers
{
    /// <summary>
    /// Responsible for generating a pdf version of a wordsuite.
    /// </summary>

    public class WordSuitePdfController : ApiController
    {
        private readonly IPdfGenerator<WordSuite> _generator;
        private readonly IWordSuiteService _service;

        public WordSuitePdfController(IWordSuiteService service, IPdfGenerator<WordSuite> generator)
        {
            _service = service;
            _generator = generator;
        }

        /// <summary>
        /// Gets a pdf version of a wordsuite with a given id.
        /// </summary>
        /// <param name="id">The id of the wordsuite.</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("api/wordsuite/pdftask")]
        public async Task<IHttpActionResult> GetTask(int id)
        {
            var wordsuite = await _service.GetByIDAsync(id);
            var pdfDocument = _generator.GetTaskInPdf(wordsuite);
            return new FileActionResult(pdfDocument);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/wordsuite/pdfwords")]
        public async Task<IHttpActionResult> GetWords(int id)
        {
            var wordsuite = await _service.GetByIDAsync(id);
            var pdfDocument = _generator.GetWordsInPdf(wordsuite);
            return new FileActionResult(pdfDocument);
        }
    }
}