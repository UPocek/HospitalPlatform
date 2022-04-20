window.addEventListener("load", function () {

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

                    

                    newRow.appendChild(examinationDate);
                    newRow.appendChild(examinationDone);
                    newRow.appendChild(examinationRoom);
                    newRow.appendChild(examinationType);
                    newRow.appendChild(isUrgent);
                    table.appendChild(newRow);
                    feather.replace();
                }
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/doctor/examinations');
    request.send();
});