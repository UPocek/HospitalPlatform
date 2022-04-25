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

//*helper functions
function removeAllChildNodes(parent) {
    try{
        while (parent.firstChild) {
            parent.removeChild(parent.firstChild);
        }
    }catch{
        ;
    }    
}

function showWindow(section) {
    let sectionOne = document.getElementById('one');
    let sectionTwo = document.getElementById('two');
    let sectionThree = document.getElementById('three');

    sectionOne.classList.remove('active');
    sectionTwo.classList.remove('active');
    sectionThree.classList.remove('active');

    switch (section) {
        case 1: sectionOne.classList.add('active'); break;
        case 2: sectionTwo.classList.add('active'); break;
        case 3: sectionThree.classList.add('active'); break;
    }
}

function getParamValue(name) {
    var location = decodeURI(window.location.toString());
    if (location[-1] == "#"){
        location = location.substring(0, location.length -1);
    }
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

var doctorsExaminations;
var doctorId = getParamValue('id');
var jwtoken = getParamValue('token');
var user;

function getDoctor(){
    let getUserRequest = new XMLHttpRequest();
    getUserRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                let doctor = JSON.parse(this.responseText);
                doctorFirstName = doctor['firstName'];
                doctorLastName = doctor['lastName'];
            }
        }
    }
    getUserRequest.open('GET', 'https://localhost:7291/api/my/users/' + doctorId);
    getUserRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    getUserRequest.send();
}

function setUpMenu() {
    
    let menu = document.getElementById("mainMenu");
    menu.innerHTML += `
    <li id="option1" class="navbar__item">
        <a class="navbar__link"><i data-feather="archive"></i><span>All examinations</span></a>
    </li>
    <li id="option2" class="navbar__item">
        <a class="navbar__link"><i data-feather="calendar"></i><span>Schedule</span></a>
    </li>
    <li id="option3" class="navbar__item">
        <a class="navbar__link"><i data-feather="briefcase"></i><span>Free days</span></a>
    </li>
    `;
    feather.replace();

    let item1 = document.getElementById("option1");
    let item2 = document.getElementById("option2");
    let item3 = document.getElementById("option3");
    
    item1.addEventListener('click', (e) => {
        showWindow(1);
        displayExaminations();
    });
    item2.addEventListener('click', (e) => {
        showWindow(2);
        document.getElementById("scheduleDate").value = (new Date()).toDateString;
        document.getElementById("scheduleDateOption").value = new Date().toISOString().split('T')[0];
        searchSchedule();
    });
    item3.addEventListener('click', (e) => {
        showWindow(3);
    });
    
}

function displayExaminations(){
    let table = document.getElementById("examinationsTable");
    table.innerHTML = "";
    for (let examination of doctorsExaminations) {

        let newRow = document.createElement("tr");

        let examinationDate = document.createElement("td");
        examinationDate.innerText = (new Date(examination["date"])).toLocaleString();
        let examinationDone = document.createElement("td");
        examinationDone.innerText = examination["done"];
        let examinationDuration = document.createElement("td");
        examinationDuration.innerText = examination["duration"];
        let examinationRoom = document.createElement("td");
        examinationRoom.innerText = examination["room"];
        let examinationType = document.createElement("td");
        examinationType.innerText = examination["type"];
        let isUrgent = document.createElement("td");
        isUrgent.innerText = examination["urgent"];

        let one = document.createElement("td");
        let patientBtn = document.createElement("button");
        patientBtn.innerHTML = '<i data-feather="user"></i>';
        patientBtn.setAttribute("key", examination["patient"]);
        patientBtn.classList.add('send');
        patientBtn.addEventListener('click', function (e) {
            window.location.replace("patientMedicalCard.php" + "?patientId=" + patientBtn.getAttribute("key") + '&token=' + jwtoken + '&doctorId=' + doctorId);
        });
        one.appendChild(patientBtn);

        let two = document.createElement("td");
        let reportBtn = document.createElement("button");
        reportBtn.innerHTML = '<i data-feather="clipboard"></i>';
        reportBtn.setAttribute("key", examination["id"]);
        reportBtn.classList.add('send');
        reportBtn.addEventListener('click', function (e) {
            reviewReport(parseInt(reportBtn.getAttribute('key')));
        });
        two.appendChild(reportBtn);

        let three = document.createElement("td");
        let delBtn = document.createElement("button");
        delBtn.innerHTML = '<i data-feather="trash"></i>';
        delBtn.classList.add("delBtn");
        delBtn.setAttribute("key", examination["id"]);
        delBtn.addEventListener('click', function(e){
            deleteExamination(delBtn.getAttribute('key'));
            });
        three.appendChild(delBtn);

        let four = document.createElement("td");
        let updateBtn = document.createElement("button");
        updateBtn.innerHTML = '<i data-feather="upload"></i>';
        updateBtn.classList.add("updateBtn");
        updateBtn.setAttribute("key", examination["id"]);
        updateBtn.addEventListener('click', function(e){ 
            updateExamination(updateBtn.getAttribute("key"));
        });
        four.appendChild(updateBtn);

        newRow.appendChild(examinationDate);
        newRow.appendChild(examinationDuration);
        newRow.appendChild(examinationDone);
        newRow.appendChild(examinationRoom);
        newRow.appendChild(examinationType);
        newRow.appendChild(isUrgent);
        newRow.appendChild(one);
        newRow.appendChild(two);
        newRow.appendChild(three);
        newRow.appendChild(four);
        table.appendChild(newRow);
        feather.replace();
    }
}

function showExaminations(){

    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                doctorsExaminations = JSON.parse(this.responseText);
                displayExaminations();
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/doctor/examinations/doctorId/' + doctorId);
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}

function setUpPage(){
    let hi = document.querySelector('#hi h1');
    hi.innerText += `${user.firstName} ${user.lastName}`;
    setUpMenu();
    showExaminations();
}

document.addEventListener('DOMContentLoaded', function () {
    let request = new XMLHttpRequest();

    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                let response = JSON.parse(this.responseText);
                user = new User(response);
                setUpPage();
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/my/users/' + doctorId);
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
});

var main = document.getElementsByTagName("main")[0];

function reviewExamination(id){
    let currentExamination;
    for (let examination of doctorsExaminations){
        if (examination["id"] == id){
            currentExamination = examination;
            break;
        }
    }

    let popUp = document.getElementById('reviewExaminationDiv');
    popUp.classList.remove("off");
    main.classList.add("hideMain");

    document.getElementById("reportDescription").innerText = currentExamination['anamnesis'];

    if (examination['type'] == "operation"){
        let equipmentDiv = document.getElementById('equipmentDiv');
        equipmentDiv.innerHTML += `<div class="listContainer">
                                        <div id= "eqDiv" class="divList">
                                            <h3>Equipment used:&nbsp</h3>
                                            <ul id="equipmentList">
                                            </ul>
                                        </div>
                                        <button class="delBtn"><i data-feather="trash"></i></button>
                                    </div>`;
        let equipmentList = document.getElementById('equipmentList');
        for (let equipment of examination['equipmentUsed']){
            let item = document.createElement('li');
            item.innerText = equipment;
            equipmentList.appendChild(item);
        }
        document.getElementById('eqDiv').appendChild(equipmentList);

        equipmentDiv.innerHTML += `<div>
                                        <input></input>
                                        <button class="add"><i data-feather="plus"></i></button>
                                    </div>`
    }

    feather.replace();
    
    let request = new XMLHttpRequest();
    
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                let patient = JSON.parse(this.responseText);
                let patientFName = document.getElementById("patientFName");
                patientFName.innerText = patient["firstName"];
                let patientLName = document.getElementById("patientLName");
                patientLName.innerText = patient["lastName"];
                let patientHeight = document.getElementById("patientHeight");
                patientHeight.value = patient["medicalRecord"]["height"];
                let patientWeight = document.getElementById("patientWeight");
                patientWeight.value = patient["medicalRecord"]["weight"];
                let patientBlood = document.getElementById("patientBlood");
                patientBlood.value = patient["medicalRecord"]["bloodType"];
                let patientDiseases = document.getElementById("diseasesList");
                for (let disease of patient["medicalRecord"]['diseases']){
                    let diseaseItem = document.createElement('li');
                    diseaseItem.innerText = disease;
                    patientDiseases.appendChild(diseaseItem);
                }
                let patientAlergies = document.getElementById("alergiesList");
                for (let alergie of patient["medicalRecord"]['alergies']){
                    let alergieItem = document.createElement('li');
                    alergieItem.innerText = alergie;
                    patientAlergies.appendChild(alergieItem);
                }
            }
        }
    }
    request.open('GET', 'https://localhost:7291/api/doctor/examinations/patientMedicalCard/' + currentExamination['patient']);
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}

var updateMedicalCardBtn = document.getElementById("updateMedicalCard");

updateMedicalCardBtn.addEventListener('click', function(e){
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
            }
        }
    }
    postRequest.open('PUT', 'https://localhost:7291/api/doctor/examinations/' + id);
        postRequest.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);

        postRequest.send(JSON.stringify({ "_id": updatedExamination["_id"], "id": updatedExamination["id"], "done":false, "date": selectedDate, "duration": selectedDuration,"room": selectedRoom, "patient": selectedPatient, "doctor": doctorId, "urgent": isUrgent, "type": selectedType, "anamnesis":""}));
    request.open('PUT', 'https://localhost:7291/api/doctor/examinations/patientMedicalCard/' + patientId);
    request.send();
})

function reviewReport(id){
    let currentExamination;
    for (let examination of doctorsExaminations){
        if (examination["id"] == id){
            currentExamination = examination;
            break;
        }
    }

    let popUp = document.getElementById('reportPopUpNew');
    popUp.classList.remove('off');
    main.classList.add('hideMain');

    if (currentExamination['anamnesis'] == ""){
        popUp.classList.add('off');
        main.classList.remove('hideMain');
        alert("No report present");
    }else{
        document.getElementById("reportDescriptionNew").innerText = currentExamination['anamnesis'];

        if (examination['type'] == "operation"){
            let equipmentDiv = document.getElementById('reportEquipmentNew');
            equipmentDiv.classList.add('divList');
            let equipmentList = document.createElement('ul');
            let title = document.createElement('h3');
            title.innerText = "Equipment used:";
            equipmentDiv.appendChild(title);
            for (let equipment of examination['equipmentUsed']){
                let item = document.createElement('li');
                item.innerText = equipment;
                equipmentList.appendChild(item);
            }
            equipmentDiv.appendChild(equipmentList);
            popUp.appendChild(equipmentDiv);
        }

    }
}

var scheduleDateButton = document.getElementById("scheduleDateBtn");

function searchSchedule(){
    let inputDate = document.getElementById("scheduleDateOption").value;

    const convertedInputDate = new Date(inputDate);
    const lastDayInSchedule = new Date(inputDate);
    lastDayInSchedule.setDate(convertedInputDate.getDate() + 3).set;
    convertedInputDate.setHours(7,0,0);
    lastDayInSchedule.setHours(23,0,0);

    let table = document.getElementById("examinationsTableSchedule");
    removeAllChildNodes(table);

    for (let i in doctorsExaminations){
        
        let examinationDate = new Date(doctorsExaminations[i]['date']);
        if (examinationDate >= convertedInputDate && examinationDate <= lastDayInSchedule){
            
            let examination = doctorsExaminations[i];
            let newRow = document.createElement("tr");

            let examinationDate = document.createElement("td");
            examinationDate.innerText = (new Date(examination["date"])).toLocaleString();
            let examinationDuration = document.createElement("td");
            examinationDuration.innerText = examination["duration"];
            let examinationDone = document.createElement("td");
            examinationDone.innerText = examination["done"];
            let examinationRoom = document.createElement("td");
            examinationRoom.innerText = examination["room"];
            let examinationType = document.createElement("td");
            examinationType.innerText = examination["type"];
            let isUrgent = document.createElement("td");
            isUrgent.innerText = examination["urgent"];

            let one = document.createElement("td");
            let patientBtn = document.createElement("button");
            patientBtn.innerHTML = '<i data-feather="user"></i>';
            patientBtn.setAttribute("key", examination["patient"]);
            patientBtn.classList.add('send');
            patientBtn.addEventListener('click', function (e) {
                window.location.replace("patientMedicalCard.php" + "?patientId=" + patientBtn.getAttribute("key") + '&token=' + jwtoken + '&doctorId=' + doctorId);
            });
            one.appendChild(patientBtn);

            let two = document.createElement("td");
            let reviewBtn = document.createElement("button");
            reviewBtn.innerHTML = '<i data-feather="check-square"></i>';
            reviewBtn.classList.add("add");
            reviewBtn.setAttribute("key", examination["id"]);
            reviewBtn.addEventListener('click', function(e){
                reviewExamination(parseInt(reviewBtn.getAttribute('key')));
                });
            two.appendChild(reviewBtn);

            newRow.appendChild(examinationDate);
            newRow.appendChild(examinationDuration);
            newRow.appendChild(examinationDone);
            newRow.appendChild(examinationRoom);
            newRow.appendChild(examinationType);
            newRow.appendChild(isUrgent);
            newRow.appendChild(one);
            newRow.appendChild(two);
            table.appendChild(newRow);
            feather.replace();
        }
    }
}

scheduleDateButton.addEventListener("click", function(e){
    searchSchedule();
});

function deleteExamination(id){
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert("Examination has been successfully deleted");
                showExaminations();
            }
            else {
                alert("Error: Selected examination couldn't be deleted");
            }
        }
    }
    request.open('DELETE', 'https://localhost:7291/api/doctor/examinations/' + id);
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
};

function validateTimeOfExamination(date,duration){

    let currentDate = new Date();
    let newExaminationBegging = new Date(date);
    let newExaminationEnding = new Date(date);

    newExaminationEnding.setTime(newExaminationBegging.getTime() + 6000 * duration);
    
    if (currentDate > newExaminationBegging){
        return false;
    }

    for (let examination of doctorsExaminations){

        let examinationBegging = new Date(examination["date"]);
        let examinationEnding = new Date(examination["date"]);
        examinationEnding.setTime(examinationBegging.getTime() + 6000 * examination["duration"]);

        if ((newExaminationBegging >= examinationBegging && newExaminationBegging <= examinationEnding) 
            | (newExaminationEnding >= examinationBegging && newExaminationEnding <= examinationEnding)){
                return false;
            }
    }

    return true;
}

function validateTimeOfExaminationPut(date, duration, id){

    let currentDate = new Date();
    let newExaminationBegging = new Date(date);
    let newExaminationEnding = new Date(date);

    newExaminationEnding.setTime(newExaminationBegging.getTime() + 6000 * duration);
    
    if (currentDate > newExaminationBegging){
        return false;
    }

    for (let examination of doctorsExaminations){
        if (examination["id"] != id){

            let examinationBegging = new Date(examination["date"]);
            let examinationEnding = new Date(examination["date"]);
            examinationEnding.setTime(examinationBegging.getTime() + 6000 * examination["duration"]);

            if ((newExaminationBegging >= examinationBegging && newExaminationBegging <= examinationEnding) 
                | (newExaminationEnding >= examinationBegging && newExaminationEnding <= examinationEnding)){
                    return false;
                }
            }
    }

    return true;
}

function submitForm(e) {
    let popUp = document.getElementById("examinationPopUp");
    popUp.classList.add("off");
    main.classList.remove("hideMain");
    e.preventDefault();
    e.stopImmediatePropagation();

    let selectedType = document.getElementById("examinationType").value;
    let selectedDate = document.getElementById("scheduleDate").value;
    let selectedDuration = document.getElementById("examinationDuration").value;

    if (validateTimeOfExamination(selectedDate, selectedDuration)
    && !(selectedType == "visit" && selectedDuration != 15)){
      
        let postRequest = new XMLHttpRequest();

        postRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    alert("Examination sucessfuly created");
                    showExaminations();
                } else {
                    alert("Error: Entered examination informations are invalid");
                }
            }
        };

        let selectedRoom = document.getElementById("examinationRoom").value;
        let selectedPatient = document.getElementById("examinationPatient").value;
        let isUrgent = document.getElementById("urgent").checked ? true : false;

        postRequest.open('POST', 'https://localhost:7291/api/doctor/examinations');
        postRequest.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
        postRequest.send(JSON.stringify({ "done":false, "date": selectedDate, "duration": selectedDuration,"room": selectedRoom, "patient": selectedPatient, "doctor": doctorId, "urgent": isUrgent, "type": selectedType, "anamnesis":""}));       
    }
}

var rooms;

function addOptions(element, roomOptions){
    let valueOfType = element.value;
    if (valueOfType == "visit"){
        for (let room of rooms){
            if (room["type"] == "examination room"){
                let newOption = document.createElement('option');
                newOption.setAttribute('value', room['name']);
                newOption.innerText = room['name'];
                roomOptions.appendChild(newOption);
            }
        }
    }else{
        for (let room of rooms){
            if (room["type"] == "operation room"){
                let newOption = document.createElement('option');
                newOption.setAttribute('value', room['name']);
                newOption.innerText = room['name'];
                roomOptions.appendChild(newOption);
            }
        }
    }
}

function createExamination() {
    let getRequest = new XMLHttpRequest();
    getRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                rooms = JSON.parse(this.responseText);
                let popUp = document.getElementById("examinationPopUp");
                popUp.classList.remove("off");
                main.classList.add("hideMain");

                document.getElementById("examinationDuration").value = 15;
                document.getElementById("examinationPatient").value = "";
                document.getElementById("urgent").checked = false;

                let form = document.getElementById("examinationForm");
                let title = document.getElementById("examinationFormId");
                title.innerText = "Create examination";
                     
                let roomOptions = document.getElementById("examinationRoom");
                let examinationType = document.getElementById("examinationType");
                addOptions(examinationType, roomOptions);
                examinationType.addEventListener('change', function(e){
                    removeAllChildNodes(roomOptions);
                    addOptions(examinationType, roomOptions);
                })

                form.addEventListener('submit', function(e){
                    submitForm(e)
                });
            }
        }
    }
    getRequest.open('GET', 'https://localhost:7291/api/manager/rooms');
    getRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    getRequest.send();
}

function submitUpdate(e, updatedExamination, id, popUp){
    popUp.classList.add("off");
    main.classList.remove("hideMain");
    e.preventDefault();
    e.stopImmediatePropagation();

    let selectedType = document.getElementById("examinationType").value;
    let selectedDate = document.getElementById("scheduleDate").value;
    let selectedDuration = document.getElementById("examinationDuration").value;
    
    if (validateTimeOfExaminationPut(selectedDate, selectedDuration, id)
        && !(selectedType == "visit" && selectedDuration != 15)){

        let postRequest = new XMLHttpRequest();

        postRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    alert("Examination sucessfuly updated");
                    showExaminations();
                } else {
                    alert("Error: Entered examination informations are invalid");
                }
            }
        };

        let selectedRoom = document.getElementById("examinationRoom").value;
        let selectedPatient = document.getElementById("examinationPatient").value;
        let isUrgent = document.getElementById("urgent").checked ? true : false;

        postRequest.open('PUT', 'https://localhost:7291/api/doctor/examinations/' + id);
        postRequest.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);

        postRequest.send(JSON.stringify({ "_id": updatedExamination["_id"], "id": updatedExamination["id"], "done":false, "date": selectedDate, "duration": selectedDuration,"room": selectedRoom, "patient": selectedPatient, "doctor": doctorId, "urgent": isUrgent, "type": selectedType, "anamnesis":""}));
    }
    else{
        alert("Error: Entered examination informations are invalid");
        popUp.classList.remove("off");
        main.classList.add("hideMain");
        let title = document.getElementById("examinationFormId");
        title.innerText = "Update examination"
    }
}

function updateExamination(id){
    let getRequest = new XMLHttpRequest();
    getRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                rooms = JSON.parse(this.responseText);
                let updatedExamination;
                for (let examination of doctorsExaminations){
                    if (examination.id == id){
                        updatedExamination = examination;
                        break
                    }
                }

                let popUp = document.getElementById("examinationPopUp");
                popUp.classList.remove("off");
                main.classList.add("hideMain");
                let title = document.getElementById("examinationFormId");
                title.innerText = "Update examination";

                let form = document.getElementById("examinationForm");
                document.getElementById("scheduleDate").value = updatedExamination["date"];
                document.getElementById("examinationDuration").value = updatedExamination["duration"];
                document.getElementById("examinationPatient").value = updatedExamination["patient"];
                document.getElementById("examinationType").value = updatedExamination["type"];
                document.getElementById("urgent").checked = false;
                if (updatedExamination["urgent"]){
                    document.getElementById("urgent").checked = true;
                }
                     
                let roomOptions = document.getElementById("examinationRoom");
                let examinationType = document.getElementById("examinationType");
                addOptions(examinationType, roomOptions);
                examinationType.addEventListener('change', function(e){
                    removeAllChildNodes(roomOptions);
                    addOptions(examinationType, roomOptions);
                })

                form.addEventListener('submit', function (e) {
                    submitUpdate(e, updatedExamination, id, popUp);
                });
            }
        }
    }
    getRequest.open('GET', 'https://localhost:7291/api/manager/rooms');
    getRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    getRequest.send();
}

var createBtn = document.getElementById("addBtn");

createBtn.addEventListener("click", createExamination);

var closeReportBtn = document.getElementById('closeReportBtn');

closeReportBtn.addEventListener('click', function(e){
    let equipment = document.getElementById('reportEquipment');
    removeAllChildNodes(equipment);
    let popUp = document.getElementById('reportPopUpNew');
    popUp.classList.add('off');
    main.classList.remove('hideMain');
})
