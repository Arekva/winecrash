<?php

function ValidNicknameCharacters()
{
    return array(
        "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
        "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
        "0","1","2","3","4","5","6","7","8","9",
        "_", "-"
        );
}

function ValidUsernameCharacters()
{
    return array(
        "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
        "0","1","2","3","4","5","6","7","8","9",
        "_", "-"
    );
}

function GUID()
{
    if (function_exists('com_create_guid') === true)
    {
        return trim(com_create_guid(), '{}');
    }

    return sprintf('%04X%04X-%04X-%04X-%04X-%04X%04X%04X', mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(16384, 20479), mt_rand(32768, 49151), mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(0, 65535));
}

function IsValidEmail($email)
{
    return filter_var($email, FILTER_VALIDATE_EMAIL) !== false;
}

function IsValidUsername($username)
{
    $lengthValid = strlen($username) <= 16;
    $charsValid = true;

    $validChars = ValidUsernameCharacters();
    for ($i = 0; $i < strlen($username); $i++)
    {
        if(!in_array($username[$i], $validChars))
        {
            $charsValid = false;
            break;
        }
    }

    return $lengthValid && $charsValid;
}