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
    $speed     = mysql_real_escape_string($_POST['speed']);   
    $v0     = mysql_real_escape_string($_POST['v0']);   
	$v1     = mysql_real_escape_string($_POST['v1']);   
	$ti     = mysql_real_escape_string($_POST['ti']);   
	$texp     = mysql_real_escape_string($_POST['texp']);   
	$tval     = mysql_real_escape_string($_POST['tval']);   
	$continuity     = mysql_real_escape_string($_POST['continuity']);   
	$bias     = mysql_real_escape_string($_POST['bias']);   
	$t0     = mysql_real_escape_string($_POST['t0']);   
	$t1     = mysql_real_escape_string($_POST['t1']);   
	$tr     = mysql_real_escape_string($_POST['tr']);   
	$tf     = mysql_real_escape_string($_POST['tf']);   
	$hr     = mysql_real_escape_string($_POST['hr']);   
	$hf     = mysql_real_escape_string($_POST['hf']);   	
	$squash     = mysql_real_escape_string($_POST['squash']);   
	$wb     = mysql_real_escape_string($_POST['wb']);   
	$wx     = mysql_real_escape_string($_POST['wx']);   
	$wt     = mysql_real_escape_string($_POST['wt']);   
	$wf     = mysql_real_escape_string($_POST['wf']);   
	$et     = mysql_real_escape_string($_POST['et']);   
	$d     = mysql_real_escape_string($_POST['d']);  
	$ef     = mysql_real_escape_string($_POST['ef']);   
	$encSpr0     = mysql_real_escape_string($_POST['encSpr0']);
	$sinRis0     = mysql_real_escape_string($_POST['sinRis0']);   
	$retAdv0     = mysql_real_escape_string($_POST['retAdv0']);   
	$encSpr1     = mysql_real_escape_string($_POST['encSpr1']);
	$sinRis1     = mysql_real_escape_string($_POST['sinRis1']);   
	$retAdv1     = mysql_real_escape_string($_POST['retAdv1']);   
	$armLX     = mysql_real_escape_string($_POST['armLX']);   
	$armLY     = mysql_real_escape_string($_POST['armLY']);   
	$armLZ     = mysql_real_escape_string($_POST['armLZ']);   
	$armRX     = mysql_real_escape_string($_POST['armRX']);   
	$armRY     = mysql_real_escape_string($_POST['armRY']);   
	$armRZ     = mysql_real_escape_string($_POST['armRZ']);   
	
    
    
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
   $dName = "d";
   $efName = "ef";
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
   
   
      
    $insert   = "INSERT INTO $table (userId, driveInd, speed, v0, v1, ti, texp, tval, continuity, bias, t0, t1, tr, tf, hr, hf, squash, wb, wx, wt, wf, et, ef, d, encSpr0, sinRis0, retAdv0, encSpr1, sinRis1, retAdv1, armLX, armLY, armLZ, armRX, armRY, armRZ) 
                 VALUES ('$userId', '$driveInd', '$speed','$v0','$v1','$ti','$texp','$tval','$continuity','$bias','$t0','$t1','$tr', '$tf','$hr', '$hf', '$squash', '$wb', '$wx','$wt', '$wf', '$et','$ef','$d','$encSpr0','$sinRis0','$retAdv0','$encSpr1','$sinRis1','$retAdv1', '$armLX','$armLY','$armLZ','$armRX','$armRY','$armRZ')";
    
     $select   = "SELECT count(*) FROM $table WHERE  $userIdName  = '$userId' AND $driveIndName = '$driveInd'";
    
  
     $exists = mysql_query($select, $connection);

    $row = mysql_fetch_row($exists);
    
    $update = "UPDATE $table SET $speedName = $speed,  $v0Name = $v0,$v1Name = $v1,  $tiName = $ti,
    $texpName = $texp, $tvalName = $tval,$continuityName = $continuity,$biasName = $bias, $t0Name = $t0, $t1Name  = $t1,  $trName = $tr, $tfName = $tf, $hrName = $hr, $hfName = $hf, $squashName = $squash, 
	$wbName = $wb, $wxName = $wx, $wtName = $wt, $wfName = $wf, $etName = $et,   $efName = $ef, $dName = $d, $encSpr0Name = $encSpr0, $sinRis0Name = $sinRis0, $retAdv0Name = $retAdv0, 
	$encSpr1Name = $encSpr1, $sinRis1Name = $sinRis1, $retAdv1Name = $retAdv1, $armLXName = $armLX, $armLYName = $armLY, $armLZName = $armLZ, $armRXName = $armRX, $armRYName = $armRY, $armRZName = $armRZ  WHERE 
	$userIdName = '$userId' AND  $driveIndName = '$driveInd'";
    
    if ($row[0] > 0) //exists then update
       $result = mysql_query($update, $connection);
    else //new entry
      $result = mysql_query($insert, $connection);
    
    
      

   

  
    // Close the connection, we're done here.
    
    mysql_close($connection);
?>