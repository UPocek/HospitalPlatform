class User {
    constructor(data) {
        this.id = data['id']
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

function showWindow(section) {
    let sectionOne = document.getElementById('one');
    let sectionTwo = document.getElementById('two');
    let sectionThree = document.getElementById('three');
    let sectionFour = document.getElementById('four')

    sectionOne.classList.remove('active');
    sectionTwo.classList.remove('active');
    sectionThree.classList.remove('active');
    sectionFour.classList.remove('active');

    let hi = document.getElementById('hi');

    switch (section) {
        case 1: 
                sectionOne.classList.add('active'); 
                hi.classList.remove('off');
                hi.classList.add('active');
                break;
        case 2: 
                sectionTwo.classList.add('active'); 
                hi.classList.remove('off');
                hi.classList.add('active'); 
                break;
        case 3: 
                sectionThree.classList.add('active'); 
                hi.classList.remove('off');
                hi.classList.add('active');
                break;
        case 4: 
                hi.classList.remove('active');
                hi.classList.add('off')
                sectionFour.classList.add('active'); 
                break;
    }
}

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

var main = document.getElementsByTagName("main")[0];
var secretaryId = getParamValue('id');
var jwtoken = getParamValue('token');

function setUpMenu() {
    let menu = document.getElementById("mainMenu");
    menu.innerHTML += `
    <li id="option1" class="navbar__item">
        <a class="navbar__link"><i data-feather="user"></i><span>Patient Managment</span></a>
    </li>
    <li id="option2" class="navbar__item">
        <a class="navbar__link"><i data-feather="user-x"></i><span>Blocked Patients</span></a>
    </li>
    <li id="option3" class="navbar__item">
        <a class="navbar__link"><i data-feather="inbox"></i><span>Examination requests</span></a>
    </li>
    <li id="option4" class="navbar__item">
        <a class="navbar__link"><i data-feather="alert-triangle"></i><span>Urgent examination</span></a>
    </li>
    `;
    feather.replace();

    let menuItem1 = document.getElementById('option1');
    let menuItem2 = document.getElementById('option2');
    let menuItem3 = document.getElementById('option3');
    let menuItem4 = document.getElementById('option4');

    menuItem1.addEventListener('click', (e) => {
        showWindow(1);
    });
    menuItem2.addEventListener('click', (e) => {
        showWindow(2);
    });
    menuItem3.addEventListener('click', (e) => {
        showWindow(3);
    });
    menuItem4.addEventListener('click', (e) => {
        showWindow(4);
    });
}

function setUpFunctionality() {
    setUpBlockedPatients();
    setupExaminationRequests()
}

var mainResponse;
function setUpPatients() {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                mainResponse = JSON.parse(this.responseText);
                let table = document.getElementById('patientTable');
                table.innerHTML = '';
                for (let i in mainResponse) {
                    let patient = mainResponse[i];
                    let newRow = document.createElement('tr');

                    let pName = document.createElement('td');
                    pName.innerText = patient['firstName'];

                    let pSurname = document.createElement('td');
                    pSurname.innerText = patient['lastName'];

                    let pEmail = document.createElement('td');
                    pEmail.innerText = patient['email'];

                    let pPassword = document.createElement('td');
                    pPassword.innerText = patient['password'];

                    let pId = document.createElement('td');
                    pId.innerText = patient['id'];

                    let pMedRecord = document.createElement('td');

                    let recordBtn = document.createElement('button');
                    recordBtn.innerHTML = '<i data-feather="user"></i>';
                    recordBtn.setAttribute('key', patient['id']);
                    recordBtn.addEventListener('click', function (e) {
                        window.location.replace('patientMedicalCard.php' + '?patientId=' + recordBtn.getAttribute('key') + '&token=' + jwtoken + '&secretaryId=' + secretaryId);
                    });

                    pMedRecord.appendChild(recordBtn);

                    let delBtnContainer = document.createElement('td');
                    delBtnContainer.classList.add('smallerWidth');

                    let delBtn = document.createElement('button');
                    delBtn.innerHTML = '<i data-feather="trash"></i>';
                    delBtn.classList.add('delBtn');
                    delBtn.setAttribute('key', patient['id']);
                    delBtn.addEventListener('click', function (e) {
                        deletePatient(this.getAttribute('key'));
                    });

                    delBtnContainer.appendChild(delBtn);

                    let putBtnContainer = document.createElement('td');
                    putBtnContainer.classList.add('smallerWidth');

                    let putBtn = document.createElement('button');
                    putBtn.innerHTML = '<i data-feather="edit-2"></i>';
                    putBtn.classList.add('updateBtn');
                    putBtn.setAttribute('key', patient['id']);
                    putBtn.addEventListener('click', function (e) {
                        updatePatient(this.getAttribute('key'));
                    });

                    putBtnContainer.appendChild(putBtn);

                    let blockBtnContainer = document.createElement('td');
                    blockBtnContainer.classList.add('smallerWidth');

                    let blockBtn = document.createElement('button');
                    blockBtn.innerHTML = '<i data-feather="x-octagon"></i>';;
                    blockBtn.classList.add('blockBtn');
                    blockBtn.setAttribute('key', patient['id']);
                    blockBtn.addEventListener('click', function (e) {
                        blockPatient(this.getAttribute('key'));
                    });

                    blockBtnContainer.appendChild(blockBtn);

                    newRow.appendChild(pName);
                    newRow.appendChild(pSurname);
                    newRow.appendChild(pEmail);
                    newRow.appendChild(pPassword);
                    newRow.appendChild(pId);
                    newRow.appendChild(pMedRecord);
                    newRow.appendChild(delBtnContainer);
                    newRow.appendChild(putBtnContainer);
                    newRow.appendChild(blockBtnContainer);
                    table.appendChild(newRow);
                    feather.replace();
                }
                setUpFunctionality();
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/secretary/patients');
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}

function setUpBlockedPatients(){
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                mainResponse = JSON.parse(this.responseText);
                let table = document.getElementById('blockedPatientTable');
                table.innerHTML = '';
                for (let i in mainResponse) {
                    let patient = mainResponse[i];
                    let newRow = document.createElement('tr');

                    let pName = document.createElement('td');
                    pName.innerText = patient['firstName'];

                    let pSurname = document.createElement('td');
                    pSurname.innerText = patient['lastName'];

                    let pEmail = document.createElement('td');
                    pEmail.innerText = patient['email'];

                    let pPassword = document.createElement('td');
                    pPassword.innerText = patient['password'];

                    let pId = document.createElement('td');
                    pId.innerText = patient['id'];

                    let pMedRecord = document.createElement('td');

                    let recordBtn = document.createElement('button');

                    recordBtn.innerHTML = '<i data-feather="user"></i>';
                    recordBtn.setAttribute('key', patient['id']);
                    recordBtn.addEventListener('click', function (e) {
                        window.location.replace('patientMedicalCard.php' + '?patientId=' + recordBtn.getAttribute('key') + '&token=' + jwtoken + '&secretaryId=' + secretaryId);
                    });

                    pMedRecord.appendChild(recordBtn);

                    let pBlockedBy = document.createElement('td');

                    if (patient['active'] == "1"){
                        pBlockedBy.innerText = 'SECRETARY'
                    }
                    else if(patient['active'] == "2"){
                        pBlockedBy.innerText = 'SYSTEM';
                    }

                    let unblockBtnContainer = document.createElement("td");
                    unblockBtnContainer.classList.add('smallerWidth');

                    let unblockBtn = document.createElement("button");
                    unblockBtn.innerHTML = '<i data-feather="user-check"></i>';
                    unblockBtn.classList.add('unblockBtn');
                    unblockBtn.setAttribute('key', patient['id']);
                    unblockBtn.addEventListener('click', function (e) {
                        unblockPatient(this.getAttribute('key'),e);
                    });

                    unblockBtnContainer.appendChild(unblockBtn);

                    let delBtnContainer = document.createElement("td");
                    delBtnContainer.classList.add('smallerWidth');

                    let delBtn = document.createElement("button");
                    delBtn.innerHTML = '<i data-feather="trash"></i>';
                    delBtn.classList.add('delBtn');
                    delBtn.setAttribute('key', patient['id']);
                    delBtn.addEventListener('click', function (e) {
                        deletePatient(this.getAttribute('key'));
                    });

                    delBtnContainer.appendChild(delBtn);

                    newRow.appendChild(pName);
                    newRow.appendChild(pSurname);
                    newRow.appendChild(pEmail);
                    newRow.appendChild(pPassword);
                    newRow.appendChild(pId);
                    newRow.appendChild(pMedRecord);
                    newRow.appendChild(pBlockedBy);
                    newRow.appendChild(unblockBtnContainer);
                    newRow.appendChild(delBtnContainer);
                    table.appendChild(newRow);
                    feather.replace();
                }
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/secretary/patients/blocked');
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}


function setupExaminationRequests() {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                mainResponse = JSON.parse(this.responseText);
                let table = document.getElementById('examinationRequestTable');
                table.innerHTML = '';
                for (let i in mainResponse) {
                    let examinationRequest = mainResponse[i];

                    let examination = examinationRequest['examination']

                    let newRow = document.createElement('tr');

                    let examinationType = document.createElement('td');
                    examinationType.classList.add("examinationType")
                    examinationType.innerText = examination['type'];

                    let examinationDoctor = document.createElement('td');
                    examinationDoctor.classList.add("examinationDoctor")
                    examinationDoctor.innerText = examination['doctor'];

                    let examinationDate = document.createElement('td');
                    examinationDate.classList.add("examinationDate")
                    examinationDate.innerText = examination['date'];

                    let examinationRoom = document.createElement('td');
                    examinationRoom.classList.add("examinationRoom")
                    examinationRoom.innerText = examination['room'];

                    let examinationPatient = document.createElement('td');
                    examinationPatient.classList.add("examinationPatient")
                    examinationPatient.innerText = examination['patient'];

                    let examinationChange = document.createElement('td');

                    let oldExaminationContainer  = document.createElement('td');

                    if (examinationRequest['status'] == '0'){
                        examinationChange.innerText = 'DELETION'
                    }
                    else{
                        examinationChange.innerText = 'MODIFICATION'
                        let oldExamination = document.createElement('button');
                        oldExamination.innerHTML = '<i data-feather="arrow-up"></i>';
                        oldExamination.classList.add('showBtn');
                        oldExaminationContainer.classList.add('showBtnContainer')
                        oldExamination.addEventListener('click', function (e) {
                        showOldExamination(examination,newRow);
                        });
                        oldExaminationContainer.appendChild(oldExamination);
                    }

                    let acceptBtnContainer = document.createElement('td');
                    let acceptBtn = document.createElement('button');
                    acceptBtn.innerHTML = '<i data-feather="check"></i>';
                    acceptBtn.classList.add('acceptBtn');
                    acceptBtn.setAttribute('key', examinationRequest['_id']);
                    acceptBtn.addEventListener('click', function (e) {
                        acceptRequest(this.getAttribute('key'));
                    });
                    acceptBtnContainer.classList.add('smallerWidth');
                    acceptBtnContainer.appendChild(acceptBtn);

                    let declineBtnContainer = document.createElement('td');
                    let declineBtn = document.createElement('button');
                    declineBtn.innerHTML = '<i data-feather="x"></i>';
                    declineBtn.classList.add('declineBtn');
                    declineBtn.setAttribute('key', examinationRequest['_id']);
                    declineBtn.addEventListener('click', function (e) {
                        declineRequest(this.getAttribute('key'));
                    });
                    declineBtnContainer.classList.add('smallerWidth')
                    declineBtnContainer.appendChild(declineBtn);

                    oldExaminationContainer.classList.add('smallerWidth');

                    newRow.appendChild(examinationType);
                    newRow.appendChild(examinationDoctor);
                    newRow.appendChild(examinationDate);
                    newRow.appendChild(examinationRoom);
                    newRow.appendChild(examinationPatient);
                    newRow.appendChild(examinationChange);
                    newRow.appendChild(acceptBtnContainer);
                    newRow.appendChild(declineBtnContainer);
                    newRow.appendChild(oldExaminationContainer);

                    table.appendChild(newRow);

                    feather.replace();
                }
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/secretary/examinationRequests');
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}

function setUpPage() {
    let hi = document.querySelector('#hi h1');
    hi.innerText += `${user.firstName} ${user.lastName}`;
    setUpPatients();
}

function deletePatient(key) {
    let deleteRequest = new XMLHttpRequest();

    deleteRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert('Selected patient was successfully deleted');
                setUpPatients();
            } else {
                alert("Error: Selected patient couldn't be deleted");
            }
        }
    }

    deleteRequest.open('DELETE', 'https://localhost:7291/api/secretary/patients/' + key)
    deleteRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    deleteRequest.send();
}



function updatePatient(key) {
    let prompt = document.getElementById('editPatientPrompt');
    prompt.classList.remove('off');
    main.classList.add('hideMain');
    let request = new XMLHttpRequest();
    let patient;
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                patient = JSON.parse(this.responseText);
                
                let fFirstName = document.getElementById('editPatientFirstName');
                fFirstName.value = patient['firstName'];

                let fLastName = document.getElementById('editPatientLastName');
                fLastName.value = patient['lastName'];

                let fEmail = document.getElementById('editPatientEmail');
                fEmail.value = patient['email'];

                let fPassword = document.getElementById('editPatientPassword');
                fPassword.value = patient['password'];

                let medRecord = patient['medicalRecord'];

                let fHeight = document.getElementById('editPatientHeight');
                fHeight.value = medRecord['height'];

                let fWeight = document.getElementById('editPatientWeight');
                fWeight.value = medRecord['weight'];

                let fBloodType = document.getElementById('editPatientBloodType');
                fBloodType.value = medRecord['bloodType'];  

                let form = document.getElementById('editPatientForm');

                form.addEventListener('submit', function (e) {
                    prompt.classList.add('off');
                    main.classList.remove('hideMain');
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    let postRequest = new XMLHttpRequest();

                    postRequest.onreadystatechange = function () {
                        if (this.readyState == 4) {
                            if (this.status == 200) {
                                alert('Patient sucessfuly updated');
                                setUpPatients();
                            }else{
                                if (this.responseText == ''){
                                    alert('Error: Entered patient information is invalid');
                                }
                                else{
                                    alert(this.responseText);
                                }
                            }
                            
                        }
                    }

                    let finalName = document.getElementById('editPatientFirstName').value;
                    let finalLastName = document.getElementById('editPatientLastName').value;
                    let finalEmail = document.getElementById('editPatientEmail').value;
                    let finalPassword = document.getElementById('editPatientPassword').value;
                    let finalHeight = document.getElementById('editPatientHeight').value;
                    let finalWeight = document.getElementById('editPatientWeight').value;
                    let finalBloodType = document.getElementById('editPatientBloodType').value

                    if (finalName.length == 0 || finalLastName.length == 0) {
                        alert("Error: Name can't be empty!");
                    } else if(/\S+@\S+\.\S+/.test(toString(finalEmail))) {
                        alert('Error: Email in wrong format! (example:markomarkovic@gmail.com)');
                    } else if(finalPassword.length == 0){
                        alert("Error: Password can't be empty!");
                    } else {
                        postRequest.open('PUT', 'https://localhost:7291/api/secretary/patients/'+key);
                        postRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
                        postRequest.send(JSON.stringify(
                            { 
                            'firstName': finalName, 
                            'lastName': finalLastName,
                            'role': 'patient',
                            'email': finalEmail,
                            'password': finalPassword,
                            'active' : '0',
                            'id' : 0,
                            'medicalRecord':
                                {
                                    'height': finalHeight,
                                    'weight': finalWeight,
                                    'bloodType': finalBloodType,
                                },
                            }
                        ));
                    }
                });
            }
        }
    }
    request.open('GET', 'https://localhost:7291/api/secretary/patients/'+key);
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();

    
    
}


//POST Patient
let createBtn = document.getElementById('addBtn');
createBtn.addEventListener('click', function (e) {
    let prompt = document.getElementById('createPatientPrompt');
    prompt.classList.remove('off');
    main.classList.add('hideMain');

    let fFirstName = document.getElementById('createPatientFirstName');
    fFirstName.setAttribute('placeholder', 'First Name');

    let fLastName = document.getElementById('createPatientLastName');
    fLastName.setAttribute('placeholder', 'Last Name');

    let fEmail = document.getElementById('createPatientEmail');
    fEmail.setAttribute('placeholder', 'Email');

    let fPassword = document.getElementById('createPatientPassword');
    fPassword.setAttribute('placeholder', 'Password');

    let fHeight = document.getElementById('createPatientHeight');
    fHeight.setAttribute('placeholder', 'Height');
    
    let fWeight = document.getElementById('createPatientWeight');
    fWeight.setAttribute('placeholder', 'Weight');

    let form = document.getElementById('createPatientForm');

    form.addEventListener('submit', function (e) {
        prompt.classList.add('off');
        main.classList.remove('hideMain');
        e.preventDefault();
        e.stopImmediatePropagation();
        let postRequest = new XMLHttpRequest();

        postRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    alert('Patient sucessfuly created');
                    setUpPatients();
                }else{
                    if (this.responseText == ''){
                        alert('Error: Entered patient information is invalid');
                    }
                    else{
                        alert(this.responseText);
                    }
                }
                
            }
        }

        let finalName = document.getElementById('createPatientFirstName').value;
        let finalLastName = document.getElementById('createPatientLastName').value;
        let finalEmail = document.getElementById('createPatientEmail').value;
        let finalPassword = document.getElementById('createPatientPassword').value;
        let finalHeight = document.getElementById('createPatientHeight').value;
        let finalWeight = document.getElementById('createPatientWeight').value;
        let finalBloodType = document.getElementById('createPatientBloodType').value

        if (finalName.length == 0 || finalLastName.length == 0) {
            alert("Error: Name can't be empty!");
        } else if(/\S+@\S+\.\S+/.test(toString(finalEmail))) {
            alert('Error: Email in wrong format! (example:markomarkovic@gmail.com)');
        } else if(finalPassword.length == 0){
            alert("Error: Password can't be empty!");
        } else {
            postRequest.open('POST', 'https://localhost:7291/api/secretary/patients');
            postRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
            postRequest.send(JSON.stringify(
                { 
                'firstName': finalName, 
                'lastName': finalLastName,
                'role': 'patient',
                'email': finalEmail,
                'password': finalPassword,
                'active' : '0',
                'id' : 0,
                'medicalRecord':
                    {
                        'height': finalHeight,
                        'weight': finalWeight,
                        'bloodType': finalBloodType,
                    },
                }
            ));
        }
    });
});

function blockPatient(key){
    let putRequest = new XMLHttpRequest();
    putRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert("Patient sucessfuly blocked");
                setUpPatients();
            }else{
                alert(this.responseText);
            }
            
        }
    }
    putRequest.open('PUT', 'https://localhost:7291/api/secretary/patients/block/'+key+"/1");
    putRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    putRequest.send();
}

function unblockPatient(key,e){
    e.stopImmediatePropagation();
    e.preventDefault();
    let putRequest = new XMLHttpRequest();
    putRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert("Patient sucessfuly unblocked");
                setUpPatients();
            }else{
                alert(this.responseText);
            }
            
        }
    }
    putRequest.open('PUT', 'https://localhost:7291/api/secretary/patients/block/'+key+"/0");
    putRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    putRequest.send();
}


function acceptRequest(key){
    let putRequest = new XMLHttpRequest();
    putRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert("Examination request accepted");
                setupExaminationRequests();
            }else{
                alert(this.responseText);
            }
            
        }
    }
    putRequest.open('PUT', 'https://localhost:7291/api/secretary/examinationRequests/accept/'+key);
    putRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    putRequest.send();
}


function declineRequest(key){
    let putRequest = new XMLHttpRequest();
    putRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert("Examination request declined");
                setupExaminationRequests();
            }else{
                alert(this.responseText);
            }
            
        }
    }
    putRequest.open('PUT', 'https://localhost:7291/api/secretary/examinationRequests/decline/'+key);
    putRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    putRequest.send();
}


function showOldExamination(newExamination,examRow){
    let putRequest = new XMLHttpRequest();
    putRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                oldExamination = JSON.parse(this.responseText);

                let examinationType = examRow.getElementsByClassName('examinationType')[0];
                examinationType.innerHTML = oldExamination['type'];

                let examinationDoctor = examRow.getElementsByClassName('examinationDoctor')[0];
                examinationDoctor.innerHTML = oldExamination['doctor'];

                let examinationDate = examRow.getElementsByClassName('examinationDate')[0];
                examinationDate.innerHTML = oldExamination['date'];

                let examinationRoom = examRow.getElementsByClassName('examinationRoom')[0];
                examinationRoom.innerHTML = oldExamination['room'];

                let examinationPatient = examRow.getElementsByClassName('examinationPatient')[0];
                examinationPatient.innerHTML = oldExamination['patient'];

                let oldShowBtnContainer = examRow.getElementsByClassName("showBtnContainer")[0];
                let oldShowBtn = examRow.getElementsByClassName("showBtn")[0];

                let showBtn = document.createElement('button');
                showBtn.innerHTML = '<i data-feather="arrow-down"></i>';
                showBtn.classList.add('showBtn');
                showBtn.addEventListener('click', function (e) {
                showNewExamination(newExamination,examRow);
                });

                oldShowBtnContainer.replaceChild(showBtn,oldShowBtn);

                feather.replace();
                
            }else{
                alert(this.responseText);
            }
            
        }
    }
    putRequest.open('GET', 'https://localhost:7291/api/secretary/examination/'+newExamination['id']);
    putRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    putRequest.send();
}


function showNewExamination(newExamination,examRow){
    let examinationType = examRow.getElementsByClassName('examinationType')[0];
    examinationType.innerHTML = newExamination['type'];

    let examinationDoctor = examRow.getElementsByClassName('examinationDoctor')[0];
    examinationDoctor.innerHTML = newExamination['doctor'];

    let examinationDate = examRow.getElementsByClassName('examinationDate')[0];
    examinationDate.innerHTML = newExamination['date'];

    let examinationRoom = examRow.getElementsByClassName('examinationRoom')[0];
    examinationRoom.innerHTML = newExamination['room'];

    let examinationPatient = examRow.getElementsByClassName('examinationPatient')[0];
    examinationPatient.innerHTML = newExamination['patient'];

    let oldShowBtnContainer = examRow.getElementsByClassName("showBtnContainer")[0];
    let oldShowBtn = examRow.getElementsByClassName("showBtn")[0];

    let showBtn = document.createElement('button');
    showBtn.innerHTML = '<i data-feather="arrow-up"></i>';
    showBtn.classList.add('showBtn');
    showBtn.addEventListener('click', function (e) {
    showOldExamination(newExamination,examRow);
    });

    oldShowBtnContainer.replaceChild(showBtn,oldShowBtn);

    feather.replace();
    

}


// Main

window.addEventListener('load', function () {
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

    request.open('GET', 'https://localhost:7291/api/my/users/' + secretaryId);
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
});