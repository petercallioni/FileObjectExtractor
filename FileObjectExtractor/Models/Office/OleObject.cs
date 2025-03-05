namespace FileObjectExtractor.Models.Office
{
    public class OleObject
    {
        public string Rid { get; set; }
        public bool HasIcon { get; set; }
        public OleObject(string rid, bool hasIcon)
        {
            this.Rid = rid;
            this.HasIcon = hasIcon;
        }
    }
}