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
    $table    = "userComparisonDataRCTAMAN";
    
    
    $connection = mysql_connect($server, $username, $password);
    

    
    $result = mysql_select_db($database, $connection);    
   
    // o--------------------------------------------------------
    // | Input & Settings
    // o--------------------------------------------------------

    // These variables are sent from Unity, we access them via
    // $_POST and make sure to santitize the input to mysql.
    
    $userId     = mysql_real_escape_string($_POST['userId']);
    $animName     = mysql_real_escape_string($_POST['animName']);   
    $effortInd     = mysql_real_escape_string($_POST['effortInd']);       
    $answerOE = mysql_real_escape_string($_POST['answerOE']);   
    $answerNCA = mysql_real_escape_string($_POST['answerNCA']);   
	$areSwapped = mysql_real_escape_string($_POST['areSwapped']);   

       // Connect to the server with our settings defined above.
    
   $userIdName = "userID";
   $animNameName = "animName";
   $effortIndName = "effortInd";   
   $answerOEName = "answerOE";
   $answerNCAName = "answerNCA";
   $areSwappedName = "areSwapped";
   
      
    $insert   = "INSERT INTO $table (userId, animName, effortInd,  answerOE, answerNCA, areSwapped) 
                 VALUES ('$userId', '$animName', '$effortInd','$answerOE','$answerNCA','$areSwapped')";
    
     $select   = "SELECT count(*) FROM $table WHERE  $userIdName  = '$userId' AND $animNameName = '$animName' AND $effortIndName = '$effortInd'";
    
  
     $exists = mysql_query($select, $connection);

    $row = mysql_fetch_row($exists);
    
    $update = "UPDATE $table SET 
    $answerOEName = $answerOE, $answerNCAName = $answerNCA , $areSwappedName = $areSwapped  WHERE 
    $userIdName = '$userId' AND  $animNameName = '$animName' AND $effortIndName = '$effortInd'";
    
    if ($row[0] > 0) //exists then update
       $result = mysql_query($update, $connection);
    else //new entry
      $result = mysql_query($insert, $connection);
    
   
    // Close the connection, we're done here.
    
    mysql_close($connection);
?>