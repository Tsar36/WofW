namespace WorldOfWords.API.Models.Models
{
    public class TokenModel
    {
        public string EmailAndIdToken { get; set; }
        public string EmailConfirmationToken { get; set; }
        public string ForgotPasswordToken { get; set; }
        public string RolesToken { get; set; }
        public string HashToken { get; set; }
    }
}
