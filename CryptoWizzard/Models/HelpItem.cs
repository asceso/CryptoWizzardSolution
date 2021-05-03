namespace CryptoWizzard.Models
{
    public class HelpItem
    {
        public string ChapterHeader { get; set; }
        public string ChapterBody { get; set; }
        public HelpItem(string chapterHeader, string chapterBody)
        {
            ChapterHeader = chapterHeader;
            ChapterBody = chapterBody;
        }
    }
}
