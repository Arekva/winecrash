<?php

function CreateUser($username, $password, $email, $nickname)
{
    require "dbConnect.php";
    require_once "utils.php";

    $username = strtolower($username);
    $email = strtolower($email);

    $error = "none";

    if(!IsValidUsername($username))
    {
        $error = "Username is not valid. Must be <=16 and alphabetic,-,_ .";
        return $error;
    }
    else if(!IsValidEmail($email))
    {
        $error = "Email is not valid.";
        return $error;
    }

    $guid = strtolower(GUID());
    $nowStamp = (new DateTime)->getTimestamp();
    $hashedPass = hash("sha256", $password);


    ////////////////////////
    /// Duplicate check. ///
    ////////////////////////
    $sql =
        "SELECT user.guid AS guid, user.username, user.email, nick.value, nick.id FROM nickname as nick INNER JOIN user ON guid = nick.userGuid WHERE
         nick.id = (SELECT MAX(nm.id) FROM nickname as nm WHERE nm.userGuid = guid GROUP BY guid) AND (LOWER(nick.value) = LOWER(:nick) OR LOWER(user.username) = LOWER(:user) OR LOWER(user.email) = LOWER(:email)) GROUP BY guid";
    $exec = $bdd->prepare($sql);
    $exec->bindParam(':nick', $nickname);
    $exec->bindParam(':user', $username);
    $exec->bindParam(':email', $email);
    $exec->execute();
    $curseur=$exec->fetchAll();

    if(count($curseur) > 0)
    {
        if($curseur[0]['username'] == $username)
        {
            $error = "This username is already taken.";
        }
        else if($curseur[0]['email'] == $email)
        {
            $error = "An account with this email already exists.";
        }
        else if(strtolower($curseur[0]['value']) == strtolower($nickname))
        {
            $error = "This nickname is already taken.";
        }
        else
        {
            $error = "Unknown error. Please retry.";
        }

        return $error;
    }

    ////////////////////////
    /// New user insert. ///
    ////////////////////////
    $sql = "INSERT INTO user VALUES ('$guid', :username, '$hashedPass', :email, $nowStamp)";

    $exec = $bdd->prepare($sql);
    $exec->bindParam(':username', $username);
    $exec->bindParam(':email', $email);
    $exec->execute();


    ////////////////////////
    /// Nickname insert. ///
    ////////////////////////
    $sql = "INSERT INTO nickname(userGuid, value, changeStamp) VALUES ('$guid', :nickname, $nowStamp)";
    $exec = $bdd->prepare($sql);
    $exec->bindParam(':nickname', $nickname);
    $exec->execute();

}

function LogUserLogin($login, $password)
{
    require "dbConnect.php";
    require_once "utils.php";


    $hashedPass = hash("sha256", $password);



    $isUsername = IsValidUsername($login);

    $sql = "SELECT * FROM user WHERE user." . ($isUsername ? "username" : "email") . " = :login AND user.password = :pass";

    $exec = $bdd->prepare($sql);
    $exec->bindParam(':login', $login);
    $exec->bindParam(':pass', $hashedPass);
    $exec->execute();
    $curseur=$exec->fetchAll();
    if(count($curseur) == 0)
    {
        return "No user found.";
    }

    require_once "Token.php";

    $token = getRandomHex(32);
    $nowStamp = (new DateTime)->getTimestamp();
    $expireStamp = $nowStamp + 6912000; //expire time : 30 days
    $userGuid = $curseur[0]['guid'];

    $sql = "INSERT INTO token VALUES('$token', $nowStamp, '$userGuid')";
    $exec = $bdd->prepare($sql);
    $exec->execute();

    return $token;
}

