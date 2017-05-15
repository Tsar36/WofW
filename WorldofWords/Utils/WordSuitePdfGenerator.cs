using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using WorldOfWords.Domain.Models;

namespace WorldofWords.Utils
{
    /// <summary>
    /// Generates a PDF version of the WordSuite.
    /// </summary>
    public sealed class WordSuitePdfGenerator : IPdfGenerator<WordSuite>
    {
        private static PdfPCell TextCell(string text, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.BorderColor = BaseColor.BLACK;
            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            cell.PaddingBottom = 6;
            cell.PaddingTop = 0;
            return cell;
        }

        /// <summary>
        /// Transforms a WordSuite into a byte array containing its PDF version.
        /// </summary>
        /// <param name="wordsuite"></param>
        /// <returns></returns>
        byte[] IPdfGenerator<WordSuite>.GetWordsInPdf(WordSuite source)
        {
            byte[] result;

            using (MemoryStream stream = new MemoryStream())
            {
                string fg = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
                BaseFont baseFont = BaseFont.CreateFont(fg, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                Font normalFont = new Font(
                    bf: baseFont,
                    size: 12,
                    style: Font.NORMAL,
                    color: BaseColor.BLACK);
                Font boldFont = new Font(
                    bf: baseFont,
                    size: 12,
                    style: Font.ITALIC,
                    color: BaseColor.BLACK);
                Font headerFont = new Font(
                    bf: baseFont,
                    size: 24,
                    style: Font.NORMAL,
                    color: BaseColor.BLACK);

                Document document = new Document(PageSize.A4, 128, 128, 32, 32);

                PdfWriter writer = PdfWriter.GetInstance(document, stream);
                document.Open();

                Paragraph header = new Paragraph(
                    str: source.Name,
                    font: headerFont);
                header.Alignment = Element.ALIGN_CENTER;
                header.SpacingAfter = 30;
                document.Add(header);

                PdfPTable contentsTable = new PdfPTable(numColumns: 3);
                contentsTable.SetWidths(new int[] { 10, 45, 45 });

                contentsTable.HeaderRows = 1;
                contentsTable.AddCell(TextCell("#", boldFont));
                contentsTable.AddCell(TextCell(source.Language.Name, boldFont));
                contentsTable.AddCell(TextCell("Ukrainian", boldFont));

                int i = 1;
                source.WordProgresses.ToList().ForEach(progress =>
                {
                    contentsTable.AddCell(TextCell((i++).ToString(), normalFont));
                    contentsTable.AddCell(TextCell(progress.WordTranslation.OriginalWord.Value, normalFont));
                    contentsTable.AddCell(TextCell(progress.WordTranslation.TranslationWord.Value, normalFont));
                });
                document.Add(contentsTable);

                document.Close();
                result = stream.ToArray();
                stream.Close();
            }
            return result;
        }

        byte[] IPdfGenerator<WordSuite>.GetTaskInPdf(WordSuite source)
        {
            byte[] result;

            using (MemoryStream stream = new MemoryStream())
            {
                string fg = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
                BaseFont baseFont = BaseFont.CreateFont(fg, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                Font normalFont = new Font(
                    bf: baseFont,
                    size: 12,
                    style: Font.NORMAL,
                    color: BaseColor.BLACK);
                Font boldFont = new Font(
                    bf: baseFont,
                    size: 12,
                    style: Font.ITALIC,
                    color: BaseColor.BLACK);
                Font headerFont = new Font(
                    bf: baseFont,
                    size: 24,
                    style: Font.NORMAL,
                    color: BaseColor.BLACK);

                Document document = new Document(PageSize.A4, 128, 128, 32, 32);

                PdfWriter writer = PdfWriter.GetInstance(document, stream);
                document.Open();

                Paragraph header = new Paragraph(
                    str: source.Name,
                    font: headerFont);
                header.Alignment = Element.ALIGN_CENTER;
                header.SpacingAfter = 30;
                document.Add(header);

                PdfPTable contentsTable = new PdfPTable(numColumns: 3);
                contentsTable.SetWidths(new int[] { 10, 45, 45 });

                contentsTable.HeaderRows = 1;
                contentsTable.AddCell(TextCell("#", boldFont));
                contentsTable.AddCell(TextCell("Ukrainian", boldFont));
                contentsTable.AddCell(TextCell(source.Language.Name, boldFont));

                int i = 1;
                Shuffle(source.WordProgresses.ToList()).ToList().ForEach(progress =>
                {
                    contentsTable.AddCell(TextCell((i++).ToString(), normalFont));
                    contentsTable.AddCell(TextCell(progress.WordTranslation.TranslationWord.Value, normalFont));
                    contentsTable.AddCell(TextCell("", normalFont));
                });
                document.Add(contentsTable);

                document.Close();
                result = stream.ToArray();
                stream.Close();
            }
            return result;
        }

        private static IList<T> Shuffle<T>(IList<T> list)
        {
            Random random = new Random();
            int currentNode = list.Count;
            while (currentNode > 1)
            {
                currentNode--;
                int randomNode = random.Next(currentNode + 1);
                T value = list[randomNode];
                list[randomNode] = list[currentNode];
                list[currentNode] = value;
            }
            return list;
        }
    }
}