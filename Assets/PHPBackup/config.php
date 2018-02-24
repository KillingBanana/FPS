<?php

        ini_set('display_errors', 1);

        error_reporting(E_ALL);

        $servername = "kbanana.cspwhlmdmsou.eu-west-3.rds.amazonaws.com";
        $username = "admin";
        $password = "M1m1ch3D4b0um";
        $dbname = "kbananafps";

        $con = new mysqli($servername, $username, $password, $dbname);

        if(!$con){
                echo "Connection failed. ".mysqli_connect_error;
        }

?>
