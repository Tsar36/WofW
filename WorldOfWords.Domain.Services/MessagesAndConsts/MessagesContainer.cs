namespace WorldOfWords.Domain.Services.MessagesAndConsts
{
    public class MessagesContainer
    {
        public static string TRANSLATION_LANGUAGE = "Ukrainian"; 

        public static string ConfiramtionMessage = 
            "<p>You received this message because you have recently registered at World of Words.</p>"
            + "<p>Please confirm your registration by clicking the link below.</p>";

        public static string ForgotPasswordMessage =
             "<p>You received this message because you have recently said that you need to reset your password at World of Words.</p>" 
             + "<p>Please click the link below to reset your password.</p>";

        public static string ActionContextNull = "ActionContext: null exception.";
        public static string TeacherRole = "Teacher";
        public static string AdminRole = "Admin";
        public static string StudentRole = "Student";
        public static string NullString = "null";
        public static string NullRequestHeader = "HttpMessageHandler: null request header.";
        public static string UserNotFound = "User Not Found";
        public static string OneQuizPerDay = "Sorry, you can take only one quiz per day!";
        public static string NotYourQuiz = "Sorry, but this quiz doesn`t belond to you!";
        public static string TimeCheating = "It is not nice - don't cheat anymore!";
        public static string IncorrectData = "Sorry, incorrect incoming data!";
        public static string DeleteGroupFirst = "Someone is enroll to this course. Delete group first!";
        public static string StringCharacters = "1234567890abcdefhijklmnopqrstyuxzwvAQWERTYUIOPSDFGHJKLZXCVBNM";
        public static string X2 = "x2";
        public static string BadDataInRequest = "Bad data in request";
    }
}