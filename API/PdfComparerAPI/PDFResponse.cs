namespace PdfComparerAPI
{
    public class PDFResponse
    {
        public List<int> Difference { get; set; } = new List<int>();
        public ExtraLines ExtraLines { get; set; } = new ExtraLines();
    }

    public class ExtraLines
    {
        public List<int> PDF1 { get; set; } = new List<int>();
        public List<int> PDF2 { get; set; } = new List<int>();
    }
}

