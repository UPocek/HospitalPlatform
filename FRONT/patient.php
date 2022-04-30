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

    <div id="createExaminationPrompt" class="form-container sign-in-container off prompt">
		<form id="createExaminationForm" class="colDir myForm">
			<h1>Schedule examination</h1>
      <input id="timeCreateExamination" type="datetime-local" /> 
      <select id="doctorCreateExamination" name="doctors"> 
      </select> 
      <button class="mainBtn">OK</button>
		</form>
	</div>

  <div id="editExaminationPrompt" class="form-container sign-in-container off prompt">
		<form id="editExaminationForm" class="colDir myForm">
			<h1>Edit examination</h1>
      <input id="timeEditExamination" type="datetime-local" /> 
      <select id="doctorEditExamination" name="doctors"> 
      </select>    
			<button class="mainBtn">OK</button>
		</form>
	</div>

  <main>
 
 <section id="one" class="active">
     <div id="hi">
          <h1>Welcome back &nbsp; </h1>
      </div>
        <div class="plusDiv">
            <button id="addBtn" class="add"><i data-feather="plus-circle"></i></button>
        </div>
        <div id="examinations">
            <div class="tbl-content">
                <table cellpadding="0" cellspacing="0" border="0">
                <thead>
                    <tr>
                        <th>Type</th>
                        <th>Doctor</th>
                        <th>Specialization</th>
                        <th>Date</th>
                        <th>Room</th>
                        <!-- <th>Anamnesis</th> -->
                        <th>Urgent</th>
                        <th>Delete</th>
                        <th>Update</th>
                    </tr>
                </thead>
                <tbody id="examinationTable">

                </tbody>
                </table>
            </div>
        </div>
  </section>

  <section id="two">
  <div id="patientInfo" >
    <div class="basicInfo">
      <h1>Medical record</h1>
      <div>
         <p>First name:&nbsp<span id="patientFName"></span></p>
         <p>Last name:&nbsp<span id="patientLName"></span></p>
         <p>Height:&nbsp<span id="patientHeight"></span>cm</p>
         <p>Weight:&nbsp<span id="patientWeight"></span>kg</p>
         <p>Blood type:&nbsp<span id="patientBlood"></span></p>
         <div class="divList">
            <p>Diseases:&nbsp</p>
            <ul id="diseasesList">
              <!-- patients diseases -->
            </ul>
          </div>
          <div class="divList">
            <p>Alergies:&nbsp</p>
            <ul id="alergiesList">
              <!-- patients diseases -->
            </ul>
          </div>
        </div>
      </div>
   </div>

   <form id="patientFilters" class="myForm ">
        <div>
        <label for="examinationSearch">Search:</label>
        <input id="examinationSearch" type="text" value="" placeholder="Enter search term">
      </div>
      </form>
  <div id="examinations">
      <div class="tbl-content">
          <table id="searchExaminations" cellpadding="0" cellspacing="0" border="0">
              <thead>
                  <tr>
                      <th >Type</th>
                      <th onclick="sortTable(1, 'searchExaminations')">Doctor</th>
                      <th onclick="sortTable(2, 'searchExaminations')">Specialization</th>
                      <th onclick="sortTable(3, 'searchExaminations')">Date</th>
                      <th>Anamnesis</th>
                      <th>Urgent</th>
                    </tr>
                </thead>
                <tbody id="searchExaminationTable">

                </tbody>
                </table>
            </div>
        </div>
</section>

  <section id="three">
    <div>
      <form id="patientFilters" class="myForm">
        <div id="search">
        <label for="doctorSearch">Search:</label>
        <input id="doctorSearch" type="text" value="" placeholder="Enter search term">
      </div>
      </form>
      
      <div id="doctors">
            <div class="tbl-content">
                <table id ='searchsDoctors' cellpadding="0" cellspacing="0" border="0">
                <thead>
                    <tr>
                        <th onclick="sortTable(0, 'searchsDoctors')">Name</th>
                        <th onclick="sortTable(1, 'searchsDoctors')">Specialization</th>
                        <th>email</th>
                        <th onclick="sortTable(3, 'searchsDoctors')">Score</th>
                        <th>Schedule examination</th>
                    </tr>
                </thead>
                <tbody id="doctorsTable">

                </tbody>
                </table>
            </div>
        </div>

    </div>
</section>

   <section id="four">
</section>
      
  </main>

 

  <!-- <?php include 'footer.html';?> -->


  <script src="https://code.jquery.com/jquery-3.3.1.min.js"
    integrity="sha256-FgpCb/KJQlLNfOu91ta32o/NMZxltwRo8QtmkMRdAu8=" crossorigin="anonymous"></script>
  <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
  <script src="https://code.jquery.com/ui/1.12.0/jquery-ui.js"
    integrity="sha256-0YPKAwZP7Mp3ALMRVB2i8GXeEndvCq3eSl/WsAl1Ryk=" crossorigin="anonymous"></script>

  <script src="patient.js">
  </script>
  <script>
    $(window).on("load resize ", function() {
    var scrollWidth = $('.tbl-content').width() - $('.tbl-content table').width();
    $('.tbl-header').css({'padding-right':scrollWidth});
    }).resize();
  </script>
</body>

</html>