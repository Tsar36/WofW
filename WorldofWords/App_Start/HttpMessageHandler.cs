using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.WebPages;
using WorldOfWords.Domain.Services.IServices;
using WorldOfWords.Domain.Services.MessagesAndConsts;

namespace WorldofWords
{
    public class HttpMessageHandler : DelegatingHandler
    {
        private string _id;
        private string[] _roles;
        private string _hashFromRequest;
        private string _hashedToken;
        private readonly IRequestIdentityService _service;

        public HttpMessageHandler(IRequestIdentityService service)
        {
            _service = service;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (CheckHeader(request))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            CheckAndGenerateIdentity(request);
            return await base.SendAsync(request, cancellationToken);
        }

        private void CheckAndGenerateIdentity(HttpRequestMessage request)
        {
            ParseHeader(request);
            CheckDataFromRequest();
            if (_service.CheckIdentity(_hashFromRequest, _hashedToken, _roles, out _id))
            {
                GenerateIdentity(_id, _roles);
                return;
            }
            request.CreateErrorResponse(HttpStatusCode.NotFound, MessagesContainer.UserNotFound);
        }

        private static void GenerateIdentity(string id, string[] roles)
        {
            GenericIdentity identity = new GenericIdentity(id);
            Thread.CurrentPrincipal =
                new GenericPrincipal(
                    identity,
                    roles
                    );
        }

        private void ParseHeader(HttpRequestMessage request)
        {
            string[] items = request.Headers.Authorization.Parameter.Split(' ');
            _hashedToken = items[ConstContainer.ZeroElem];
            _hashFromRequest = items[ConstContainer.FirstElem];
            _roles = new string[items.Count() - ConstContainer.SecondElem];

            for (int i = ConstContainer.SecondElem; i < items.Count(); i++)
            {
                if (items[i] != null)
                {
                    _roles[i - ConstContainer.SecondElem] = items[i];
                }
            }
        }

        private static bool CheckHeader(HttpRequestMessage request)
        {
            return (NullToString(request.Headers.Authorization) == MessagesContainer.NullString)
                   || (NullToString(request.Headers.Authorization).IsEmpty());
        }

        private static string NullToString(object value)
        {
            return value == null ? "" : value.ToString();
        }

        private void CheckDataFromRequest()
        {
            if (_hashFromRequest.IsEmpty() || _hashedToken.IsEmpty())
            {
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    ReasonPhrase = MessagesContainer.BadDataInRequest,
                    StatusCode = HttpStatusCode.BadRequest
                });
            }
        }
    }
}