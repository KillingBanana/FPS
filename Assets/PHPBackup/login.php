<?php

        require_once "config.php";

        $username = strtolower($_POST["username"]);
        $password = $_POST["password"];

        $query = "SELECT password_hash FROM accounts WHERE userid = '".$username."'";

        $result = mysqli_query($con, $query);

        if(mysqli_num_rows($result) > 0){
                while($row = mysqli_fetch_assoc($result)){
                        if(password_verify($password, $row["password_hash"])){
                                echo "Success";
                        } else {
                                echo "PasswordError";
                        }
                }
        } else {
                echo "UsernameError";
        }

?>
