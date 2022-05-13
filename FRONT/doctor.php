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

        <div id="reviewExaminationDiv" class="off">
            <div id="reviewExaminationPopUp">
                <form id="reportPopUp" class="myForm">
                    <h1>Report</h1>
                    <div id="descReport">
                        <label for="reportDescription">Report description:</label>
                        <textarea  id="reportDescription"></textarea>
                    </div>
                    <div id="equipmentDiv">
                        <!-- equipmentUsed -->
                    </div>
                    <button id="prescribeReviews" class="mainBtn">Prescribe drugs</button>
                    <button id="createReferall" class="mainBtn">Create referall</button>
                    <button id="endReview" class="mainBtn">End review</button>
                </form>
                <form class="basicInfo">
                    <h1>Medical record</h1>
                    <div class="myForm">
                        <h5>First name:&nbsp<span id="patientFName"></span></h5>
                        <h5>Last name:&nbsp<span id="patientLName"></span></h5>
                        <div>
                            <label for="patientHeight">Height:&nbsp</label>
                            <input id="patientHeight" type="number" min="0"></input>
                        </div>
                        <div>
                            <label for="patientWeight">Weight:&nbsp</label>
                            <input id="patientWeight" type="number" min="0"></input>
                        </div>
                        <div>
                            <label for="patientBlood">Blood type:&nbsp</label>
                            <input id="patientBlood"></input>
                        </div>
                        
                        <div>
                            <div class="listContainer">
                                <div class="divList">
                                    <p>Diseases:&nbsp</p>
                                    <select id="diseasesList" multiple>
                                        <!-- patients diseases -->
                                    </select>
                                </div>
                                <button id="deleteDiseases" class="delBtn"><i data-feather="trash"></i></button>
                            </div>
                            <div>
                                <input id="diseaseInput"></input>
                                <button id="addDiseases" class="add"><i data-feather="plus"></i></button>
                            </div>
                        </div>
                        <div>
                            <div class="listContainer">
                                <div class="divList">
                                    <p>Alergies:&nbsp</p>

                                    <select id="alergiesList" multiple>
                                        <!-- patients diseases -->
                                    </select>
                                </div>
                                <button id="deleteAlergies" class="delBtn"><i data-feather="trash"></i></button>
                            </div>
                            <div>
                                <input id="alergieInput"></input>
                                <button id="addAlergies" class="add"><i data-feather="plus"></i></button>
                            </div>
                        </div>
                    </div>
                    <button id="updateMedicalCard" class="mainBtn">Update medical card</button>
                </form>
            </div>
        </div>
        <div id="perscriptionDiv" class = "off">
            <div id="perscriptionPopUp">
                <form>
                    <h1>Add prescription</h1>
                    <div class="myForm">
                        <div>
                            <label for="perscriptionTime">Time:&nbsp</label>
                            <input id="perscriptionTime" type="time"></input>
                        </div>
                        <div>
                            <label for="perscriptionFrequency">Frequency:&nbsp</label>
                            <input id="perscriptionFrequency" type="number" min="1"></input>
                        </div>
                        <div>
                            <div>
                                <p>Drug:&nbsp</p>
                                <select id="drugOptionsList">
                                       <!--addible drugs--> 
                                </select>
                            </div>
                        </div>
                        <div>
                            <div class="listContainer">
                                <div class="divList">
                                    <p>How to drink:&nbsp</p>
                                    <select id="howList">
                                        <option value="before" >Before</option>
                                        <option value="during" >During</option>
                                        <option value="after" >After</option>
                                        <option value="not_important" selected>Not important</option>
                                    </select>
                                </div>
                            </div>
                            <div>
                                <label for="perscriptionDuration">Duration:&nbsp</label>
                                <input id="perscriptionDuration" type="number" min='1'></input>
                            </div>
                        </div>
                    </div>
                    <button id="addPrescription" class="mainBtn">Add prescription</button>
                </form>
            </div>
        </div>

        <div id="referallDiv" class="form-container sign-in-container off prompt">
            <h1>Create referral</h1>
            <div class="myForm">
                <label for="referallType">Referall type:</label>
                <select id="referallType">
                    <option value="doctor" selected>doctor</option>
                    <option value="speciality">speciality</option>
                </select>
            </div>
            <div class="myForm">
                <label for="referallOption">Option:</label>
                <select id="referallOption">
                    <!--options for referall-->
                </select>
            </div>
            <button id="addReferall" class="mainBtn">Add referall</button>
        </div>

        <div id="reportPopUpNew" class="form-container sign-in-container off prompt">
            <div id="reportHeader">
                <h1>Report</h1>
                <button id="closeReportBtn" class="delBtn"><i data-feather="x-circle"></i></button>
            </div>
            <div id="descView">
                <h3>Report description:</h3>
                <p id="reportDescriptionNew"></p>
            </div>
            <div id="reportEquipmentNew">
                    <!-- js generating -->
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

        <div id="messageDrugPrompt" class="form-container off prompt">
            <form id="messageDrugForm" class="colDir myForm">
                <h1> Message </h1>
                <textarea  id="drugReviewMessage"></textarea>
                <button id="sendDrugMessage" class="mainBtn">Send</button>
            </form>
        </div>

        <main class = "myMain">
            <div id="hi">
                <h1>Welcome back &nbsp; </h1>
            </div>
            <section id="one" class="active">
                <div class="plusDiv">
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
            <section id="two">
            <div class="tableHeaderDiv">
                <div id="scheduleOption" class="scheduleDiv">
                    <label for="scheduleDateOption">Schedule for date:</label>
                    <input type="date" id="scheduleDateOption">
                    <button id="scheduleDateBtn" class="send"><i data-feather="arrow-right-circle"></i></button>
                </div>
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
                                <th></th>
                            </tr>
                        </thead>
                        <tbody id="examinationsTableSchedule">
                            <!-- this is where data from api comes -->
                        </tbody>
                    </table>
                </div>
            </div>
            </section>
            <section id="three">
                <div id="drugs">
                    <div class="tbl-content">
                        <table cellpadding="0" cellspacing="0" border="0">
                            <thead>
                                <tr>
                                    <th>Name</th>
                                    <th>Ingredients</th>
                                    <th></th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody id="drugTable">

                            </tbody>
                        </table>
                    </div>
                </div>
            </section>
            <section id="four">
                <div id="freeDays">
                        <div class="tbl-content">
                            <table cellpadding="0" cellspacing="0" border="0">
                            <thead>
                                <tr>
                                    <th>Begging</th>
                                    <th>Duration</th>
                                    <th>Status</th>
                                </tr>
                            </thead>
                            <tbody id="freeDayTable">

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

        <script src="doctor.js"></script>
        <script>
            $(window).on("load resize ", function() {
            var scrollWidth = $('.tbl-content').width() - $('.tbl-content table').width();
            $('.tbl-header').css({'padding-right':scrollWidth});
            }).resize();
        </script>
    </body>
</html>