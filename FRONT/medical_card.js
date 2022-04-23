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

let patient;
let patientId = getParamValue("patientId");

window.addEventListener("DOMContentLoaded", function () {
    setUpMenu();
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                
                patient = JSON.parse(this.responseText);
                console.log(patient);

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