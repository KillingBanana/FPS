<?php
        ini_set('display_errors', 1);

        error_reporting(E_ALL);

        require_once "config.php";

        $username = $_POST["username"];
        $userid = strtolower($username);
        $password = $_POST["password"];

        $password_hash = password_hash($password, PASSWORD_DEFAULT);

        $query = "SELECT userid FROM accounts WHERE userid = '".$userid."'";

        $result = mysqli_query($con, $query);

        if(mysqli_num_rows($result) > 0){
                echo "UsernameTaken";
        } else {
                $query = "INSERT INTO accounts (userid, username, password_hash) VALUES('".$userid."','".$username."','".$password_hash."')";

                $result = mysqli_query($con, $query);

                $query = "INSERT INTO stats (userid, kills, deaths) VALUES('".$userid."', 0, 0)";

                $result = mysqli_query($con, $query);

                echo "Success";
        }
?>
