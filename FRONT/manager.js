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

document.addEventListener("DOMContentLoaded", function () {
    let request = new XMLHttpRequest();

    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                let response = JSON.parse(this.responseText);
                user = new User(response);
            }
        }
    }

    request.open('GET', 'https://localhost:7291/api/my/users/' + id);
    request.send();
});