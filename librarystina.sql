-- --------------------------------------------------------
-- Värd:                         127.0.0.1
-- Serverversion:                8.0.46 - MySQL Community Server - GPL
-- Server-OS:                    Linux
-- HeidiSQL Version:             12.21.0.7344
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumpar databasstruktur för librarystina
CREATE DATABASE IF NOT EXISTS `librarystina` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `librarystina`;

-- Dumpar struktur för tabell librarystina.category
DROP TABLE IF EXISTS `category`;
CREATE TABLE IF NOT EXISTS `category` (
  `sabcode` int NOT NULL AUTO_INCREMENT,
  `description` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`sabcode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell librarystina.copy
DROP TABLE IF EXISTS `copy`;
CREATE TABLE IF NOT EXISTS `copy` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `FORmedia` int DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `FORmedia` (`FORmedia`),
  CONSTRAINT `copy_ibfk_1` FOREIGN KEY (`FORmedia`) REFERENCES `media` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell librarystina.invoice
DROP TABLE IF EXISTS `invoice`;
CREATE TABLE IF NOT EXISTS `invoice` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `amount` decimal(10,0) DEFAULT NULL,
  `enddate` date DEFAULT NULL,
  `status` varchar(255) DEFAULT NULL,
  `FORuser` int DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `FORuser` (`FORuser`),
  CONSTRAINT `invoice_ibfk_1` FOREIGN KEY (`FORuser`) REFERENCES `user` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell librarystina.loan
DROP TABLE IF EXISTS `loan`;
CREATE TABLE IF NOT EXISTS `loan` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `loanstartdate` date DEFAULT NULL,
  `returndate` date DEFAULT NULL,
  `status` varchar(255) DEFAULT NULL,
  `FORuser` int DEFAULT NULL,
  `FORcopy` int DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `FORuser` (`FORuser`),
  KEY `FORcopy` (`FORcopy`),
  CONSTRAINT `loan_ibfk_1` FOREIGN KEY (`FORuser`) REFERENCES `user` (`ID`),
  CONSTRAINT `loan_ibfk_2` FOREIGN KEY (`FORcopy`) REFERENCES `copy` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell librarystina.media
DROP TABLE IF EXISTS `media`;
CREATE TABLE IF NOT EXISTS `media` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `mediatype` varchar(255) DEFAULT NULL,
  `purchasevalue` decimal(10,0) DEFAULT NULL,
  `replacementvalue` decimal(10,0) DEFAULT NULL,
  `barcode` varchar(255) DEFAULT NULL,
  `isbn` varchar(255) DEFAULT NULL,
  `ean` varchar(255) DEFAULT NULL,
  `FORcategory` int DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `FORcategory` (`FORcategory`),
  CONSTRAINT `media_ibfk_1` FOREIGN KEY (`FORcategory`) REFERENCES `category` (`sabcode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell librarystina.person
DROP TABLE IF EXISTS `person`;
CREATE TABLE IF NOT EXISTS `person` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `fname` varchar(255) DEFAULT NULL,
  `ename` varchar(255) DEFAULT NULL,
  `title` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell librarystina.personmedia
DROP TABLE IF EXISTS `personmedia`;
CREATE TABLE IF NOT EXISTS `personmedia` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `FORmedia` int DEFAULT NULL,
  `FORperson` int DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `FORmedia` (`FORmedia`),
  KEY `FORperson` (`FORperson`),
  CONSTRAINT `personmedia_ibfk_1` FOREIGN KEY (`FORmedia`) REFERENCES `media` (`ID`),
  CONSTRAINT `personmedia_ibfk_2` FOREIGN KEY (`FORperson`) REFERENCES `person` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dataexport var bortvalt.

-- Dumpar struktur för tabell librarystina.user
DROP TABLE IF EXISTS `user`;
CREATE TABLE IF NOT EXISTS `user` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `role` varchar(255) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `password` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dataexport var bortvalt.

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
