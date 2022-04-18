class User {
    constructor(data) {
        this.id = data["id"]
        this.firstName = data["firstName"];
        this.lastName = data["lastName"];
        this.email = data["email"];
        this.role = data["role"];
        if (this.role == "doctor") {
            this.specialization = data["specialization"];
            this.score = data["score"];
            this.freeDays = data["freeDays"];
            this.examinations = data["examinations"];
        } else if (this.role == "patient") {
            this.medicalRecord = data["medicalRecord"];
        }
    }
}
var user;

function showWindow(section) {
    let s1 = document.getElementById("one");

    s1.classList.remove("active");

    switch (section) {
        case 1: s1.classList.add("active"); break;
    }
}

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

var main = document.getElementsByTagName("main")[0];
var id = getParamValue('id');

function setUpMenu() {
    let menu = document.getElementById("mainMenu");
    menu.innerHTML += `
    <li id="option1" class="navbar__item">
        <a href="#" class="navbar__link"><i data-feather="user"></i><span>Patient Managment</span></a>
    </li>
    `;
    feather.replace();

    let item1 = document.getElementById("option1");

    item1.addEventListener('click', (e) => {
        showWindow(1);
    });
}

var mainResponse;
function setUpPatients() {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                mainResponse = JSON.parse(this.responseText);
                let table = document.getElementById("patientTable");
                table.innerHTML = "";
                for (let i in mainResponse) {
                    let patient = mainResponse[i];
                    let newRow = document.createElement("tr");

                    let pName = document.createElement("td");
                    pName.innerText = patient["firstName"];
                    let pSurname = document.createElement("td");
                    pSurname.innerText = patient["lastName"];
                    let pEmail = document.createElement("td");
                    pEmail.innerText = patient["email"];
                    let pPassword = document.createElement("td");
                    pPassword.innerText = patient["password"];
                    let pId = document.createElement("td");
                    pId.innerText = patient["id"];
                    let pMedRecord = document.createElement("td");
                    let recordBtn = document.createElement("button");
                    recordBtn.innerHTML = '<i data-feather="file-text"></i>';;
                    recordBtn.classList.add("recordBtn");
                    pMedRecord.appendChild(recordBtn);

                    let one = document.createElement("td");
                    let delBtn = document.createElement("button");
                    delBtn.innerHTML = '<i data-feather="trash"></i>';
                    delBtn.classList.add("delBtn");
                    one.appendChild(delBtn);

                    let two = document.createElement("td");
                    let putBtn = document.createElement("button");
                    putBtn.innerHTML = '<i data-feather="edit-2"></i>';
                    putBtn.classList.add("updateBtn");
                    two.appendChild(putBtn);

                    let three = document.createElement("td");
                    let blockBtn = document.createElement("button");
                    blockBtn.innerHTML = '<i data-feather="x-octagon"></i>';;
                    blockBtn.classList.add("blockBtn");
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
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/secretary/patients');
    request.send();
}

function setUpPage() {
    let hi = document.getElementById("hi");
    hi.innerText += user.firstName + " " + user.lastName;
    setUpPatients();
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