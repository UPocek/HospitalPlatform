namespace APP.Models
{
    public class Patient
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string role { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public int active  { get; set; }
        public string id { get; set; }
        public MedicalRecord medicalRecord { get; set; } = null!;



    }
}