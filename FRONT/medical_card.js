function getParamValue(name) {
    var location = decodeURI(window.location.toString());
    var index = location.indexOf("?") + 1;
    var subs = location.substring(index, location.length);
    var splitted = subs.split("&");

    for (var i = 0; i < splitted.length; i++) {
        var s = splitted[i].split("=");
        var pName = s[0];
        var pValue = s[1];
        if (pName == name)
            return pValue;
    }
};

var patient;
var patientId = getParamValue("patientId");
var patientsExaminations;

function setupPatientBasicInfo(){
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                
                patient = JSON.parse(this.responseText);

                let patientFName = document.getElementById("patientFName");
                patientFName.innerText = patient["firstName"];
                let patientLName = document.getElementById("patientLName");
                patientLName.innerText = patient["lastName"];
                let patientHeight = document.getElementById("patientHeight");
                patientHeight.innerText = patient["medicalRecord"]["height"];
                let patientWeight = document.getElementById("patientWeight");
                patientWeight.innerText = patient["medicalRecord"]["weight"];
                let patientBlood = document.getElementById("patientBlood");
                patientBlood.innerText = patient["medicalRecord"]["bloodType"];
                let patientDiseases = document.getElementById("diseasesList");
                for (disease of patient["medicalRecord"]['diseases']){
                    let diseaseItem = document.createElement('li');
                    diseaseItem.innerText = disease;
                    patientDiseases.appendChild(diseaseItem);
                }
                let patientAlergies = document.getElementById("alergiesList");
                for (alergie of patient["medicalRecord"]['alergies']){
                    let alergieItem = document.createElement('li');
                    alergieItem.innerText = alergie;
                    patientAlergies.appendChild(alergieItem);
                }
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/doctor/examinations/patientMedicalCard/' + patientId);
    request.send();
}

function displayExaminations(){
    let table = document.getElementById("examinationsTable");
    table.innerHTML = "";
    for (let examination of patientsExaminations) {

        let newRow = document.createElement("tr");

        let examinationDate = document.createElement("td");
        examinationDate.innerText = (new Date(examination["date"])).toLocaleString();
        let examinationDone = document.createElement("td");
        examinationDone.innerText = examination["done"];
        let examinationDuration = document.createElement("td");
        examinationDuration.innerText = examination["duration"];
        let examinationRoom = document.createElement("td");
        examinationRoom.innerText = examination["room"];
        let examinationType = document.createElement("td");
        examinationType.innerText = examination["type"];
        let isUrgent = document.createElement("td");
        isUrgent.innerText = examination["urgent"];

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
    request.send();
}

function displayInstructions(doctors){
    let table = document.getElementById("instructionsTable");
    
    for (instruction of patient["medicalRecord"]["medicalInstructions"]){
        let newRow = document.createElement("tr");

        let instructionDate = document.createElement("td");
        instructionDate.innerText = instruction["date"];
        newRow.appendChild(instructionDate);

        for(doctor of doctors){
            if (doctor["id"] == instruction['doctor']){
                let instructionOfDoctor = document.createElement("td");
                instructionOfDoctor.innerText = doctor["firstName"] + " " + doctor["lastName"];
                newRow.appendChild(instructionOfDoctor);
            }
        }
        
        table.appendChild(newRow);

    }
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
    request.send();
}


window.addEventListener("load", function () {
    setUpMenu();
    setupPatientBasicInfo();
    setUpPatientExaminations();
    setUpPatientInstructions();
});

function setUpMenu() {
    
    let menu = document.getElementById("mainMenu");
    menu.innerHTML = `
    <li id="option1" class="navbar__item">
        <a href="#" class="navbar__link"><i data-feather="chevron-left"></i><span>Back</span></a>
    </li>
    `;
    feather.replace();

    let item1 = document.getElementById("option1");

    item1.addEventListener('click', (e) => {
        
    });
}