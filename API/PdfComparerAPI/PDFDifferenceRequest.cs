namespace PdfComparerAPI
{
    public class PDFDifferenceRequest
    {
        public IFormFile file1 { get; set; }
        public IFormFile file2 { get; set; }
    }
}
