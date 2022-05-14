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
            var filter = Builders<Examination>.Filter.Lt(e=> e.DateAndTimeOfExamination, DateTime.Now.ToString("yyyy-MM-ddTHH:mm")) & Builders<Examination>.Filter.Eq("patient", id);
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
        // GET: api/Secretary/patients/901/activity
        public async Task<String> GetPatientActivity(int id)
        {
            var patients = database.GetCollection<Patient>("Patients");;

            return patients.Find(p=> p.Id == id).FirstOrDefault().Active;   
        }


        [HttpGet("doctors/speciality")]
        // GET: api/Secretary/doctors/speciality
        public async Task<List<String>> GetDoctorSpeciality(int id)
        {
            var collection = database.GetCollection<Employee>("Employees");
            var doctors = collection.Find(d => d.Role == "doctor").ToList();

            List<string> allSpecializations = new List<string>();
            
            foreach(Employee e in doctors){
                allSpecializations.Add(e.Specialization);
            }

            allSpecializations = allSpecializations.Distinct().ToList();

            return allSpecializations;   
        }

        // GET: api/Secretary/examinationRequests
        [HttpGet("examinationRequests")]
        public async Task<List<ExaminationRequest>> GetExaminationRequests()
        {
            var requests = database.GetCollection<ExaminationRequest>("ExaminationRequests");
            
            //Delete deprecated requests
            var filter = Builders<ExaminationRequest>.Filter.Lt(e=>e.Examination.DateAndTimeOfExamination,DateTime.Now.ToString("yyyy-MM-ddTHH:mm"));
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


        public bool CheckExaminationTimeValidity(Examination e){
             var isValidPatient = IsValidPatient(e.PatinetId);
            var isValidRoom = IsRoomValid(e.RoomName);
            var isOccupiedRoom = IsRoomOccupied(e.RoomName, e.DateAndTimeOfExamination.ToString(), e.DurationOfExamination);
            var isRoomInRenovation = IsRoomInRenovation(e.RoomName, e.DateAndTimeOfExamination.ToString());
            var isPatientFree = IsPatientFree(e.PatinetId, e.DateAndTimeOfExamination.ToString());
            var isDoctorFree = IsDoctorFree(e.DoctorId, e.DateAndTimeOfExamination.ToString());
            if (isValidRoom && isValidPatient && !isRoomInRenovation && !isOccupiedRoom && isPatientFree && isDoctorFree){
                return true;
            }
            else{
                return false;
            }
            
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
            

            DateTime upperlimit;
            DateTime lowerlimit;

            while(true){
                newExamination.DateAndTimeOfExamination = newExaminationDate.ToString("yyyy-MM-ddTHH:mm");
                if (CheckExaminationTimeValidity(newExamination)){
                    lowerlimit = new DateTime(newExaminationDate.Year,newExaminationDate.Month,newExaminationDate.Day,8,0,0);
                    upperlimit = new DateTime(newExaminationDate.Year,newExaminationDate.Month,newExaminationDate.Day,23,59,0);
                    if (newExaminationDate >= lowerlimit && newExaminationDate <= upperlimit){ 
                        break;
                    }
                    else{
                        continue;
                    }
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

        // GET: api/Secretary/examinations
        [HttpGet("examination/movable")]
        public async Task<List<Examination>> GetMovableExaminations(Examination newExamination)
        {
            var examinations = database.GetCollection<Examination>("MedicalExaminations");

            var dateFilter = Builders<Examination>.Filter.Gt(e=> e.DateAndTimeOfExamination, DateTime.Now.ToString("yyyy-MM-ddTHH:mm"));
            var roomFilter = Builders<Examination>.Filter.Eq(e => e.RoomName,newExamination.RoomName);
            var urgentFilter = Builders<Examination>.Filter.Eq(e => e.IsUrgent,false);
            var doctorFilter = Builders<Examination>.Filter.Eq(e => e.DoctorId,newExamination.DoctorId);

            var filter = dateFilter & roomFilter & urgentFilter & doctorFilter;

            var examinationsAfterNow = examinations.Find(filter).SortBy(e=>e.DateAndTimeOfExamination).ToList();

            List<Examination> fiveExaminations = new List<Examination>();

            fiveExaminations = examinationsAfterNow.Take(5).ToList();
            
            return fiveExaminations;
        }





        [HttpPost("examination/create/urgent/{specialization}")]
        public async Task<IActionResult> CreateUrgentExamination(Examination newExamination,string specialization)
        {

            var examinations = database.GetCollection<Examination>("MedicalExaminations");

            var patients = database.GetCollection<Patient>("Patients");

            string roomType;
            if (newExamination.TypeOfExamination == "visit"){
                roomType = "examination room";
            }
            else{
                roomType = "operation room";
            }

            var room = database.GetCollection<Room>("Rooms").Find(r=>r.Type == roomType).FirstOrDefault();
            
            newExamination.RoomName = room.Name;


            if(patients.Find(p=>p.Id == newExamination.PatinetId).CountDocuments() == 0){
                return BadRequest("Patient doesnt exist");
            }

            var employees = database.GetCollection<Employee>("Employees");
            List<Employee> specializedDoctors = employees.Find(e => e.Role == "doctor" && e.Specialization == specialization).ToList();

            var urgentExaminationDate = DateTime.Now.AddMinutes(5);
            var urgentExaminationEnd = DateTime.Now.AddHours(2);


            while(urgentExaminationDate <= urgentExaminationEnd){
                newExamination.DateAndTimeOfExamination = urgentExaminationDate.ToString("yyyy-MM-ddTHH:mm");
                foreach(Employee doctor in specializedDoctors){
                    newExamination.DoctorId = doctor.Id;

                    if (CheckExaminationTimeValidity(newExamination)){
                        var rooms = database.GetCollection<Room>("Rooms");
                        var resultingRoom = rooms.Find(r => r.Name == newExamination.RoomName);

                        if (resultingRoom == null)
                        {
                            return BadRequest();
                        }
                        var id = examinations.Find(e => true).SortByDescending(e => e.Id).FirstOrDefault().Id;
                        newExamination.Id = id + 1;
                        examinations.InsertOne(newExamination);
                        return Ok();
                    }
                }
                urgentExaminationDate = urgentExaminationDate.AddMinutes(10);
            }

            
            return BadRequest("No free term found!");
        
        }






        [HttpPost("examination/create/urgent")]
        public async Task<IActionResult> CreateUrgentExaminationWithTermMoving(Examination newExamination)
        {

            var examinations = database.GetCollection<Examination>("MedicalExaminations");

            var reservedTimeFrames = examinations.Find(e=>e.RoomName == newExamination.RoomName && e.DoctorId == newExamination.DoctorId).ToList();

            List<Examination> toMove = new List<Examination>();

            var newExaminationBegin = DateTime.Parse(newExamination.DateAndTimeOfExamination);
            var newExaminationEnd = newExaminationBegin.AddMinutes(newExamination.DurationOfExamination);

            DateTime toMoveExamBegin;

            foreach (Examination e in reservedTimeFrames){
                toMoveExamBegin = DateTime.Parse(e.DateAndTimeOfExamination);
                if(newExaminationBegin >= toMoveExamBegin && newExaminationEnd <= toMoveExamBegin){
                    toMove.Add(e);
                }
            }
            
            foreach (Examination e in toMove){
                examinations.DeleteOne(exam => exam.Id == e.Id);
            }

            if (CheckExaminationTimeValidity(newExamination)){
                examinations.InsertOne(newExamination);
            }
            else{
                return BadRequest("NESTO NE NIJE DOBRO KOJE SRANJE");
            }


            var iterationDate = DateTime.Now;


            foreach (Examination e in toMove){
                while(true){
                    if (CheckExaminationTimeValidity(e)){
                        e.DateAndTimeOfExamination = iterationDate.ToString("yyyy-MM-ddTHH:mm");
                        examinations.InsertOne(newExamination);
                        break;
                    }

                    else{
                        iterationDate = iterationDate.AddMinutes(5);
                    }
                }
            } 

            return Ok();
        }

    }
}
