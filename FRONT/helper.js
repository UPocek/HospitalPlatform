var loginForm = document.getElementById("signInForm2");

// Login
loginForm.addEventListener('submit', function (e) {
    e.preventDefault();
    let request = new XMLHttpRequest();

    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                let response = JSON.parse(this.responseText);
                if (response["role"] == "manager") {
                    window.location.replace("manager.php" + "?id=" + response["id"]);
                }
                else if (response["role"] == "patient") {
                    window.location.replace("patient.php" + "?id=" + response["id"]);
                }
                else if (response["role"] == "doctor") {
                    window.location.replace("doctor.php" + "?id=" + response["id"]);
                }
            }
            else {
                alert("Invalid username or password");
            }
        }
    }

    let finalEmail = document.getElementById("signInEmail2").value;
    let finalPassword = document.getElementById("signInPassword2").value;

    request.open('GET', 'https://localhost:7291/api/my/login/' + finalEmail + "&" + finalPassword);
    request.send();
});
