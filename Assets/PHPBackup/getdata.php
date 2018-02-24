<?php

        require_once "config.php";

        $userid = strtolower($_POST["userid"]);

        $query = "SELECT accounts.username, stats.kills, stats.deaths FROM accounts, stats WHERE accounts.userid = '".$userid."' AND stats.userid = '".$userid."'";

        $result = mysqli_query($con, $query);

        if($result){
                if(mysqli_num_rows($result) > 0){
                        $row = mysqli_fetch_assoc($result);
                        echo $userid.";".$row["username"].";".$row["kills"].";".$row["deaths"];
                } else {
                        echo "UserIdError";
                }
        } else {
                echo "RequestError";
        }

?>
