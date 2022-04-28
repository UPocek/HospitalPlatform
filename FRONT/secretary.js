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

    sectionOne.classList.remove('active');
    sectionTwo.classList.remove('active');

    switch (section) {
        case 1: sectionOne.classList.add('active'); break;
        case 2: sectionTwo.classList.add('active'); break;
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
    `;
    feather.replace();

    let menuItem1 = document.getElementById("option1");
    let menuItem2 = document.getElementById("option2");

    menuItem1.addEventListener('click', (e) => {
        showWindow(1);
    });
    menuItem2.addEventListener('click', (e) => {
        showWindow(2);
    });
}

function setUpFunctionality() {
    setUpBlockedPatients();
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

                    let one = document.createElement('td');
                    one.classList.add('smallerWidth');
                    let delBtn = document.createElement('button');
                    delBtn.innerHTML = '<i data-feather="trash"></i>';
                    delBtn.classList.add('delBtn');
                    delBtn.setAttribute('key', patient['id']);
                    delBtn.addEventListener('click', function (e) {
                        deletePatient(this.getAttribute('key'));
                    });
                    one.appendChild(delBtn);

                    let two = document.createElement('td');
                    two.classList.add('smallerWidth');
                    let putBtn = document.createElement('button');
                    putBtn.innerHTML = '<i data-feather="edit-2"></i>';
                    putBtn.classList.add('updateBtn');
                    putBtn.setAttribute('key', patient['id']);
                    putBtn.addEventListener('click', function (e) {
                        updatePatient(this.getAttribute('key'));
                    });
                    two.appendChild(putBtn);

                    let three = document.createElement('td');
                    three.classList.add('smallerWidth');
                    let blockBtn = document.createElement('button');
                    blockBtn.innerHTML = '<i data-feather="x-octagon"></i>';;
                    blockBtn.classList.add('blockBtn');
                    blockBtn.setAttribute('key', patient['id']);
                    blockBtn.addEventListener('click', function (e) {
                        blockPatient(this.getAttribute('key'));
                    });
                    three.appendChild(blockBtn);

                    newRow.appendChild(pName);
                    newRow.appendChild(pSurname);
                    newRow.appendChild(pEmail);
                    newRow.appendChild(pPassword);
                    newRow.appendChild(pId);
                    newRow.appendChild(pMedRecord);
                    newRow.appendChild(one);
                    newRow.appendChild(two);
                    newRow.appendChild(three);
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
                    recordBtn.innerHTML = '<i data-feather="file-text"></i>';;
                    recordBtn.classList.add('recordBtn');
                    pMedRecord.appendChild(recordBtn);

                    let pBlockedBy = document.createElement('td');
                    if (patient['active'] == "1"){
                        pBlockedBy.innerText = 'SECRETARY'
                    }
                    else if(patient['active'] == "2"){
                        pBlockedBy.innerText = 'SYSTEM';
                    }

                    let one = document.createElement("td");
                    one.classList.add('smallerWidth');
                    let unblockBtn = document.createElement("button");
                    unblockBtn.innerHTML = '<i data-feather="user-check"></i>';
                    unblockBtn.classList.add('unblockBtn');
                    unblockBtn.setAttribute('key', patient['id']);
                    unblockBtn.addEventListener('click', function (e) {
                        unblockPatient(this.getAttribute('key'));
                    });
                    one.appendChild(unblockBtn);

                    newRow.appendChild(pName);
                    newRow.appendChild(pSurname);
                    newRow.appendChild(pEmail);
                    newRow.appendChild(pPassword);
                    newRow.appendChild(pId);
                    newRow.appendChild(pMedRecord);
                    newRow.appendChild(pBlockedBy);
                    newRow.appendChild(one);
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
                let fHeight = document.getElementById('editPatientHeight');

                let medRecord = patient['medicalRecord'];
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
    let postRequest = new XMLHttpRequest();
    postRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert("Patient sucessfuly blocked");
                setUpPatients();
            }else{
                alert(this.responseText);
            }
            
        }
    }
    postRequest.open('PUT', 'https://localhost:7291/api/secretary/patients/block/'+key+"/1");
    postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    postRequest.send();
}

function unblockPatient(key){
    let postRequest = new XMLHttpRequest();
    postRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert("Patient sucessfuly unblocked");
                setUpPatients();
            }else{
                alert(this.responseText);
            }
            
        }
    }
    postRequest.open('PUT', 'https://localhost:7291/api/secretary/patients/block/'+key+"/0");
    postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    postRequest.send();
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