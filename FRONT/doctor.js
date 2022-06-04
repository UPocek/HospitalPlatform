var currentMedicalRecord;
var currentPatientMedicalRecord;
var currentExamination;
var roomOfExamination;

//function for setting up doctors view
function setUpExaminations() {

    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                doctorsExaminations = JSON.parse(this.responseText);
                setUpFunctionality();
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/doctor/examinations/doctorId/' + userId);
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}

//function apstracts all work for setting up doctors view
function setUpFunctionality() {
    displayExaminations();
    document.getElementById('scheduleDate').value = (new Date()).toDateString;
    document.getElementById('scheduleDateOption').value = new Date().toISOString().split('T')[0];
    searchSchedule();
    setUpDrugsForReview();
}

//function thath dispays examination table of examinations for currently logged doctor
function displayExaminations() {
    let table = document.getElementById('examinationsTable');
    table.innerHTML = "";
    for (let examination of doctorsExaminations) {
        let newRow = document.createElement("tr");

        //creating row for one examination
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

        //button for displaying patients medical card
        let one = document.createElement("td");
        let patientBtn = document.createElement("button");
        patientBtn.innerHTML = '<i data-feather="user"></i>';
        patientBtn.setAttribute("key", examination["patient"]);
        patientBtn.classList.add('send');
        patientBtn.addEventListener('click', function (e) {
            window.location.replace("patientMedicalCard.php" + "?patientId=" + patientBtn.getAttribute("key") + '&token=' + jwtoken + '&doctorId=' + userId);
        });
        one.appendChild(patientBtn);

        //button for dispalying report for current examination
        let two = document.createElement("td");
        let reportBtn = document.createElement("button");
        reportBtn.innerHTML = '<i data-feather="clipboard"></i>';
        reportBtn.setAttribute("key", examination["id"]);
        reportBtn.classList.add('send');
        reportBtn.addEventListener('click', function (e) {
            reviewReport(parseInt(reportBtn.getAttribute('key')));
        });
        two.appendChild(reportBtn);

        //button for deleting current examination
        let three = document.createElement("td");
        let delBtn = document.createElement("button");
        delBtn.innerHTML = '<i data-feather="trash"></i>';
        delBtn.classList.add("delBtn");
        delBtn.setAttribute("key", examination["id"]);
        delBtn.addEventListener('click', function (e) {
            deleteExamination(delBtn.getAttribute('key'));
        });
        three.appendChild(delBtn);

        //button for updating current examination
        let four = document.createElement("td");
        let updateBtn = document.createElement("button");
        updateBtn.innerHTML = '<i data-feather="upload"></i>';
        updateBtn.classList.add("updateBtn");
        updateBtn.setAttribute("key", examination["id"]);
        updateBtn.addEventListener('click', function (e) {
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

/*function for reviewing examination,
 setting up pop-up for it*/
function reviewExamination(id) {
    //find examination that doctor wants to review
    for (let examination of doctorsExaminations) {
        if (examination["id"] == id) {
            currentExamination = examination;
            break;
        }
    }

    //if the examination is not yet reviewed, show pop-up for reviewing it
    //hide other content on page that isn't pop-up
    if (currentExamination['done'] != true) {
        let popUp = document.getElementById('reviewExaminationDiv');
        popUp.classList.remove("off");
        main.classList.add("hideMain");

        document.getElementById("reportDescription").innerText = currentExamination['anamnesis'];

        //let doctor enter equipment used in that examination
        let getEquipmentRequest = new XMLHttpRequest();
        getEquipmentRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    let equipmentDiv = document.getElementById('equipmentDiv');
                    roomOfExamination = JSON.parse(this.responseText);
                    equipmentDiv.innerHTML = '';
                    for (let equipment of roomOfExamination['equipment']) {
                        let equipmentField = document.createElement('p');
                        equipmentField.innerText = `${equipment['name']} ( quantity: ${equipment['quantity']} )`;
                        equipmentField.setAttribute('equipmentType', equipment['type'])
                        equipmentField.classList.add('pushLeft');
                        let quantityField = document.createElement('input');
                        quantityField.setAttribute('type', 'text');
                        quantityField.setAttribute('autocomplete', 'off')
                        quantityField.setAttribute('placeholder', 'Enter how much to transfer');
                        equipmentDiv.appendChild(equipmentField);
                        equipmentDiv.appendChild(quantityField);
                    }
                }
            }
        }
        getEquipmentRequest.open('GET', 'https://localhost:7291/api/doctor/examinations/room/' + currentExamination['room']);
        getEquipmentRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
        getEquipmentRequest.send();
        
        feather.replace();

        let getMedicalRecordRequest = new XMLHttpRequest();

        //display patient medical record and let doctor update it
        getMedicalRecordRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    let patient = JSON.parse(this.responseText);
                    currentPatientMedicalRecord = patient;
                    currentMedicalRecord = patient["medicalRecord"];

                    let patientFName = document.getElementById("patientFName");
                    patientFName.setAttribute('key', patient['id']);
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
                    for (let disease of currentMedicalRecord['diseases']) {
                        let diseaseItem = document.createElement('option');
                        diseaseItem.innerText = disease;
                        patientDiseases.appendChild(diseaseItem);
                    }
                    let patientAlergies = document.getElementById("alergiesList");
                    for (let alergie of currentMedicalRecord['alergies']) {
                        let alergieItem = document.createElement('option');
                        alergieItem.innerText = alergie;
                        patientAlergies.appendChild(alergieItem);
                    }
                }
            }
        }
        getMedicalRecordRequest.open('GET', 'https://localhost:7291/api/doctor/examinations/patientMedicalCard/' + currentExamination['patient']);
        getMedicalRecordRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
        getMedicalRecordRequest.send();
    } else {
        //if examination is already reviewed
        alert('Examination already reviewed.');
    }
}

var updateMedicalCardBtn = document.getElementById("updateMedicalCard");

//action when doctor wants to update patients medical card while reviewing examination
updateMedicalCardBtn.addEventListener('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    currentMedicalRecord['weight'] = document.getElementById('patientWeight').value;
    currentMedicalRecord['height'] = document.getElementById('patientHeight').value;
    currentMedicalRecord['bloodType'] = document.getElementById('patientBlood').value;
    currentPatientMedicalRecord['medicalRecord'] = currentMedicalRecord;

    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert("Record updated")
            }
        }
    }
    request.open('PUT', 'https://localhost:7291/api/doctor/examinations/medicalrecord/' + currentPatientMedicalRecord['id']);
    request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send(JSON.stringify(currentMedicalRecord));
})

//if doctor wants to see report made for current, already reviewed examination
function reviewReport(id) {
    let currentExamination;
    for (let examination of doctorsExaminations) {
        if (examination["id"] == id) {
            currentExamination = examination;
            break;
        }
    }

    let popUp = document.getElementById('reportPopUpNew');

    popUp.classList.remove('off');
    main.classList.add('hideMain');

    if (currentExamination['anamnesis'] == "") {
        popUp.classList.add('off');
        main.classList.remove('hideMain');
        alert("No report present");
    } 
    else {

        document.getElementById("reportDescriptionNew").innerText = currentExamination['anamnesis'];
        let equipmentDiv = document.getElementById('reportEquipmentNew');
        removeAllChildNodes(equipmentDiv);
        
        equipmentDiv.classList.add('divList');
        let equipmentList = document.createElement('ul');
        let title = document.createElement('h3');
        title.innerText = "Equipment used:";
        equipmentDiv.appendChild(title);
        for (let equipment of currentExamination['equipmentUsed']) {
            let item = document.createElement('li');
            item.innerText = equipment;
            equipmentList.appendChild(item);
        }
        equipmentDiv.appendChild(equipmentList);
        popUp.appendChild(equipmentDiv);
    }
}

var scheduleDateButton = document.getElementById("scheduleDateBtn");

//function for showing doctors examinations based on the date he selects
function searchSchedule() {
    let inputDate = document.getElementById("scheduleDateOption").value;

    const convertedInputDate = new Date(inputDate);
    const lastDayInSchedule = new Date(inputDate);
    lastDayInSchedule.setDate(convertedInputDate.getDate() + 3).set;
    convertedInputDate.setHours(7, 0, 0);
    lastDayInSchedule.setHours(23, 0, 0);

    let table = document.getElementById("examinationsTableSchedule");
    removeAllChildNodes(table);

    for (let i in doctorsExaminations) {

        let examinationDate = new Date(doctorsExaminations[i]['date']);
        if (examinationDate >= convertedInputDate && examinationDate <= lastDayInSchedule) {

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
                window.location.replace("patientMedicalCard.php" + "?patientId=" + patientBtn.getAttribute("key") + '&token=' + jwtoken + '&doctorId=' + userId);
            });
            one.appendChild(patientBtn);

            let two = document.createElement("td");
            let reviewBtn = document.createElement("button");
            reviewBtn.innerHTML = '<i data-feather="check-square"></i>';
            reviewBtn.classList.add("add");
            reviewBtn.setAttribute("key", examination["id"]);
            reviewBtn.addEventListener('click', function (e) {
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

scheduleDateButton.addEventListener("click", function (e) {
    searchSchedule();
});

//function for deleting examination
function deleteExamination(id) {
    let deleteRequest = new XMLHttpRequest();
    deleteRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert("Examination has been successfully deleted");
                setUpExaminations();
            }
            else {
                alert("Error: Selected examination couldn't be deleted");
            }
        }
    }
    deleteRequest.open('DELETE', 'https://localhost:7291/api/doctor/examinations/' + id);
    deleteRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    deleteRequest.send();
};

//helper function for checking if time of examination is not ovelaping with others
function validateTimeOfExamination(date, duration) {

    let currentDate = new Date();
    let newExaminationBegging = new Date(date);
    let newExaminationEnding = new Date(date);

    newExaminationEnding.setTime(newExaminationBegging.getTime() + 6000 * duration);

    if (currentDate > newExaminationBegging) {
        return false;
    }

    for (let examination of doctorsExaminations) {

        let examinationBegging = new Date(examination["date"]);
        let examinationEnding = new Date(examination["date"]);
        examinationEnding.setTime(examinationBegging.getTime() + 6000 * examination["duration"]);

        if ((newExaminationBegging >= examinationBegging && newExaminationBegging <= examinationEnding)
            | (newExaminationEnding >= examinationBegging && newExaminationEnding <= examinationEnding)) {
            return false;
        }
    }

    return true;
}

//helper function for checking if time of examination is not ovelaping with others
//this one is used when updating because we don't want to take the updated examination in consideration 
function validateTimeOfExaminationPut(date, duration, id) {

    let currentDate = new Date();
    let newExaminationBegging = new Date(date);
    let newExaminationEnding = new Date(date);

    newExaminationEnding.setTime(newExaminationBegging.getTime() + 60000 * duration);

    if (currentDate > newExaminationBegging) {
        return false;
    }

    for (let examination of doctorsExaminations) {
        if (examination["id"] != id) {

            let examinationBegging = new Date(examination["date"]);
            let examinationEnding = new Date(examination["date"]);
            examinationEnding.setTime(examinationBegging.getTime() + 60000 * examination["duration"]);

            if ((newExaminationBegging >= examinationBegging && newExaminationBegging <= examinationEnding)
                | (newExaminationEnding >= examinationBegging && newExaminationEnding <= examinationEnding)) {
                return false;
            }
        }
    }

    return true;
}

//function that sets up pop-up for creating new examination
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
                examinationType.addEventListener('change', function (e) {
                    removeAllChildNodes(roomOptions);
                    addOptions(examinationType, roomOptions);
                })

                form.addEventListener('submit', function (e) {
                    submitForm(e)
                });
            }
        }
    }
    getRequest.open('GET', 'https://localhost:7291/api/manager/rooms');
    getRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    getRequest.send();
}

//submiting form when creating new examination
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
        && !(selectedType == "visit" && selectedDuration != 15)) {

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

        let selectedRoom = document.getElementById("examinationRoom").value;
        let selectedPatient = document.getElementById("examinationPatient").value;
        let isUrgent = document.getElementById("urgent").checked ? true : false;

        postRequest.open('POST', 'https://localhost:7291/api/doctor/examinations');
        postRequest.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
        postRequest.send(JSON.stringify({ "done": false, "date": selectedDate, "duration": selectedDuration, "room": selectedRoom, "patient": selectedPatient, "doctor": userId, "urgent": isUrgent, "type": selectedType, "anamnesis": "" }));
    }
    else {
        alert("Error: Entered examination informations are invalid");
    }
}

var rooms;

//helper function when filtering rooms only used for certain type of examination
function addOptions(element, roomOptions) {
    let valueOfType = element.value;
    if (valueOfType == "visit") {
        for (let room of rooms) {
            if (room["type"] == "examination room") {
                let newOption = document.createElement('option');
                newOption.setAttribute('value', room['name']);
                newOption.innerText = room['name'];
                roomOptions.appendChild(newOption);
            }
        }
    } else {
        for (let room of rooms) {
            if (room["type"] == "operation room") {
                let newOption = document.createElement('option');
                newOption.setAttribute('value', room['name']);
                newOption.innerText = room['name'];
                roomOptions.appendChild(newOption);
            }
        }
    }
}

//function for setting up pop-up for updating examination
function updateExamination(id) {
    let getRequest = new XMLHttpRequest();
    getRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                rooms = JSON.parse(this.responseText);
                let updatedExamination;
                for (let examination of doctorsExaminations) {
                    if (examination.id == id) {
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
                if (updatedExamination["urgent"]) {
                    document.getElementById("urgent").checked = true;
                }

                let roomOptions = document.getElementById("examinationRoom");
                let examinationType = document.getElementById("examinationType");
                addOptions(examinationType, roomOptions);
                examinationType.addEventListener('change', function (e) {
                    removeAllChildNodes(roomOptions);
                    addOptions(examinationType, roomOptions);
                })

                form.addEventListener('submit', function (e) {
                    submitUpdate(e, updatedExamination, id);
                });
            }
        }
    }
    getRequest.open('GET', 'https://localhost:7291/api/manager/rooms');
    getRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    getRequest.send();
}

//action when submitting an update for examination
function submitUpdate(e, updatedExamination, id) {
    let popUp = document.getElementById('examinationPopUp');
    popUp.classList.add("off");
    main.classList.remove("hideMain");
    e.preventDefault();
    e.stopImmediatePropagation();

    let selectedType = document.getElementById("examinationType").value;
    let selectedDate = document.getElementById("scheduleDate").value;
    let selectedDuration = document.getElementById("examinationDuration").value;
    if (validateTimeOfExaminationPut(selectedDate, selectedDuration, id)
        && !(selectedType == "visit" && selectedDuration != 15)) {

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

        let selectedRoom = document.getElementById("examinationRoom").value;
        let selectedPatient = document.getElementById("examinationPatient").value;
        let isUrgent = document.getElementById("urgent").checked ? true : false;

        postRequest.open('PUT', 'https://localhost:7291/api/doctor/examinations/' + id);
        postRequest.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
        postRequest.send(JSON.stringify({ "_id": updatedExamination["_id"], "id": updatedExamination["id"], "done": false, "date": selectedDate, "duration": selectedDuration, "room": selectedRoom, "patient": selectedPatient, "doctor": userId, "urgent": isUrgent, "type": selectedType, "anamnesis": "" }));
    }
    else {
        alert("Error: Entered examination informations are invalid");
        popUp.classList.remove("off");
        main.classList.add("hideMain");
        let title = document.getElementById("examinationFormId");
        title.innerText = "Update examination"
    }
}

var createBtn = document.getElementById("addBtn");

createBtn.addEventListener("click", createExamination);

var closeReportBtn = document.getElementById('closeReportBtn');

//action when doctor wants to exit report of current examination
closeReportBtn.addEventListener('click', function (e) {
    let equipment = document.getElementById('reportEquipment');
    removeAllChildNodes(equipment);

    let popUp = document.getElementById('reportPopUpNew');

    popUp.classList.add('off');
    main.classList.remove('hideMain');
})

var diseaseDelBtn = document.getElementById('deleteDiseases');

//action for deleting diseases to patients medical record
diseaseDelBtn.addEventListener('click', function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    let diseases = document.getElementById('diseasesList');
    let deletedDiseases = diseases.value;
    currentMedicalRecord["diseases"] = currentMedicalRecord["diseases"].filter(function (item) {
        return item !== deletedDiseases
    });
    removeAllChildNodes(diseases);
    for (let disease of currentMedicalRecord['diseases']) {
        let diseaseItem = document.createElement('option');
        diseaseItem.innerText = disease;
        diseases.appendChild(diseaseItem);
    }
})

var alergiesDelBtn = document.getElementById('deleteAlergies');

//action for deleting alergies to patients medical record
alergiesDelBtn.addEventListener('click', function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    let alergies = document.getElementById('alergiesList');
    let deletedAlergies = alergies.value;
    currentMedicalRecord["alergies"] = currentMedicalRecord["alergies"].filter(function (item) {
        return item !== deletedAlergies
    });
    removeAllChildNodes(alergies);
    for (let alergie of currentMedicalRecord['alergies']) {
        let alergieItem = document.createElement('option');
        alergieItem.innerText = alergie;
        alergies.appendChild(alergieItem);
    }
})

var diseaseAddBtn = document.getElementById('addDiseases');

//action for adding diseases to patients medical record
diseaseAddBtn.addEventListener('click', function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    let diseasesInput = document.getElementById('diseaseInput');
    let diseases = document.getElementById('diseasesList');
    let addedDiseases = diseasesInput.value;
    currentMedicalRecord["diseases"].push(addedDiseases);
    removeAllChildNodes(diseases);
    for (let disease of currentMedicalRecord['diseases']) {
        let diseaseItem = document.createElement('option');
        diseaseItem.innerText = disease;
        diseases.appendChild(diseaseItem);
    }
    diseasesInput.value = "";
})

var alergiesAddBtn = document.getElementById('addAlergies');

//action for adding alergies to patients medical record
alergiesAddBtn.addEventListener('click', function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    let alergiesInput = document.getElementById('alergieInput');
    let alergies = document.getElementById('alergiesList');
    let addedAlergies = alergiesInput.value;
    currentMedicalRecord["alergies"].push(addedAlergies);
    removeAllChildNodes(alergies);
    for (let alergie of currentMedicalRecord['alergies']) {
        let alergieItem = document.createElement('option');
        alergieItem.innerText = alergie;
        alergies.appendChild(alergieItem);
    }
    alergiesInput.value = "";
})

var endReviewBtn = document.getElementById('endReview');

//action when doctor wants to end reviewing some examination
endReviewBtn.addEventListener('click', function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    currentExamination['anamnesis'] = document.getElementById('reportDescription').value;
    let ok = true;
    let equipmentUsed = [];
    
    //gethering equipment used
    let children = document.getElementById('equipmentDiv').children
    for (let i = 0; i < children.length; i += 2) {
        if (children[i + 1].value) {
            if (+children[i].innerText.split(' ')[3] >= +children[i + 1].value) {
                let el = {
                    name: children[i].innerText.split(' ')[0],
                    type: children[i].getAttribute('equipmentType'),
                    quantity: children[i + 1].value
                };
                equipmentUsed.push(el);
            } else {
                ok = false;
                break;
            }
        }
    }

    //updating examination
    let reviewExaminationRequest = new XMLHttpRequest();
    reviewExaminationRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert("Successful review");
                searchSchedule();
            } else {
                alert("Bad review");
            }
        }
    }

    //updating equipment in room of current examination
    let roomRequest = new XMLHttpRequest();
    roomRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert("Successful room equipment");
                searchSchedule();
            } else {
                alert("Bad room equipment");
            }
        }
    }

    //if the quantity used is valid update
    if (ok) {
        let popUp = document.getElementById('reviewExaminationDiv');
        popUp.classList.add('off');
        main.classList.remove('hideMain');
    
        for (let i in roomOfExamination['equipment']) {
            for (let equipmentInUse of equipmentUsed) {
                if (roomOfExamination['equipment'][i]['name'] == equipmentInUse['name']) {
                    roomOfExamination['equipment'][i]['quantity'] -= +equipmentInUse['quantity'];
                }
            }
        }

        let equipmenForExamination = []

        for (let equipmentItem of equipmentUsed) {
            equipmenForExamination.push(equipmentItem['name']);
        }
        currentExamination['equipmentUsed'] = equipmenForExamination;
        roomRequest.open('PUT', 'https://localhost:7291/api/doctor/examinations/room/' + roomOfExamination['name']);
        roomRequest.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        roomRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
        roomRequest.send(JSON.stringify({ "id": roomOfExamination["id"], "name": roomOfExamination["name"], "type": roomOfExamination["type"], "inRenovation": roomOfExamination["inRenovation"], "equipment": roomOfExamination["equipment"] }));
    
        for (let examination of doctorsExaminations) {
            if (examination["id"] == currentExamination['id']) {
                examination["done"] = true;
                break;
            }
        }
        reviewExaminationRequest.open('PUT', 'https://localhost:7291/api/doctor/examinations/' + currentExamination['id']);
        reviewExaminationRequest.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        reviewExaminationRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
        reviewExaminationRequest.send(JSON.stringify({ "_id": currentExamination["_id"], "id": currentExamination["id"], "done": true, "date": currentExamination['date'], "duration": currentExamination['duration'], "room": currentExamination['room'], "patient": currentExamination['patient'], "doctor": currentExamination['doctor'], "urgent": currentExamination['urgent'], "type": currentExamination['type'], "anamnesis": currentExamination['anamnesis'], "equipmentUsed": currentExamination['equipmentUsed'] }));

    } else {
        alert('Quantity inputed badly.');
    }
})

var drugs;
var percsriptionBtn = document.getElementById('prescribeReviews');

//action for enabling doctor to perscribe medication when reviewing examination
// setting pop-up for it
percsriptionBtn.addEventListener('click', function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    let popUp = document.getElementById('reviewExaminationDiv');
    popUp.classList.add('off');

    let perscriptionPopUp = document.getElementById('perscriptionDiv');
    perscriptionPopUp.classList.remove('off');

    let getDrugsRequest = new XMLHttpRequest();
    getDrugsRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                drugs = JSON.parse(this.response);
                let drugOptions = document.getElementById('drugOptionsList');
                removeAllChildNodes(drugOptions);
                for (let drug of drugs) {
                    let drugItem = document.createElement('option');
                    drugItem.innerText = drug['name'];
                    drugOptions.appendChild(drugItem);
                }
                drugOptions.firstElementChild.setAttribute('selected', true);
                let percsriptionTime = document.getElementById('perscriptionTime');
                percsriptionTime.value = new Date().toISOString().split('T')[1].split('Z')[0];
            }
        }
    }

    getDrugsRequest.open('GET', 'https://localhost:7291/api/manager/drugs');
    getDrugsRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    getDrugsRequest.send();
})

var addpercsriptionBtn = document.getElementById('addPrescription');

//action for submiting perscription
addpercsriptionBtn.addEventListener('click', function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    let drugOptions = document.getElementById('drugOptionsList');
    let pickedDrug = drugOptions.value;
    let drug = findDrug(pickedDrug);
    let answer = validationOfPrescription(drug);
    if (answer == "allergic") {
        alert('Patient is alergic to ingredients of this drug.');
    }
    else {
        if (answer == "modified") {
            modifyLastPerscription(pickedDrug);
            addMedicalInstruction(pickedDrug);
        }
        else {
            addPerscriptionToRecord(pickedDrug);
            addMedicalInstruction(pickedDrug);
        }

        let request = new XMLHttpRequest();
        request.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    alert("Perscription added.")
                }
            }
        }

        request.open('PUT', 'https://localhost:7291/api/doctor/examinations/medicalrecord/' + currentPatientMedicalRecord['id']);
        request.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
        request.send(JSON.stringify(currentMedicalRecord));
    }

    let perscriptionPopUp = document.getElementById('perscriptionDiv');
    perscriptionPopUp.classList.add('off');

    let popUp = document.getElementById('reviewExaminationDiv');
    popUp.classList.remove('off');
})

//helper function for checking the status of perscription
// modified, alergic or new
function validationOfPrescription(drug) {
    let patientAllergies = currentMedicalRecord["alergies"];
    let isAllergic = checkIfAllergicToIngredients(patientAllergies, drug['ingredients'], drug['name']);
    if (isAllergic) {
        return "allergic";
    } else if (checkIfDrugPerscribed(drug['name'])) {
        return "modified";
    } else {
        return "accepted";
    }
}

//helper function for finding the enterd drug
function findDrug(drugName) {
    for (let drug of drugs) {
        if (drug['name'] == drugName) {
            return drug;
        }
    }
    return NaN;
}

//helper function for checking if the patient is already taken that medication and updating it
function checkIfDrugPerscribed(drugName) {
    for (let drug of currentMedicalRecord['drugs']) {
        if (drugName == drug['name']) {
            return true;
        }
    }
    return false;
}

//helper function for finding drugs that patient has taken before
function getDrugPerscribed(drugName) {
    for (let drug of currentMedicalRecord['drugs']) {
        if (drugName == drug['name']) {
            return drug;
        }
    }
    return Nan;
}

//helper function for checking if patinet is alergic to the drug perscribed
function checkIfAllergicToIngredients(allergies, ingredients, drugName) {
    if (allergies.includes(drugName)) {
        return true;
    }
    for (let ingredient of ingredients) {
        if (allergies.includes(ingredient)) {
            return true;
        }
    }
    return false;
}

//heleper function for updating the perscription if patinet has taken the same drug before in past
function modifyLastPerscription(drugName) {
    let drugPerscription = getDrugPerscribed(drugName);
    let time = document.getElementById('perscriptionTime').value;
    drugPerscription['when'] = time;
    let frequency = document.getElementById('perscriptionFrequency').value;
    drugPerscription['frequency'] = frequency;
    let when = document.getElementById('perscriptionFrequency').value;
    drugPerscription['how'] = when;
}

//function for adding perscription to patients medical instructions history
function addMedicalInstruction(drugName) {
    let perscriptionDuration = document.getElementById('perscriptionDuration').value;
    let start = new Date();
    let end = new Date();
    end.setDate(start.getDate() + +perscriptionDuration).set;

    let convertedStart = start.toISOString().split('T')[0];
    let convertedEnd = end.toISOString().split('T')[0]
    let medicalInstruction = { 'startDate': convertedStart, 'endDate': convertedEnd, 'doctor': userId, 'drug': drugName };
    currentMedicalRecord['medicalInstructions'].push(medicalInstruction);
}

// helper function for adding new perscription to patients medical record
function addPerscriptionToRecord(drugName) {
    let time = document.getElementById('perscriptionTime').value;
    let frequency = document.getElementById('perscriptionFrequency').value;
    let how = document.getElementById('howList').value;

    let newPerscription = { 'name': drugName, 'when': time, 'how': how, 'frequency': frequency };
    currentMedicalRecord['drugs'].push(newPerscription);
}

var referallBtn = document.getElementById('createReferall');
var doctors;
var specialities;

//action for creating referral
referallBtn.addEventListener('click', function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    let popUp = document.getElementById('reviewExaminationDiv');
    popUp.classList.add("off");
    let referallDiv = document.getElementById('referallDiv');
    referallDiv.classList.remove('off');
    getDoctors();
    let referallOption = document.getElementById('referallType');
    referallOption.addEventListener('change', function (e) {
        addReferallOptions();
    })
})

//helper function for getting all possible doctors for wich referral can be directed
function getDoctors() {
    let getDoctorsRequest = new XMLHttpRequest();
    getDoctorsRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                doctors = [];
                doctors = doctors.concat((JSON.parse(this.response)));
                getSpecialities();
                addReferallOptions();
            }
        }
    }
    getDoctorsRequest.open('GET', 'https://localhost:7291/api/my/users/doctors');
    getDoctorsRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    getDoctorsRequest.send();
}

//helper function for getting all possible specialities for wich referral can be directed
function getSpecialities() {
    specialities = [];
    for (let doctor of doctors) {
        if (!specialities.includes(doctor['specialization'])) {
            specialities.push(doctor['specialization']);
        }
    }
}

//helper function for setting options on pop-up for creating referral
function addReferallOptions() {
    let valueOfReferallType = document.getElementById('referallType').value;
    let referallOptions = document.getElementById('referallOption');
    removeAllChildNodes(referallOptions);
    if (valueOfReferallType == "doctor") {
        for (let doctor of doctors) {
            let newOption = document.createElement('option');
            newOption.setAttribute('value', doctor['id']);
            newOption.innerText = doctor['firstName'] + " " + doctor['lastName'];
            referallOptions.appendChild(newOption);
        }
    } else {
        for (let speciality of specialities) {
            let newOption = document.createElement('option');
            newOption.setAttribute('value', speciality);
            newOption.innerText = speciality;
            referallOptions.appendChild(newOption);
        }
    }
    referallOptions.firstElementChild.setAttribute('selected', true);
}

let addReferallBtn = document.getElementById('addReferall');

//action when referral is created
addReferallBtn.addEventListener('click', function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    let addReferallRequest = new XMLHttpRequest();
    addReferallRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert("Referral created");
            }
        }
    }
    let valueOfReferallType = document.getElementById('referallType').value;
    let referallOption = document.getElementById('referallOption').value;
    if (valueOfReferallType == 'doctor') {
        currentMedicalRecord['referrals'].push({ "doctorId": referallOption });
    }
    else {
        currentMedicalRecord['referrals'].push({ "speciality": referallOption });
    }

    addReferallRequest.open('PUT', 'https://localhost:7291/api/doctor/examinations/referral/' + currentPatientMedicalRecord['id']);
    addReferallRequest.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    addReferallRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    addReferallRequest.send(JSON.stringify(currentMedicalRecord));

    let referallDiv = document.getElementById('referallDiv');
    referallDiv.classList.add('off');
    let popUp = document.getElementById('reviewExaminationDiv');
    popUp.classList.remove("off");
})

//showing doctor drugs
function setUpDrugsForReview() {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                let reviewDrugs = JSON.parse(this.responseText);
                let table = document.getElementById('drugTable');
                table.innerHTML = '';
                for (let drug of reviewDrugs) {
                    let newRow = document.createElement('tr');

                    let tableDataName = document.createElement('td');
                    tableDataName.innerText = drug['name'];
                    let tableDataIngredients = document.createElement('td');
                    tableDataIngredients.innerText = '';
                    for (let ingredient of drug['ingredients']) {
                        tableDataIngredients.innerText += `${ingredient}, `;
                    }
                    if (tableDataIngredients.innerText.endsWith(', ')) {
                        tableDataIngredients.innerText = tableDataIngredients.innerText.slice(0, -2);
                    }

                    newRow.appendChild(tableDataName);
                    newRow.appendChild(tableDataIngredients);
                    
                    if(drug['status'] == "inReview"){
                    let tableSendBackButton = document.createElement('td');
                    let sendBackBtn = document.createElement('button');
                    sendBackBtn.innerHTML = `<i data-feather='edit-2'></i>`;
                    sendBackBtn.classList.add('updateBtn');
                    sendBackBtn.setAttribute('key', drug['name']);
                    sendBackBtn.addEventListener('click', function (e) {
                        sendBackDrug(sendBackBtn.getAttribute('key'));
                    });
                    tableSendBackButton.appendChild(sendBackBtn);

                    let tableDataApproveButton = document.createElement('td');
                    let approveBtn = document.createElement('button');
                    approveBtn.innerHTML = `<i data-feather='check'></i>`;
                    approveBtn.classList.add('add');
                    approveBtn.setAttribute('key', drug['name']);
                    approveBtn.addEventListener('click', function (e) {
                        approveDrug(this.getAttribute('key'));
                    });
                    tableDataApproveButton.appendChild(approveBtn);

                    newRow.appendChild(tableSendBackButton);
                    newRow.appendChild(tableDataApproveButton);
                }

                    table.appendChild(newRow);
                    feather.replace();
                }
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/doctor/drugs');
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}

//function that allowes doctor to send back some drug if he isn't satisfied with it
function sendBackDrug(key) {
    let sendBtn = document.getElementById('sendDrugMessage');
    sendBtn.setAttribute('key', key);
    sendBtn.addEventListener('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        let sendMessageRequest = new XMLHttpRequest();
        sendMessageRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    let messageDiv = document.getElementById('messageDrugPrompt');
                    messageDiv.value = "";
                    messageDiv.classList.add('off');
                    main.classList.remove("hideMain");
                    alert('Message sent sucessfuly.');
                }
            }
        }
        let message = document.getElementById('drugReviewMessage').value;
        sendMessageRequest.open('PUT', 'https://localhost:7291/api/doctor/drugs/' + key);
        sendMessageRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
        sendMessageRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
        sendMessageRequest.send(JSON.stringify({ "message": message }));
    })
    main.classList.add("hideMain");
    let messageDiv = document.getElementById('messageDrugPrompt');
    messageDiv.classList.remove('off');
}

//function for allowing doctor to approve some new drug and let it be in useg from now on
function approveDrug(key) {
    let sendMessageRequest = new XMLHttpRequest();
    sendMessageRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert('Drug approved sucessfuly.');
                location.reload();
                setUpDrugsForReview();
                showWindow(3);
            }
        }
    }
    sendMessageRequest.open('PUT', 'https://localhost:7291/api/doctor/drugs/approve/' + key);
    sendMessageRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
    sendMessageRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    sendMessageRequest.send();
}

