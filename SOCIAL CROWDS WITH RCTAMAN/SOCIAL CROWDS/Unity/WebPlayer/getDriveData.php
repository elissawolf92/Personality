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
    $table    = "driveDataRCTAMAN";
    
    
    $connection = mysql_connect($server, $username, $password);
    

    
    $result = mysql_select_db($database, $connection);    
   
    // o--------------------------------------------------------
    // | Input & Settings
    // o--------------------------------------------------------

    // These variables are sent from Unity, we access them via
    // $_POST and make sure to santitize the input to mysql.
    
    $userId     = mysql_real_escape_string($_POST['userId']);
    
    $driveInd     = mysql_real_escape_string($_POST['driveInd']);   
    
    
       // Connect to the server with our settings defined above.
    
   $userIdName = "userID";
   $driveIndName = "driveInd";
   $speedName = "speed";
   $v0Name = "v0";
   $v1Name = "v1";
   $tiName = "ti";
   $texpName = "texp";
   $tvalName = "tval";
   $continuityName = "continuity";
   $biasName = "bias";
   $t0Name = "t0";
   $t1Name = "t1";
   $trName = "tr";
   $tfName = "tf";
   $hrName = "hr";
   $hfName = "hf";
   $squashName = "squash";
   $wbName = "wb";
   $wxName = "wx";
   $wtName = "wt";
   $wfName = "wf";
   $etName = "et";
   $efName = "ef";
   $dName = "d";
   $encSpr0Name = "encSpr0";
   $sinRis0Name = "sinRis0";
   $retAdv0Name = "retAdv0";
   $encSpr1Name = "encSpr1";
   $sinRis1Name = "sinRis1";
   $retAdv1Name = "retAdv1";
   $armLXName = "armLX";
   $armLYName = "armLY";
   $armLZName = "armLZ";
   $armRXName = "armRX";
   $armRYName = "armRY";
   $armRZName = "armRZ";
    
     $fetch   = "SELECT * FROM $table WHERE  $userIdName  = '$userId' AND $driveIndName = '$driveInd'";
    
    $result = mysql_query($fetch, $connection);
    
  $num_results = mysql_num_rows($result);  
 
    for($i = 0; $i < $num_results; $i++)
    {
         $row = mysql_fetch_array($result);
         echo $row[ $speedName ] . "\t" . $row[ $v0Name] . "\t" . $row[ $v1Name ] . "\t" . $row[ $tiName ] . "\t" . $row[ $texpName ] . "\t" . $row[ $tvalName ] . "\t" . $row[ $continuityName ] . "\t" . $row[ $biasName ] . "\t" . 
		 $row[ $t0Name ] . "\t" . $row[ $t1Name ] . "\t" . $row[ $trName ] . "\t" . $row[ $tfName ] . "\t" . $row[ $hrName ] . "\t" . $row[ $hfName ] . "\t" . $row[ $squashName ] . "\t" . $row[ $wbName ] . "\t". $row[ $wxName ] . "\t". 
		 $row[ $wtName ] . "\t". $row[ $wfName ] . "\t". $row[ $etName ] . "\t". $row[ $efName ] . "\t". $row[ $dName ] . "\t" . 
		 $row[ $encSpr0Name ] . "\t" . $row[ $sinRis0Name ] . "\t" . $row[ $retAdv0Name ] . "\t" . $row[ $encSpr1Name ] . "\t" . $row[ $sinRis1Name ] . "\t" . $row[ $retAdv1Name ]. "\t" .
		 $row[ $armLXName ] . "\t" .  $row[ $armLYName ] . "\t" .  $row[ $armLZName ] . "\t" . $row[ $armRXName ] . "\t" .  $row[ $armRYName ] . "\t" .  $row[ $armRZName ] . "\n";
    }
      
	  
    // Close the connection, we're done here.
    
    mysql_close($connection);
?>