<?php

   // These settings define where the server is located, and
    // which credentials we use to connect to that server.  
    
    $server   = "fling.seas.upenn.edu";
    $username = "fundad";
    $password = "fundasql";
    
    
    $database = "fundad";
    //$table    = "userData";
	$link = mysql_connect($server, $username, $password) or die("Can not connect." . mysql_error());
	
	mysql_select_db($database) or die("Can not connect.");
  
  $file = 'export';	
  $csv_output = "";
   for ($cnt=0;$cnt<2;$cnt++) {
		if($cnt ==  0){
			$table = "shapeDataRCTAMAN";
					
		}
		else if ($cnt == 1){
			$table = "driveDataRCTAMAN";
		
		}
		
  
		$result = mysql_query("SHOW COLUMNS FROM ".$table."");

		$i = 0;
		if (mysql_num_rows($result) > 0) {
			while ($row = mysql_fetch_assoc($result)) {
				$csv_output .= $row['Field']."\t";
				$i++;
			}
		}

		$csv_output .= "\n";
		$values = mysql_query("SELECT * FROM ".$table."");		
			
		while ($rowr = mysql_fetch_row($values)) {
			for ($j=0;$j<$i;$j++) {	
				$csv_output .= $rowr[$j]."\t";
			}
			$csv_output .= "\n";
		}
		
	}
	$filename = $file."_".date("Y-m-d_H-i",time());
	header("Content-type: application/vnd.ms-excel");
	header("Content-disposition: csv" . date("Y-m-d") . ".csv");
	header( "Content-disposition: filename=".$filename.".csv");
	print $csv_output;
	exit;
	?>