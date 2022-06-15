var jwtoken;
var loginForm;
url = 'https://localhost:7291/'

if (window.innerWidth > 768) {
    loginForm = document.getElementById('signInForm');
}
else {
    loginForm = document.getElementById("signInForm2");
}

// Login
loginForm.addEventListener('submit', function (e) {
    e.preventDefault();
    let loginRequest = new XMLHttpRequest();

    loginRequest.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                let response = JSON.parse(this.responseText);
                jwtoken = response['token'];
                let user = response['user'];
                if (user.role == 'manager') {
                    window.location.replace('manager.php' + '?id=' + user.id + '&token=' + jwtoken);
                }
                else if (user.role == 'secretary') {
                    window.location.replace('secretary.php' + '?id=' + user.id + '&token=' + jwtoken);
                }
                else if (user.role == 'patient') {
                    window.location.replace('patient.php' + '?id=' + user.id + '&token=' + jwtoken);
                }
                else if (user.role == 'doctor') {
                    window.location.replace('doctor.php' + '?id=' + user.id + '&token=' + jwtoken);
                }
            }
            else {
                alert('Invalid username or password');
            }
        }
    }

    if (window.innerWidth > 768) {
        var finalEmail = document.getElementById('signInEmail').value;
        var finalPassword = document.getElementById('signInPassword').value;
    }
    else {
        var finalEmail = document.getElementById("signInEmail2").value;
        var finalPassword = document.getElementById("signInPassword2").value;
    }

    loginRequest.open('POST', url + 'api/user/login/' + finalEmail + '&' + finalPassword);
    loginRequest.send();
});

const signUpButton = document.getElementById('signUp');
const signInButton = document.getElementById('signIn');
const container = document.getElementById('container');

signUpButton.addEventListener('click', () => {
    container.classList.add("right-panel-active");
});

signInButton.addEventListener('click', () => {
    container.classList.remove("right-panel-active");
});
