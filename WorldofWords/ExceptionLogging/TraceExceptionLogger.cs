using System.Diagnostics;
using System.Web.Http.ExceptionHandling;

namespace WorldofWords.ExceptionLogging
{
    class TraceExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            if (context != null)
            {
                Trace.TraceError(context.ExceptionContext.Exception.ToString());
            }
        }
    }
}