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
    $table    = "userScalingData";
    
    
    $connection = mysql_connect($server, $username, $password);
    

    
    $result = mysql_select_db($database, $connection);    
   
    // o--------------------------------------------------------
    // | Input & Settings
    // o--------------------------------------------------------

    // These variables are sent from Unity, we access them via
    // $_POST and make sure to santitize the input to mysql.
    
    $userId     = mysql_real_escape_string($_POST['userId']);
    $animName     = mysql_real_escape_string($_POST['animName']);   
    $driveInd     = mysql_real_escape_string($_POST['driveInd']);   
	
	
    
    
       // Connect to the server with our settings defined above.
    
   $userIdName = "userID";
   $animNameName = "animName";
   $driveIndName = "driveInd";
  
   $answerOEName = "answerOE";
   $answerNCAName = "answerNCA";
   
    
     $fetch   = "SELECT * FROM $table WHERE  $userIdName  = '$userId' AND $driveIndName = '$driveInd' AND $animNameName = '$animName'";
    
    $result = mysql_query($fetch, $connection);
    
  $num_results = mysql_num_rows($result);  
 
    for($i = 0; $i < $num_results; $i++)
    {
         $row = mysql_fetch_array($result);
         echo $row[ $answerOEName ] . "\t" . $row[ $answerNCAName] . "\n";
    }
      
	  
    // Close the connection, we're done here.
    
    mysql_close($connection);
?>