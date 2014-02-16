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
    $head     = mysql_real_escape_string($_POST['head']);   
    $neck     = mysql_real_escape_string($_POST['neck']);   
	$spine     = mysql_real_escape_string($_POST['spine']);   
	$spine1     = mysql_real_escape_string($_POST['spine1']);   
	$shouldersX     = mysql_real_escape_string($_POST['shouldersX']);   
	$shouldersY     = mysql_real_escape_string($_POST['shouldersY']);   
	$shouldersZ     = mysql_real_escape_string($_POST['shouldersZ']);   
	$claviclesX     = mysql_real_escape_string($_POST['claviclesX']);   
	$claviclesY     = mysql_real_escape_string($_POST['claviclesY']);   
	$claviclesZ     = mysql_real_escape_string($_POST['claviclesZ']);   
	$pelvisLX     = mysql_real_escape_string($_POST['pelvisLX']);   
	$pelvisRX     = mysql_real_escape_string($_POST['pelvisRX']);   
	$pelvisY     = mysql_real_escape_string($_POST['pelvisY']);   
	$pelvisZ     = mysql_real_escape_string($_POST['pelvisZ']);   
	$kneesX     = mysql_real_escape_string($_POST['kneesX']); 
	$hipsX     = mysql_real_escape_string($_POST['hipsX']);
	$toesX     = mysql_real_escape_string($_POST['toesX']);
	$spineLength     = mysql_real_escape_string($_POST['spineLength']);

    
    
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
   
   
      
    $insert   = "INSERT INTO $table (userId, shapeInd, head, neck, spine, spine1, shouldersX, shouldersY, shouldersZ, claviclesX, claviclesY, claviclesZ, pelvisLX,  pelvisRX, pelvisY, pelvisZ, kneesX, hipsX, toesX, spineLength) 
                 VALUES ('$userId', '$shapeInd', '$head','$neck','$spine','$spine1','$shouldersX','$shouldersY','$shouldersZ','$claviclesX','$claviclesY','$claviclesZ', '$pelvisLX', 
				 '$pelvisRX','$pelvisY','$pelvisZ','$kneesX','$hipsX','$toesX','$spineLength')";
    
     $select   = "SELECT count(*) FROM $table WHERE  $userIdName  = '$userId' AND $shapeIndName = '$shapeInd'";
    
  
     $exists = mysql_query($select, $connection);

    $row = mysql_fetch_row($exists);
    
    $update = "UPDATE $table SET $headName = $head,  $neckName = $neck, $spineName = $spine, $spine1Name = $spine1,
    $shouldersXName = $shouldersX, $shouldersYName = $shouldersY, $shouldersZName = $shouldersZ, $claviclesXName  = $claviclesX,  $claviclesYName = $claviclesY, $claviclesZName = $claviclesZ, 
	$pelvisLXName = $pelvisLX, $pelvisRXName = $pelvisRX, $pelvisYName = $pelvisY, $pelvisZName = $pelvisZ , $kneesXName = $kneesX, $hipsXName = $hipsX, $toesXName = $toesX, $spineLengthName = $spineLength WHERE
    $userIdName = '$userId' AND  $shapeIndName = '$shapeInd'";
    
    if ($row[0] > 0) //exists then update
       $result = mysql_query($update, $connection);
    else //new entry
      $result = mysql_query($insert, $connection);
    
    
      

   

  
    // Close the connection, we're done here.
    
    mysql_close($connection);
?>