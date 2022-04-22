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
let patientId = getParamValue("patient_id");

window.addEventListener("load", function () {

    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                
                patient = JSON.parse(this.responseText);

                let patientFName = document.getElementById("patient_fname");
                patientFName.innerText = patient["firstName"];
                let patientLName = document.getElementById("patient_lname");
                patientLName.innerText = patient["lastName"];
                let patientHeight = document.getElementById("patient_height");
                patientHeight.innerText = patient["medicalRecord"]["height"];
                let patientWeight = document.getElementById("patient_weight");
                patientWeight.innerText = patient["medicalRecord"]["weight"];
                let patientBlood = document.getElementById("patient_blood");
                patientBlood.innerText = patient["medicalRecord"]["bloodType"];
                let patientDiseases = document.getElementById("patient_diseases");
                patientDiseases.innerText = patient["medicalRecord"]["diseases"];
                let patientAlergies = document.getElementById("patient_alergies");
                patientAlergies.innerText = patient["medicalRecord"]["alergies"];
                
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/doctor/examinations/patient_medical_card/' + patientId);
    request.send();
});