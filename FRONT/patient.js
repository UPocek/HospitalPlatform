// Globals
class User {
    constructor(data) {
        this.id = data['id'];
        this.firstName = data['firstName'];
        this.lastName = data['lastName'];
        this.email = data['email'];
        this.role = data['role'];
        if (this.role == 'doctor') {
            this.specialization = data['specialization'];
            this.score = data['score'];
            this.freeDays = data['freeDays'];
            this.examinations = data['examinations'];
        } else if (this.role == 'patient') {
            this.medicalRecord = data['medicalRecord'];
        }
    }
}
var user;

// Helpers
function getParamValue(name) {
    let location = decodeURI(window.location.toString());
    let index = location.indexOf('?') + 1;
    let subs = location.substring(index, location.length);
    let splitted = subs.split('&');

    for (let i = 0; i < splitted.length; i++) {
        let s = splitted[i].split('=');
        let pName = s[0];
        let pValue = s[1];
        if (pName == name)
            return pValue;
    }
}

var doctors = new Map();
// function getDoctor(doctorID){
//     let request = new XMLHttpRequest();

//     request.onreadystatechange = function () {
//         if (this.readyState == 4) {
//             if (this.status == 200) {
//                 let response = JSON.parse(this.responseText);
//                 return new User(response);
//             }
//         }
//     }

//     request.open('GET', 'https://localhost:7291/api/my/users/' + doctorID);
//     request.send();
// }

// Main

document.addEventListener('DOMContentLoaded', function () {
    let request = new XMLHttpRequest();

    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                let response = JSON.parse(this.responseText);
                user = new User(response);
                setUpMenu();
                setUpPage();
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/my/users/' + id);
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
});


function showWindow(section) {
    let s1 = document.getElementById('one');
    let s2 = document.getElementById('two');
    let s3 = document.getElementById('three');
    let s4 = document.getElementById('four');

    s1.classList.remove('active');
    s2.classList.remove('active');
    s3.classList.remove('active');
    s4.classList.remove('active');

    switch (section) {
        case 1: s1.classList.add('active'); break;
        case 2: s2.classList.add('active'); break;
        case 3: s3.classList.add('active'); break;
        case 4: s4.classList.add('active'); break;
    }
}

var main = document.getElementsByTagName('main')[0];
var jwtoken = getParamValue('token');
var id = getParamValue('id');
var date = new Date();

function setUpMenu() {
    let menu = document.getElementById('mainMenu');
    menu.innerHTML += `
    <li id='option1' class='navbar__item'>
        <a href='#' class='navbar__link'><i data-feather='activity'></i><span>Examinations</span></a>
    </li>
    <li id='option2' class='navbar__item'>
        <a href='#' class='navbar__link'><i data-feather='user'></i><span>Profile</span></a>
    </li>
    <li id='option3' class='navbar__item'>
        <a href='#' class='navbar__link'><i data-feather='users'></i><span>Doctors</span></a>
    </li>
    <li id='option4' class='navbar__item'>
        <a href='#' class='navbar__link'><i data-feather='file-text'></i><span>Polls</span></a>
    </li>
    `;
    feather.replace();

    let item1 = document.getElementById('option1');
    let item2 = document.getElementById('option2');
    let item3 = document.getElementById('option3');
    let item4 = document.getElementById('option4');

    item1.addEventListener('click', (e) => {
        showWindow(1);
    });
    item2.addEventListener('click', (e) => {
        showWindow(2);
    });
    item3.addEventListener('click', (e) => {
        showWindow(3);
    });
    item4.addEventListener('click', (e) => {
        showWindow(4);
    });
}

function setUpPage() {
    let hi = document.querySelector('#hi h1');
    hi.innerText += `${user.firstName} ${user.lastName}`;

    setUpFunctionality();
}


function setUpFunctionality() {
    setUpDoctors('empty');
    setUpExaminations();
    setUpMedicalRecord();
    doctorOptions('doctorCreateExamination');
    doctorOptions('doctorEditExamination');
    setUpSearchExaminations('empty');

}

var mainResponse;
function setUpExaminations() {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                mainResponse = JSON.parse(this.responseText);
                let table = document.getElementById('examinationTable');
                table.innerHTML = '';
                for (let i in mainResponse) {

                    let examination = mainResponse[i];
                    let newRow = document.createElement('tr');

                    let cType = document.createElement('td');
                    cType.innerText = examination['type'];

                    var doctor = doctors.get(examination['doctor']);
                    let cDoctor = document.createElement('td');
                    cDoctor.innerText = doctor.firstName + " " + doctor.lastName;
                    let cSpecialization = document.createElement('td');
                    cSpecialization.innerText = doctor.specialization;

                    let cDate = document.createElement('td');
                    var examinationDate = new Date(examination['date']);
                    cDate.innerText = examinationDate.toLocaleString();
                    let cRoom = document.createElement('td');
                    cRoom.innerText = examination['room'];

                    // let cAnamnesis = document.createElement('td');
                    // let anamnesisBtn = document.createElement('button');
                    // anamnesisBtn.innerHTML = '<i data-feather="file"></i>';
                    // anamnesisBtn.classList.add('updateBtn');
                    // anamnesisBtn.setAttribute('key', examination['anamnesis']);
                    // anamnesisBtn.addEventListener('click', function (e) {
                    // });
                    // cAnamnesis.appendChild(anamnesisBtn);


                    let cUrgen = document.createElement('td');
                    cUrgen.innerText = examination['urgent']

                    let one = document.createElement('td');
                    let delBtn = document.createElement('button');
                    
                    delBtn.innerHTML = '<i data-feather="trash"></i>';
                    delBtn.classList.add('delBtn');
                    delBtn.setAttribute('key', examination['id']);
                    delBtn.addEventListener('click', function (e) {
                        deleteExamination(this.getAttribute('key'));
                    });

                    let two = document.createElement('td');
                    let putBtn = document.createElement('button');
                    putBtn.innerHTML = '<i data-feather="edit-2"></i>';
                    putBtn.classList.add('updateBtn');
                    putBtn.setAttribute('key', examination['id']);
                    putBtn.addEventListener('click', function (e) {
                        editExamination(this.getAttribute('key'));
                    });
                    var today = new Date();
                    if(examination['type'] == 'visit' && examinationDate > today){
                        one.appendChild(delBtn);
                        two.appendChild(putBtn);

                    }

                    newRow.appendChild(cType)
                    newRow.appendChild(cDoctor);
                    newRow.appendChild(cSpecialization);
                    newRow.appendChild(cDate);
                    newRow.appendChild(cRoom);
                    // newRow.appendChild(cAnamnesis);
                    newRow.appendChild(cUrgen);
                    newRow.appendChild(one);
                    newRow.appendChild(two);
                    table.appendChild(newRow);
                    feather.replace();
                }
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/patient/examinations/' + id);
    //postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}


function setUpSearchExaminations(myFilter) {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                mainResponse = JSON.parse(this.responseText);
                let table = document.getElementById('searchExaminationTable');
                table.innerHTML = '';
                for (let i in mainResponse) {
                    let examination = mainResponse[i];
                    var doctor = doctors.get(examination['doctor']);

                    if (myFilter.includes('term')) {
                        let tokens = myFilter.split('&');
                        let filterValue;
                        for (let token of tokens) {
                            if (token.includes('term')) {
                                filterValue = token.split('|')[1];
                                filterValue.toLowerCase();
                                break;
                            }
                        }
                        doctorsName = doctor['firstName'] + ' ' + doctor['lastName'];

                        if (!(examination['type'].includes(filterValue) || doctorsName.toLowerCase().includes(filterValue.toLowerCase()) || doctor.specialization.includes(filterValue) || (new Date(examination['date'])).toLocaleString().includes(filterValue) || examination['anamnesis'].includes(filterValue) || (examination['urgent'].toString()).includes(filterValue))) {
                            continue;
                        }
                    }



                    let newRow = document.createElement('tr');

                    let cType = document.createElement('td');
                    cType.innerText = examination['type'];
                    

                    let cDoctor = document.createElement('td');
                    cDoctor.innerText = doctor.firstName + " " + doctor.lastName;
                    let cSpecialization = document.createElement('td');
                    cSpecialization.innerText = doctor.specialization;

                    let cDate = document.createElement('td');
                    cDate.innerText = (new Date(examination['date'])).toLocaleString();
                    let cAnamnesis = document.createElement('td');
                    cAnamnesis.innerText = examination['anamnesis'];
                    let cUrgen = document.createElement('td');
                    cUrgen.innerText = examination['urgent']


                    newRow.appendChild(cType)
                    newRow.appendChild(cDoctor);
                    newRow.appendChild(cSpecialization);
                    newRow.appendChild(cDate);
                    newRow.appendChild(cAnamnesis);
                    newRow.appendChild(cUrgen);
                    table.appendChild(newRow);
                }
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/patient/examinations/' + id);
    request.send();
}
//POST - Examination

let createBtn = document.getElementById('addBtn');
createBtn.addEventListener('click', function (e) {
    let prompt = document.getElementById('createExaminationPrompt');
    prompt.classList.remove('off');
    main.classList.add('hideMain');

    let form = document.getElementById('createExaminationForm');

    form.addEventListener('submit', function (e) {
        prompt.classList.add('off');
        main.classList.remove('hideMain');
        e.preventDefault();
        e.stopImmediatePropagation();
        let examinationDate = document.getElementById('timeCreateExamination').value;      
        let doctor = document.getElementById('doctorCreateExamination').value;
   
        let postRequest = new XMLHttpRequest();

        postRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    alert('Examination sucessfuly created');
                    setUpExaminations();
                    setUpSearchExaminations('empty');
                } else {
                    alert('Error: Entered examination informations are invalid');
                }
            }
        };
        postRequest.open('POST', 'https://localhost:7291/api/patient/examinations');
        postRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
        postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
        postRequest.send(JSON.stringify({'done':false, 'date': examinationDate, 'duration': 15 ,'room': '', 'patient': user.id, 'doctor': doctor, 'urgent': false, 'type': 'visit', 'anamnesis':''}));       
   

    });

});


//PUT - Examination    
function editExamination(id){ 
    let prompt = document.getElementById('editExaminationPrompt');
    prompt.classList.remove('off');
    main.classList.add('hideMain');
    let form = document.getElementById('editExaminationForm');

    form.addEventListener('submit', function (e) {
        prompt.classList.add('off');
        main.classList.remove('hideMain');
        e.preventDefault();
        e.stopImmediatePropagation();
        let examinationDate = document.getElementById('timeEditExamination').value;      
        let doctor = document.getElementById('doctorEditExamination').value;

        let putRequest = new XMLHttpRequest();

        putRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    alert('Examination sucessfuly updated');
                    setUpExaminations();
                    setUpSearchExaminations('empty');

                } else {
                    alert('Error: Entered examination informations are invalid');
                }
            }
        };
        putRequest.open('PUT', 'https://localhost:7291/api/patient/examinations/'+ id);
        putRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
        putRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
        putRequest.send(JSON.stringify({'done':false, 'date': examinationDate, 'duration': 15 ,'room': '', 'patient': user.id, 'doctor': doctor, 'urgent': false, 'type': 'visit', 'anamnesis':''}));       
    });
}; 

//DELETE - Examination
function deleteExamination(key) {
    let deleteRequest = new XMLHttpRequest();

    deleteRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert('Selected examination was successfully deleted');
                setUpExaminations();
                setUpSearchExaminations('empty');
            } else {
                alert("Error: Selected examination couldn't be deleted");
            }
        }
    }

    deleteRequest.open('DELETE', 'https://localhost:7291/api/patient/examinations/' + key)
    deleteRequest.send();
}


function doctorOptions(elementID){
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                mainResponse = JSON.parse(this.responseText);
                let options = document.getElementById(elementID);
                for (var i in mainResponse) {
                    let doctor = mainResponse[i];
                    let newOption = document.createElement('option');
                    newOption.value = doctor['id'];
                    newOption.innerText = doctor['firstName'] + ' ' + doctor['lastName'];
  
                    options.appendChild(newOption);
                }
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/patient/doctors');
    request.send();
}

//GET - Doctors
function setUpDoctors(myFilter) {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                mainResponse = JSON.parse(this.responseText);
                let table = document.getElementById('doctorsTable');
                table.innerHTML = '';
                for (let i in mainResponse) {
                    let doctor = mainResponse[i];

                    var doc = new User(doctor);
                    doctors.set(doc.id, doc);

                    if (myFilter.includes('term')) {
                        let tokens = myFilter.split('&');
                        let filterValue;
                        for (let token of tokens) {
                            if (token.includes('term')) {
                                filterValue = token.split('|')[1];
                                break;
                            }
                        }
                        doctorsName = doctor['firstName'] + ' ' + doctor['lastName'];
                        if (!(doctorsName.toLowerCase().includes(filterValue.toLowerCase())  || doctor['specialization'].includes(filterValue.toLowerCase()))) {
                            continue;
                        }
                    }




                    let newRow = document.createElement('tr');
                    let cName = document.createElement('td');
                    cName.innerText = doctor['firstName'] + ' ' + doctor['lastName'];
                    let cSpecialization = document.createElement('td');
                    cSpecialization.innerText = doctor['specialization'];
                    let cMail = document.createElement('td');
                    cMail.innerText = doctor['email'];
                    let cScore = document.createElement('td');
                    var total = 0;
                    for (var j = 0; j < 1; j++){
                        total += parseInt(doctor['score'][j]['efficiency']);
                        total += parseInt(doctor['score'][j]['expertise']);
                        total += parseInt(doctor['score'][j]['communicativeness']);
                        total += parseInt(doctor['score'][j]['kindness']);
                    }
                    cScore.innerText = total/(4*1);
                    

                    let one = document.createElement('td');
                    let putBtn = document.createElement('button');
                    putBtn.innerHTML = '<i data-feather="file-plus"></i>';
                    putBtn.classList.add('updateBtn');
                    putBtn.setAttribute('key', doctor['id']);
                    putBtn.addEventListener('click', function (e) {
                        updateRoom(this.getAttribute('key'));
                    });
                    one.appendChild(putBtn);

                    newRow.appendChild(cName)
                    newRow.appendChild(cSpecialization);
                    newRow.appendChild(cMail);
                    newRow.appendChild(cScore);
                    newRow.appendChild(one);
                    table.appendChild(newRow);
                    feather.replace();
                    
                }
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/patient/doctors');
    request.send();
}

function setUpMedicalRecord(){
     let getMedicalRecordRequest = new XMLHttpRequest();
        
        getMedicalRecordRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    let patient = JSON.parse(this.responseText);
                    currentMedicalRecord = patient['medicalRecord'];
                    
                    let patientFName = document.getElementById('patientFName');
                    patientFName.setAttribute('id', 'patientId')
                    patientFName.setAttribute('key', patient['id']);
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
                    for (let disease of currentMedicalRecord['diseases']){
                        let diseaseItem = document.createElement('option');
                        diseaseItem.innerText = disease;
                        patientDiseases.appendChild(diseaseItem);
                    }
                    let patientAlergies = document.getElementById('alergiesList');
                    for (let alergie of currentMedicalRecord['alergies']){
                        let alergieItem = document.createElement('option');
                        alergieItem.innerText = alergie;
                        patientAlergies.appendChild(alergieItem);
                    }
                }
            }
        }
        getMedicalRecordRequest.open('GET', 'https://localhost:7291/api/doctor/examinations/patientMedicalCard/' + user.id);
        getMedicalRecordRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
        getMedicalRecordRequest.send();
}


var examinationSearchFilter = document.getElementById('examinationSearch');
examinationSearchFilter.addEventListener('input', updateExaminationTable);

function updateExaminationTable(e) {
    e.preventDefault();

    let finalFilter = '';
    let filter = examinationSearchFilter.value;

    if (filter) {
        finalFilter += `term|${filter}&`;
    }
    if (finalFilter.endsWith('&')) {
        finalFilter = finalFilter.slice(0, -1);
    }
    if (!finalFilter) {
        finalFilter = 'empty';
    }

    setUpSearchExaminations(finalFilter);
}


var doctorSearchFilter = document.getElementById('doctorSearch');
doctorSearchFilter.addEventListener('input', updateDoctorsTable);

function updateDoctorsTable(e) {
    e.preventDefault();

    let finalFilter = '';
    let filter = doctorSearchFilter.value;

    if (filter) {
        finalFilter += `term|${filter}&`;
    }
    if (finalFilter.endsWith('&')) {
        finalFilter = finalFilter.slice(0, -1);
    }
    if (!finalFilter) {
        finalFilter = 'empty';
    }

    setUpDoctors(finalFilter);
}


function sortTable(n, table){
    var table, rows, switching, i, x, y, compareA, compareB, shouldSwitch, dir, switchcount = 0;
    table = document.getElementById(table);
    switching = true;
    dir = 'asc';
    while (switching) {
        switching = false;
        rows = table.rows;
        for (i = 1; i < (rows.length - 1); i++) {
            shouldSwitch = false;
            x = rows[i].getElementsByTagName('TD')[n];
            y = rows[i + 1].getElementsByTagName('TD')[n];

            if(n==3 && table == 'searchExaminations'){
                compareA = Date.parse(x.innerHTML);
                console.log(compareA);
                compareB = Date.parse(y.innerHTML);

            }else{
                compareA = x.innerHTML.toLowerCase();
                compareB = y.innerHTML.toLowerCase();
            }

            if (dir == 'asc') {
                if (compareA > compareB) {
                    shouldSwitch = true;
                    break;
                }
            } else if (dir == 'desc') {
                if (compareA < compareB) {
                    shouldSwitch = true;
                    break;
                }
            }
        }
        if (shouldSwitch) {
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
            switchcount ++;
            } else {
                if (switchcount == 0 && dir == 'asc') {
                    dir = 'desc';
                    switching = true;
                }
            }
    }

}