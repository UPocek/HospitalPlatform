var loginForm = document.getElementById("signInForm");

// Login
loginForm.addEventListener('submit', function (e) {
    e.preventDefault();
    let request = new XMLHttpRequest();

    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                var response = JSON.parse(this.responseText);
                console.log(response);
            }
        }
    }

    let finalEmail = document.getElementById("signInEmail").value;
    let finalPassword = document.getElementById("signInPassword").value;

    request.open('GET', 'https://localhost:7291/api/my/login/' + finalEmail + "&" + finalPassword);
    request.send();
});