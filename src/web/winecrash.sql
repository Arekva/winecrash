-- phpMyAdmin SQL Dump
-- version 5.0.2
-- https://www.phpmyadmin.net/
--
-- Hôte : 127.0.0.1:3306
-- Généré le : Dim 04 oct. 2020 à 01:08
-- Version du serveur :  5.7.31
-- Version de PHP : 7.3.21

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de données : `winecrash`
--

-- --------------------------------------------------------

--
-- Structure de la table `nickname`
--

DROP TABLE IF EXISTS `nickname`;
CREATE TABLE IF NOT EXISTS `nickname` (
  `userGuid` varchar(36) NOT NULL COMMENT 'The GUID of the user.',
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'The identifier of this nickname. Relative to user.',
  `value` varchar(16) NOT NULL COMMENT 'The actual value of the nickname (max 16 chars) in alphanumeric + -/_',
  `changeStamp` int(11) NOT NULL COMMENT 'When this nickname has been set.',
  PRIMARY KEY (`userGuid`,`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1 COMMENT='Relative to user.';

-- --------------------------------------------------------

--
-- Structure de la table `skin`
--

DROP TABLE IF EXISTS `skin`;
CREATE TABLE IF NOT EXISTS `skin` (
  `userGuid` varchar(36) NOT NULL COMMENT 'The user using this skin.',
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'This skin identifier relative to the user.',
  `path` varchar(80) NOT NULL COMMENT 'The skin path relative to the image storage root.',
  PRIMARY KEY (`userGuid`,`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `token`
--

DROP TABLE IF EXISTS `token`;
CREATE TABLE IF NOT EXISTS `token` (
  `value` varchar(64) NOT NULL COMMENT 'The actual key of the token.',
  `creationStamp` int(11) NOT NULL COMMENT 'The creation timestamp of the token.',
  `userGuid` varchar(36) NOT NULL COMMENT 'The user''s unique identifier.',
  PRIMARY KEY (`value`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `user`
--

DROP TABLE IF EXISTS `user`;
CREATE TABLE IF NOT EXISTS `user` (
  `guid` varchar(36) NOT NULL COMMENT 'The global unique identifier - GUID - (128 bits) of the user. ',
  `username` varchar(16) NOT NULL COMMENT 'The username of the user. Must be in alphanumerial + -/_',
  `password` varchar(64) NOT NULL COMMENT 'The password of the user. Must be encrypted.',
  `email` varchar(32) NOT NULL COMMENT 'The email of the user.',
  `creationStamp` int(14) NOT NULL COMMENT 'The timestamp of the creation of this user.',
  PRIMARY KEY (`guid`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
