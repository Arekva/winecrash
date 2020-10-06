<?php
header('Content-Type: application/json');

require "RequestError.php";

if(!isset($_REQUEST['l']) || !isset($_REQUEST['p']))
{
    echo json_encode(new RequestError(400, "Invalid request: the user (u) + password (p) must be set OR token (t) must be set."));
    return;
}

require_once "utils.php";

require "dbManage.php";

$login = strtolower($_REQUEST['l']);

if(!IsValidUsername($login) && !IsValidEmail($login))
{
    if(!isset($_REQUEST['t']))
    {
        echo json_encode(new RequestError(401, "Authorisation refused: login is not valid, must be email or username."));
        return;
    }
    else if (IsValidToken(strtolower($_REQUEST['t']))
    {
        echo json_encode(new RequestError(400, "Invalid request: token is not valid."));
        return;
    }

    $token = strtolower($_REQUEST['t']);
}

else
{
    $result = LogUserLogin($login, $_REQUEST['p']);

    if(IsValidToken($result))
    {
        echo json_encode(new Token($result));
    }

    else
    {
        echo json_encode(new RequestError(401, "Authorisation refused: invalid credentials."));
    }
}



