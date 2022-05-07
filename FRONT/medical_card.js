function getParamValue(name) {
    var location = decodeURI(window.location.toString());
    var index = location.indexOf('?') + 1;
    var subs = location.substring(index, location.length);
    var splitted = subs.split('&');

    for (var i = 0; i < splitted.length; i++) {
        var s = splitted[i].split('=');
        var pName = s[0];
        var pValue = s[1];
        if (pName == name)
            return pValue;
    }
};

var patient;
var patientId = getParamValue('patientId');
var doctorId = getParamValue('doctorId');
var secretaryId = getParamValue('secretaryId');
var jwtoken = getParamValue('token');
var patientsExaminations;

function setupPatientBasicInfo(){
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                
                patient = JSON.parse(this.responseText);

                let patientFName = document.getElementById('patientFName');
                patientFName.innerText = patient['firstName'];
                let patientLName = document.getElementById('patientLName');
                patientLName.innerText = patient['lastName'];
                let patientHeight = document.getElementById('patientHeight');
                patientHeight.innerText = patient['medicalRecord']['height'];
                let patientWeight = document.getElementById('patientWeight');
                patientWeight.innerText = patient['medicalRecord']['weight'];
                let patientBlood = document.getElementById('patientBlood');
                patientBlood.innerText = patient['medicalRecord']['bloodType'];
                let patientDiseases = document.getElementById('diseasesList');
                for (let disease of patient['medicalRecord']['diseases']){
                    let diseaseItem = document.createElement('li');
                    diseaseItem.innerText = disease;
                    patientDiseases.appendChild(diseaseItem);
                }
                let patientAlergies = document.getElementById('alergiesList');
                for (let alergie of patient['medicalRecord']['alergies']){
                    let alergieItem = document.createElement('li');
                    alergieItem.innerText = alergie;
                    patientAlergies.appendChild(alergieItem);
                }
                setUpMenu();
                setUpPatientExaminations();
                setUpPatientInstructions();
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/doctor/examinations/patientMedicalCard/' + patientId);
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}

function displayExaminations(){
    let table = document.getElementById('examinationsTable');
    table.innerHTML = '';
    for (let examination of patientsExaminations) {

        let newRow = document.createElement('tr');

        let examinationDate = document.createElement('td');
        examinationDate.innerText = (new Date(examination['date'])).toLocaleString();
        let examinationDone = document.createElement('td');
        examinationDone.innerText = examination['done'];
        let examinationDuration = document.createElement('td');
        examinationDuration.innerText = examination['duration'];
        let examinationRoom = document.createElement('td');
        examinationRoom.innerText = examination['room'];
        let examinationType = document.createElement('td');
        examinationType.innerText = examination['type'];
        let isUrgent = document.createElement('td');
        isUrgent.innerText = examination['urgent'];

        newRow.appendChild(examinationDate);
        newRow.appendChild(examinationDuration);
        newRow.appendChild(examinationDone);
        newRow.appendChild(examinationRoom);
        newRow.appendChild(examinationType);
        newRow.appendChild(isUrgent);
        table.appendChild(newRow);
        feather.replace();
    }
}

function setUpPatientExaminations(){
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                patientsExaminations = JSON.parse(this.responseText);
                displayExaminations();
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/doctor/examinations/patientId/' + patientId);
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}

function displayInstructions(doctors){
    let table = document.getElementById('instructionsTable');
    
    for (let instruction of patient['medicalRecord']['medicalInstructions']){
        let newRow = document.createElement('tr');

        let instructionStartDate = document.createElement('td');
        instructionStartDate.innerText = instruction['startDate'];
        newRow.appendChild(instructionStartDate);

        let instructionEndDate = document.createElement('td');
        instructionEndDate.innerText = instruction['endDate'];
        newRow.appendChild(instructionEndDate);

        let drugName = document.createElement('td');
        drugName.innerText = instruction['drug'];
        newRow.appendChild(drugName);

        for(let doctor of doctors){
            if (doctor['id'] == instruction['doctor']){
                let instructionOfDoctor = document.createElement('td');
                instructionOfDoctor.innerText = doctor['firstName'] + ' ' + doctor['lastName'];
                newRow.appendChild(instructionOfDoctor);
            }
        }
        
        table.appendChild(newRow);

    }
}

function displayInstructions(doctors){
    let table = document.getElementById('instructionsTable');
    
    for (let instruction of patient['medicalRecord']['medicalInstructions']){
        let newRow = document.createElement('tr');

        let instructionDate = document.createElement('td');
        instructionDate.innerText = instruction['date'];
        newRow.appendChild(instructionDate);

        for(let doctor of doctors){
            if (doctor['id'] == instruction['doctor']){
                let instructionOfDoctor = document.createElement('td');
                instructionOfDoctor.innerText = doctor['firstName'] + ' ' + doctor['lastName'];
                newRow.appendChild(instructionOfDoctor);
            }
        }
        
        table.appendChild(newRow);

    }
}


function displayReferrals(){
    let table = document.getElementById('referralsTable');
    
    for (let referral of patient['medicalRecord']['referrals']){
        let newRow = document.createElement('tr');

        let anyConst = 'ANY ';

        let doctorId = document.createElement('td');
        let doctorSpeciality = document.createElement('td');

        let referralBtnContainer = document.createElement('td');
        let referralBtn = document.createElement('button');
        referralBtn.innerHTML = '<i data-feather="paperclip"></i>';
        referralBtn.classList.add('referralBtn');
        referralBtnContainer.classList.add('smallerWidth');
        referralBtnContainer.appendChild(referralBtn);


        if (referral['doctorId'] == null){
            doctorId.innerText = anyConst;
            doctorSpeciality.innerText = referral['speciality'];

            newRow.appendChild(doctorId);
            newRow.appendChild(doctorSpeciality);
            newRow.appendChild(referralBtnContainer);
            table.appendChild(newRow);
        }
        else{
        doctorId.innerText = referral['doctorId'];
        doctorSpeciality.innerText = anyConst;


        newRow.appendChild(doctorId);
        newRow.appendChild(doctorSpeciality);
        newRow.appendChild(referralBtnContainer);

        table.appendChild(newRow);
        



        }

        feather.replace();

    }
}

async function checkPatientActivity(patientId){
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                let activityValue = JSON.parse(this.responseText);
                
                if (activityValue == ''){
                    return true;
                }
                else{
                    return false;
                }
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/Secretary/patients/'+patientId+'/activity');
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
    await request;

}


function setUpPatientInstructions(){
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                let doctors = JSON.parse(this.responseText);
                displayInstructions(doctors);
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/my/users/doctors');
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}



document.addEventListener('DOMContentLoaded', function () {
    setupPatientBasicInfo();
});

function setUpMenu() {
    
    let menu = document.getElementById('mainMenu');
    menu.innerHTML = `
    <li id='option1' class='navbar__item'>
        <a href='#' class='navbar__link'><i data-feather='chevron-left'></i><span>Back</span></a>
    </li>
    `;
    feather.replace();

    let item1 = document.getElementById('option1');

    if (doctorId != undefined){
        item1.addEventListener('click', (e) => {
            window.location.replace('doctor.php' + '?id=' + doctorId + '&token=' + jwtoken);
        });
    }
    else if(secretaryId != undefined){
        let referralSection = document.getElementById('referralSection');
        referralSection.classList.remove('off');
        displayReferrals();

        item1.addEventListener('click', (e) => {
            window.location.replace('secretary.php' + '?id=' + secretaryId + '&token=' + jwtoken);
        });
    }
}