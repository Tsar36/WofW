namespace WorldOfWords.Domain.Models
{
    public class IncomingUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int LanguageId { get; set; }
        public string Token { get; set; }
        public string PagesUrl { get; set; }
    }
}
