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

    <div id="roomPrompt" class="form-container sign-in-container off prompt">
		<form id="roomForm" class="colDir myForm">
			<h1>Update room</h1>
			<input id="roomName" type="text" placeholder="" />
            <select name="roomType" id="roomType">
                <option value="operation room" selected>Operation room</option>
                <option value="examination room">Examination room</option>
                <option value="rest room">Rest room</option>
                <option value="other">Other</option>
            </select>
			<button class="mainBtn">OK</button>
		</form>
	</div>

    <div id="createRoomPrompt" class="form-container sign-in-container off prompt">
		<form id="createRoomForm" class="colDir myForm">
			<h1>Create room</h1>
			<input id="createRoomName" type="text" placeholder="" />
            <select name="createRoomType" id="createRoomType">
                <option value="operation room" selected>Operation room</option>
                <option value="examination room">Examination room</option>
                <option value="rest room">Rest room</option>
                <option value="other">Other</option>
            </select>
			<button class="mainBtn">OK</button>
		</form>
	</div>

    <div id="renovationPrompt" class="form-container sign-in-container off prompt">
		<form id="renovationForm" class="colDir myForm">
			<h1>Schedule renovation</h1>
			<input id="fromRenovation" type="text" placeholder="From (yyyy-MM-dd)" />
            <input id="toRenovation" type="text" placeholder="To (yyyy-MM-dd)" />
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
        <div id="rooms">
            <div class="tbl-content">
                <table cellpadding="0" cellspacing="0" border="0">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Type</th>
                        <th>In Renovation</th>
                        <th></th>
                        <th></th>
                        <th></th>
                    </tr>
                </thead>
                <tbody id="roomTable">

                </tbody>
                </table>
            </div>
        </div>
        <div id="complexRenovations" class="myForm">
            <div class="box">
            <h2>Schedule complex renovations</h2>
            <select name="complexRenovation" id="complexRenovation">
                <option value="">?</option>
                <option value="devide">Devide (1 --> 2)</option>
                <option value="merge">Merge (2 --> 1)</option>
            </select>
            </div>
            <div id="ifDevide" class="off">
                <h3>Devide 1 room into 2 (the equipment will be evenly distributed)</h3>
                <form>
                    <div>
                    <label for="complexDevide">Room:</label>
                    <select name="complexDevide" id="complexDevide"></select>
                    </div>
                    <div>
                    <label for="fromComplexRenovation">Renovation start date:</label>
                    <input id="fromComplexRenovation" type="text" placeholder="From (yyyy-MM-dd)" autocomplete="off"/>
                    </div>
                    <div>
                    <label for="toComplexRenovation">Renovation end date:</label>
                    <input id="toComplexRenovation" type="text" placeholder="To (yyyy-MM-dd)" autocomplete="off"/>
                    </div>
                    <div>
                    <label for="">&nbsp;</label>
                    <button class="mainBtn">Schedule</button>
                    </div>
                    
                </form>
            </div>
            <div id="ifMerge" class="off">
                <h3>Merge 2 rooms into 1 (the equipment will be automatically transferred to the new room)</h3>
                <form>
                    <div>
                    <label for="complexMerge1">Room 1:</label>
                    <select name="complexMerge1" id="complexMerge1"></select><br><br>
                    <label for="complexMerge2">Room 2:</label>
                    <select name="complexMerge2" id="complexMerge2"></select>
                    </div>
                    <div>
                    <label for="fromComplexRenovation1">Renovation start date:</label>
                    <input id="fromComplexRenovation1" type="text" placeholder="From (yyyy-MM-dd)" autocomplete="off"/>
                    </div>
                    <div>
                    <label for="toComplexRenovation1">Renovation end date:</label>
                    <input id="toComplexRenovation1" type="text" placeholder="To (yyyy-MM-dd)" autocomplete="off"/>
                    </div>
                    <div>
                    <label for="">&nbsp;</label>
                    <button class="mainBtn">Schedule</button>
                    </div>
                </form>
            </div>
        </div>
    </section>

    <section id="two">
    <div>
        <form id="filters" class="myForm">
            <div>
            <label for="filterSearch">Search:</label>
            <input id="filterSearch" type="text" value="" placeholder="Enter search term">
            </div>
            <div>
            <label for="filterRoomType">Room type:</label>
            <select name="filterRoomType" id="filterRoomType">
                <option value="" selected>Any</option>
                <option value="warehouse">Warehouse</option>
                <option value="examination room">Examination room</option>
                <option value="operation room">Operation room</option>
                <option value="rest room">Rest room</option>
                <option value="other">Other</option>
            </select>
            </div>
            <div>
            <label for="filterEquipmentQuantity">Equipment quantity:</label>
            <select name="filterEquipmentQuantity" id="filterEquipmentQuantity">
                <option value="" selected>Any</option>
                <option value="0">Nema na stanju</option>
                <option value="1-10">1-10</option>
                <option value="11-20">11-20</option>
                <option value="20+">20+</option>
            </select>
            </div>
            <div>
            <label for="filterEquipmentType">Equipment type:</label>
            <select name="filterEquipmentType" id="filterEquipmentType">
                <option value="" selected>Any</option>
                <option value="examination equipment">Examination equipment</option>
                <option value="operation equipment">Operation equipment</option>
                <option value="room furniture">Room furniture</option>
                <option value="hallway equipment">Hallway equipment</option>
            </select>
            </div>
        </form>
    </div>
    <div id="equipment">
        <div class="tbl-content">
            <table cellpadding="0" cellspacing="0" border="0">
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Type</th>
                    <th>Quantity</th>
                    <th>In Room</th>
                </tr>
            </thead>
            <tbody id="equipmentTable">

            </tbody>
            </table>
        </div>
    </div>
    <div id="transfers">
        <h3>Transfer equipment</h3>
        <form id="transferForm" class="myForm">
            <div>
                <div>
                <label for="transfer1">Room 1 (From):</label>
                <select name="transfer1" id="transfer1"></select>
                </div>
                <div>
                <label for="transfer2">Room 2 (To):</label>
                <select name="transfer2" id="transfer2"></select>
                </div>
            </div>
            <div class="spacious">
                <div>
                <h4>What to transfer:</h4>
                </div>
                <div>
                <label for="transferDate">Transfer date:</label>
                <input id="transferDate" type="text" placeholder="Date (yyyy-MM-dd)" autocomplete="off"/>
                </div>
            </div>
            <div id="transferOptions">
                <input id="t" type="text" autocomplete="off"/>
                <input id="t" type="text" autocomplete="off"/>
            </div>
            <div id="correctPos">
            <button class="mainBtn">Transfer</button>
            </div>
        </form>
    </div>

    </section>

    <section id="three">

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

  <script src="manager.js">
  </script>
  <script>
    $(window).on("load resize ", function() {
    var scrollWidth = $('.tbl-content').width() - $('.tbl-content table').width();
    $('.tbl-header').css({'padding-right':scrollWidth});
    }).resize();
  </script>
</body>

</html>