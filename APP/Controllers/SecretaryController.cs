#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Cors;

namespace APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecretaryController : ControllerBase
    {

        private IMongoDatabase database;
        public SecretaryController()
        {
            var settings = MongoClientSettings.FromConnectionString("mongodb+srv://admin:admin@cluster0.ctjt6.mongodb.net/USI?retryWrites=true&w=majority");
            var client = new MongoClient(settings);
            database = client.GetDatabase("USI");
        }

        // GET: api/Secretary/patients
        [HttpGet("patients")]
        public async Task<List<Patient>> GetUnblockedPatients()
        {
            var patients = database.GetCollection<Patient>("Patients");

            return patients.Find(item => item.Active == "0").ToList();
        }

        // GET by Id: api/Secretary/patients/901
        [HttpGet("patients/{id}")]
        public async Task<Patient> GetUnblockedPatient(int id)
        {
            var patients = database.GetCollection<Patient>("Patients");
            
            return patients.Find(item => item.Id == id && item.Active=="0").FirstOrDefault();
        }

        // GET: api/Secretary/patients/blocked
        [HttpGet("patients/blocked")]
        public async Task<List<Patient>> GetBlockedPatients()
        {
            var patients = database.GetCollection<Patient>("Patients");

            return patients.Find(item => item.Active != "0").ToList();
        }
        
        // POST: api/Secretary/patients
        [HttpPost("patients")]
        public async Task<IActionResult> CreatePatient(int id, Patient patient)
        {
            var patients = database.GetCollection<Patient>("Patients");

            if(patients.Find(item => item.Email == patient.Email).ToList().Count != 0){
                return BadRequest("Error: email already exists!");
            }

            Random rnd = new Random();
            patient.Id = rnd.Next(901,10000);

            // If patient with that id already exists generate another
            do
            {
                patient.Id = rnd.Next(901,10000);
            }
            while(patients.Find(item => item.Id == id).ToList().Count != 0);

            patients.InsertOne(patient);

            return Ok();
        }

        // POST: api/Secretary/patients/901
        [HttpPut("patients/{id}")]
        public async Task<IActionResult> UpdatePatient(int id, Patient patient)
        {
            var patients = database.GetCollection<Patient>("Patients");
            Patient updatedPatient = patients.Find(p=> p.Id == id).FirstOrDefault();

            updatedPatient.FirstName = patient.FirstName;
            updatedPatient.LastName = patient.LastName;
            updatedPatient.Email = patient.Email;
            updatedPatient.Password = patient.Password;
            updatedPatient.MedicalRecord.Weight = patient.MedicalRecord.Weight;
            updatedPatient.MedicalRecord.Height = patient.MedicalRecord.Height;
            updatedPatient.MedicalRecord.BloodType = patient.MedicalRecord.BloodType;

            patients.ReplaceOne(p => p.Id == id, updatedPatient);
            return Ok();   
        }

        // DELETE: api/Secretary/patients/901

        [HttpDelete("patients/{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patients = database.GetCollection<Patient>("Patients");
            patients.DeleteOne(p => p.Id == id);
            
            var examinations = database.GetCollection<Examination>("MedicalExaminations");
            var filter = Builders<Examination>.Filter.Gt(e=> e.DateAndTimeOfExamination, DateTime.Now.ToString("yyyy-MM-ddTHH:mm")) & Builders<Examination>.Filter.Eq("patient", id);
            examinations.DeleteMany(filter);

            return Ok(); 
        }

        [HttpPut("patients/block/{id}/{activityValue}")]
        // PUT: api/Secretary/patients/901/1
        public async Task<IActionResult> ChangePatientActivity(int id, string activityValue)
        {
            var patients = database.GetCollection<Patient>("Patients");
            Patient updatedPatient = patients.Find(p=> p.Id == id).FirstOrDefault();

            updatedPatient.Active = activityValue;

            patients.ReplaceOne(p => p.Id == id, updatedPatient);
            return Ok();   
        }


        [HttpGet("patients/{id}/activity")]
        // PUT: api/Secretary/patients/901/1
        public async Task<String> GetPatientActivity(int id)
        {
            var patients = database.GetCollection<Patient>("Patients");;

            return patients.Find(p=> p.Id == id).FirstOrDefault().Active;   
        }

        // GET: api/Secretary/examinationRequests
        [HttpGet("examinationRequests")]
        public async Task<List<ExaminationRequest>> GetExaminationRequests()
        {
            var requests = database.GetCollection<ExaminationRequest>("ExaminationRequests");
            
            //Delete deprecated requests
            var filter = Builders<ExaminationRequest>.Filter.Gt(e=>e.Examination.DateAndTimeOfExamination,DateTime.Now.ToString("yyyy-MM-ddTHH:mm"));
            requests.DeleteMany(filter);

            return requests.Find(item => true).ToList();
        }


        // GET: api/Secretary/examinations/100
        [HttpGet("examination/{id}")]
        public async Task<Examination> GetExamination(int id)
        {
            var examinations = database.GetCollection<Examination>("MedicalExaminations");
            
            return examinations.Find(item => item.Id == id).FirstOrDefault();
        }

        
        // PUT: api/Secretary/examinationRequests/accept/1
        [HttpPut("examinationRequests/accept/{id}")]
        public async Task<IActionResult> AcceptExaminationRequest(string id)
        {
            var requests = database.GetCollection<ExaminationRequest>("ExaminationRequests");
            ExaminationRequest examinationRequest = requests.Find(e=> e._Id == id).FirstOrDefault();

            var examination = examinationRequest.Examination;

            
            var examinations = database.GetCollection<Examination>("Examinations");

            if(examinationRequest.Status == 0){
                examinations.DeleteOne(e => e.Id == examination.Id);
            }
            else{
                examinations.ReplaceOne(e => e.Id == examination.Id,examination);
            }
            requests.DeleteOne(e=> e._Id == id);
            return Ok();
        }


        // PUT: api/Secretary/examinationRequests/decline/1
        [HttpPut("examinationRequests/decline/{id}")]
        public async Task<IActionResult> DeclineExaminationRequest(string id)
        {
            var requests = database.GetCollection<ExaminationRequest>("ExaminationRequests");

            requests.DeleteOne(e => e._Id == id);
            
            return Ok();
        }

        public bool validateTimeOfExamination(DateTime date,int duration,string roomName,int doctorId){

            var currentDate = DateTime.Now;
            var newExaminationBegging = date;
            var newExaminationEnding = date.AddMinutes(duration);
            
            if (currentDate > newExaminationBegging){
                return false;
            }

            var examinations = database.GetCollection<Examination>("MedicalExaminations");

            var examinationsInRoom = examinations.Find(e => e.RoomName == roomName).ToList();
            
            foreach (Examination examinationRoom in examinationsInRoom){

                var examinationBeggingRoom = DateTime.Parse(examinationRoom.DateAndTimeOfExamination);
                var examinationEndingRoom = DateTime.Parse(examinationRoom.DateAndTimeOfExamination).AddMinutes(examinationRoom.DurationOfExamination);

                if ((newExaminationBegging >= examinationBeggingRoom && newExaminationBegging <= examinationEndingRoom) 
                    | (newExaminationEnding >= examinationBeggingRoom && newExaminationEnding <= examinationEndingRoom)){

                    var examinationsWithDoctor = examinations.Find(e => e.DoctorId == doctorId).ToList();
                    
                    foreach (Examination examinationDoctor in examinationsWithDoctor){

                    var examinationBeggingDoctor = DateTime.Parse(examinationDoctor.DateAndTimeOfExamination);
                    var examinationEndingDoctor = DateTime.Parse(examinationDoctor.DateAndTimeOfExamination).AddMinutes(examinationDoctor.DurationOfExamination);

                    if ((newExaminationBegging >= examinationBeggingDoctor && newExaminationBegging <= examinationEndingDoctor) 
                        |(newExaminationEnding >= examinationBeggingDoctor && newExaminationEnding <= examinationEndingDoctor)){
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public void RemovePatientReferral(int referralid,Examination newExamination){
            var patients = database.GetCollection<Patient>("Patients");
            Patient updatedPatient = patients.Find(p => p.Id == newExamination.PatinetId).FirstOrDefault();

                foreach (Referral patientReferral in updatedPatient.MedicalRecord.Referrals){
                    if (patientReferral.ReferralId == referralid){
                        updatedPatient.MedicalRecord.Referrals.Remove(patientReferral);
                        break;
                    }
                }

            patients.ReplaceOne(p => p.Id == newExamination.PatinetId, updatedPatient);
        }




         // GET: api/Secretary/examination/referral/create/none
        [HttpPost("examination/referral/create/{specialization}/{referralid}")]
        public async Task<IActionResult> CreateRefferedExamination(Examination newExamination,string specialization,int referralid)
        {
            var patients = database.GetCollection<Patient>("Patients");

            if (specialization != "none"){
                var employees = database.GetCollection<Employee>("Employees");
                List<Employee> specializedDoctors = employees.Find(e => e.Role == "doctor" && e.Specialization == specialization).ToList();
                if (specializedDoctors.Count()-1 < 0){
                    
                    RemovePatientReferral(referralid,newExamination);

                    return BadRequest("Error: No such specialist exists");
                }
                Random rnd = new Random();
                newExamination.DoctorId = specializedDoctors[rnd.Next(0,specializedDoctors.Count()-1)].Id;
            }

            if (newExamination.DurationOfExamination <= 15 || newExamination.DurationOfExamination >= 200){
                return BadRequest();
            }

            var examinations = database.GetCollection<Examination>("MedicalExaminations");

            var newExaminationDate = DateTime.Now.AddDays(1);


            while(true){
                
                if (validateTimeOfExamination(newExaminationDate,newExamination.DurationOfExamination,newExamination.RoomName,newExamination.DoctorId)){
                     newExamination.DateAndTimeOfExamination = newExaminationDate.ToString("yyyy-MM-ddTHH:mm");
                     break;
                 }

                else{
                    newExaminationDate = newExaminationDate.AddMinutes(30);
                }

            }

            var rooms = database.GetCollection<Room>("Rooms");
            var resultingRoom = rooms.Find(r => r.Name == newExamination.RoomName);

            if (resultingRoom == null)
            {
                return BadRequest();
            }
            var id = examinations.Find(e => true).SortByDescending(e => e.Id).FirstOrDefault().Id;
            newExamination.Id = id + 1;
            examinations.InsertOne(newExamination);

            RemovePatientReferral(referralid,newExamination);
            
            return Ok();
        
        }       

    }
}
