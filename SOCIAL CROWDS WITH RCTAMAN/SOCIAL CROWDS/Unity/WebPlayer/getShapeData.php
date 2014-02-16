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
    $table    = "shapeDataRCTAMAN";
    
    
    $connection = mysql_connect($server, $username, $password);
    

    
    $result = mysql_select_db($database, $connection);    
   
    // o--------------------------------------------------------
    // | Input & Settings
    // o--------------------------------------------------------

    // These variables are sent from Unity, we access them via
    // $_POST and make sure to santitize the input to mysql.
    
    $userId     = mysql_real_escape_string($_POST['userId']);
    
    $shapeInd     = mysql_real_escape_string($_POST['shapeInd']);   

    
       // Connect to the server with our settings defined above.
    
   $userIdName = "userId";
   $shapeIndName = "shapeInd";
   $headName = "head";
   $neckName = "neck";
   $spineName = "spine";
   $spine1Name = "spine1";
   $shouldersXName = "shouldersX";
   $shouldersYName = "shouldersY";
   $shouldersZName = "shouldersZ";
   $claviclesXName = "claviclesX";
   $claviclesYName = "claviclesY";
   $claviclesZName = "claviclesZ";
   $pelvisLXName = "pelvisLX";
   $pelvisRXName = "pelvisRX";
   $pelvisYName = "pelvisY";
   $pelvisZName = "pelvisZ";
   $kneesXName = "kneesX";
   $hipsXName = "hipsX";
   $toesXName = "toesX";
   $spineLengthName = "spineLength";
   
   
      
     
	$fetch   = "SELECT * FROM $table WHERE  $userIdName  = '$userId' AND $shapeIndName = '$shapeInd'";
		  $result = mysql_query($fetch, $connection);
		  
		  
  $num_results = mysql_num_rows($result);  
 
    for($i = 0; $i < $num_results; $i++)
    {
         $row = mysql_fetch_array($result);
         echo $row[ $headName ] . "\t" . $row[ $neckName] . "\t". $row[ $spineName ] . "\t" . $row[ $spine1Name ] . "\t" . $row[ $shouldersXName ] .
		 "\t" . $row[ $shouldersYName ] . "\t" . $row[ $shouldersZName ] . "\t" . $row[ $claviclesXName ] . "\t" . $row[ $claviclesYName ] . "\t" . $row[ $claviclesZName ] . 
		 "\t" . $row[ $pelvisLXName ] . "\t". $row[ $pelvisRXName ] . "\t" . $row[ $pelvisYName ] . "\t". $row[ $pelvisZName ] . "\t". $row[ $kneesXName ] . "\t". 
		 $row[ $hipsXName ] . "\t". $row[ $toesXName ]. "\t". $row[ $spineLengthName ]. "\n";
    }
      
	
  
    // Close the connection, we're done here.
    
    mysql_close($connection);
?>