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

  <div id="drugInstructionPrompt" class="form-container sign-in-container off prompt">
		<form id="drugInstructionForm" class="colDir myForm">
			<h1>Drug instruction</h1>
      <p id="drug">&nbsp; </p>
      <p id="when">&nbsp; </p>
      <p id="frequency">&nbsp; a day </p> 
      <p id="note">&nbsp; </p>  
      <p>Notify </p>
      <input id="notifyTime" type="time">
			<button class="mainBtn">OK</button>
		</form>
	</div>

  <div id="doctorPollPrompt" class="form-container sign-in-container off prompt">
		<form id="doctorPoll" class="colDir2 myForm">
			<h1>Doctor poll</h1>
      <p>How would you evaluate this doctor (1-5)</p>
      <div class="radios" id="doctorScore">
        <label for="input1a"></label>
        <input  id="input1a" name="radioDoctor" type="radio" value=1 />
        <label for="input2a"></label>
        <input  id="input2a" name="radioDoctor" type="radio" value=2 />
        <label for="input3a"></label>
        <input  id="input3a" name="radioDoctor" type="radio" value=3 />
        <label for="input4a"></label>
        <input  id="input4a" name="radioDoctor" type="radio" value=4 />
        <label for="input5a"></label>
        <input  id="input5a" name="radioDoctor" type="radio" value=5 />
        <span id="slider1"></span>
      </div>
      <p>Would you recommend this doctor to your friend (1-5)</p>
      <div class="radios" id="recommendationScore">
        <label for="input1b"></label>
        <input  id="input1b" name="radioRecommendation" type="radio" value=1 />
        <label for="input2b"></label>
        <input  id="input2b" name="radioRecommendation" type="radio" value=2 />
        <label for="input3b"></label>
        <input  id="input3b" name="radioRecommendation" type="radio" value=3 />
        <label for="input4b"></label>
        <input  id="input4b" name="radioRecommendation" type="radio" value=4 />
        <label for="input5b"></label>
        <input  id="input5b" name="radioRecommendation" type="radio" value=5 />
        <span id="slider2"></span>
      </div>

      <p>Comment:</p>
      <textarea name="comment" id="doctorComment" rows="4"></textarea>
			<button class="mainBtn">OK</button>
		</form>
	</div>

  <main class="myMain">
 
    <div id="hi">
      <h1>Welcome back &nbsp; </h1>
    </div>
    <section id="one" class="active">
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
  <div id="createAdvancedExamination" class="form-container sign-in-container">
		<form id="createAdvancedExaminationForm" class="myForm">
			<h1>Advanced examination scheduler</h1>

      <p>Please select due date and doctor</p>
      <input id="dueDate" type="date" placeholder="Due to"/> 
      <select id="doctorAdvancedCreateExamination" name="doctors"></select>

      <p>Please select begining and end of time interval</p>
      <input id="timeFrom" type="time"/> 
      <input id="timeTo" type="time"/> 

      <p>Please select your priority:  </p>
      <input type="radio" id="priorityDoctor" name="priority" value="doctor">
      <label for="priorityDoctor" id="labelDoctor">Doctor</label>
      <input type="radio" id="priorityTime" name="priority" value="time">
      <label for="priorityTime" id="labelTime">Time interval</label>

      <button class="mainBtn advancedBtn" id="addAdvancedBtn">OK</button>
		</form>
	</div>

  <div id="advancedExaminations">
            <div class="tbl-content">
                <table cellpadding="0" cellspacing="0" border="0">
                <thead>
                    <tr>
                        <th>Type</th>
                        <th>Doctor</th>
                        <th>Specialization</th>
                        <th>Date</th>
                        <th>Choose</th>
                    </tr>
                </thead>
                <tbody id="advancedExaminationTable">

                </tbody>
                </table>
            </div>
    </div>

</section>

<section id="three">
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
    
   <div id="drugs">
    <div class="tbl-content">
                <table cellpadding="0" cellspacing="0" border="0">
                <thead>
                    <tr>
                        <th>Drug</th>
                        <th>From</th>
                        <th>To</th>
                        <th>Instructions</th>
                    </tr>
                </thead>
                <tbody id="medicalInstructions">

                </tbody>
                </table>
            </div>
    </tbody>
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
                      <th id="pinkTh" onclick="sortTable(1, 'searchExaminations')">Doctor</th>
                      <th id="pinkTh" onclick="sortTable(2, 'searchExaminations')">Specialization</th>
                      <th id="pinkTh" onclick="sortTable(3, 'searchExaminations')">Date</th>
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

<section id="four">
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
                        <th id="pinkTh" onclick="sortTable(0, 'searchsDoctors')">Name</th>
                        <th id="pinkTh" onclick="sortTable(1, 'searchsDoctors')">Specialization</th>
                        <th>email</th>
                        <th id="pinkTh" onclick="sortTable(3, 'searchsDoctors')">Score</th>
                        <th>Schedule examination</th>
                        <th>Poll</th>
                    </tr>
                </thead>
                <tbody id="doctorsTable">

                </tbody>
                </table>
            </div>
        </div>

    </div>
</section>
      
</main>    
  <script src="helper.js"></script>
  <script src="patient.js"></script>
</body>

</html>