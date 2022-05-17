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
                    let tableDataPutButton = document.createElement('td');
                    let tableDataRenovateButton = document.createElement('td');
                    if (room['type'] != 'warehouse') {
                        let delBtn = document.createElement('button');
                        delBtn.innerHTML = `<i data-feather='trash'></i>`;
                        delBtn.classList.add('delBtn');
                        delBtn.setAttribute('key', room['name']);
                        delBtn.addEventListener('click', function (e) {
                            deleteRoom(this.getAttribute('key'));
                        });
                        tableDataDeleteButton.appendChild(delBtn);

                        let putBtn = document.createElement('button');
                        putBtn.innerHTML = `<i data-feather='edit-2'></i>`;
                        putBtn.classList.add('updateBtn');
                        putBtn.setAttribute('key', room['name']);
                        putBtn.addEventListener('click', function (e) {
                            updateRoom(this.getAttribute('key'));
                        });
                        tableDataPutButton.appendChild(putBtn);

                        let renovateBtn = document.createElement('button');
                        renovateBtn.innerHTML = `<i data-feather='refresh-ccw'></i>`;
                        renovateBtn.classList.add('renovateBtn');
                        renovateBtn.setAttribute('key', room['name']);
                        renovateBtn.addEventListener('click', function (e) {
                            renovateRoom(this.getAttribute('key'));
                        });
                        tableDataRenovateButton.appendChild(renovateBtn);
                    }

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
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}

function setUpFunctionality() {
    setUpRenovations();
    setUpEquipment('empty');
    setUpTransfer();
    setUpDrugs();
    setUpIngredients();
    setUpCharts();
}

// Room management

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
            postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
            postRequest.send(JSON.stringify({ 'name': finalName, 'type': finalType, 'inRenovation': false, 'equipment': [] }));
        } else {
            alert("Error: Name can't be empty!");
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
                    setUpRooms();
                } else {
                    alert('Error: Selected room cannot be renovated during this period');
                }
            }
        }

        let finalFromDate = document.getElementById('fromRenovation').value;
        let finalToDate = document.getElementById('toRenovation').value;

        if (areDatesValid(finalFromDate, finalToDate)) {
            postRequest.open('POST', 'https://localhost:7291/api/manager/renovations');
            postRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
            postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
            postRequest.send(JSON.stringify({ 'room': key, 'startDate': finalFromDate, 'endDate': finalToDate, 'done': false, 'kind': 'simple' }));
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
            putRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
            putRequest.send(JSON.stringify({ 'name': finalName, 'type': finalType, 'inRenovation': false, 'equipment': [] }));
        } else {
            alert("Error: Name can't be empty!");
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

    deleteRequest.open('DELETE', 'https://localhost:7291/api/manager/rooms/' + key);
    deleteRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    deleteRequest.send();
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

    if (areDatesValid(finalFromDate, finalToDate)) {
        devideRequest.open('POST', 'https://localhost:7291/api/manager/renovationdevide');
        devideRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
        devideRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
        devideRequest.send(JSON.stringify({ 'room': finalRoom, 'startDate': finalFromDate, 'endDate': finalToDate, 'done': false, 'kind': 'devide' }));
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
        mergeRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
        mergeRequest.send(JSON.stringify({ 'room': finalRoom1, 'room2': finalRoom2, 'startDate': finalFromDate, 'endDate': finalToDate, 'done': false, 'kind': 'merge' }));
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
            // Filter by search term
            if (myFilter.includes('term')) {
                let tokens = myFilter.split('&');
                let filterValue;
                for (let token of tokens) {
                    if (token.includes('term')) {
                        filterValue = token.split('|')[1];
                        break;
                    }
                }
                let searchTermInQuery = (item['type'].includes(filterValue) || item['quantity'] == filterValue || item['name'].includes(filterValue) || room['name'].includes(filterValue))
                if (!searchTermInQuery) {
                    continue;
                }
            }
            // Filter by room type
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
            // Filter by equipment type
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
            // Filter by equipment quantity
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

            // Display items that satisfy given criterion
            let newRow = document.createElement('tr');

            let tableDataName = document.createElement('td');
            tableDataName.innerText = item['name'];
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

    let validTransfer = ok && finalRoom1 != finalRoom2 && arr.length != 0 && isDateFormatOk(finalDate) && finalDate >= date.toISOString().split('T')[0];
    if (validTransfer) {
        transferRequest.open('POST', 'https://localhost:7291/api/manager/transfer');
        transferRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
        transferRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
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
                }
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/manager/drugs');
    request.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    request.send();
}

// POST - Drug
var ingredients;
var createDrug = document.getElementById('addDrugBtn');
createDrug.addEventListener('click', function (e) {
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
            postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
            postRequest.send(JSON.stringify({ 'name': finalName, 'ingredients': finalIngredients, 'status': 'inReview' }));
        } else {
            alert("Error: Name can't be empty");
        }
    });
});

function setUpIngredients() {
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

    getIngredientsRequest.open('GET', 'https://localhost:7291/api/manager/ingredients');
    getIngredientsRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
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
                    setUpIngredients();
                } else {
                    alert('Error: Entered ingredient informations are invalid');
                }
            }
        }

        let finalName = document.getElementById('createIngredientName').value;

        if (finalName) {
            postRequest.open('POST', 'https://localhost:7291/api/manager/ingredients');
            postRequest.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
            postRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
            postRequest.send(JSON.stringify({ 'name': finalName }));
        } else {
            alert("Error: Name can't be empty");
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
            putRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
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
                    setUpIngredients();
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
            putRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
            putRequest.send(JSON.stringify({ 'name': finalName }));
        } else {
            alert("Error: Name can't be empty");
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

    deleteRequest.open('DELETE', 'https://localhost:7291/api/manager/drugs/' + key);
    deleteRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    deleteRequest.send();
}

// DELETE - Ingredient
function deleteIngredient(key) {
    let deleteRequest = new XMLHttpRequest();

    deleteRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                alert('Selected drug was successfully deleted');
                setUpIngredients();
            } else {
                alert("Error: Selected drug couldn't be deleted");
            }
        }
    }

    deleteRequest.open('DELETE', 'https://localhost:7291/api/manager/ingredients/' + key);
    deleteRequest.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    deleteRequest.send();
}

// Polls
function setUpCharts() {

    let getRequestHospital = new XMLHttpRequest();
    let getRequestDoctors = new XMLHttpRequest();

    getRequestHospital.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                let container = document.getElementById('hospitalPolls');
                let charts = { 'hygiene': [], 'courtesyOfStaff': [], 'staffExpertise': [], 'efficiency': [], 'equipment': [], 'serviceSatisfaction': [] };
                let comments = [];

                let polls = JSON.parse(this.responseText)['polls'];

                for (let poll of polls) {
                    let score = 0;
                    for (let field in poll) {
                        if (field != 'comment') {
                            score += +poll[field];
                            charts[field].push(poll[field]);
                        } else {
                            comments.push([poll[field], Math.round(score / Object.keys(charts).length * 100) / 100]);
                        }
                    }
                }

                for (key in charts) {

                    let avgScore = Math.round(charts[key].reduce((partialSum, a) => +partialSum + +a, 0) / charts[key].length * 100) / 100;

                    let data = {
                        labels: ['1', '2', '3', '4', '5'],
                        datasets: [{
                            label: `${key} - AVG = ${avgScore}`,
                            backgroundColor: '#FF416C',
                            borderColor: '#FF416C',
                            data: [charts[key].filter(x => x == 1).length, charts[key].filter(x => x == 2).length, charts[key].filter(x => x == 3).length, charts[key].filter(x => x == 4).length, charts[key].filter(x => x == 5).length],
                        }]
                    };

                    let config = {
                        type: 'bar',
                        data: data,
                        options: {
                            parsing: {
                                xAxisKey: 'Score',
                                yAxisKey: 'Number of votes'
                            }
                        }
                    };

                    let chartContainer = document.createElement('div');
                    let chart = document.createElement('canvas');
                    chart.setAttribute('id', `${key}Chart`);

                    chartContainer.appendChild(chart);
                    container.appendChild(chartContainer);


                    let myChart = new Chart(
                        document.getElementById(`${key}Chart`),
                        config
                    );
                }

                let hTable = document.getElementById('hospitalPollsTable');
                for (let comment of comments) {
                    let newRow = document.createElement('tr');

                    let tableDataComment = document.createElement('td');
                    tableDataComment.innerText = comment[0];
                    tableDataComment.setAttribute('colspan', '2');
                    let tableDataScore = document.createElement('td');
                    tableDataScore.innerText = comment[1];

                    newRow.appendChild(tableDataComment);
                    newRow.appendChild(tableDataScore);
                    hTable.appendChild(newRow);
                }

            } else {
                alert("Error: Polls couldn't be acquired");
            }
        }
    }

    getRequestDoctors.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                let container = document.getElementById('doctorPolls');
                let doctors = JSON.parse(this.responseText);

                let scoreBoard = {}

                for (let doctor of doctors) {
                    let charts = { 'efficiency': [], 'expertise': [], 'communicativeness': [], 'kindness': [] };
                    let comments = [];
                    let finalScore = 0;
                    for (let poll of doctor["score"]) {
                        let score = 0;
                        for (let field in poll) {
                            if (field != 'comment') {
                                score += +poll[field];
                                charts[field].push(poll[field]);
                            } else {
                                let myScore = Math.round(score / Object.keys(charts).length * 100) / 100;
                                finalScore += myScore;
                                comments.push([poll[field], myScore]);
                            }
                        }
                    }


                    let data = {
                        labels: ['efficiency', 'expertise', 'communicativeness', 'kindness'],
                        datasets: [{
                            label: `${doctor['firstName']} ${doctor['lastName']} - ${doctor["score"].length} votes`,
                            backgroundColor: '#FF416C',
                            borderColor: '#FF416C',
                            data: [Math.round(charts['efficiency'].reduce((partialSum, a) => +partialSum + +a, 0) / charts['efficiency'].length * 100) / 100, Math.round(charts['expertise'].reduce((partialSum, a) => +partialSum + +a, 0) / charts['expertise'].length * 100) / 100, Math.round(charts['communicativeness'].reduce((partialSum, a) => +partialSum + +a, 0) / charts['communicativeness'].length * 100) / 100, Math.round(charts['kindness'].reduce((partialSum, a) => +partialSum + +a, 0) / charts['kindness'].length * 100) / 100]
                        }]
                    };

                    let config = {
                        type: 'bar',
                        data: data,
                        options: {}
                    };

                    let chartContainer = document.createElement('div');
                    let chart = document.createElement('canvas');
                    chart.setAttribute('id', `Chart${doctor["id"]}`);

                    chartContainer.appendChild(chart);
                    container.appendChild(chartContainer);


                    let myChart = new Chart(
                        document.getElementById(`Chart${doctor["id"]}`),
                        config
                    );


                    let dTable = document.getElementById('doctorPollsTable');
                    for (let comment of comments) {
                        let newRow = document.createElement('tr');

                        let tableDataWho = document.createElement('td');
                        tableDataWho.innerText = `${doctor['firstName']} ${doctor['lastName']}`;
                        let tableDataComment = document.createElement('td');
                        tableDataComment.innerText = comment[0];
                        let tableDataScore = document.createElement('td');
                        tableDataScore.innerText = comment[1];

                        newRow.appendChild(tableDataWho);
                        newRow.appendChild(tableDataComment);
                        newRow.appendChild(tableDataScore);
                        dTable.appendChild(newRow);
                    }
                    scoreBoard[`${doctor['firstName']} ${doctor['lastName']}`] = Math.round(finalScore / doctor["score"].length * 100) / 100;
                }
                let bestDoctorsContainer = document.getElementById('bestDoctors');
                let worstDoctorsContainer = document.getElementById('worstDoctors');
                // Create doctors array
                let items = Object.keys(scoreBoard).map(function (key) {
                    return [key, scoreBoard[key]];
                });

                // Sort the array based on the second element
                items.sort(function (first, second) {
                    return second[1] - first[1];
                });

                for (let el of items.slice(0, 3)) {
                    let doctorInfo = document.createElement('p');
                    doctorInfo.innerText = el[0];
                    bestDoctorsContainer.appendChild(doctorInfo);
                }

                items = items.reverse();
                for (let el of items.slice(0, 3)) {
                    let doctorInfo = document.createElement('p');
                    doctorInfo.innerText = el[0];
                    worstDoctorsContainer.appendChild(doctorInfo);
                }
            } else {
                alert("Error: Selected drug couldn't be deleted");
            }
        }
    }

    getRequestHospital.open('GET', 'https://localhost:7291/api/manager/polls');
    getRequestHospital.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    getRequestHospital.send();

    getRequestDoctors.open('GET', 'https://localhost:7291/api/manager/doctorpolls');
    getRequestDoctors.setRequestHeader('Authorization', 'Bearer ' + jwtoken);
    getRequestDoctors.send();
}
