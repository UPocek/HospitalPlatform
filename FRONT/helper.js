// Classes
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

// Helper functions
function getParamValue(name) {
    let location = decodeURI(window.location.toString());
    let index = location.indexOf('?') + 1;
    let subs = location.substring(index, location.length);
    let splitted = subs.split('&');

    for (let item of splitted) {
        let s = item.split('=');
        let pName = s[0];
        let pValue = s[1];
        if (pName == name)
            return pValue;
    }
}

function setUpMenu(text1, text2, text3, text4, icon1, icon2, icon3, icon4) {
    let menu = document.getElementById('mainMenu');
    menu.innerHTML += `
    <li id='option1' class='navbar__item'>
        <a class='navbar__link'><i data-feather='${icon1}'></i><span>${text1}</span></a>
    </li>
    <li id='option2' class='navbar__item'>
        <a class='navbar__link'><i data-feather='${icon2}'></i><span>${text2}</span></a>
    </li>
    <li id='option3' class='navbar__item'>
        <a class='navbar__link'><i data-feather='${icon3}'></i><span>${text3}</span></a>
    </li>
    <li id='option4' class='navbar__item'>
        <a class='navbar__link'><i data-feather='${icon4}'></i><span>${text4}</span></a>
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

function setUpPage() {
    let hi = document.querySelector('#hi h1');
    hi.innerText += `${user.firstName} ${user.lastName}`;
}

function isDateFormatOk(testDate) {
    return /^2[0-9]{3}-([0][1-9]|1[0-2])-([0][1-9]|[1-2]\d|3[01])$/.test(testDate);
}

function areDatesValid(from, to) {
    return (isDateFormatOk(from) && isDateFormatOk(to) && from >= date.toISOString().split('T')[0] && to > from);
}

function showWindow(section) {
    let sectionOne = document.getElementById('one');
    let sectionTwo = document.getElementById('two');
    let sectionThree = document.getElementById('three');
    let sectionFour = document.getElementById('four');

    sectionOne.classList.remove('active');
    sectionTwo.classList.remove('active');
    sectionThree.classList.remove('active');
    sectionFour.classList.remove('active');

    switch (section) {
        case 1: sectionOne.classList.add('active'); break;
        case 2: sectionTwo.classList.add('active'); break;
        case 3: sectionThree.classList.add('active'); break;
        case 4: sectionFour.classList.add('active'); break;
    }
}

function removeAllChildNodes(parent) {
    try {
        while (parent.firstChild) {
            parent.removeChild(parent.firstChild);
        }
    } catch {
        ;
    }
}

document.addEventListener('DOMContentLoaded', function () {
    let request = new XMLHttpRequest();

    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                let response = JSON.parse(this.responseText);
                user = new User(response);
                setUpPage();
                if (user['role'] == 'manager') {
                    setUpMenu('Room Management', 'Equipment Management', 'Drug Management', 'Polls', 'log-in', 'tool', 'shield', 'file-text');
                    setUpRooms();
                }
                else if (user['role'] == 'secretary') {
                    setUpMenu('Patient Managment', 'Blocked Patients', 'Examination requests', 'Polls', 'user', 'user-x', 'inbox', 'file-text');
                    setUpPatients();
                }
                else if (user['role'] == 'patient') {
                    setUpMenu('Examinations', 'Profile', 'Doctors', 'Polls', 'activity', 'user', 'users', 'file-text');
                    setUpFunctionality();
                }
                else if (user['role'] == 'doctor') {
                    setUpMenu('All examinations', 'Schedule', 'Drug Review', 'Free days', 'archive', 'calendar', 'shield', 'briefcase');
                    setUpExaminations();
                }
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/my/users/' + userId);
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
});

// Global variables
var user;
var main = document.getElementsByTagName('main')[0];
var userId = getParamValue('id');
var jwtoken = getParamValue('token');
var date = new Date();
var doctors = new Map();

var doctorsExaminations;