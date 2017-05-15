namespace WorldOfWords.Domain.Services.IServices
{
    public interface IRequestIdentityService
    {
        bool CheckIdentity(string hashedToken, string id);
        bool CheckIdentity(string hashFromRequest, string hashedToken, string[] roles, out string id);
    }
}