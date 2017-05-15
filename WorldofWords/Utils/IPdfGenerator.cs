namespace WorldofWords.Utils
{
    public interface IPdfGenerator<T>
    {
        byte[] GetWordsInPdf(T source);
        byte[] GetTaskInPdf(T source);
    }
}
