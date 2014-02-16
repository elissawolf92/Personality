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
    $table    = "userData";
    
    
    $connection = mysql_connect($server, $username, $password);
    

    
    $result = mysql_select_db($database, $connection);    
   
    // o--------------------------------------------------------
    // | Input & Settings
    // o--------------------------------------------------------

    // These variables are sent from Unity, we access them via
    // $_POST and make sure to santitize the input to mysql.
    
    $userId     = mysql_real_escape_string($_POST['userId']);
    $animName     = mysql_real_escape_string($_POST['animName']);   
    $persName     = mysql_real_escape_string($_POST['persName']);   
    $space     = mysql_real_escape_string($_POST['space']);   
    $weight     = mysql_real_escape_string($_POST['weight']);   
    $time     = mysql_real_escape_string($_POST['time']);   
    $flow     = mysql_real_escape_string($_POST['flow']);   
    $horArm     = mysql_real_escape_string($_POST['horArm']);   
    $verArm     = mysql_real_escape_string($_POST['verArm']);   
    $sagArm     = mysql_real_escape_string($_POST['sagArm']);   
    $horTorso     = mysql_real_escape_string($_POST['horTorso']);   
    $verTorso     = mysql_real_escape_string($_POST['verTorso']);   
    $sagTorso     = mysql_real_escape_string($_POST['sagTorso']);   
    
    //$player   = mysql_real_escape_string($_POST['playerName']);
    //$score    = mysql_real_escape_string($_POST['score']);

       // Connect to the server with our settings defined above.
    
   $userIdName = "userID";
   $animNameName = "animName";
   $persNameName = "persName";
   $spaceName = "space";
   $weightName = "weight";
   $timeName = "time";
   $flowName = "flow";
   $horArmName = "horArm";
   $verArmName = "verArm";
   $sagArmName = "sagArm";
   $horTorsoName = "horTorso";
   $verTorsoName = "verTorso";
   $sagTorsoName = "sagTorso";
   
      
    $insert   = "INSERT INTO $table (userId, animName, persName, space, weight, time, flow, horArm, verArm, sagArm, horTorso, verTorso, sagTorso) 
                 VALUES ('$userId', '$animName', '$persName','$space','$weight','$time','$flow','$horArm','$verArm','$sagArm','$horTorso','$verTorso', '$sagTorso' )";
    
     $select   = "SELECT count(*) FROM $table WHERE  $userIdName  = '$userId' AND $animNameName = '$animName' AND $persNameName = '$persName'";
    
  
     $exists = mysql_query($select, $connection);

    $row = mysql_fetch_row($exists);
    
    $update = "UPDATE $table SET $spaceName = $space,  $weightName = $weight,$timeName = $time,  $flowName = $flow,
    $horArmName = $horArm, $verArmName = $verArm, $sagArmName = $sagArm, $horTorsoName  = $horTorso,  $verTorsoName = $verTorso, $sagTorsoName = $sagTorso WHERE 
    $userIdName = '$userId' AND  $animNameName = '$animName' AND $persNameName = '$persName'";
    
    if ($row[0] > 0) //exists then update
       $result = mysql_query($update, $connection);
    else //new entry
      $result = mysql_query($insert, $connection);
    
    
      

   

  
    // Close the connection, we're done here.
    
    mysql_close($connection);
?>