<?php if ((($_FILES["file"]["type"] == "image/gif") || ($_FILES["file"]["type"] == "image/jpeg") || ($_FILES["file"]["type"] == "image/pjpeg")) && ($_FILES["file"]["size"] < 20000000000)) { if ($_FILES["file"]["error"] > 0) { echo "Return Code: " . $_FILES["file"]["error"] . "
"; } else { echo "Upload: " . $_FILES["file"]["name"] . "
"; echo "Type: " . $_FILES["file"]["type"] . "
"; echo "Size: " . ($_FILES["file"]["size"] / 1024) . " Kb
"; echo "Temp file: " . $_FILES["file"]["tmp_name"] . "
";

 if (file_exists("upload/" . $_FILES["file"]["name"]))
  {
  echo $_FILES["file"]["name"] . " already exists. ";
  }
else
  {
  move_uploaded_file($_FILES["file"]["tmp_name"], "upload/" . $_FILES["file"]["name"]);
  echo "Stored in: " . "upload/" . $_FILES["file"]["name"];
  }
}
} else { echo "Invalid file"; } ?>