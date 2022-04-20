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

let mainResponse;
let doctorId = getParamValue('id');

function showExaminations(){
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                mainResponse = JSON.parse(this.responseText);
                let table = document.getElementById("roomTable");
                table.innerHTML = "";
                for (let i in mainResponse) {
                    let examination = mainResponse[i];
                    let newRow = document.createElement("tr");

                    let examinationDate = document.createElement("td");
                    examinationDate.innerText = examination["date"];
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
                        window.location.replace("patient_medical_card.php" + "?patient_id=" + patientBtn.getAttribute("key"));
                    });
                    one.appendChild(patientBtn);

                    let two = document.createElement("td");
                    let delBtn = document.createElement("button");
                    delBtn.innerHTML = '<i data-feather="trash"></i>';
                    delBtn.classList.add("delBtn");
                    delBtn.setAttribute("key", examination["id"]);
                    delBtn.addEventListener('click', function (e) {
                        deleteExamination(delBtn.getAttribute('key'));
                    });
                    two.appendChild(delBtn);

                    newRow.appendChild(examinationDate);
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
    }

    request.open('GET', 'https://localhost:7291/api/doctor/examinations/doctor_id/' + doctorId);
    request.send();
}

window.addEventListener("load", showExaminations);

let scheduleDateButton = document.getElementById("scheduleDateBtn");

scheduleDateButton.addEventListener("click", function(e){

    let inputDate = document.getElementById("scheduleDate").value;
    const convertedInputDate = new Date(inputDate);
    const lastDayInSchedule = new Date(inputDate);

    lastDayInSchedule.setDate(convertedInputDate.getDate() + 3).set;
    convertedInputDate.setHours(7,0,0);
    lastDayInSchedule.setHours(23,0,0);

    let table = document.getElementById("roomTable");
    removeAllChildNodes(table);

    for (let i in mainResponse){
        
        let examinationDate = new Date(mainResponse[i]['date']);
        
        if (examinationDate >= convertedInputDate && examinationDate <= lastDayInSchedule){
            
            let examination = mainResponse[i];
            let newRow = document.createElement("tr");

            let examinationDate = document.createElement("td");
            examinationDate.innerText = examination["date"];
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
                window.location.replace("patient_medical_card.php" + "?patient_id=" + patientBtn.getAttribute("key"));
            });
            one.appendChild(patientBtn);

            let two = document.createElement("td");
            let delBtn = document.createElement("button");
            delBtn.innerHTML = '<i data-feather="trash"></i>';
            delBtn.classList.add("delBtn");
            delBtn.setAttribute("key", examination["id"]);
            delBtn.addEventListener('click', function (e) {
                deleteExamination(delBtn.getAttribute('key'));
            });
            two.appendChild(delBtn);

            newRow.appendChild(examinationDate);
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
})

function deleteExamination(id){
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                showExaminations();
            }
            else {
                alert("Error: Selected room couldn't be deleted");
            }
        }
    }
    request.open('DELETE', 'https://localhost:7291/api/doctor/examinations/' + id);
    request.send();
}