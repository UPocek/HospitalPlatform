<!DOCTYPE html>
<html lang="en" dir="ltr">

<head>
  <meta charset="utf-8">
  <meta name="description" content="USI medical institution team #9">
  <meta name="author" content="Tamara Ilic, Uros Pocek, Tamara Dzambic, Marko Erdelji">
  <meta name="keywords" content="usi">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">

  <link rel="icon" href="logo.jpeg">

  <link rel="stylesheet" type="text/css" href="style.css">
  <script src="https://kit.fontawesome.com/b4c27ec53d.js" crossorigin="anonymous"></script>

  <title>USI Team #9</title>

</head>

<body>
    <?php include 'header.html';?>

  <main id="first" class="myForm">
      <h1 id="hi" class="mainH">Medical Institution Team #9</h1>
    <div class="container container1" id="container">
      <div class="form-container sign-up-container">
        <form id="signUpForm">
          <h1>Create Account</h1>
          <div class="social-container">
            <a href="#" class="social"><i class="fab fa-facebook-f"></i></a>
            <a href="#" class="social"><i class="fab fa-google-plus-g"></i></a>
            <a href="#" class="social"><i class="fab fa-linkedin-in"></i></a>
          </div>
          <input id="signUpName" type="text" placeholder="Name" />
          <input id="signUpEmail" type="email" placeholder="Email" />
          <input id="signUpPassword" type="password" placeholder="Password" />
          <button class="mainBtn">Sign Up</button>
        </form>
      </div>
      <div class="form-container sign-in-container">
        <form id="signInForm">
          <h1>Sign in</h1>
          <div class="social-container">
            <a href="#" class="social"><i class="fab fa-facebook-f"></i></a>
            <a href="#" class="social"><i class="fab fa-google-plus-g"></i></a>
            <a href="#" class="social"><i class="fab fa-linkedin-in"></i></a>
          </div>
          <input id="signInEmail" type="email" placeholder="Email" />
          <input id="signInPassword" type="password" placeholder="Password" />
          <a href="#">Forgot your password?</a>
          <button class="mainBtn">Sign In</button>
        </form>
      </div>
      <div class="overlay-container">
        <div class="overlay">
          <div class="overlay-panel overlay-left">
                <h1>Hello, Friend!</h1>
                <p>Enter your personal details and levrage power of our app</p>
                <button class="mainBtn ghost" id="signIn">Sign In</button>
          </div>
          <div class="overlay-panel overlay-right">
                <h1>Welcome Back!</h1>
                <p>Enter your's personal information to login into your account</p>
                <button class="mainBtn ghost" id="signUp">Sign Up</button>
          </div>
        </div>
      </div>
    </div>

    <div class="container2" id="container">
      <div>
        <form id="signInForm2">
          <h1>Sign in</h1>
          <div class="social-container">
            <a href="#" class="social"><i class="fab fa-facebook-f"></i></a>
            <a href="#" class="social"><i class="fab fa-google-plus-g"></i></a>
            <a href="#" class="social"><i class="fab fa-linkedin-in"></i></a>
          </div>
          <input id="signInEmail2" type="email" placeholder="Email" />
          <input id="signInPassword2" type="password" placeholder="Password" />
          <a href="#">Forgot your password?</a>
          <button class="mainBtn">Sign In</button>
        </form>
      </div>
    </div>

  </main>

  <?php include 'footer.html';?>

  <script src="app.js"></script>
  
  <script>
    const signUpButton = document.getElementById('signUp');
    const signInButton = document.getElementById('signIn');
    const container = document.getElementById('container');

    signUpButton.addEventListener('click', () => {
        container.classList.add("right-panel-active");
    });

    signInButton.addEventListener('click', () => {
        container.classList.remove("right-panel-active");
    });
  </script>
</body>

</html>