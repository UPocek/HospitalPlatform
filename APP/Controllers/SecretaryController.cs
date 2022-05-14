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

        public bool IsRoomOccupied(string examinationRoomName, string dateAndTimeOfExamination, int durationOfExamination){
        var examinations = database.GetCollection<Examination>("MedicalExaminations");
        var possiblyOccupiedRooms = examinations.Find(item => true).ToList();

        foreach (Examination item in possiblyOccupiedRooms){
            if(item.RoomName == examinationRoomName){
                DateTime itemBegin = DateTime.Parse(item.DateAndTimeOfExamination);
                DateTime itemEnd = itemBegin.AddMinutes(item.DurationOfExamination);
                DateTime examinationBegin = DateTime.Parse(dateAndTimeOfExamination);
                DateTime examinationEnd = examinationBegin.AddMinutes(durationOfExamination);
                if(examinationBegin >= itemBegin && examinationBegin <= itemEnd || examinationEnd >= itemBegin && examinationEnd <= itemEnd){
                    return true;
                }  
            }
        }
        return false;
        }

        public bool IsRoomValid(string roomName){
            var rooms = database.GetCollection<Room>("Rooms");
            var resultingRoom = rooms.Find(r => r.Name == roomName && r.InRenovation == false);
            if(resultingRoom == null){
                return false;
            }
            return true;
        }

        public bool IsValidPatient(int id){
            var patients = database.GetCollection<Patient>("Patients");
            var resultingPatient = patients.Find(p => p.Id == id).FirstOrDefault();
            if(resultingPatient == null){
                return false;
            }
            return true;
        }

        public bool IsDoctorFree(int doctorId, string examinationDate){
            var doctorsExaminations = database.GetCollection<Examination>("MedicalExamination").Find(e => e.DoctorId == doctorId).ToList();

            foreach(Examination e in doctorsExaminations){
                DateTime doctorsExaminationBegin = DateTime.Parse(e.DateAndTimeOfExamination);
                DateTime doctorsExaminationEnd = doctorsExaminationBegin.AddMinutes(e.DurationOfExamination);
                DateTime examinationDateParsed = DateTime.Parse(examinationDate);
                if(doctorsExaminationBegin <= examinationDateParsed && doctorsExaminationEnd >= examinationDateParsed){
                    return false;
                }
            }
            return true;

        }

        public bool IsPatientFree(int patientId, string examinationDate){
            var patientsExaminations = database.GetCollection<Examination>("MedicalExamination").Find(e => e.PatinetId == patientId).ToList();

            foreach(Examination e in patientsExaminations){
                DateTime patientsExaminationBegin = DateTime.Parse(e.DateAndTimeOfExamination);
                DateTime patientsExaminationEnd = patientsExaminationBegin.AddMinutes(e.DurationOfExamination);
                DateTime examinationDateParsed = DateTime.Parse(examinationDate);
                if(patientsExaminationBegin <= examinationDateParsed && patientsExaminationEnd >= examinationDateParsed){
                    return false;
                }
            }
            return true;

        }

        public bool IsRoomInRenovation(string doctorid, string examinationDate){
            var renovations = database.GetCollection<Renovation>("Renovations").Find(renovation => true).ToList();

            foreach(Renovation r in renovations){
                DateTime renovationBegin = DateTime.Parse(r.StartDate);
                DateTime renovationEnd = DateTime.Parse(r.EndDate);
                DateTime examinationDateParsed = DateTime.Parse(examinationDate);
                if(renovationBegin <= examinationDateParsed && renovationEnd >= examinationDateParsed){
                    return true;
                }
            }
            return false;

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
                var isValidPatient = IsValidPatient(newExamination.PatinetId);
                var isValidRoom = IsRoomValid(newExamination.RoomName);
                var isOccupiedRoom = IsRoomOccupied(newExamination.RoomName, newExaminationDate.ToString(), newExamination.DurationOfExamination);
                var isRoomInRenovation = IsRoomInRenovation(newExamination.RoomName, newExaminationDate.ToString());
                var isPatientFree = IsPatientFree(newExamination.PatinetId, newExaminationDate.ToString());
                var isDoctorFree = IsDoctorFree(newExamination.DoctorId, newExaminationDate.ToString());
                if (isValidRoom && isValidPatient && !isRoomInRenovation && !isOccupiedRoom && isPatientFree && isDoctorFree){
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
