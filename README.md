<div align="center">
  <img src="https://github.com/user-attachments/assets/758c76d5-1bd5-423a-a8cf-fdd0efd0f5e6"/>
</div>

# About

![image](https://github.com/user-attachments/assets/aa67b4fb-072e-44f6-8253-3701daa63ba6)

![ThetaFTP Server Op](https://github.com/user-attachments/assets/498e05f1-f1a1-40e0-a691-2c16eb3820b8)



**ThetaDrive** is a cross-platform easily configurable, ready to deploy, FTP server application that works on both Windows and Linux. It has advanced features such as: two step authentication via SMTP, connection encryption via SSL/TLS, SSL certificate generation, and the ability to customise multiple attributes related to security and databases.

<br/>
<br/>

## Tech stack

<img width="2074" alt="Tech Stack Diagram (Copy) (1)" src="https://github.com/user-attachments/assets/d1d24f2e-935b-4204-bc79-bebca80d4f95">

<br/>
<br/>

# Usage
To download the application, go to the [Release](https://github.com/CSharpTeoMan911/ThetaFTP/releases/tag/ThetaFTP-v1.0.0) section and download the application binary executables. For instructions about how the application must be configured and its behaviour please visit the [Wiki](https://github.com/CSharpTeoMan911/ThetaFTP/wiki) section of this repository.

<br/>
<br/>

# Ftp features

## Upload
![UploadFiles-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/e004908c-f866-4b2e-baed-8caf790b9bbb)

## Download
![DownloadFiles-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/03a876b4-4418-40ae-a636-25de83af6615)

## Move
![MoveFiles-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/6835f5b8-ce18-4b41-bebb-abfabf230dfe)

## Rename
![RenameFile-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/69cf6cf2-3311-4e76-b0b9-b0f20c3ab1f6)

## Delete
![DeleteFiles-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/d9ec2f1a-e0dc-4f11-b204-bd7478c8a138)

# Security features

## Connection encryption
The application can encrypt the client/server connection using the TLS/SSL protocols by using self-signed certificates or trusted publisher certificates. 

## Two-step registration

![Two-step registration](https://github.com/user-attachments/assets/bbd1e542-0655-40af-b7f0-ac1393689347)



When the user is creating an account, the server will verify if the account already exists, and if it does not exist, it will generate a **registration code** and store it in the database. The **registration code** is associated with the account, and as long as the registration code exists, the account is marked as invalid. Afterwards, the server will send the **registration code** to the user's email address and prompt the user for the **registration code**. If the **registration code** is valid, the server will delete the **registration code** from the database and send a **log in session key** to the user, and thus making the account valid as well as logging in the user. The **registration code** has an expiration date of 1 hour. If the user does not validate the **registration code**, both the account and the **registration code** will be deleted from the database.

<br/>
<br/>

## Two-step authentication

![Two step auth diagram](https://github.com/user-attachments/assets/5b0f2a12-e483-4c92-af0d-4e180db4314c)



When the user is logging in, the server will verify the user credentials and, if the credentials are valid, it will generate a **log in session key**, as well as a **log in code**, both the **log in session key** and the **log in code** will be stored in the database. The **log in code** is associated with the **log in session key** and as long as the **log in code** associated with the **log in session key** exist, the **log in session key** is invalid. Afterwards, the server will send the **log in code** to the user's email address and prompt the user for the **log in code**. If the **log in code** is valid, the server will delete the **log in code** from the database and thus making the **log in session key** valid. The **log in code** has an expiration date of 2 minutes. If the user does not validate the **log in code**, both the **log in session key** and the **log in code** will be deleted from the database.

<br/>
<br/>

## Log in session validation

For every operation requested by the user, such as uploading or downloading a file, the server will request for the client its **log in session key**. If the **log in session key** is expired or the **log in session key** is invalid, the server will log out the user. If the **log in session key** is valid, the server will only process request and information for the account associated with the **log in session key**, thus preventing mallicious attacks.

<br/>
<br/>

## Salting and hashing

All sesitive information to be stored in the database is both hashed and salted. The hashing algorithm used is **SHA512**. 

<br/>
<br/>

## Path traversal attack prevention

Each user has its own directory associated with its account. Every time a user is performing an operation related to the file system, the server will process the path given by the user and verify if it has as its root the directory associated with the account. If the path to be processed does not have as its root, the folder associated with the account, the operation is cancelled, thus preventing mallicious attacks on the server's directory level, as well as preventing every user's information being compromised.

<br/>
<br/>

## SQL injection prevention

The application is using parametrised parameter injection within SQL commands, which in turn is escaping any special charater from the SQL string, thus making any SQL injection attack impossible

![SQL command C#](https://github.com/user-attachments/assets/9b6d7a8b-76b8-474e-bbba-8ff9e62c2a83)





