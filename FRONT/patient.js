var mainResponse;

function setUpExaminations() {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                var response = JSON.parse(this.responseText);
                let table = document.getElementById('examinationTable');
                table.innerHTML = '';
                for (let i in response) {

                    let examination = response[i];
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
                    if (examination['type'] == 'visit' && examinationDate > today) {
                        one.appendChild(delBtn);
                        two.appendChild(putBtn);
                    }

                    newRow.appendChild(cType)
                    newRow.appendChild(cDoctor);
                    newRow.appendChild(cSpecialization);
                    newRow.appendChild(cDate);
                    newRow.appendChild(cRoom);
                    newRow.appendChild(cUrgen);
                    newRow.appendChild(one);
                    newRow.appendChild(two);
                    table.appendChild(newRow);
                    feather.replace();
                }
            }
        }
    }

    request.open('GET', url + 'api/examination/patient/' + userId);
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}

function setUpFunctionality() {
    setUpDoctors('empty');
    setUpExaminations();
    setUpMedicalRecord();
    doctorOptions('doctorCreateExamination');
    doctorOptions('doctorEditExamination');
    doctorOptions('doctorAdvancedCreateExamination');
    setUpSearchExaminations('empty');
    setUpDrugs();
}

function setUpSearchExaminations(myFilter) {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                var response = JSON.parse(this.responseText);
                let table = document.getElementById('searchExaminationTable');
                table.innerHTML = '';
                for (let i in response) {
                    let examination = response[i];
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

    request.open('GET', url + 'api/examination/patient/' + userId);
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}
//POST - Examination

let createBtn = document.getElementById('addBtn');
createBtn.addEventListener('click', function (e) {
    createExamination(0);
});

function createExamination(doctorId) {
    let prompt = document.getElementById('createExaminationPrompt');
    prompt.classList.remove('off');
    main.classList.add('hideMain');
    let form = document.getElementById('createExaminationForm');

    if (doctorId != 0) {
        let doctor = document.getElementById('doctorCreateExamination');
        let selectedDoctor = document.getElementById(doctorId);
        doctor.value = selectedDoctor.value;
    }

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
        postRequest.open('POST', url + 'api/examination/patient');
        postRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
        postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
        postRequest.send(JSON.stringify({ 'done': false, 'date': examinationDate, 'duration': 15, 'room': '', 'patient': userId, 'doctor': doctor, 'urgent': false, 'type': 'visit', 'anamnesis': '' }));
    });
};


let advancedForm = document.getElementById('createAdvancedExaminationForm');
advancedForm.addEventListener('submit', function (e) {
    e.preventDefault();
    let dueDate = document.getElementById('dueDate').value;
    let doctor = document.getElementById('doctorAdvancedCreateExamination').value;
    let intervalBegin = document.getElementById('timeFrom').value;
    let intervalEnd = document.getElementById('timeTo').value;
    let priority = document.querySelector('input[name="priority"]:checked').value;

    let postRequest = new XMLHttpRequest();

    postRequest.onreadystatechange = function () {
        if (this.readyState == 4) {

            if (this.status == 200) {
                var response = JSON.parse(this.responseText);
                let table = document.getElementById('advancedExaminationTable');
                table.innerHTML = '';
                for (let i in response) {
                    let examination = response[i];
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

                    let one = document.createElement('td');
                    let choseBtn = document.createElement('button');
                    choseBtn.innerHTML = '<i data-feather="check"></i>';
                    choseBtn.classList.add('updateBtn');
                    choseBtn.setAttribute('key', examination);
                    choseBtn.addEventListener('click', function (e) {
                        createAdvancedExamination(examination);
                    });

                    newRow.appendChild(cType)
                    newRow.appendChild(cDoctor);
                    newRow.appendChild(cSpecialization);
                    newRow.appendChild(cDate);
                    one.appendChild(choseBtn);
                    newRow.appendChild(one);
                    table.appendChild(newRow);
                    feather.replace();
                }
            }
        }
    };
    postRequest.open('POST', url + 'api/examination/filter');
    postRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
    postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    postRequest.send(JSON.stringify({ 'dueDate': dueDate.toLocaleString(), 'doctor': parseInt(doctor), 'patient': parseInt(user.id), 'timeFrom': intervalBegin.toString(), 'timeTo': intervalEnd.toString(), 'priority': priority }));
});

function createAdvancedExamination(examination) {
    let postRequest = new XMLHttpRequest();
    postRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert('Examination sucessfuly created');
                setUpExaminations();
                setUpSearchExaminations('empty');

                let table = document.getElementById('advancedExaminationTable');
                while (table.hasChildNodes()) {
                    table.removeChild(table.firstChild);
                }
            } else {
                alert('Error: Entered examination informations are invalid');
            }
        }
    };
    postRequest.open('POST', url + 'api/examination/patient');
    postRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
    postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    postRequest.send(JSON.stringify({ 'done': false, 'date': examination['date'], 'duration': 15, 'room': '', 'patient': user.id, 'doctor': examination['doctor'], 'urgent': false, 'type': 'visit', 'anamnesis': '' }));

};


//PUT - Examination    
function editExamination(editId) {
    let prompt = document.getElementById('editExaminationPrompt');
    prompt.classList.remove('off');
    main.classList.add('hideMain');
    let form = document.getElementById('editExaminationForm');

    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                var oldExamination = JSON.parse(this.responseText);

                let examinationDate = document.getElementById('timeEditExamination');
                examinationDate.value = oldExamination['date']
                let doctor = document.getElementById('doctorEditExamination');
                let selectedDoctor = document.getElementById(oldExamination['doctor']);
                doctor.value = selectedDoctor.value;

            }
        }
    }
    request.open('GET', url + 'api/examination/' + editId);
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();

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
        putRequest.open('PUT', url + 'api/examination/patient/' + editId);
        putRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
        putRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
        putRequest.send(JSON.stringify({ 'done': false, 'date': examinationDate, 'duration': 15, 'room': '', 'patient': userId, 'doctor': doctor, 'urgent': false, 'type': 'visit', 'anamnesis': '' }));
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

    deleteRequest.open('DELETE', url + 'api/examination/patient/' + key);
    deleteRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    deleteRequest.send();
}

function doctorOptions(elementID) {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                var response = JSON.parse(this.responseText);
                let options = document.getElementById(elementID);
                for (var i in response) {
                    let doctor = response[i];
                    let newOption = document.createElement('option');
                    newOption.id = doctor['id'];
                    newOption.value = doctor['id'];
                    newOption.innerText = doctor['firstName'] + ' ' + doctor['lastName'];

                    options.appendChild(newOption);
                }
            }
        }
    }

    request.open('GET', url + 'api/user/doctors');
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}

//GET - Doctors
function setUpDoctors(myFilter) {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                var response = JSON.parse(this.responseText);
                let table = document.getElementById('doctorsTable');
                table.innerHTML = '';
                for (let i in response) {
                    let doctor = response[i];
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
                        if (!(doctorsName.toLowerCase().includes(filterValue.toLowerCase()) || doctor['specialization'].includes(filterValue.toLowerCase()))) {
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
                    for (var j = 0; j < 1; j++) {
                        total += parseInt(doctor['score'][j]['efficiency']);
                        total += parseInt(doctor['score'][j]['expertise']);
                        total += parseInt(doctor['score'][j]['communicativeness']);
                        total += parseInt(doctor['score'][j]['kindness']);
                    }
                    cScore.innerText = total / (4 * 1);

                    let one = document.createElement('td');
                    let createBtn = document.createElement('button');
                    createBtn.innerHTML = '<i data-feather="file-plus"></i>';
                    createBtn.classList.add('updateBtn');
                    createBtn.setAttribute('key', doctor['id']);
                    createBtn.addEventListener('click', function (e) {
                        createExamination(this.getAttribute('key'));
                    });
                    one.appendChild(createBtn);

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

    request.open('GET', url + 'api/user/doctors');
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}

function setUpMedicalRecord() {
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
                for (let disease of currentMedicalRecord['diseases']) {
                    let diseaseItem = document.createElement('option');
                    diseaseItem.innerText = disease;
                    patientDiseases.appendChild(diseaseItem);
                }
                let patientAlergies = document.getElementById('alergiesList');
                for (let alergie of currentMedicalRecord['alergies']) {
                    let alergieItem = document.createElement('option');
                    alergieItem.innerText = alergie;
                    patientAlergies.appendChild(alergieItem);
                }
            }
        }
    }
    getMedicalRecordRequest.open('GET', url + 'api/medicalcard/' + userId);
    getMedicalRecordRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    getMedicalRecordRequest.send();
}

function setUpDrugs() {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                var response = JSON.parse(this.responseText);
                let table = document.getElementById('medicalInstructions');
                table.innerHTML = '';
                for (let i in response) {

                    let instruction = response[i];
                    let newRow = document.createElement('tr');

                    let cDrug = document.createElement('td');
                    cDrug.innerText = instruction['drug'];

                    let cFrom = document.createElement('td');
                    var instructionFrom = new Date(instruction['startDate']);
                    cFrom.innerText = instructionFrom.toLocaleString();
                    
                    let cTo = document.createElement('td');
                    var instructionTo = new Date(instruction['endDate']);
                    cTo.innerText = instructionTo.toLocaleString();

                    let one = document.createElement('td');
                    let putBtn = document.createElement('button');

                    putBtn.innerHTML = '<i data-feather="file-text"></i>';
                    putBtn.classList.add('putBtn');
                    putBtn.setAttribute('key', instruction['drug']);
                    putBtn.addEventListener('click', function (e) {
                        drugInstruction(this.getAttribute('key'), instructionTo);
                    });

                    one.appendChild(putBtn);
                        
                    newRow.appendChild(cDrug)
                    newRow.appendChild(cFrom);
                    newRow.appendChild(cTo);
                    newRow.appendChild(one);
                    table.appendChild(newRow);
                    feather.replace();
                }
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/medicalrecord/prescription/' + userId);
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}

function drugInstruction(drug, endDate){
    let prompt = document.getElementById('drugInstructionPrompt');
    prompt.classList.remove('off');
    main.classList.add('hideMain');
    let form = document.getElementById('drugInstructionForm');


    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                var response = JSON.parse(this.responseText);
                let cDrug = document.getElementById('drug');
                cDrug.innerText = 'Drug: ' + `${drug}`;
                    
                let time = document.getElementById('when');
                time.innerText = 'Time: ' + `${response['when']}`;

                let frequency = document.getElementById('frequency');
                frequency.innerText  = 'Frequency: ' + `${response['frequency']}`;

                let note = document.getElementById('note');
                note.innerText  ='Note: ' +  `${response['how']}`;      
                }
            }
        }
    request.open('GET', 'https://localhost:7291/api/medicalrecord/prescription/'+ drug + '/' + userId);
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();

    form.addEventListener('submit', function (e) {  
        prompt.classList.add('off');
        main.classList.remove('hideMain');
        e.preventDefault();
        e.stopImmediatePropagation();
        
        let time = document.getElementById('notifyTime').value;
        if (time == ""){
            alert('Error: Insert time!');
            return;
        }
    
        let postRequest = new XMLHttpRequest();
    
        postRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                        alert('Notification sucessfuly created');
                    } else {
                        alert('Error: Entered notification informations are invalid');
                    }
                }
            };
            postRequest.open('POST', 'https://localhost:7291/api/drug/notifications');
            postRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
            postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
            postRequest.send(JSON.stringify({ 'drug': drug, 'time': time , 'patient': user.email, 'endDate': endDate}));
      

     });
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

function sortTable(n, tableName) {

    var table, rows, switching, i, x, y, compareA, compareB, shouldSwitch, dir, switchcount = 0;
    table = document.getElementById(tableName);
    switching = true;
    dir = 'asc';
    while (switching) {
        switching = false;
        rows = table.rows;
        for (i = 1; i < (rows.length - 1); i++) {

            shouldSwitch = false;
            x = rows[i].getElementsByTagName('TD')[n];
            y = rows[i + 1].getElementsByTagName('TD')[n];

            if (parseInt(n) == 3 && tableName == 'searchExaminations') {
                compareA = Date.parse(x.innerHTML);
                compareB = Date.parse(y.innerHTML);
            } else {
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
            switchcount++;
        } else {
            if (switchcount == 0 && dir == 'asc') {
                dir = 'desc';
                switching = true;
            }
        }
    }
}