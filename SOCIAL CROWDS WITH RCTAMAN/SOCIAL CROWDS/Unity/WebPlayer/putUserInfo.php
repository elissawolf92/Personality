<?php

   // These settings define where the server is located, and
    // which credentials we use to connect to that server.  
    
    $server   = "fling.seas.upenn.edu";
    $username = "fundad";
    $password = "fundasql";
    
    // This is the database and table we are going to access in
    // the mysql server. In this example, we assume that we have
    // the table 'highscores' in the database 'gamedb'.
    
    $database = "fundad";
    $table    = "userInfoRCTAMAN";
    
    
    $connection = mysql_connect($server, $username, $password);
    

    
    $result = mysql_select_db($database, $connection);    
   
    // o--------------------------------------------------------
    // | Input & Settings
    // o--------------------------------------------------------

    // These variables are sent from Unity, we access them via
    // $_POST and make sure to santitize the input to mysql.
    
    $userId     = mysql_real_escape_string($_POST['userId']);
    $age     = mysql_real_escape_string($_POST['age']);   
    $gender     = mysql_real_escape_string($_POST['gender']);   
    
       // Connect to the server with our settings defined above.
    
   
      
    $insert   = "INSERT INTO $table (userId, age, gender) 
                 VALUES ('$userId', '$age', '$gender')";
    
  
    $result = mysql_query($insert, $connection);

  
    // Close the connection, we're done here.
    
    mysql_close($connection);
?>