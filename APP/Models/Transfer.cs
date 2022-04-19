namespace APP.Models
{
    public class Transfer
    {
        public string room1 { get; set; }
        public string room2 { get; set; }
        public string when { get; set; }
        public List<Dictionary<string, string>> equipment { get; set; }
    }
}