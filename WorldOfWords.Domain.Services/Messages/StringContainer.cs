namespace WorldOfWords.Domain.Services.Messages
{
    public static class StringContainer
    {
        public static string TRANSLATION_LANGUAGE = "Ukrainian"; 

        public static string ConfiramtionMessage = 
            "<p>You received this message because you have recently registered at World of Words.</p>"
            + "<p>Please confirm your registration by clicking the link below.</p>";

        public static string LoginContollerName = "Login";

        public static string RegisterControllerName = "Register";

        public static string ForgotPasswordMessage =
             "<p>You received this message because you have recently said that you need to reset your password at World of Words.</p>" 
             + "<p>Please click the link below to reset your password.</p>";

        public static string MessageMaker(string body)
        {
            return "<div align=\"center\"> <table style=\"background-color: #FFFFFF; font-family: verdana, tahoma, sans-serif; color: black;\">"
                + "<tr> <td> <h2>Hello,</h2>"
                + "<p>This is a confirmation message from World of Words.</p>"
                + body
                + "<p>Happy learning!</p>"
                + "Love,<br/> Your friends at World of Words</p>"
                + "<p> <img src=\"http://i.imgur.com/MgwBlHe.png\" alt=\"World of Words\" height=\"50\" width=\"250\" /> </td> </tr> </table></div>";
        }

        public static string X2 = "x2";
    }
}