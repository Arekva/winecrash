<?php

function CreateUser($username, $password, $email)
{
    require "dbConnect.php";
    require "utils.php";

    $guid = strtolower(GUID());
    $nowStamp = (new DateTime)->getTimestamp();
    $hashedPass = hash("sha256", $password);

    $sql = "INSERT INTO user VALUES ('$guid', :username, '$hashedPass', :email, $nowStamp)";


    echo $username;
    $exec = $bdd->prepare($sql);
    $exec->bindParam(':username', $username);
    $exec->bindParam(':email', $email);

    $exec->execute();
}