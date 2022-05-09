var loginForm = document.getElementById('signInForm');
var jwtoken;

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
                if (user['role'] == 'manager') {
                    window.location.replace('manager.php' + '?id=' + user['id'] + '&token=' + jwtoken);
                }
                else if (user['role'] == 'secretary') {
                    window.location.replace('secretary.php' + '?id=' + user['id'] + '&token=' + jwtoken);
                }
                else if (user['role'] == 'patient') {
                    window.location.replace('patient.php' + '?id=' + user['id'] + '&token=' + jwtoken);
                }
                else if (user['role'] == 'doctor') {
                    window.location.replace('doctor.php' + '?id=' + user['id'] + '&token=' + jwtoken);
                }
            }
            else {
                alert('Invalid username or password');
            }
        }
    }

    let finalEmail = document.getElementById('signInEmail').value;
    let finalPassword = document.getElementById('signInPassword').value;

    loginRequest.open('POST', 'https://localhost:7291/api/my/authenticate/' + finalEmail + '&' + finalPassword);
    loginRequest.send();
});
