using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace WorldofWords.Utils
{
    public class FileActionResult : IHttpActionResult
    {
        private readonly byte[] _file;

        public FileActionResult(byte[] file)
        {
            this._file = file;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var result = new HttpResponseMessage {Content = new ByteArrayContent(_file)};
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            return Task.FromResult(result);
        }
    }
}