<?php


class RequestError
{
    public $code;

    public $message;

    public function __construct($errorCore, $errorMessage)
    {
        $this->code = $errorCore;
        $this->message = $errorMessage;
    }
}