<!DOCTYPE html>
<html lang="en" dir="ltr">
    <head>
        <meta charset="utf-8">
        <meta name="description" content="USI medical institution team #9">
        <meta name="author" content="Tamara Ilic, Uros Pocek, Tamara Dzambic, Marko Erdelji">
        <meta name="keywords" content="usi">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">

        <link rel="icon" href="logo.jpeg">

        <link rel="stylesheet" type="text/css" href="doctor.css">
        <script src="https://kit.fontawesome.com/b4c27ec53d.js" crossorigin="anonymous"></script>

        <title>USI Team #9</title>

    </head>
    <body>
        
        <?php include 'header.html';?>

        <div class="container  off" id="container">
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

        <div id="examinationPopUp" class="form-container sign-in-container off prompt">
            <form id="examinationForm" class="colDir myForm">
                <h1 id="examinationFormId" >Create examination</h1>
                <div class="formDiv">
                    <label for="scheduleDate">Date:</label>
                    <input type="datetime-local" id="scheduleDate">
                </div>
                <div class="formDiv">
                    <label for="examinationType">Examination type:</label>
                    <select id="examinationType">
                        <option value="visit" selected>Visit</option>
                        <option value="operation">Operation</option>
                    </select>
                </div>
                <div class="formDiv">
                    <label for="examinationDuration">Duration:</label>
                    <input type="number" id="examinationDuration" min="15">
                </div>
                <div class="formDiv">
                    <label for="examinationRoom">Room:</label>
                    <select id="examinationRoom">
                        <!-- get rooms from api -->
                    </select>
                </div>
                <div class="formDiv">
                    <label for="examinationPatient">Patient id:</label>
                    <input id="examinationPatient" type="number"/>
                </div>
                <div class="urgentDiv">
                    <label for="urgent"> Urgent </label>
                    <input type="checkbox" id="urgent" value="urgent">
                    
                </div>
                <button class="mainBtn">OK</button>
            </form>
	    </div>

        <main>
            <div id="hi">
                <h1>Welcome back &nbsp; </h1>
            </div>
            <section id="one" class="active">
                <div class="tableHeaderDiv">
                    <div id="scheduleOption" class="hideMain">
                        <label for="scheduleDateOption">Schedule for date:</label>
                        <input type="date" id="scheduleDateOption">
                        <button id="scheduleDateBtn" class="send"><i data-feather="arrow-right-circle"></i></button>
                    </div>
                    <button id="addBtn" class="add"><i data-feather="plus-circle"></i></button>
                </div>
                <div id="rooms">
                    <div class="tbl-content">
                        <table cellpadding="0" cellspacing="0" border="0">
                            <thead>
                                <tr>
                                    <th>Date</th>
                                    <th>Duration</th>
                                    <th>Done</th>
                                    <th>Examination room</th>
                                    <th>Type</th>
                                    <th>Urgent</th>
                                    <th>Patient</th>
                                </tr>
                            </thead>
                            <tbody id="examinationsTable">
                                <!-- this is where data from api comes -->
                            </tbody>
                        </table>
                    </div>
                </div>
            </section>
        </main>

        <!-- <?php include 'footer.html';?> -->

        <script src="https://code.jquery.com/jquery-3.3.1.min.js"
            integrity="sha256-FgpCb/KJQlLNfOu91ta32o/NMZxltwRo8QtmkMRdAu8=" crossorigin="anonymous"></script>
        <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
        <script src="https://code.jquery.com/ui/1.12.0/jquery-ui.js"
            integrity="sha256-0YPKAwZP7Mp3ALMRVB2i8GXeEndvCq3eSl/WsAl1Ryk=" crossorigin="anonymous"></script>

        <script src="doctor.js">
        </script>
        <script>
            $(window).on("load resize ", function() {
            var scrollWidth = $('.tbl-content').width() - $('.tbl-content table').width();
            $('.tbl-header').css({'padding-right':scrollWidth});
            }).resize();
        </script>
    </body>
</html>