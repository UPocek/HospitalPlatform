// Globals
class User {
    constructor(data) {
        this.id = data["id"]
        this.firstName = data["firstName"];
        this.lastName = data["lastName"];
        this.email = data["email"];
        this.role = data["role"];
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
// var doctor;

// Helpers
function getParamValue(name) {
    let location = decodeURI(window.location.toString());
    let index = location.indexOf("?") + 1;
    let subs = location.substring(index, location.length);
    let splitted = subs.split("&");

    for (let i = 0; i < splitted.length; i++) {
        let s = splitted[i].split("=");
        let pName = s[0];
        let pValue = s[1];
        if (pName == name)
            return pValue;
    }
}

// function getDoctor(doctorID){
//     let request = new XMLHttpRequest();

//     request.onreadystatechange = function () {
//         if (this.readyState == 4) {
//             if (this.status == 200) {
//                 let response = JSON.parse(this.responseText);
//                 doctor= new User(response);
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
    let s1 = document.getElementById("one");
    let s2 = document.getElementById("two");
    let s3 = document.getElementById("three");
    let s4 = document.getElementById("four");

    s1.classList.remove("active");
    s2.classList.remove("active");
    s3.classList.remove("active");
    s4.classList.remove("active");

    switch (section) {
        case 1: s1.classList.add("active"); break;
        case 2: s2.classList.add("active"); break;
        case 3: s3.classList.add("active"); break;
        case 4: s4.classList.add("active"); break;
    }
}

var main = document.getElementsByTagName("main")[0];
var jwtoken = getParamValue('token');
var id = getParamValue('id');
var date = new Date();

function setUpMenu() {
    let menu = document.getElementById("mainMenu");
    menu.innerHTML += `
    <li id="option1" class="navbar__item">
        <a href="#" class="navbar__link"><i data-feather="activity"></i><span>Examinations</span></a>
    </li>
    <li id="option2" class="navbar__item">
        <a href="#" class="navbar__link"><i data-feather="user"></i><span>Profile</span></a>
    </li>
    <li id="option3" class="navbar__item">
        <a href="#" class="navbar__link"><i data-feather="users"></i><span>Doctors</span></a>
    </li>
    <li id="option4" class="navbar__item">
        <a href="#" class="navbar__link"><i data-feather="file-text"></i><span>Polls</span></a>
    </li>
    `;
    feather.replace();

    let item1 = document.getElementById("option1");
    let item2 = document.getElementById("option2");
    let item3 = document.getElementById("option3");
    let item4 = document.getElementById("option4");

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
    let hi = document.getElementById("hi");
    hi.innerText += `${user.firstName} ${user.lastName}`;

    setUpFunctionality();
}


function setUpFunctionality() {
    setUpExaminations();
    setUpDoctors();
    doctorOptions("doctorCreateExamination");
    doctorOptions("doctorEditExamination");
    setUpSearchExaminations();

}

var mainResponse;
function setUpExaminations() {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                mainResponse = JSON.parse(this.responseText);
                let table = document.getElementById("examinationTable");
                table.innerHTML = "";
                for (let i in mainResponse) {

                    let examination = mainResponse[i];
                    let newRow = document.createElement("tr");

                    let cType = document.createElement("td");
                    cType.innerText = examination["type"];
                    let cDoctor = document.createElement("td");
                    cDoctor.innerText = examination["doctor"];
                    let cDate = document.createElement("td");
                    cDate.innerText = (new Date(examination["date"])).toLocaleString();
                    let cRoom = document.createElement("td");
                    cRoom.innerText = examination["room"];

                    let cAnamnesis = document.createElement("td");
                    let anamnesisBtn = document.createElement("button");
                    anamnesisBtn.innerHTML = '<i data-feather="file"></i>';
                    anamnesisBtn.classList.add("updateBtn");
                    anamnesisBtn.setAttribute("key", examination["anamnesis"]);
                    anamnesisBtn.addEventListener('click', function (e) {
                    });
                    cAnamnesis.appendChild(anamnesisBtn);


                    let cUrgen = document.createElement("td");
                    cUrgen.innerText = examination["urgent"]

                    let one = document.createElement("td");
                    let delBtn = document.createElement("button");
                    
                    delBtn.innerHTML = '<i data-feather="trash"></i>';
                    delBtn.classList.add("delBtn");
                    delBtn.setAttribute("key", examination["id"]);
                    delBtn.addEventListener('click', function (e) {
                        deleteExamination(this.getAttribute('key'));
                    });

                    let two = document.createElement("td");
                    let putBtn = document.createElement("button");
                    putBtn.innerHTML = '<i data-feather="edit-2"></i>';
                    putBtn.classList.add("updateBtn");
                    putBtn.setAttribute("key", examination["id"]);
                    putBtn.addEventListener('click', function (e) {
                        updateExamination(this.getAttribute('key'));
                    });

                    if(examination['type'] == 'visit'){
                        one.appendChild(delBtn);
                        two.appendChild(putBtn);

                    }

                    newRow.appendChild(cType)
                    newRow.appendChild(cDoctor);
                    newRow.appendChild(cDate);
                    newRow.appendChild(cRoom);
                    newRow.appendChild(cAnamnesis);
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

//POST - Examination

let createBtn = document.getElementById("addBtn");
createBtn.addEventListener("click", function (e) {
    let prompt = document.getElementById("createExaminationPrompt");
    prompt.classList.remove("off");
    main.classList.add("hideMain");

    let form = document.getElementById("createExaminationForm");

    form.addEventListener('submit', function (e) {
        prompt.classList.add("off");
        main.classList.remove("hideMain");
        e.preventDefault();
        e.stopImmediatePropagation();
        let examinationDate = document.getElementById("timeCreateExamination").value;      
        let doctor = document.getElementById("doctorCreateExamination").value;
   
        let postRequest = new XMLHttpRequest();

        postRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    alert("Examination sucessfuly created");
                    setUpExaminations();
                } else {
                    alert("Error: Entered examination informations are invalid");
                }
            }
        };
        postRequest.open('POST', 'https://localhost:7291/api/patient/examinations');
        postRequest.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        postRequest.send(JSON.stringify({ "done":false, "date": examinationDate, "duration": 15,"room": "", "patient": user.id, "doctor": doctor, "urgent": false, "type": "visit", "anamnesis":""}));       
   

    });

});


//PUT - Examination    
function updateExamination(key) {
    let prompt = document.getElementById("editExaminationPrompt");
    prompt.classList.remove("off");
    main.classList.add("hideMain");

    let form = document.getElementById("editExaminationForm");

    form.addEventListener('submit', function (e) {
        prompt.classList.add("off");
        main.classList.remove("hideMain");
        e.preventDefault();
        e.stopImmediatePropagation();
        let examinationDate = document.getElementById("timeEditExamination").value;      
        let doctor = document.getElementById("doctorEditExamination").value;
   
        let postRequest = new XMLHttpRequest();

        postRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    alert("Examination sucessfuly updated");
                    setUpExaminations();

                } else {
                    alert("Error: Entered examination informations are invalid");
                }
            }
        };
        postRequest.open('PUT', 'https://localhost:7291/api/patient/examinations/'+key);
        postRequest.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        postRequest.send(JSON.stringify({ "done":false, "date": examinationDate, "duration": 15,"room": "", "patient": user.id, "doctor": doctor, "urgent": false, "type": "visit", "anamnesis":""}));       
    });
   
};

//DELETE - Examination
function deleteExamination(key) {
    let deleteRequest = new XMLHttpRequest();

    deleteRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert("Selected examination was successfully deleted");
                setUpExaminations();
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
                    let newOption = document.createElement("option");
                    newOption.value = doctor['id'];
                    newOption.innerText = doctor["firstName"] + " " + doctor["lastName"];
  
                    options.appendChild(newOption);
                }
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/patient/doctors');
    request.send();
}


function setUpSearchExaminations() {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                mainResponse = JSON.parse(this.responseText);
                let table = document.getElementById("searchExaminationTable");
                table.innerHTML = "";
                for (let i in mainResponse) {

                    let examination = mainResponse[i];
                    let newRow = document.createElement("tr");

                    let cType = document.createElement("td");
                    cType.innerText = examination["type"];
                    let cDoctor = document.createElement("td");
                    cDoctor.innerText = examination["doctor"];
                    let cDate = document.createElement("td");
                    cDate.innerText = (new Date(examination["date"])).toLocaleString();
                    let cAnamnesis = document.createElement("td");
                    cAnamnesis.innerText = examination["anamnesis"];
                    let cUrgen = document.createElement("td");
                    cUrgen.innerText = examination["urgent"]


                    newRow.appendChild(cType)
                    newRow.appendChild(cDoctor);
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

//GET - Doctors
function setUpDoctors() {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                mainResponse = JSON.parse(this.responseText);
                let table = document.getElementById("doctorsTable");
                table.innerHTML = "";
                for (let i in mainResponse) {
                    let doctor = mainResponse[i];
                    let newRow = document.createElement("tr");

                    let cName = document.createElement("td");
                    cName.innerText = doctor["firstName"] + " " + doctor["lastName"];
                    let cSpecialization = document.createElement("td");
                    cSpecialization.innerText = doctor["specialization"];
                    let cMail = document.createElement("td");
                    cMail.innerText = doctor["email"];
                    

                    let one = document.createElement("td");
                    let putBtn = document.createElement("button");
                    putBtn.innerHTML = '<i data-feather="file-plus"></i>';
                    putBtn.classList.add("updateBtn");
                    putBtn.setAttribute("key", doctor["id"]);
                    putBtn.addEventListener('click', function (e) {
                        updateRoom(this.getAttribute('key'));
                    });
                    one.appendChild(putBtn);

                    newRow.appendChild(cName)
                    newRow.appendChild(cSpecialization);
                    newRow.appendChild(cMail);
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