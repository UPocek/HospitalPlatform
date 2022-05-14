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
    <div id="createPatientPrompt" class="form-container sign-in-container off prompt patientPrompt">
      <form id="createPatientForm" class="colDir myForm patientForm">
        <h1 class="createPatientHeader">Create patient</h1>
        <input id="createPatientFirstName" type="text" placeholder="" />
        <input id="createPatientLastName" type="text" placeholder="" />
        <input id="createPatientEmail" type="text" placeholder="" />
        <input id="createPatientPassword" type="text" placeholder="" />
        <input id="createPatientHeight" type="number" max=300 min=0 placeholder="" />
        <input id="createPatientWeight" type="number" max=600 min=0 placeholder="" />
        <select name="createPatientBloodType" id="createPatientBloodType">
          <option value="A">A</option>
          <option value="A+">A+</option>
          <option value="A-">A-</option>
          <option value="B">B</option>
          <option value="B+">B+</option>
          <option value="B-">B-</option>
          <option value="AB">AB</option>
          <option value="AB+">AB+</option>
          <option value="AB-">AB-</option>
          <option value="O">O</option>
          <option value="O+">O+</option>
          <option value="O-">O-</option>
        </select>
        <button class="mainBtn">OK</button>
      </form>
	  </div>

    <div id="editPatientPrompt" class="form-container sign-in-container off prompt patientPrompt">
      <form id="editPatientForm" class="colDir myForm patientForm">
        <h1 class="editPatientHeader">Create patient</h1>
        <input id="editPatientFirstName" type="text" placeholder="" />
        <input id="editPatientLastName" type="text" placeholder="" />
        <input id="editPatientEmail" type="text" placeholder="" />
        <input id="editPatientPassword" type="text" placeholder="" />
        <input id="editPatientHeight" type="number" max=300 min=0 placeholder="" />
        <input id="editPatientWeight" type="number" max=600 min=0 placeholder="" />
        <select name="editPatientBloodType" id="editPatientBloodType">
          <option value="A">A</option>
          <option value="A+">A+</option>
          <option value="A-">A-</option>
          <option value="B">B</option>
          <option value="B+">B+</option>
          <option value="B-">B-</option>
          <option value="AB">AB</option>
          <option value="AB+">AB+</option>
          <option value="AB-">AB-</option>
          <option value="O">O</option>
          <option value="O+">O+</option>
          <option value="O-">O-</option>
        </select>
        <button class="mainBtn">OK</button>
      </form>
	  </div>

    <main class='myMain'>

    <div id="hi">
        <h1>Welcome back &nbsp; </h1>
    </div>
    <section id="one" class="active">
        <div class="plusDiv">
            <button id="addBtn" class="add"><i data-feather="plus-circle"></i></button>
        </div>
        <div id="patients">
            <div class="tbl-content">
                <table cellpadding="0" cellspacing="0" border="0" class="patientTable">
                  <thead>
                      <tr>
                        <th>First Name</th>
                        <th>Last Name</th>
                        <th>Email</th>
                        <th>Password</th>
                        <th>Id</th>
                        <th class="smallerWidth">Medical Record</th>
                        <th class="smallerWidth"></th>
                        <th class="smallerWidth"></th>
                        <th class="smallerWidth"></th>
                      </tr>
                  </thead>
                  <tbody id="patientTable">

                  </tbody>
                </table>
            </div>
        </div>
    </section>

    <section id="two">
      <div id="patients">
        <div class="tbl-content">
          <table cellpadding="0" cellspacing="0" border="0" class="patientTable">
          <thead>
              <tr>
                <th>First Name</th>
                <th>Last Name</th>
                <th>Email</th>
                <th width = 150>Password</th>
                <th>Id</th>
                <th>Medical Record</th>
                <th>Blocked by</th>
                <th class="smallerWidth"></th>
              </tr>
          </thead>
          <tbody id="blockedPatientTable">

          </tbody>
          </table>
        </div>
    </div>
    </section>

    <section id="three">
      <div id="examinations">
        <div class="tbl-content">
          <table cellpadding="0" cellspacing="0" border="0" class="patientTable">
              <thead>
                <tr>
                  <th>Type</th>
                  <th>Doctor</th>
                  <th>Date</th>
                  <th>Room</th>
                  <th>Patient</th>
                  <th>Change</th>
                </tr>
              </thead>
              <tbody id="examinationRequestTable">
                  
              </tbody>
          </table>
        </div>
      </div>
    </section>

    <div id="urgentMovingTable" class = "off">
      <div class="tbl-content">
          <h1>Couldn't find a free term in the next 2 hours,please select a term and the system will replace the term with an urgent one and move it"</h1>
          <table  cellpadding="0" cellspacing="0" border="0">
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
              <tbody id="examinationsUrgentTable">
                  <!-- this is where data from api comes -->
              </tbody>
          </table>
      </div>
    </div>


    <section id="four">
      <div id="urgentSecretary" class = "prompt">
        <form id="urgentForm" class="colDir myForm">
          <h1 id="examinationFormId" >Create urgent examination</h1>
          <div class="formDiv">
              <label for="examinationTypeUrgent">Examination type:</label>
              <select id="examinationTypeUrgent">
                  <option value="visit" selected>Visit</option>
                  <option value="operation">Operation</option>
              </select>
          </div>
          <div class="formDiv">
              <label for="examinationDurationUrgent">Duration:</label>
              <input type="number" id="examinationDurationUrgent" min="15">
          </div>
          <div class="formDiv">
              <label for="examinationPatienUrgent">Patient id:</label>
              <input id="examinationPatienUrgent" type="number"/>
          </div>
          <div class="formDiv">
              <label for="examinationSpecialityUrgent">Specialization</label>
              <select id="examinationSpecialityUrgent">
              </select>
          </div>
          <button class="mainBtn">OK</button>
        </form>
      </div>
    </section>


      
  </main>

  <!-- <?php include 'footer.html';?> -->


  <script src="https://code.jquery.com/jquery-3.3.1.min.js"
    integrity="sha256-FgpCb/KJQlLNfOu91ta32o/NMZxltwRo8QtmkMRdAu8=" crossorigin="anonymous"></script>
  <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
  <script src="https://code.jquery.com/ui/1.12.0/jquery-ui.js"
    integrity="sha256-0YPKAwZP7Mp3ALMRVB2i8GXeEndvCq3eSl/WsAl1Ryk=" crossorigin="anonymous"></script>

  <script src="secretary.js">
  </script>
  <script>
    $(window).on("load resize ", function() {
    var scrollWidth = $('.tbl-content').width() - $('.tbl-content table').width();
    $('.tbl-header').css({'padding-right':scrollWidth});
    }).resize();
  </script>
</body>

</html>