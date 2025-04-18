-- MySQL dump 10.13  Distrib 8.0.41, for Win64 (x86_64)
--
-- Host: localhost    Database: thetaftp
-- ------------------------------------------------------
-- Server version	8.0.41

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `accounts_waiting_for_approval`
--

DROP TABLE IF EXISTS `accounts_waiting_for_approval`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `accounts_waiting_for_approval` (
  `Account_Validation_Code` varchar(100) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Expiration_Date` datetime NOT NULL,
  UNIQUE KEY `Account_Validation_Code` (`Account_Validation_Code`),
  UNIQUE KEY `Email` (`Email`),
  CONSTRAINT `accounts_waiting_for_approval_ibfk_1` FOREIGN KEY (`Email`) REFERENCES `credentials` (`Email`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `accounts_waiting_for_approval`
--

LOCK TABLES `accounts_waiting_for_approval` WRITE;
/*!40000 ALTER TABLE `accounts_waiting_for_approval` DISABLE KEYS */;
/*!40000 ALTER TABLE `accounts_waiting_for_approval` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `accounts_waiting_for_deletion`
--

DROP TABLE IF EXISTS `accounts_waiting_for_deletion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `accounts_waiting_for_deletion` (
  `Account_Deletion_Code` varchar(100) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Expiration_Date` datetime NOT NULL,
  UNIQUE KEY `Account_Deletion_Code` (`Account_Deletion_Code`),
  KEY `Email` (`Email`),
  CONSTRAINT `accounts_waiting_for_deletion_ibfk_1` FOREIGN KEY (`Email`) REFERENCES `credentials` (`Email`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `accounts_waiting_for_deletion`
--

LOCK TABLES `accounts_waiting_for_deletion` WRITE;
/*!40000 ALTER TABLE `accounts_waiting_for_deletion` DISABLE KEYS */;
/*!40000 ALTER TABLE `accounts_waiting_for_deletion` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `accounts_waiting_for_password_change`
--

DROP TABLE IF EXISTS `accounts_waiting_for_password_change`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `accounts_waiting_for_password_change` (
  `Account_Password_Change_Code` varchar(100) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Expiration_Date` datetime NOT NULL,
  UNIQUE KEY `Account_Password_Change_Code` (`Account_Password_Change_Code`),
  KEY `Email` (`Email`),
  CONSTRAINT `accounts_waiting_for_password_change_ibfk_1` FOREIGN KEY (`Email`) REFERENCES `credentials` (`Email`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `accounts_waiting_for_password_change`
--

LOCK TABLES `accounts_waiting_for_password_change` WRITE;
/*!40000 ALTER TABLE `accounts_waiting_for_password_change` DISABLE KEYS */;
/*!40000 ALTER TABLE `accounts_waiting_for_password_change` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `credentials`
--

DROP TABLE IF EXISTS `credentials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `credentials` (
  `Email` varchar(100) NOT NULL,
  `Password` varchar(100) NOT NULL,
  PRIMARY KEY (`Email`),
  UNIQUE KEY `Email` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `credentials`
--

LOCK TABLES `credentials` WRITE;
/*!40000 ALTER TABLE `credentials` DISABLE KEYS */;
/*!40000 ALTER TABLE `credentials` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `google_accounts_waiting_for_deletion`
--

DROP TABLE IF EXISTS `google_accounts_waiting_for_deletion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `google_accounts_waiting_for_deletion` (
  `Account_Deletion_Code` varchar(100) NOT NULL,
  `Google_UID` varchar(100) NOT NULL,
  `Expiration_Date` datetime NOT NULL,
  PRIMARY KEY (`Account_Deletion_Code`),
  KEY `Google_UID` (`Google_UID`),
  CONSTRAINT `google_accounts_waiting_for_deletion_ibfk_1` FOREIGN KEY (`Google_UID`) REFERENCES `google_credentials` (`Google_UID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `google_accounts_waiting_for_deletion`
--

LOCK TABLES `google_accounts_waiting_for_deletion` WRITE;
/*!40000 ALTER TABLE `google_accounts_waiting_for_deletion` DISABLE KEYS */;
/*!40000 ALTER TABLE `google_accounts_waiting_for_deletion` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `google_credentials`
--

DROP TABLE IF EXISTS `google_credentials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `google_credentials` (
  `Google_UID` varchar(100) NOT NULL,
  PRIMARY KEY (`Google_UID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `google_credentials`
--

LOCK TABLES `google_credentials` WRITE;
/*!40000 ALTER TABLE `google_credentials` DISABLE KEYS */;
/*!40000 ALTER TABLE `google_credentials` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `google_log_in_sessions`
--

DROP TABLE IF EXISTS `google_log_in_sessions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `google_log_in_sessions` (
  `Log_In_Session_Key` varchar(100) NOT NULL,
  `Google_UID` varchar(100) NOT NULL,
  `Expiration_Date` datetime NOT NULL,
  PRIMARY KEY (`Log_In_Session_Key`),
  KEY `Google_UID` (`Google_UID`),
  CONSTRAINT `google_log_in_sessions_ibfk_1` FOREIGN KEY (`Google_UID`) REFERENCES `google_credentials` (`Google_UID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `google_log_in_sessions`
--

LOCK TABLES `google_log_in_sessions` WRITE;
/*!40000 ALTER TABLE `google_log_in_sessions` DISABLE KEYS */;
/*!40000 ALTER TABLE `google_log_in_sessions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `log_in_sessions`
--

DROP TABLE IF EXISTS `log_in_sessions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `log_in_sessions` (
  `Log_In_Session_Key` varchar(100) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Expiration_Date` datetime NOT NULL,
  UNIQUE KEY `Log_In_Session_Key` (`Log_In_Session_Key`),
  KEY `Email` (`Email`),
  CONSTRAINT `log_in_sessions_ibfk_1` FOREIGN KEY (`Email`) REFERENCES `credentials` (`Email`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `log_in_sessions`
--

LOCK TABLES `log_in_sessions` WRITE;
/*!40000 ALTER TABLE `log_in_sessions` DISABLE KEYS */;
/*!40000 ALTER TABLE `log_in_sessions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `log_in_sessions_waiting_for_approval`
--

DROP TABLE IF EXISTS `log_in_sessions_waiting_for_approval`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `log_in_sessions_waiting_for_approval` (
  `Log_In_Code` varchar(100) NOT NULL,
  `Log_In_Session_Key` varchar(100) NOT NULL,
  `Expiration_Date` datetime NOT NULL,
  UNIQUE KEY `Log_In_Code` (`Log_In_Code`),
  UNIQUE KEY `Log_In_Session_Key` (`Log_In_Session_Key`),
  CONSTRAINT `log_in_sessions_waiting_for_approval_ibfk_1` FOREIGN KEY (`Log_In_Session_Key`) REFERENCES `log_in_sessions` (`Log_In_Session_Key`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `log_in_sessions_waiting_for_approval`
--

LOCK TABLES `log_in_sessions_waiting_for_approval` WRITE;
/*!40000 ALTER TABLE `log_in_sessions_waiting_for_approval` DISABLE KEYS */;
/*!40000 ALTER TABLE `log_in_sessions_waiting_for_approval` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-03-31  3:19:01
