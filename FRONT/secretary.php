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
      <select name="createPatientBloodType" id="filterEquipmentType">
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
    <main>

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
                        <th class="smallerWidth">First Name</th>
                        <th class="smallerWidth">Last Name</th>
                        <th>Email</th>
                        <th width = 150>Password</th>
                        <th class="smallerWidth">Id</th>
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