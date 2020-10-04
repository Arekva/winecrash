<?php
    //header('Content-Type: application/json');

    $requestType = "none";

    if(isset($_REQUEST['token'])) $requestType = "token";
    else if(isset($_REQUEST['nickname'])) $requestType = "nickname";
    else if(isset($_REQUEST['guid'])) $requestType = "identifier";


    require "RequestError.php";

    require "dbManage.php";

    echo CreateUser("Arekva", "test", "arekvaa@protonmail.com", "Arekva");


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
                $error = new RequestError(400, "Invalid request code. Use 'token', 'nickname', 'guid'.");
                echo json_encode($error);
            }
            break;

    }


    //require "UserData.php";
    //$user = new User("Arthur", "https://www.youtube.com/watch?v=dQw4w9WgXcQ");
    //echo json_encode($user);
?>