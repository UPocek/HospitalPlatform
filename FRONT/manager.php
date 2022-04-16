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

    <div id="roomPrompt" class="form-container sign-in-container off prompt">
		<form id="roomForm" class="colDir">
			<h1>Update room</h1>
			<input id="roomName" type="text" placeholder="" />
            <select name="roomType" id="roomType">
                <option value="operation room" selected>Operation room</option>
                <option value="examination room">Examination room</option>
                <option value="rest room">Rest room</option>
                <option value="other">Other</option>
            </select>
			<button class="main-btn">OK</button>
		</form>
	</div>

    <div id="createRoomPrompt" class="form-container sign-in-container off prompt">
		<form id="createRoomForm" class="colDir">
			<h1>Create room</h1>
			<input id="createRoomName" type="text" placeholder="" />
            <select name="createRoomType" id="createRoomType">
                <option value="operation room" selected>Operation room</option>
                <option value="examination room">Examination room</option>
                <option value="rest room">Rest room</option>
                <option value="other">Other</option>
            </select>
			<button class="main-btn">OK</button>
		</form>
	</div>

    <div id="renovationPrompt" class="form-container sign-in-container off prompt">
		<form id="renovationForm" class="colDir">
			<h1>Schedule renovation</h1>
			<input id="fromRenovation" type="text" placeholder="From (yyyy-MM-dd)" />
            <input id="toRenovation" type="text" placeholder="To (yyyy-MM-dd)" />
			<button class="main-btn">OK</button>
		</form>
	</div>

  <main>

    <div>
        <h1 id="hi">Welcome back &nbsp; </h1>
    </div>
    <section id="one" class="active">
        <div class="plusDiv">
            <button id="addBtn" class="add"><i data-feather="plus-circle"></i></button>
        </div>
            <section id="rooms">
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
        </section>
    </section>

    <section id="two">

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