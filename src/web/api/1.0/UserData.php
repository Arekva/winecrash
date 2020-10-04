<?php
class User
{
    public $nickname;

    public $skin;

    public function __construct($nick, $skinLink)
    {
        $this->nickname = $nick;
        $this->skin = $skinLink;
    }
}

?>