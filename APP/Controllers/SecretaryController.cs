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

            return patients.Find(item => item.active == "0").ToList();
        }

        // GET by Id: api/Secretary/patients/901
        [HttpGet("patients/{id}")]
        public async Task<Patient> GetUnblockedPatient(int id)
        {
            var patients = database.GetCollection<Patient>("Patients");
            
            return patients.Find(item => item.id == id && item.active=="0").FirstOrDefault();
        }

        // GET: api/Secretary/patients/blocked
        [HttpGet("patients/blocked")]
        public async Task<List<Patient>> GetBlockedPatients()
        {
            var patients = database.GetCollection<Patient>("Patients");

            return patients.Find(item => item.active != "0").ToList();
        }
        
        // POST: api/Secretary/patients
        [HttpPost("patients")]
        public async Task<IActionResult> CreatePatient(int id, Patient patient)
        {
            var patients = database.GetCollection<Patient>("Patients");

            if(patients.Find(item => item.email == patient.email).ToList().Count != 0){
                return BadRequest("Error: email already exists!");
            }

            Random rnd = new Random();
            patient.id = rnd.Next(901,10000);

            // If patient with that id already exists generate another
            do
            {
                patient.id = rnd.Next(901,10000);
            }
            while(patients.Find(item => item.id == id).ToList().Count != 0);

            patients.InsertOne(patient);

            return Ok();
        }

        // POST: api/Secretary/patients/901
        [HttpPut("patients/{id}")]
        public async Task<IActionResult> UpdatePatient(int id, Patient patient)
        {
            var patients = database.GetCollection<Patient>("Patients");
            Patient updatedPatient = patients.Find(p=> p.id == id).FirstOrDefault();

            updatedPatient.firstName = patient.firstName;
            updatedPatient.lastName = patient.lastName;
            updatedPatient.email = patient.email;
            updatedPatient.password = patient.password;
            updatedPatient.medicalRecord.weight = patient.medicalRecord.weight;
            updatedPatient.medicalRecord.height = patient.medicalRecord.height;
            updatedPatient.medicalRecord.bloodType = patient.medicalRecord.bloodType;

            patients.ReplaceOne(p => p.id == id, updatedPatient);
            return Ok();   
        }

        // DELETE: api/Secretary/patients/901

        [HttpDelete("patients/{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patients = database.GetCollection<Patient>("Patients");
            patients.DeleteOne(p => p.id == id);
            
            var examinations = database.GetCollection<Examination>("MedicalExaminations");
            var filter = Builders<Examination>.Filter.Lt("date", DateTime.Now.ToString()) & Builders<Examination>.Filter.Eq("patient", id);
            examinations.DeleteMany(filter);

            return Ok(); 
        }

        [HttpPut("patients/block/{id}/{activityValue}")]
        // PUT: api/Secretary/patients/901/1
        public async Task<IActionResult> ChangePatientActivity(int id, string activityValue)
        {
            var patients = database.GetCollection<Patient>("Patients");
            Patient updatedPatient = patients.Find(p=> p.id == id).FirstOrDefault();

            updatedPatient.active = activityValue;

            patients.ReplaceOne(p => p.id == id, updatedPatient);
            return Ok();   
        }


        [HttpGet("patients/{id}/activity")]
        // PUT: api/Secretary/patients/901/1
        public async Task<String> GetPatientActivity(int id)
        {
            var patients = database.GetCollection<Patient>("Patients");;

            return patients.Find(p=> p.id == id).FirstOrDefault().active;   
        }

        // GET: api/Secretary/examinationRequests
        [HttpGet("examinationRequests")]
        public async Task<List<ExaminationRequest>> GetExaminationRequests()
        {
            var requests = database.GetCollection<ExaminationRequest>("ExaminationRequests");
            
            //Delete deprecated requests
            var filter = Builders<ExaminationRequest>.Filter.Lt(e=>e.examination.dateAndTimeOfExamination,DateTime.Now.ToString());
            requests.DeleteMany(filter);

            return requests.Find(item => true).ToList();
        }


        // GET: api/Secretary/examinations/100
        [HttpGet("examination/{id}")]
        public async Task<Examination> GetExamination(int id)
        {
            var examinations = database.GetCollection<Examination>("MedicalExaminations");
            
            return examinations.Find(item => item.id == id).FirstOrDefault();
        }

        
        // PUT: api/Secretary/examinationRequests/accept/1
        [HttpPut("examinationRequests/accept/{id}")]
        public async Task<IActionResult> AcceptExaminationRequest(string id)
        {
            var requests = database.GetCollection<ExaminationRequest>("ExaminationRequests");
            ExaminationRequest examinationRequest = requests.Find(e=> e._id == id).FirstOrDefault();

            var examination = examinationRequest.examination;

            
            var examinations = database.GetCollection<Examination>("Examinations");

            if(examinationRequest.status == 0){
                examinations.DeleteOne(e => e.id == examination.id);
            }
            else{
                examinations.ReplaceOne(e => e.id == examination.id,examination);
            }
            requests.DeleteOne(e=> e._id == id);
            return Ok();
        }


        // PUT: api/Secretary/examinationRequests/decline/1
        [HttpPut("examinationRequests/decline/{id}")]
        public async Task<IActionResult> DeclineExaminationRequest(string id)
        {
            var requests = database.GetCollection<ExaminationRequest>("ExaminationRequests");

            requests.DeleteOne(e => e._id == id);
            
            return Ok();
        }

        public bool validateTimeOfExaminationRoom(DateTime date,int duration,string roomName){

            var currentDate = DateTime.Now;
            var newExaminationBegging = date;
            var newExaminationEnding = date.AddMinutes(duration);
            
            if (currentDate > newExaminationBegging){
                return false;
            }

            var examinations = database.GetCollection<Examination>("MedicalExaminations");

            var examinationsInRoom = examinations.Find(e => e.roomName == roomName &&  e.dateAndTimeOfExamination.CompareTo(newExaminationBegging.ToString()) >= 0).SortBy(e => e.dateAndTimeOfExamination).ToList();
            
            foreach (Examination examination in examinationsInRoom){

                var examinationBegging = DateTime.Parse(examination.dateAndTimeOfExamination);
                var examinationEnding = DateTime.Parse(examination.dateAndTimeOfExamination).AddMinutes(examination.durationOfExamination);

                if ((newExaminationBegging >= examinationBegging && newExaminationBegging <= examinationEnding) 
                    | (newExaminationEnding >= examinationBegging && newExaminationEnding <= examinationEnding)){
                        return false;
                    }
            }

            return true;
        }


        public bool validateTimeOfExaminationDoctor(DateTime date,int duration,int doctorId){

            var currentDate = DateTime.Now;
            var newExaminationBegging = date;
            var newExaminationEnding = date.AddMinutes(duration);
            
            if (currentDate > newExaminationBegging){
                return false;
            }

            var examinations = database.GetCollection<Examination>("MedicalExaminations");

            var examinationsWithDoctor = examinations.Find(e => e.doctorId == doctorId &&  e.dateAndTimeOfExamination.CompareTo(newExaminationBegging.ToString()) >= 0).SortBy(e => e.dateAndTimeOfExamination).ToList();
            
            foreach (Examination examination in examinationsWithDoctor){

                var examinationBegging = DateTime.Parse(examination.dateAndTimeOfExamination);
                var examinationEnding = DateTime.Parse(examination.dateAndTimeOfExamination).AddMinutes(examination.durationOfExamination);

                if ((newExaminationBegging >= examinationBegging && newExaminationBegging <= examinationEnding) 
                    | (newExaminationEnding >= examinationBegging && newExaminationEnding <= examinationEnding)){
                        return false;
                    }
            }

            return true;
        }



         // GET: api/Secretary/examination/referral/create/none
        [HttpPost("examination/referral/create/{specialization}/{referralid}")]
        public async Task<IActionResult> CreateRefferedExamination(Examination examination,string specialization,int referralid)
        {
            if (specialization != "none"){
                var employees = database.GetCollection<Employee>("Employees");
                List<Employee> specializedDoctors = employees.Find(e => e.role == "doctor" && e.specialization == specialization).ToList();

                Random rnd = new Random();
                examination.doctorId = specializedDoctors[rnd.Next(0,specializedDoctors.Count()-1)].id;
            }

            if (examination.durationOfExamination <= 15 || examination.durationOfExamination >= 200){
                BadRequest();
            }
            var examinations = database.GetCollection<Examination>("MedicalExaminations");

            var checkFrom = DateTime.Now.AddDays(1);


            while(true){
                
                if (validateTimeOfExaminationRoom(checkFrom,examination.durationOfExamination,examination.roomName) &&
                 validateTimeOfExaminationDoctor(checkFrom,examination.durationOfExamination,examination.id)){
                     examination.dateAndTimeOfExamination = checkFrom.ToString();
                     break;
                 }

                else{
                    checkFrom.AddMinutes(5);
                }

            }

            var rooms = database.GetCollection<Room>("Rooms");
            var resultingRoom = rooms.Find(r => r.name == examination.roomName);

            if (resultingRoom == null)
            {
                return BadRequest();
            }
            var id = examinations.Find(e => true).SortByDescending(e => e.id).FirstOrDefault().id;
            examination.id = id + 1;
            examinations.InsertOne(examination);

            var patients = database.GetCollection<Patient>("Patients");
            Patient newPatient = patients.Find(p => p.id == examination.patinetId).FirstOrDefault();

            
            newPatient.medicalRecord.referrals.Remove(newPatient.medicalRecord.referrals[referralid]);
            
            return Ok();
        
        }       

    }
}
