<!DOCTYPE html>
<html lang="en" dir="ltr">

<head>
  <meta charset="utf-8">
  <meta name="description" content="USI medical institution team #9">
  <meta name="author" content="Tamara Ilic, Uros Pocek, Tamara Dzambic, Marko Erdelji">
  <meta name="keywords" content="usi">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">

  <!-- FONTS -->


  <!-- /FONTS -->

  <link rel="icon" href="black.jpg">

  <link rel="stylesheet" type="text/css" href="style.css">
  <script src="https://kit.fontawesome.com/b4c27ec53d.js" crossorigin="anonymous"></script>

  <title>USI Team #9</title>

</head>

<body>
    <?php include 'header.html';?>

  <main id="first">
      <h1 class="main-h">Medical Institution Team #9</h1>
<div class="container" id="container">
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
			<button class="main-btn">Sign Up</button>
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
			<button class="main-btn">Sign In</button>
		</form>
	</div>
	<div class="overlay-container">
		<div class="overlay">
			<div class="overlay-panel overlay-left">
            <h1>Hello, Friend!</h1>
            <p>Enter your personal details and levrage power of our app</p>
            <button class="main-btn ghost" id="signIn">Sign In</button>
			</div>
			<div class="overlay-panel overlay-right">
            <h1>Welcome Back!</h1>
            <p>Enter your's personal information to login into your account</p>
            
            <button class="main-btn ghost" id="signUp">Sign Up</button>
			</div>
		</div>
	</div>
</div>
  </main>

  <?php include 'footer.html';?>


  <!-- <link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.8.1/css/all.css" integrity="sha384-50oBUHEmvpQ+1lW4y57PTFmhCaXp0ML5d60M1M7uH2+nqUivzIebhndOJK28anvf" crossorigin="anonymous"> -->
  <script src="https://code.jquery.com/jquery-3.3.1.min.js"
    integrity="sha256-FgpCb/KJQlLNfOu91ta32o/NMZxltwRo8QtmkMRdAu8=" crossorigin="anonymous"></script>
  <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
  <script src="https://code.jquery.com/ui/1.12.0/jquery-ui.js"
    integrity="sha256-0YPKAwZP7Mp3ALMRVB2i8GXeEndvCq3eSl/WsAl1Ryk=" crossorigin="anonymous"></script>

  <script src="app.js">
  </script>
  
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