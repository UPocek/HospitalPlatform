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
                <div id="patient_info" >
                    <div class="basic_info">
                        <h3>Medical record</h3>
                        <div>
                            <p>First name:&nbsp<span id="patient_fname"></span></p>
                            <p>Last name:&nbsp<span id="patient_lname"></span></p>
                            <p>Height:&nbsp<span id="patient_height"></span></p>
                            <p>Weight:&nbsp<span id="patient_weight"></span></p>
                            <p>Blood type:&nbsp<span id="patient_blood"></span></p>
                            <p>Diseases:&nbsp<span id="patient_diseases"></span></p>
                            <p>Alergies:&nbsp<span id="patient_alergies"></span></p>
                        </div>
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

        <script src="medical_card.js">
        </script>
        <script>
            $(window).on("load resize ", function() {
            var scrollWidth = $('.tbl-content').width() - $('.tbl-content table').width();
            $('.tbl-header').css({'padding-right':scrollWidth});
            }).resize();
        </script>
    </body>
</html>