// Globals
class User {
    constructor(data) {
        this.id = data["id"]
        this.firstName = data["firstName"];
        this.lastName = data["lastName"];
        this.email = data["email"];
        this.role = data["role"];
        if (this.role == "patient") {
            this.medicalRecord = data["medicalRecord"];
        }
    }
}
var user;

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

// Main

window.addEventListener("load", function () {
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
    hi.innerText += user.firstName + " " + user.lastName;

    setUpExaminations();
}


function setUpFunctionality() {
    getDoctors();
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
                    //OVO IZMENI ISPISUJE ID DOKTORA, A NE IME I PREZIME
                    cDoctor.innerText = examination["doctor"];
                    let cDate = document.createElement("td");
                    cDate.innerText = examination["date"].substring(0,10) + "\n" +  examination["date"].substring(11,19);
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
                    one.appendChild(delBtn);

                    let two = document.createElement("td");
                    let putBtn = document.createElement("button");
                    putBtn.innerHTML = '<i data-feather="edit-2"></i>';
                    putBtn.classList.add("updateBtn");
                    putBtn.setAttribute("key", examination["id"]);
                    putBtn.addEventListener('click', function (e) {
                        editExamination(this.getAttribute('key'));
                    });
                    two.appendChild(putBtn);

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
                    setUpFunctionality()
                }
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/patient/examinations/' + id);
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
        // let postRequest = new XMLHttpRequest();

        // postRequest.onreadystatechange = function () {
        //     if (this.readyState == 4) {
        //         if (this.status == 200) {
        //             alert("Examination sucessfuly created");
        //             setUpRooms();
        //         } else {
        //             alert("Error: Examination can't be created");
        //         }
        //     }
        // }

    });

});


//PUT - Examination
function editExamination(key) {
    let prompt = document.getElementById("editExaminationPrompt");
    prompt.classList.remove("off");
    main.classList.add("hideMain");

    let form = document.getElementById("editExaminationForm");

    form.addEventListener('submit', function (e) {
        prompt.classList.add("off");
        main.classList.remove("hideMain");
        e.preventDefault();
        e.stopImmediatePropagation();
        // let putRequest = new XMLHttpRequest();

        // putRequest.onreadystatechange = function () {
        //     if (this.readyState == 4) {
        //         if (this.status == 200) {
        //             alert("Selected examinaton was successfully edited");
        //             setUpRooms();
        //         } else {
        //             alert("Error: You can't edit this examination");
        //         }
        //     }
        // }

    });
}

//DELETE - Examination
function deleteExamination(key) {
    let deleteRequest = new XMLHttpRequest();

    deleteRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert("Selected examination was successfully deleted");
                setUpRooms();
            } else {
                alert("Error: Selected examination couldn't be deleted");
            }
        }
    }

    deleteRequest.open('DELETE', 'https://localhost:7291/api/patient/examination/' + key)
    deleteRequest.send();
}



//GET - Doctors
function getDoctors() {
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