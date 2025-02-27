<div align="center">
  <img src="https://github.com/user-attachments/assets/758c76d5-1bd5-423a-a8cf-fdd0efd0f5e6"/>
</div>

# About❓

![image](https://github.com/user-attachments/assets/aa67b4fb-072e-44f6-8253-3701daa63ba6)

![ThetaFTP Server Op](https://github.com/user-attachments/assets/498e05f1-f1a1-40e0-a691-2c16eb3820b8)



**ThetaDrive** is a cross-platform, easily configurable FTP server that works on both Windows and Linux. It comes ready to deploy with advanced features like two-step authentication via SMTP, SSL/TLS connection encryption, SSL certificate generation, and customizable security and database attributes.

<br/>
<br/>

# 📥Download & Setup
* ➡️ To download the application, go to the [Release](https://github.com/CSharpTeoMan911/ThetaFTP/releases/tag/ThetaFTP-v1.0.0) section and download the application binary executables. 
* ➡️ For instructions about how the application must be configured and its behaviour please visit the [Wiki](https://github.com/CSharpTeoMan911/ThetaFTP/wiki) section of this repository.

<br/>
<br/>

## 📚 Tech stack

<img width="2074" alt="Tech Stack Diagram (Copy) (1)" src="https://github.com/user-attachments/assets/d1d24f2e-935b-4204-bc79-bebca80d4f95">

<br/>
<br/>

# 🗂️ Ftp features

## 🔼 Upload: Upload files securely.
![UploadFiles-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/e004908c-f866-4b2e-baed-8caf790b9bbb)

## 🔽 Download: Download files from your server.
![DownloadFiles-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/03a876b4-4418-40ae-a636-25de83af6615)

## 📦  Move: Move files within your directories.
![MoveFiles-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/6835f5b8-ce18-4b41-bebb-abfabf230dfe)

## ✏️ Rename: Rename files or directories.
![Rename-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/016c41a7-9145-45eb-80b5-e6c283924f43)

## 🗑️ Delete: Delete files or directories securely.
![DeleteFiles-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/d9ec2f1a-e0dc-4f11-b204-bd7478c8a138)

<br/>
<br/>
<br/>

# 🛡️ Security features

## 🔐 Connection encryption
The application uses SSL/TLS encryption to secure the client-server connection, preventing unauthorized access. You can configure it with either self-signed certificates or trusted publisher certificates.

<br/>
<br/>

## 🪪 Two-step registration

![Two-step registration](https://github.com/user-attachments/assets/bbd1e542-0655-40af-b7f0-ac1393689347)

1) **Account Creation:**

* When a user tries to create a new account, the server first checks if the account already exists in the system.

2) **Registration Code Generation:**

* If the account doesn't exist, the server generates a registration code and stores it in the database.
* The registration code is linked to the account, and until it is verified, the account remains invalid.

3) **Email Notification:**

* The server sends the registration code to the user's email address and prompts them to enter the code on the registration page.

4) **Code Validation:**

* The user enters the registration code, and the server verifies its validity.
* If the code is valid, the server deletes the registration code from the database and proceeds to the next step.

5) **Account Activation:**

* Once the registration code is validated, the account becomes valid, and the server sends a login session key to the user, effectively logging them into the system.

6) **Expiration & Failure:**

* The registration code has a 1-hour expiration time.
* If the user fails to enter the correct code within this period, both the account and registration code are deleted from the database.

<br/>
<br/>

## 🪪 Two-step authentication

![Two step auth diagram](https://github.com/user-attachments/assets/5b0f2a12-e483-4c92-af0d-4e180db4314c)


1) **Login Attempt:**

* When the user attempts to log in, the server first verifies the user credentials (username and password).


2) **Session Key & Login Code Generation:**

* If the credentials are valid, the server generates a login session key and a login code.
* Both the session key and the login code are stored in the database.

3) **Invalid Session Key (Until Verification):**

* The login code is tied to the session key. Until the login code is verified, the session key remains invalid.

4) **Email Notification:**

* The server sends the login code to the user's email address and prompts the user to enter the code.

5) **Code Validation:**

* If the user enters the correct login code, the server validates it and removes the code from the database.
* Once the login code is validated, the session key becomes active, and the user is granted access.

6) **Expiration & Failure:**

* The login code expires after 2 minutes.
* If the user doesn't enter the correct code within this time frame, both the session key and login code are deleted from the database, and the user must start the login process again.

<br/>
<br/>

## 🪪 Log in session validation

For every operation requested by the user, such as uploading or downloading a file, the server will request for the client its **log in session key**. If the **log in session key** is expired or the **log in session key** is invalid, the server will log out the user. If the **log in session key** is valid, the server will only process request and information for the account associated with the **log in session key**, thus preventing mallicious attacks.

<br/>
<br/>

## 🧂➕#️⃣ Salting and hashing

All sensitive information stored in the database is hashed using the **SHA-512** algorithm and salted for added security. This ensures that even if the database is compromised, the data remains unreadable

<br/>
<br/>

## 🔐 Path traversal attack prevention

* Each user has a dedicated directory.
* The server validates any file path to ensure it is within the user’s directory.
* Invalid paths are rejected to prevent unauthorized file access.

<br/>
<br/>

## 🔐 SQL injection prevention

* The application uses parameterized queries to prevent SQL injection attacks.
* Special characters are escaped to ensure safe SQL command execution.

![SQL command C#](https://github.com/user-attachments/assets/9b6d7a8b-76b8-474e-bbba-8ff9e62c2a83)





