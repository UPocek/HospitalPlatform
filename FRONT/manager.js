// Globals
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
var user;

// Helpers
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

function isDateFormatOk(testDate) {
    return /^2[0-9]{3}-([0][1-9]|1[0-2])-([0][1-9]|[1-2]\d|3[01])$/.test(testDate);
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

var main = document.getElementsByTagName('main')[0];
var id = getParamValue('id');
var date = new Date();

// POST - Room
var createRoomBtn = document.getElementById('addBtn');
createRoomBtn.addEventListener('click', function (e) {
    let prompt = document.getElementById('createRoomPrompt');
    prompt.classList.remove('off');
    main.classList.add('hideMain');
    let inputForName = document.getElementById('createRoomName');
    inputForName.setAttribute('placeholder', 'Room 1');

    let form = document.getElementById('createRoomForm');

    form.addEventListener('submit', function (e) {
        prompt.classList.add('off');
        main.classList.remove('hideMain');
        e.preventDefault();
        e.stopImmediatePropagation();
        let postRequest = new XMLHttpRequest();

        postRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    alert('Room sucessfuly created');
                    setUpRooms();
                } else {
                    alert('Error: Entered room informations are invalid');
                }
            }
        }

        let finalName = document.getElementById('createRoomName').value;
        let finalType = document.getElementById('createRoomType').value;
        if (finalName && finalType) {
            postRequest.open('POST', 'https://localhost:7291/api/manager/rooms');
            postRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
            postRequest.send(JSON.stringify({ 'name': finalName, 'type': finalType, 'inRenovation': false, 'equipment': [] }));
        } else {
            alert("Error: Name can't be empty!")
        }
    });
});

// POST - Renovation
function renovateRoom(key) {
    let prompt = document.getElementById('renovationPrompt');
    prompt.classList.remove('off');
    main.classList.add('hideMain');

    let form = document.getElementById('renovationForm');

    form.addEventListener('submit', function (e) {
        prompt.classList.add('off');
        main.classList.remove('hideMain');
        e.preventDefault();
        e.stopImmediatePropagation();
        let postRequest = new XMLHttpRequest();

        postRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    alert('Selected room is schedule for renovation');
                } else {
                    alert('Error: Selected room cannot be renovated during this period');
                }
            }
        }

        let finalFromDate = document.getElementById('fromRenovation').value;
        let finalToDate = document.getElementById('toRenovation').value;

        if (isDateFormatOk(finalFromDate) && isDateFormatOk(finalToDate) && finalFromDate >= date.toISOString().split('T')[0] && finalToDate > finalFromDate) {
            postRequest.open('POST', 'https://localhost:7291/api/manager/renovations');
            postRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
            postRequest.send(JSON.stringify({ 'room': key, 'startDate': finalFromDate, 'endDate': finalToDate }));
        } else {
            alert('Error: Dates were not entered correctly');
        }
    });
}

// PUT - Room
function updateRoom(key) {
    let prompt = document.getElementById('roomPrompt');
    prompt.classList.remove('off');
    main.classList.add('hideMain');
    let inputForName = document.getElementById('roomName');
    inputForName.setAttribute('placeholder', key);

    let form = document.getElementById('roomForm');

    form.addEventListener('submit', function (e) {
        prompt.classList.add('off');
        main.classList.remove('hideMain');
        e.preventDefault();
        e.stopImmediatePropagation();
        let putRequest = new XMLHttpRequest();

        putRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    alert('Selected room was successfully updated');
                    location.reload();
                    setUpRooms();
                } else {
                    alert('Error: New informations are invalide');
                }
            }
        }

        let finalName = document.getElementById('roomName').value;
        let finalType = document.getElementById('roomType').value;

        if (finalName && finalType) {
            putRequest.open('PUT', 'https://localhost:7291/api/manager/rooms/' + key);
            putRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
            putRequest.send(JSON.stringify({ 'name': finalName, 'type': finalType, 'inRenovation': false, 'equipment': [] }));
        } else {
            alert("Error: Name can't be empty!")
        }
    });
}

// DELETE - Room
function deleteRoom(key) {
    let deleteRequest = new XMLHttpRequest();

    deleteRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert('Selected room was successfully deleted');
                setUpRooms();
            } else {
                alert("Error: Selected room couldn't be deleted");
            }
        }
    }

    deleteRequest.open('DELETE', 'https://localhost:7291/api/manager/rooms/' + key)
    deleteRequest.send();
}

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
    request.send();
});

function setUpMenu() {
    let menu = document.getElementById('mainMenu');
    menu.innerHTML += `
    <li id='option1' class='navbar__item'>
        <a href='#' class='navbar__link'><i data-feather='log-in'></i><span>Room Management</span></a>
    </li>
    <li id='option2' class='navbar__item'>
        <a href='#' class='navbar__link'><i data-feather='tool'></i><span>Equipment Management</span></a>
    </li>
    <li id='option3' class='navbar__item'>
        <a href='#' class='navbar__link'><i data-feather='shield'></i><span>Drug Management</span></a>
    </li>
    <li id='option4' class='navbar__item'>
        <a href='#' class='navbar__link'><i data-feather='file-text'></i><span>Polls</span></a>
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

var mainResponse;
function setUpRooms() {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                mainResponse = JSON.parse(this.responseText);
                let table = document.getElementById('roomTable');
                table.innerHTML = '';
                for (let room of mainResponse) {
                    let newRow = document.createElement('tr');

                    let tableDataName = document.createElement('td');
                    tableDataName.innerText = room['name'];
                    let tableDataType = document.createElement('td');
                    tableDataType.innerText = room['type'];
                    let tableDataRenovation = document.createElement('td');
                    tableDataRenovation.innerText = room['inRenovation'];

                    let tableDataDeleteButton = document.createElement('td');
                    let delBtn = document.createElement('button');
                    delBtn.innerHTML = `<i data-feather='trash'></i>`;
                    delBtn.classList.add('delBtn');
                    delBtn.setAttribute('key', room['name']);
                    delBtn.addEventListener('click', function (e) {
                        deleteRoom(this.getAttribute('key'));
                    });
                    tableDataDeleteButton.appendChild(delBtn);

                    let tableDataPutButton = document.createElement('td');
                    let putBtn = document.createElement('button');
                    putBtn.innerHTML = `<i data-feather='edit-2'></i>`;
                    putBtn.classList.add('updateBtn');
                    putBtn.setAttribute('key', room['name']);
                    putBtn.addEventListener('click', function (e) {
                        updateRoom(this.getAttribute('key'));
                    });
                    tableDataPutButton.appendChild(putBtn);

                    let tableDataRenovateButton = document.createElement('td');
                    let renovateBtn = document.createElement('button');
                    renovateBtn.innerHTML = `<i data-feather='refresh-ccw'></i>`;
                    renovateBtn.classList.add('renovateBtn');
                    renovateBtn.setAttribute('key', room['name']);
                    renovateBtn.addEventListener('click', function (e) {
                        renovateRoom(this.getAttribute('key'));
                    });
                    tableDataRenovateButton.appendChild(renovateBtn);

                    newRow.appendChild(tableDataName);
                    newRow.appendChild(tableDataType);
                    newRow.appendChild(tableDataRenovation);
                    newRow.appendChild(tableDataDeleteButton);
                    newRow.appendChild(tableDataPutButton);
                    newRow.appendChild(tableDataRenovateButton);
                    table.appendChild(newRow);
                    feather.replace();
                }
                setUpFunctionality();
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/manager/rooms');
    request.send();
}

function setUpPage() {
    let hi = document.querySelector('#hi h1');
    hi.innerText += `${user.firstName} ${user.lastName}`;
    setUpRooms();
}

function setUpFunctionality() {
    setUpRenovations();
    setUpEquipment('empty');
    setUpTransfer();
    setUpDrugs();
}

// ComplexRenovations

var whichRenovation = document.getElementById('complexRenovation');
var devidePanel = document.getElementById('ifDevide');
var mergePanel = document.getElementById('ifMerge');
whichRenovation.addEventListener('change', function (e) {
    if (whichRenovation.value == '') {
        devidePanel.classList.add('off');
        mergePanel.classList.add('off');
    } else if (whichRenovation.value == 'devide') {
        devidePanel.classList.remove('off');
        mergePanel.classList.add('off');
    } else if (whichRenovation.value == 'merge') {
        mergePanel.classList.remove('off');
        devidePanel.classList.add('off');
    }
});

function setUpRenovations() {
    let devideRoomSelect = document.getElementById('complexDevide');
    let mergeRoom1Select = document.getElementById('complexMerge1');
    let mergeRoom2Select = document.getElementById('complexMerge2');
    devideRoomSelect.innerHTML = '';
    mergeRoom1Select.innerHTML = '';
    mergeRoom2Select.innerHTML = '';
    for (let room of mainResponse) {
        if (room['name'] != 'Main warehouse') {
            let newOption = document.createElement('option');
            newOption.setAttribute('value', room['name']);
            newOption.innerText = room['name'];
            devideRoomSelect.appendChild(newOption);
            mergeRoom1Select.appendChild(newOption.cloneNode(true));
            mergeRoom2Select.appendChild(newOption.cloneNode(true));
        }
    }
    let formDevide = devidePanel.querySelector('form');
    let formMerge = mergePanel.querySelector('form');
    formDevide.addEventListener('submit', makeDevide);
    formMerge.addEventListener('submit', makeMerge);
}

function makeDevide(e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    let devideRequest = new XMLHttpRequest();

    devideRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert('Selected room is schedule for renovation');
                setUpRooms();
            } else {
                alert('Error: Selected room cannot be renovated during this period');
            }
        }
    }

    let finalRoom = document.getElementById('complexDevide').value;
    let finalFromDate = document.getElementById('fromComplexRenovation').value;
    let finalToDate = document.getElementById('toComplexRenovation').value;

    if (isDateFormatOk(finalFromDate) && isDateFormatOk(finalToDate) && finalFromDate >= date.toISOString().split('T')[0] && finalToDate > finalFromDate) {
        devideRequest.open('POST', 'https://localhost:7291/api/manager/renovationdevide');
        devideRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
        devideRequest.send(JSON.stringify({ 'room': finalRoom, 'startDate': finalFromDate, 'endDate': finalToDate }));
    } else {
        alert('Error: Dates were not entered correctly');
    }
}

function makeMerge(e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    let mergeRequest = new XMLHttpRequest();

    mergeRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert('Selected rooms are schedule for renovation');
                setUpRooms();
            } else {
                alert('Error: Selected rooms cannot be renovated during this period');
            }
        }
    }

    let finalRoom1 = document.getElementById('complexMerge1').value;
    let finalRoom2 = document.getElementById('complexMerge2').value;
    let finalFromDate = document.getElementById('fromComplexRenovation1').value;
    let finalToDate = document.getElementById('toComplexRenovation1').value;

    if (finalRoom1 != finalRoom2 && isDateFormatOk(finalFromDate) && isDateFormatOk(finalToDate) && finalFromDate >= date.toISOString().split('T')[0] && finalToDate > finalFromDate) {
        mergeRequest.open('POST', 'https://localhost:7291/api/manager/renovationmerge');
        mergeRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
        mergeRequest.send(JSON.stringify({ 'room1': finalRoom1, 'room2': finalRoom2, 'startDate': finalFromDate, 'endDate': finalToDate }));
    } else {
        alert('Error: Informations were not entered correctly');
    }
}

// Equipment Managment

function setUpEquipment(myFilter) {
    let equipmentTable = document.getElementById('equipmentTable');
    equipmentTable.innerHTML = '';
    for (let room of mainResponse) {
        for (let item of room['equipment']) {
            if (myFilter.includes('term')) {
                let tokens = myFilter.split('&');
                let filterValue;
                for (let token of tokens) {
                    if (token.includes('term')) {
                        filterValue = token.split('|')[1];
                        break;
                    }
                }
                if (!(item['type'].includes(filterValue) || item['quantity'] == filterValue || item['name'].includes(filterValue) || room['name'].includes(filterValue))) {
                    continue;
                }
            }
            if (myFilter.includes('space')) {
                let tokens = myFilter.split('&');
                let filterValue;
                for (let token of tokens) {
                    if (token.includes('space')) {
                        filterValue = token.split('|')[1];
                        break;
                    }
                }
                if (room['type'] != filterValue) {
                    continue;
                }
            }
            if (myFilter.includes('equipment')) {
                let tokens = myFilter.split('&');
                let filterValue;
                for (let token of tokens) {
                    if (token.includes('equipment')) {
                        filterValue = token.split('|')[1];
                        break;
                    }
                }
                if (item['type'] != filterValue) {
                    continue;
                }
            }
            if (myFilter.includes('quantity')) {
                let tokens = myFilter.split('&');
                let filterValue;
                for (let token of tokens) {
                    if (token.includes('quantity')) {
                        filterValue = token.split('|')[1];
                        break;
                    }
                }
                if (filterValue.includes('-')) {
                    let x = filterValue.split('-');
                    if (item['quantity'] < x[0] || item['quantity'] > x[1]) {
                        continue;
                    }
                } else if (filterValue.includes('+')) {
                    let x = filterValue.split('+');
                    if (item['quantity'] < x[0]) {
                        continue;
                    }
                } else if (item['quantity'] != 0) {
                    continue;
                }
            }

            let newRow = document.createElement('tr');

            let tableDataName = document.createElement('td');
            tableDataName.innerText = item["name"];
            let tableDataType = document.createElement('td');
            tableDataType.innerText = item['type'];
            let cQuantity = document.createElement('td');
            cQuantity.innerText = item['quantity'];
            let cRoom = document.createElement('td');
            cRoom.innerText = room['name'];

            newRow.appendChild(tableDataName);
            newRow.appendChild(tableDataType);
            newRow.appendChild(cQuantity);
            newRow.appendChild(cRoom);
            equipmentTable.appendChild(newRow);
        }
    }
}

var equipmentSearchFilter = document.getElementById('filterSearch');
var equipmentRoomTypeFilter = document.getElementById('filterRoomType');
var equipmentQuantityFilter = document.getElementById('filterEquipmentQuantity');
var equipmentTypeFilter = document.getElementById('filterEquipmentType');

equipmentSearchFilter.addEventListener('input', updateEquipmentTable);
equipmentRoomTypeFilter.addEventListener('change', updateEquipmentTable);
equipmentQuantityFilter.addEventListener('change', updateEquipmentTable);
equipmentTypeFilter.addEventListener('change', updateEquipmentTable);

function updateEquipmentTable(e) {
    e.preventDefault();

    let finalFilter = '';
    let filter0 = equipmentSearchFilter.value;
    let filter1 = equipmentRoomTypeFilter.value;
    let filter2 = equipmentQuantityFilter.value;
    let filter3 = equipmentTypeFilter.value;

    if (filter0) {
        finalFilter += `term|${filter0}&`;
    }
    if (filter1) {
        finalFilter += `space|${filter1}&`;
    }
    if (filter2) {
        finalFilter += `quantity|${filter2}&`;
    }
    if (filter3) {
        finalFilter += `equipment|${filter3}&`;
    }
    if (finalFilter.endsWith('&')) {
        finalFilter = finalFilter.slice(0, -1);
    }
    if (!finalFilter) {
        finalFilter = 'empty';
    }
    setUpEquipment(finalFilter);
}

// Transfer
var room1 = document.getElementById('transfer1');
var room2 = document.getElementById('transfer2');
var transferContainer = document.getElementById('transferOptions');

function setUpTransfer() {
    room1.innerHTML = '';
    room2.innerHTML = '';
    for (let room of mainResponse) {
        let newOption = document.createElement('option');
        newOption.setAttribute('value', room['name']);
        newOption.innerText = room['name'];
        room1.appendChild(newOption);
        room2.appendChild(newOption.cloneNode(true));
    }
    showSelectedRoomEquipment();
}

room1.addEventListener('change', showSelectedRoomEquipment);
room2.addEventListener('change', showSelectedRoomEquipment);

function showSelectedRoomEquipment() {
    transferContainer.innerHTML = '';
    let roomFrom;
    let roomTo;
    for (let room of mainResponse) {
        if (room['name'] == room1.value) {
            roomFrom = room;
        }
        if (room['name'] == room2.value) {
            roomTo = room;
        }
    }
    for (let index1 in roomFrom['equipment']) {
        let index2 = -1;
        for (let j in roomTo['equipment']) {
            if (roomFrom['equipment'][index1]['name'] == roomTo['equipment'][j]['name']) {
                index2 = j;
                break;
            }
        }
        let inRoom1 = roomFrom['equipment'][index1]['quantity'];
        let inRoom2;
        if (index2 != -1) {
            inRoom2 = roomTo['equipment'][index2]['quantity'];
        } else {
            inRoom2 = 0;
        }
        let equipmentField = document.createElement('p');
        equipmentField.innerText = `${roomFrom['equipment'][index1]['name']} ( room1: ${inRoom1} | room2: ${inRoom2} )`;
        equipmentField.setAttribute('equipmentType', roomFrom['equipment'][index1]['type'])
        equipmentField.classList.add('pushLeft');
        let quantityField = document.createElement('input');
        quantityField.setAttribute('type', 'text');
        quantityField.setAttribute('autocomplete', 'off')
        quantityField.setAttribute('placeholder', 'Enter how much to transfer ( max ' + inRoom1 + ' )');
        transferContainer.appendChild(equipmentField);
        transferContainer.appendChild(quantityField);
    }
}

var transferForm = document.getElementById('transferForm');
transferForm.addEventListener('submit', function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    let transferRequest = new XMLHttpRequest();

    transferRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert('Transfer sucessfuly scheduled');
                setUpRooms();
            } else {
                alert('Error: Entered room informations are invalid');
            }
        }
    }

    let finalRoom1 = room1.value;
    let finalRoom2 = room2.value;
    let finalDate = document.getElementById('transferDate').value;
    let ok = true;
    let arr = []

    let children = transferContainer.children
    for (let i = 0; i < children.length; i += 2) {
        if (children[i + 1].value) {
            if (+children[i + 1].getAttribute('placeholder').split(' ')[7] >= +children[i + 1].value) {
                let el = {
                    name: children[i].innerText.split(' ')[0],
                    type: children[i].getAttribute('equipmentType'),
                    quantity: children[i + 1].value
                };
                arr.push(el);
            } else {
                ok = false;
                break;
            }
        }
    }

    if (ok && finalRoom1 != finalRoom2 && arr.length != 0 && isDateFormatOk(finalDate) && finalDate >= date.toISOString().split('T')[0]) {
        transferRequest.open('POST', 'https://localhost:7291/api/manager/transfer');
        transferRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
        transferRequest.send(JSON.stringify({ 'room1': finalRoom1, 'room2': finalRoom2, 'date': finalDate, 'done': false, 'equipment': arr }));
    } else {
        alert('Error: Transfer informations invalide');
    }
});

// Drugs
function setUpDrugs() {
    let request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                let allDrugs = JSON.parse(this.responseText);
                let table = document.getElementById('drugTable');
                table.innerHTML = '';
                for (let drug of allDrugs) {
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
                    let tableDataStatus = document.createElement('td');
                    tableDataStatus.innerText = drug['status'];

                    let tableDataDeleteButton = document.createElement('td');
                    let delBtn = document.createElement('button');
                    delBtn.innerHTML = `<i data-feather='trash'></i>`;
                    delBtn.classList.add('delBtn');
                    delBtn.setAttribute('key', drug['name']);
                    delBtn.addEventListener('click', function (e) {
                        deleteDrug(this.getAttribute('key'));
                    });
                    tableDataDeleteButton.appendChild(delBtn);

                    let tableDataPutButton = document.createElement('td');
                    let putBtn = document.createElement('button');
                    putBtn.innerHTML = `<i data-feather='edit-2'></i>`;
                    putBtn.classList.add('updateBtn');
                    putBtn.setAttribute('key', drug['name']);
                    putBtn.setAttribute('ingredients', drug['ingredients']);
                    try {
                        if (drug['comment'] != null) {
                            putBtn.setAttribute('comment', drug['comment']);
                        } else {
                            putBtn.setAttribute('comment', 'Doktor još uvek nije ostavio komentar');
                        }
                    } catch {
                        putBtn.setAttribute('comment', 'Doktor još uvek nije ostavio komentar');
                    }
                    putBtn.addEventListener('click', function (e) {
                        updateDrug(this.getAttribute('key'), this.getAttribute('ingredients'), this.getAttribute('comment'));
                    });
                    tableDataPutButton.appendChild(putBtn);

                    newRow.appendChild(tableDataName);
                    newRow.appendChild(tableDataIngredients);
                    newRow.appendChild(tableDataStatus);
                    newRow.appendChild(tableDataDeleteButton);
                    newRow.appendChild(tableDataPutButton);
                    table.appendChild(newRow);
                    feather.replace();
                    getIngredients();
                }
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/manager/drugs');
    request.send();
}

// POST - Drug
var ingredients;
var createDrugBtn = document.getElementById('addDrugBtn');
createDrugBtn.addEventListener('click', function (e) {
    let prompt = document.getElementById('createDrugPrompt');
    prompt.classList.remove('off');
    main.classList.add('hideMain');

    let ingredientsContainer = document.getElementById('selectIngredients');
    ingredientsContainer.innerHTML = '';
    for (let ingredient of ingredients) {
        let ingredientDiv = document.createElement('div');
        let ingredientLabel = document.createElement('label');
        ingredientLabel.innerText = ingredient;
        ingredientLabel.setAttribute('for', ingredient);
        let ingredientBox = document.createElement('input');
        ingredientBox.setAttribute('type', 'checkbox');
        ingredientBox.setAttribute('value', ingredient);
        ingredientBox.setAttribute('name', ingredient);
        ingredientDiv.appendChild(ingredientLabel);
        ingredientDiv.appendChild(ingredientBox);
        ingredientsContainer.appendChild(ingredientDiv);
    }

    let form = document.getElementById('createDrugForm');

    form.addEventListener('submit', function (e) {
        prompt.classList.add('off');
        main.classList.remove('hideMain');
        e.preventDefault();
        e.stopImmediatePropagation();
        let postRequest = new XMLHttpRequest();

        postRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    alert('New drug sucessfuly created');
                    setUpDrugs();
                } else {
                    alert('Error: Entered drug informations are invalid');
                }
            }
        }

        let finalIngredients = []
        let finalName = document.getElementById('createDrugName').value;
        let ingredientBoxes = document.querySelectorAll('#createDrugForm [type="checkbox"]');
        for (box of ingredientBoxes) {
            if (box.checked) {
                finalIngredients.push(box.value);
            }
        }

        if (finalName && finalIngredients.length != 0) {
            postRequest.open('POST', 'https://localhost:7291/api/manager/drugs');
            postRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
            postRequest.send(JSON.stringify({ 'name': finalName, 'ingredients': finalIngredients, 'status': 'inReview' }));
        } else {
            alert("Error: Name can't be empty")
        }
    });
});

function getIngredients() {
    let getIngredientsRequest = new XMLHttpRequest();

    getIngredientsRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                ingredients = JSON.parse(this.responseText)['ingredients'];
                let table = document.getElementById('ingredientTable');
                table.innerHTML = '';
                for (let ingredient of ingredients) {
                    let newRow = document.createElement('tr');

                    let tableDataName = document.createElement('td');
                    tableDataName.innerText = ingredient;

                    let tableDataDeleteButton = document.createElement('td');
                    let delBtn = document.createElement('button');
                    delBtn.innerHTML = `<i data-feather='trash'></i>`;
                    delBtn.classList.add('delBtn');
                    delBtn.setAttribute('key', ingredient);
                    delBtn.addEventListener('click', function (e) {
                        deleteIngredient(this.getAttribute('key'));
                    });
                    tableDataDeleteButton.appendChild(delBtn);

                    let tableDataPutButton = document.createElement('td');
                    let putBtn = document.createElement('button');
                    putBtn.innerHTML = `<i data-feather='edit-2'></i>`;
                    putBtn.classList.add('updateBtn');
                    putBtn.setAttribute('key', ingredient);
                    putBtn.addEventListener('click', function (e) {
                        updateIngredient(this.getAttribute('key'));
                    });
                    tableDataPutButton.appendChild(putBtn);

                    newRow.appendChild(tableDataName);
                    newRow.appendChild(tableDataDeleteButton);
                    newRow.appendChild(tableDataPutButton);
                    table.appendChild(newRow);
                    feather.replace();
                }
            } else {
                alert("Error: Ingredients couldn't be supplied");
            }
        }
    }

    getIngredientsRequest.open('GET', 'https://localhost:7291/api/manager/ingredients')
    getIngredientsRequest.send();
}

// CREATE - Ingredient
var createIngredientBtn = document.getElementById('addIngredientBtn');
createIngredientBtn.addEventListener('click', function (e) {

    let prompt = document.getElementById('createIngredientPrompt');
    prompt.classList.remove('off');
    main.classList.add('hideMain');

    let form = document.getElementById('createIngredientForm');

    form.addEventListener('submit', function (e) {
        prompt.classList.add('off');
        main.classList.remove('hideMain');
        e.preventDefault();
        e.stopImmediatePropagation();
        let postRequest = new XMLHttpRequest();

        postRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    alert('New ingredinet sucessfuly created');
                    getIngredients();
                } else {
                    alert('Error: Entered ingredient informations are invalid');
                }
            }
        }

        let finalName = document.getElementById('createIngredientName').value;

        if (finalName) {
            postRequest.open('POST', 'https://localhost:7291/api/manager/ingredients');
            postRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
            postRequest.send(JSON.stringify({ 'name': finalName }));
        } else {
            alert("Error: Name can't be empty")
        }
    });
});

// PUT - Drug
function updateDrug(key, myIngredients, comment) {
    let prompt = document.getElementById('updateDrugPrompt');
    prompt.classList.remove('off');
    main.classList.add('hideMain');
    let whichDrug = prompt.querySelector('h1');
    let message = prompt.querySelector('span');
    let nameField = document.getElementById('updateDrugName');
    nameField.setAttribute('placeholder', key);
    message.innerText = `Message: ${comment}`;
    whichDrug.innerText = `Update ${key}`;

    let ingredientsContainer = document.getElementById('updateSelectIngredients');
    ingredientsContainer.innerHTML = '';
    for (let ingredient of ingredients) {
        let ingredientDiv = document.createElement('div');
        let ingredientLabel = document.createElement('label');
        ingredientLabel.innerText = ingredient;
        ingredientLabel.setAttribute('for', ingredient);
        let ingredientBox = document.createElement('input');
        ingredientBox.setAttribute('type', 'checkbox');
        ingredientBox.setAttribute('value', ingredient);
        ingredientBox.setAttribute('name', ingredient);
        if (myIngredients.split(',').includes(ingredient)) {
            ingredientBox.setAttribute('checked', true);
        }
        ingredientDiv.appendChild(ingredientLabel);
        ingredientDiv.appendChild(ingredientBox);
        ingredientsContainer.appendChild(ingredientDiv);
    }

    let form = document.getElementById('updateDrugForm');

    form.addEventListener('submit', function (e) {
        prompt.classList.add('off');
        main.classList.remove('hideMain');
        e.preventDefault();
        e.stopImmediatePropagation();
        let putRequest = new XMLHttpRequest();

        putRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    alert('Drug sucessfuly updated and passed to review');
                    location.reload();
                    setUpDrugs();
                    showWindow(3);
                } else {
                    alert('Error: Entered drug informations are invalid');
                }
            }
        }

        let finalIngredients = []
        let finalName = nameField.value;
        let ingredientBoxes = document.querySelectorAll('#updateDrugForm [type="checkbox"]');
        for (box of ingredientBoxes) {
            if (box.checked) {
                finalIngredients.push(box.value);
            }
        }

        if (finalName && finalIngredients.length != 0) {
            putRequest.open('PUT', 'https://localhost:7291/api/manager/drugs/' + key);
            putRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
            putRequest.send(JSON.stringify({ 'name': finalName, 'ingredients': finalIngredients, 'status': 'inReview' }));
        } else {
            alert("Error: Name can't be empty nor can there be a drug without any ingredients")
        }
    });
}

// PUT - Ingredient
function updateIngredient(key) {

    let prompt = document.getElementById('updateIngredientPrompt');
    prompt.classList.remove('off');
    main.classList.add('hideMain');
    let whichIngredient = prompt.querySelector('h1');
    let nameField = document.getElementById('updateIngredientName');
    nameField.setAttribute('placeholder', key);
    whichIngredient.innerText = `Update ${key}`;

    let form = document.getElementById('updateIngredientForm');

    form.addEventListener('submit', function (e) {
        prompt.classList.add('off');
        main.classList.remove('hideMain');
        e.preventDefault();
        e.stopImmediatePropagation();
        let putRequest = new XMLHttpRequest();

        putRequest.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    alert('Ingredinet sucessfuly updated');
                    location.reload();
                    getIngredients();
                    showWindow(3);
                } else {
                    alert('Error: Entered ingredient informations are invalid');
                }
            }
        }

        let finalName = document.getElementById('updateIngredientName').value;

        if (finalName) {
            putRequest.open('PUT', 'https://localhost:7291/api/manager/ingredients/' + key);
            putRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
            putRequest.send(JSON.stringify({ 'name': finalName }));
        } else {
            alert("Error: Name can't be empty")
        }
    });
}

// DELETE - Drug
function deleteDrug(key) {
    let deleteRequest = new XMLHttpRequest();

    deleteRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert('Selected drug was successfully deleted');
                setUpDrugs();
            } else {
                alert("Error: Selected drug couldn't be deleted");
            }
        }
    }

    deleteRequest.open('DELETE', 'https://localhost:7291/api/manager/drugs/' + key)
    deleteRequest.send();
}

// DELETE - Ingredient
function deleteIngredient(key) {
    let deleteRequest = new XMLHttpRequest();

    deleteRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert('Selected drug was successfully deleted');
                getIngredients();
            } else {
                alert("Error: Selected drug couldn't be deleted");
            }
        }
    }

    deleteRequest.open('DELETE', 'https://localhost:7291/api/manager/ingredients/' + key)
    deleteRequest.send();
}
