using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using System.IO;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PdfComparerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PDFController : ControllerBase
    {
        // GET: api/<PDFController>
        [HttpPost("/getPDFComparison")]
        public PDFResponse GetPDFDifference([FromForm] PDFDifferenceRequest request )
        {
            PDFResponse response = new PDFResponse();
            try
            {
                ///*Console*/.WriteLine("Enter the Path 1");
                //string pdf1Path = Console.ReadLine();
                //Console.WriteLine("Enter the Path 2");
                //string pdf2Path = Console.ReadLine();

                string text1 = ExtractTextFromPdf(request.file1);
                string text2 = ExtractTextFromPdf(request.file2);

                string[] lines1 = text1.Split('\n');
                string[] lines2 = text2.Split('\n');

                int maxLength = Math.Max(lines1.Length, lines2.Length);

                

                for (int i = 0; i < maxLength; i++)
                {
                    if (i < lines1.Length && i < lines2.Length)
                    {
                        if (lines1[i].Trim() != lines2[i].Trim())
                        {
                            response.Difference.Add(i + 1);
                        }
                    }
                    else if (i < lines1.Length)
                    {
                        //Console.WriteLine($"PDF 1 has extra content at line {i + 1}: {lines1[i].Trim()}");
                        response.ExtraLines.PDF1.Add(i + 1);
                    }
                    else
                    {
                        //Console.WriteLine($"PDF 2 has extra content at line {i + 1}: {lines2[i].Trim()}");
                        response.ExtraLines.PDF2.Add(i + 2);
                    }
                }
                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string ExtractTextFromPdf(IFormFile file)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    ms.Position = 0; // Reset the stream position
                    using (PdfReader reader = new PdfReader(ms))
                    {
                        string text = string.Empty;
                        for (int i = 1; i <= reader.NumberOfPages; i++)
                        {
                            text += PdfTextExtractor.GetTextFromPage(reader, i);
                        }
                        return text;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

    
