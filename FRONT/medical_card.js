var patient;
var patientActivity;
var patientId = getParamValue('patientId');
var doctorId = getParamValue('doctorId');
var secretaryId = getParamValue('secretaryId');
var patientsExaminations;

function setupPatientBasicInfo() {
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
                patientDiseases.innerHTML = '';
                for (let disease of patient['medicalRecord']['diseases']) {
                    let diseaseItem = document.createElement('li');
                    diseaseItem.innerText = disease;
                    patientDiseases.appendChild(diseaseItem);
                }
                let patientAlergies = document.getElementById('alergiesList');
                patientAlergies.innerHTML = '';
                for (let alergie of patient['medicalRecord']['alergies']) {
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

function displayExaminations() {
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

function setUpPatientExaminations() {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                patientsExaminations = JSON.parse(this.responseText);
                displayExaminations();
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/examination/patient/' + patientId);
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}

function displayInstructions(doctors) {
    let table = document.getElementById('instructionsTable');

    for (let instruction of patient['medicalRecord']['medicalInstructions']) {
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

        for (let doctor of doctors) {
            if (doctor['id'] == instruction['doctor']) {
                let instructionOfDoctor = document.createElement('td');
                instructionOfDoctor.innerText = doctor['firstName'] + ' ' + doctor['lastName'];
                newRow.appendChild(instructionOfDoctor);
            }
        }

        table.appendChild(newRow);

    }
}

function displayReferrals() {
    let table = document.getElementById('referralsTable');

    table.innerHTML = ''

    for (let referral of patient['medicalRecord']['referrals']) {
        let newRow = document.createElement('tr');

        let anyConst = 'ANY ';

        let doctorId = document.createElement('td');
        let doctorSpeciality = document.createElement('td');
        let referralBtnContainer = document.createElement('td');
        let referralBtn = document.createElement('button');
        referralBtn.setAttribute('id', referral['referralId']);

        let request = new XMLHttpRequest();
        request.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    var response = JSON.parse(this.responseText);
                    patientActivity = response['active'];

                    if (referral['doctorId'] == null) {
                        doctorId.innerText = anyConst;
                        doctorSpeciality.innerText = referral['speciality'];

                        referralBtn.setAttribute('key', referral['speciality']);
                        referralBtn.addEventListener('click', function (e) {
                            createRefferedExaminationBySpeciality(this.getAttribute('key'), this.getAttribute('id'));
                        });

                        newRow.appendChild(doctorId);
                        newRow.appendChild(doctorSpeciality);

                        table.appendChild(newRow);
                    }

                    else {
                        doctorId.innerText = referral['doctorId'];
                        doctorSpeciality.innerText = anyConst;

                        referralBtn.setAttribute('key', referral['doctorId']);
                        referralBtn.addEventListener('click', function (e) {
                            createRefferedExaminationByDoctorId(this.getAttribute('key'), this.getAttribute('id'));
                        });

                        newRow.appendChild(doctorId);
                        newRow.appendChild(doctorSpeciality);

                        table.appendChild(newRow);

                    }


                    referralBtnContainer = document.createElement('td');
                    referralBtn.innerHTML = '<i data-feather="paperclip"></i>';
                    referralBtn.classList.add('referralBtn');
                    referralBtnContainer.classList.add('smallerWidth');
                    referralBtnContainer.appendChild(referralBtn);

                    newRow.appendChild(referralBtnContainer);

                    feather.replace();
                }
            }
        }

        request.open('GET', 'https://localhost:7291/api/Secretary/patients/' + patientId);
        request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
        request.send();

    }
}

function createRefferedExaminationByDoctorId(doctorid, referralid) {
    let getRequest = new XMLHttpRequest();
    getRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                rooms = JSON.parse(this.responseText);

                let prompt = document.getElementById('examinationRefPopUp');
                prompt.classList.remove('off');

                let main = document.getElementById('medCardMain');
                main.classList.add('hideMain');

                let eType = document.getElementById('examinationRefType');

                let eRoom = document.getElementById('examinationRefRoom');

                let form = document.getElementById('examinationRefForm');

                form.addEventListener('submit', function (e) {
                    submitDoctorIdForm(e, doctorid, referralid)
                });

                addOptions(eType, eRoom);
                eType.addEventListener('change', function (e) {
                    removeAllChildNodes(eRoom);
                    addOptions(eType, eRoom);
                })

            }
        }
    }
    getRequest.open('GET', 'https://localhost:7291/api/manager/rooms');
    getRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    getRequest.send();

}


function createRefferedExaminationBySpeciality(doctorSpeciality, referralid) {
    let getRequest = new XMLHttpRequest();
    getRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                rooms = JSON.parse(this.responseText);

                let prompt = document.getElementById('examinationRefPopUp');
                prompt.classList.remove('off');

                let main = document.getElementById('medCardMain');
                main.classList.add('hideMain');

                let eType = document.getElementById('examinationRefType');

                let eRoom = document.getElementById('examinationRefRoom');

                let form = document.getElementById('examinationRefForm');

                form.addEventListener('submit', function (e) {
                    submitSpecialityForm(e, doctorSpeciality, referralid)
                });

                addOptions(eType, eRoom);
                eType.addEventListener('change', function (e) {
                    removeAllChildNodes(eRoom);
                    addOptions(eType, eRoom);
                })

            }
        }
    }
    getRequest.open('GET', 'https://localhost:7291/api/manager/rooms');
    getRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    getRequest.send();

}


function submitDoctorIdForm(e, doctorid, referralid) {
    let popUp = document.getElementById('examinationRefPopUp');
    let main = document.getElementById('medCardMain');
    popUp.classList.add('off');
    main.classList.remove('hideMain');
    e.preventDefault();
    e.stopImmediatePropagation();

    let selectedType = document.getElementById('examinationRefType').value;
    let selectedDuration = document.getElementById('examinationRefDuration').value;


    let postRequest = new XMLHttpRequest();

    postRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert('Examination sucessfuly created');
                setupPatientBasicInfo();
            } else {
                alert('Error: Entered examination informations are invalid');
            }
        }
    };

    let selectedRoom = document.getElementById('examinationRefRoom').value;

    postRequest.open('POST', 'https://localhost:7291/api/secretary/examination/referral/create/none/' + referralid);
    postRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
    postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    postRequest.send(JSON.stringify({ 'done': false, 'date': "", 'duration': selectedDuration, 'room': selectedRoom, 'patient': patientId, 'doctor': doctorid, 'urgent': false, 'type': selectedType, 'anamnesis': '' }));
}


function submitSpecialityForm(e, speciality, referralid) {
    let popUp = document.getElementById('examinationRefPopUp');
    let main = document.getElementById('medCardMain');
    popUp.classList.add('off');
    main.classList.remove('hideMain');
    e.preventDefault();
    e.stopImmediatePropagation();

    let selectedType = document.getElementById('examinationRefType').value;
    let selectedDuration = document.getElementById('examinationRefDuration').value;


    let postRequest = new XMLHttpRequest();

    postRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert('Examination sucessfuly created');
                setupPatientBasicInfo();
            } else {
                alert('Error: Entered examination informations are invalid');
            }
        }
    };

    let selectedRoom = document.getElementById('examinationRefRoom').value;

    postRequest.open('POST', 'https://localhost:7291/api/secretary/examination/referral/create/' + speciality + '/' + referralid);
    postRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
    postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    postRequest.send(JSON.stringify({ 'done': false, 'date': "", 'duration': selectedDuration, 'room': selectedRoom, 'patient': patientId, 'doctor': -1, 'urgent': false, 'type': selectedType, 'anamnesis': '' }));
}


function addOptions(element, roomOptions) {
    let valueOfType = element.value;
    if (valueOfType == 'visit') {
        for (let room of rooms) {
            if (room['type'] == 'examination room') {
                let newOption = document.createElement('option');
                newOption.setAttribute('value', room['name']);
                newOption.innerText = room['name'];
                roomOptions.appendChild(newOption);
            }
        }
    } else {
        for (let room of rooms) {
            if (room['type'] == 'operation room') {
                let newOption = document.createElement('option');
                newOption.setAttribute('value', room['name']);
                newOption.innerText = room['name'];
                roomOptions.appendChild(newOption);
            }
        }
    }
}

function setUpPatientInstructions() {
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

    if (doctorId != undefined) {
        item1.addEventListener('click', (e) => {
            window.location.replace('doctor.php' + '?id=' + doctorId + '&token=' + jwtoken);
        });
    }
    else if (secretaryId != undefined) {
        let referralSection = document.getElementById('referralSection');
        referralSection.classList.remove('off');
        displayReferrals();

        item1.addEventListener('click', (e) => {
            window.location.replace('secretary.php' + '?id=' + secretaryId + '&token=' + jwtoken);
        });
    }
}