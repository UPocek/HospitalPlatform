var loginForm = document.getElementById("signInForm");

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
            }
        }
    }

    let finalEmail = document.getElementById("signInEmail").value;
    let finalPassword = document.getElementById("signInPassword").value;

    request.open('GET', 'https://localhost:7291/api/my/login/' + finalEmail + "&" + finalPassword);
    request.send();
});
