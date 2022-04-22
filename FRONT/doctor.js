//*helper functions
function removeAllChildNodes(parent) {
    while (parent.firstChild) {
        parent.removeChild(parent.firstChild);
    }
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

var doctorsExaminations;
var doctorId = getParamValue('id');

function setUpMenu() {
    let menu = document.getElementById("mainMenu");
    menu.innerHTML += `
    <li id="option1" class="navbar__item">
        <a href="#" class="navbar__link"><i data-feather="calendar"></i><span>Schedule</span></a>
    </li>
    <li id="option2" class="navbar__item">
        <a href="#" class="navbar__link"><i data-feather="briefcase"></i><span>Free days</span></a>
    </li>
    `;
    feather.replace();

    let item1 = document.getElementById("option1");
    let item2 = document.getElementById("option2");

    item1.addEventListener('click', (e) => {
        
    });
    item2.addEventListener('click', (e) => {
        
    });
    
}

function showExaminations(){

    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                doctorsExaminations = JSON.parse(this.responseText);
                let table = document.getElementById("examinationsTable");
                table.innerHTML = "";
                for (let examination of doctorsExaminations) {

                    let newRow = document.createElement("tr");

                    let examinationDate = document.createElement("td");
                    examinationDate.innerText = (new Date(examination["date"])).toLocaleString();
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
                    patientBtn.addEventListener('click', function (e) {
                        window.location.replace("patientMedicalCard.php" + "?patientId=" + patientBtn.getAttribute("key"));
                    });
                    one.appendChild(patientBtn);

                    let two = document.createElement("td");
                    let delBtn = document.createElement("button");
                    delBtn.innerHTML = '<i data-feather="trash"></i>';
                    delBtn.classList.add("delBtn");
                    delBtn.setAttribute("key", examination["id"]);
                    delBtn.addEventListener('click', function(e){
                        deleteExamination(delBtn.getAttribute('key'));
                     });
                    two.appendChild(delBtn);

                    let three = document.createElement("td");
                    let updateBtn = document.createElement("button");
                    updateBtn.innerHTML = '<i data-feather="upload"></i>';
                    updateBtn.classList.add("updateBtn");
                    updateBtn.setAttribute("key", examination["id"]);
                    updateBtn.addEventListener('click', function(e){ 
                        updateExamination(updateBtn.getAttribute("key"));
                    });
                    three.appendChild(updateBtn);

                    newRow.appendChild(examinationDate);
                    newRow.appendChild(examinationDone);
                    newRow.appendChild(examinationRoom);
                    newRow.appendChild(examinationType);
                    newRow.appendChild(isUrgent);
                    newRow.appendChild(one);
                    newRow.appendChild(two);
                    newRow.appendChild(three);
                    table.appendChild(newRow);
                    feather.replace();
                }
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/doctor/examinations/doctorId/' + doctorId);
    request.send();
}

function setUpPage(){
    setUpMenu();
    showExaminations();
}

window.addEventListener("load", setUpPage);

var scheduleDateButton = document.getElementById("scheduleDateBtn");

function searchSchedule(){
    let inputDate = document.getElementById("scheduleDate").value;

    const convertedInputDate = new Date(inputDate);
    const lastDayInSchedule = new Date(inputDate);
    lastDayInSchedule.setDate(convertedInputDate.getDate() + 3).set;
    convertedInputDate.setHours(7,0,0);
    lastDayInSchedule.setHours(23,0,0);

    let table = document.getElementById("examinationsTable");
    removeAllChildNodes(table);

    for (let i in doctorsExaminations){
        
        let examinationDate = new Date(doctorsExaminations[i]['date']);
        
        if (examinationDate >= convertedInputDate && examinationDate <= lastDayInSchedule){
            
            let examination = doctorsExaminations[i];
            let newRow = document.createElement("tr");

            let examinationDate = document.createElement("td");
            examinationDate.innerText = (new Date(examination["date"])).toLocaleString();
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
            patientBtn.addEventListener('click', function (e) {
                window.location.replace("patientMedicalCard.php" + "?patientId=" + patientBtn.getAttribute("key"));
            });
            one.appendChild(patientBtn);

            let two = document.createElement("td");
            let delBtn = document.createElement("button");
            delBtn.innerHTML = '<i data-feather="trash"></i>';
            delBtn.classList.add("delBtn");
            delBtn.setAttribute("key", examination["id"]);
            delBtn.addEventListener('click', function(e){
               deleteExamination(delBtn.getAttribute('key'))
            });
            two.appendChild(delBtn);

            let three = document.createElement("td");
            let updateBtn = document.createElement("button");
            updateBtn.innerHTML = '<i data-feather="upload"></i>';
            updateBtn.classList.add("updateBtn");
            updateBtn.setAttribute("key", examination["id"]);
            updateBtn.addEventListener('click', function(e){ 
                updateExamination(updateBtn.getAttribute("key"));
            });
            three.appendChild(updateBtn);

            newRow.appendChild(examinationDate);
            newRow.appendChild(examinationDone);
            newRow.appendChild(examinationRoom);
            newRow.appendChild(examinationType);
            newRow.appendChild(isUrgent);
            newRow.appendChild(one);
            newRow.appendChild(two);
            newRow.appendChild(three);
            table.appendChild(newRow);
            feather.replace();
        }
    }
}

scheduleDateButton.addEventListener("click", searchSchedule);

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
    request.send();
};

var main = document.getElementsByTagName("main")[0];

function validateTimeOfExamination(date,duration){

    let currentDate = new Date();
    let newExaminationBegging = new Date(date);
    let newExaminationEnding = new Date(date);

    newExaminationEnding.setTime(newExaminationBegging.getTime() + 6000 * duration);
    
    if (currentDate > newExaminationBegging){
        return false;
    }

    for (examination of doctorsExaminations){

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

    for (examination of doctorsExaminations){
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

    if (selectedType == "visit" && selectedDuration != 15){
        alert("Duration of visit can't be longer then 15 minutes");
    }
    if (validateTimeOfExamination(selectedDate, selectedDuration)){
      
        let postRequest = new XMLHttpRequest();

        postRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    alert("Room sucessfuly created");
                    showExaminations();
                } else {
                    alert("Error: Entered room informations are invalid");
                }
            }
        };

        let selectedRoom = document.getElementById("examinationRoom").value;
        let selectedPatient = document.getElementById("examinationPatient").value;
        let isUrgent = document.getElementById("urgent").checked ? true : false;

        postRequest.open('POST', 'https://localhost:7291/api/doctor/examinations');
        postRequest.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        postRequest.send(JSON.stringify({ "done":false, "date": selectedDate, "duration": selectedDuration,"room": selectedRoom, "patient": selectedPatient, "doctor": doctorId, "urgent": isUrgent, "type": selectedType, "anamnesis":""}));       
    }
}

var rooms;

function addOptions(element, roomOptions){
    let valueOfType = element.value;
    if (valueOfType == "visit"){
        for (room of rooms){
            if (room["type"] == "examination room"){
                let newOption = document.createElement('option');
                newOption.setAttribute('value', room['name']);
                newOption.innerText = room['name'];
                roomOptions.appendChild(newOption);
            }
        }
    }else{
        for (room of rooms){
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
    getRequest.send();
}

function submitUpdate(updatedExamination, id, popUp){
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

        console.log(JSON.stringify({ "_id": updatedExamination["_id"], "id": updatedExamination["id"], "done":false, "date": selectedDate, "duration": selectedDuration,"room": selectedRoom, "patient": selectedPatient, "doctor": doctorId, "urgent": isUrgent, "type": selectedType, "anamnesis":""}));
        postRequest.open('PUT', 'https://localhost:7291/api/doctor/examinations/' + id);
        postRequest.setRequestHeader("Content-Type", "application/json;charset=UTF-8");

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
                for (examination of doctorsExaminations){
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
                    submitUpdate(updateExamination, id, popUp);
                });
            }
        }
    }
    getRequest.open('GET', 'https://localhost:7291/api/manager/rooms');
    getRequest.send();
}

var createBtn = document.getElementById("addBtn");

createBtn.addEventListener("click", createExamination);