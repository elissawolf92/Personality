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
    
     // Connect to the server with our settings defined above.
    $connection = mysql_connect($server, $username, $password);
    

    
    $result = mysql_select_db($database, $connection);    
   
    // o--------------------------------------------------------
    // | Input & Settings
    // o--------------------------------------------------------
    
    $userId     = mysql_real_escape_string($_POST['userId']);
	$age     = mysql_real_escape_string($_POST['age']);
	$gender     = mysql_real_escape_string($_POST['gender']);
	
    $userIdName = "userId";
	$ageName = "age";
	$genderName = "gender";
   
  
    //$select   = "SELECT MAX($userId) FROM $table";

   // $select   = "SELECT count(*) FROM $table WHERE $keyName = '$userId'";
   
   	$select   = "SELECT * FROM $table WHERE  $userIdName  = '$userId'";
		 
		 $result = mysql_query($select, $connection);
		      

	   $num_results = mysql_num_rows($result);  
	   
 if ($num_results > 0) {
     echo "true";}
    else  {
      echo "false"; }
	  
	
    // $row = mysql_fetch_row($result);
    
    // if ($row[0] > 0) {
     // echo "true";}
    // else  {
      // echo "false"; }

     
    // Close the connection, we're done here.
    
    mysql_close($connection);
?>