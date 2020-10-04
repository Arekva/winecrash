<?php
    //header('Content-Type: application/json');

    $requestType = "none";

    if(isset($_REQUEST['t'])) $requestType = "token";
    else if(isset($_REQUEST['n'])) $requestType = "nickname";
    else if(isset($_REQUEST['g'])) $requestType = "identifier";


    require "RequestError.php";

    require "dbManage.php";

    CreateUser("arekva", "test", "arekva@protonmail.com");


    switch ($requestType)
    {
        case "token":
            {

            }
            break;

        case "nickname":
            {

            }
            break;

        case "identifier":
            {

            }
            break;

        default:
            {
                $error = new RequestError(400, "Invalid request code. Use t for Token, n for Nickname and g for GUID.");
                echo json_encode($error);
            }
            break;

    }


    //require "UserData.php";
    //$user = new User("Arthur", "https://www.youtube.com/watch?v=dQw4w9WgXcQ");
    //echo json_encode($user);
?>