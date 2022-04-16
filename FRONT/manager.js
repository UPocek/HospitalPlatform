// Globals
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

function setUpMenu() {
    let menu = document.getElementById("mainMenu");
    menu.innerHTML += `
    <li class="navbar__item">
            <a href="#" class="navbar__link"><i data-feather="log-in"></i><span>Room Management</span></a>
        </li>
        <li class="navbar__item">
            <a href="#" class="navbar__link"><i data-feather="tool"></i><span>Equipment Management</span></a>
        </li>
        <li class="navbar__item">
            <a href="#" class="navbar__link"><i data-feather="shield"></i><span>Drug Management</span></a>
        </li>
        <li class="navbar__item">
            <a href="#" class="navbar__link"><i data-feather="file-text"></i><span>Polls</span></a>
        </li>
    `;
    feather.replace();
}

function setUpRooms() {
    let request = new XMLHttpRequest();

    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                let response = JSON.parse(this.responseText);
                console.log(response);
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/manager/rooms');
    request.send();
}

function setUpPage() {
    let hi = document.getElementById("hi");
    hi.innerText += user.firstName + " " + user.lastName;

    setUpRooms();
}

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
}
let id = getParamValue('id');

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