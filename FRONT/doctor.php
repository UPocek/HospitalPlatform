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
        
        <?php include 'doctor_navbar.html';?>

        <main>
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
                                    <th>Done</th>
                                    <th>Examination room</th>
                                    <th>Urgent</th>
                                    <th>Type</th>
                                </tr>
                            </thead>
                            <tbody id="roomTable">
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