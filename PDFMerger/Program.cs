using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFMerger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Path to folder:\n");
            var pdfPath = @"E:\Random Applications\PDFMerger\PDFMerger\resources";
            var outputPath = @"E:\Random Applications\PDFMerger\PDFMerger\output";

            var filePaths = Directory.GetDirectories(pdfPath);

            foreach (var path in filePaths)
            {
                var fileNames = Directory.GetFiles(path, "*.pdf", SearchOption.AllDirectories).Select(x => Path.GetFileName(x)).ToArray();

                if (fileNames.Length > 0)
                {
                    PdfDocument outPdf = new PdfDocument();

                    PdfPage frontPage = new PdfPage();
                    outPdf.AddPage(frontPage);

                    int totalPageCount = 2;
                    int verticalOffset = 0;

                    foreach (var file in fileNames)
                    {
                        using (PdfDocument existingPdf = PdfReader.Open($@"{path}\{file}", PdfDocumentOpenMode.Import))
                        {
                            CopyPages(existingPdf, outPdf);

                            XFont fontNormal = new XFont("Calibri", 20, XFontStyle.Regular);
                            XGraphics gfx = XGraphics.FromPdfPage(frontPage);
                            var xrect = new XRect(240, 395 + verticalOffset, 300, 25);
                            var rect = gfx.Transformer.WorldToDefaultPage(xrect);
                            var pdfrect = new PdfRectangle(rect);
                            verticalOffset += 30;

                            //file link
                            frontPage.AddDocumentLink(pdfrect, totalPageCount);
                            totalPageCount += existingPdf.Pages.Count;

                            gfx.DrawString(file.Split('.')[0], fontNormal, XBrushes.Black, xrect, XStringFormats.TopLeft);

                            gfx.Dispose();
                        }
                    }

                    var filePathSplit = path.Split('\\');
                    var newFileName = filePathSplit[filePathSplit.Length - 1];
                    var saveLocation = $@"{outputPath}\{newFileName}.pdf";
                    outPdf.Save(saveLocation);
                }
            }
        }

        static void CopyPages(PdfDocument from, PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }
        }
    }
}
