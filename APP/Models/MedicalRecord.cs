namespace APP.Models
{
    public class MedicalRecord
    {
        public int height { get; set; }
        public int weight { get; set; }
        public string[] diseases { get; set; } = null!;
        public string[] alergies { get; set; } = null!;
        public string[] drugs { get; set; } = null!;
        public string[] examinations { get; set; } = null!;
        public string[] medicalInstructions { get; set; } = null!;





    }
}