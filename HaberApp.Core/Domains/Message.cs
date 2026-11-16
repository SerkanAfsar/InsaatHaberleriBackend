namespace HaberApp.Core.Domains
{
    public class Message : BaseEntity
    {
        public string NameSurname { get; set; }
        public string EMail { get; set; }
        public string Subject { get; set; }
        public string MessageText { get; set; }
        public bool IsReaded { get; set; } = false;
    }
}
