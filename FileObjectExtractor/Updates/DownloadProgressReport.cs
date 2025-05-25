namespace FileObjectExtractor.Updates
{
    public class DownloadProgressReport
    {
        public double Fraction { get; set; }
        public long BytesDownloaded { get; set; }
        public long TotalBytes { get; set; }
    }
}
