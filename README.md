<div align="center">
  <img src="https://github.com/user-attachments/assets/239d395b-d624-4728-a86f-a668c1b4b9db"/>
</div>

# About❓

![image](https://github.com/user-attachments/assets/aa67b4fb-072e-44f6-8253-3701daa63ba6)

![ThetaFTP Server Op](https://github.com/user-attachments/assets/498e05f1-f1a1-40e0-a691-2c16eb3820b8)


* 🚀 **ThetaDrive** is a **cross-platform**, **open-source** 🌐 **Google Drive** alternative — an **easily configurable** ⚙️ **full-stack FTP server** 💾 that works on both 🪟 **Windows** and 🐧 **Linux**.
  
* It comes **ready to deploy** 📦 with advanced features like:
  * 🔐 **Two-step authentication via 📧 SMTP**
  * 🔒 **SSL/TLS connection encryption**
  * 🔒 **AES-256 based file encryption**
  * 📜 **SSL certificate generation**
  * 🛡️ **Customizable security & database attributes**

* Perfect for **power users** and **developers** who want **control and flexibility**! 💡💼
* **The website is hosted live at: https://theta-drive.org**

<br/>
<br/>

# 🔄 Changelog
📌 Latest Version: v1.0.9

* 🆕 Fixed bug regarding the load-up of the AES key
* 🆕 Moved the server settings into the 'appconfig.json' file
* 🆕 Updated the Wiki page

<br/>
<br/>

# 📥Download & Setup
* ➡️ To download the application, go to the [Release](https://github.com/CSharpTeoMan911/ThetaFTP/releases/tag/ThetaFTP-v1.0.0) section and download the application binary executables. Alternatively, you can download the binaries and source code from [SourceForge](https://sourceforge.net/projects/thetaftp/).
* ➡️ For instructions about how to use and configure the application, please visit the [Wiki](https://github.com/CSharpTeoMan911/ThetaFTP/wiki) section of this repository.

```
{
  "Logging": {
    "LogLevel": {
      "Microsoft.AspNetCore": "Warning",
      "Default": "Information"
    }
  },
  "AllowedHosts": "*",
  "Kestrel": {
    "Endpoints": {
      "Address": {
        "Url": "https://localhost:8000"
      }
    }
  },
  "ForwardedHeaders": {
    "ForwardedHeaders": "XForwardedFor, XForwardedProto"
  },
  "ServerConfigModel": {
    "is_reverse_proxy": false,
    "use_google_secrets": false,
    "aes_encryption_key_secret_url": "!!! REPLACE WITH SECRET URL !!!",
    "aes_encryption_key_version": "1",
    "server_salt_secret_url": "!!! REPLACE WITH SECRET URL !!!",
    "server_salt_secret_version": "1",
    "firebase_admin_token_secret_url": "!!! REPLACE WITH SECRET URL !!!",
    "firebase_admin_token_secret_version": "1",
    "mysql_user_password_secret_url": "!!! REPLACE WITH SECRET URL !!!",
    "mysql_user_password_secret_version": "1",
    "smtp_password_secret_url": "!!! REPLACE WITH SECRET URL !!!",
    "smtp_password_secret_version": "1",
    "custom_server_certificate_password_secret_url": "!!! REPLACE WITH SECRET URL !!!",
    "custom_server_certificate_password_secret_version": "1",
    "server_salt": "aaabbbccdddd",
    "use_secure_local_storage": true,
    "use_firebase": false,
    "firebase_admin_token": "!!! REPLACE WITH FIREBASE DATABASE ADMIN TOKEN !!!",
    "firebase_database_url": "!!! REPLACE WITH FIREBASE DATABASE URL !!!",
    "firebase_api_key": "!!! REPLACE WITH THE APP'S API KEY !!!",
    "firebase_auth_domain": "!!! REPLACE WITH THE APP'S AUTH DOMAIN !!!",
    "mysql_server_address": "127.0.0.1",
    "mysql_server_port": 3306,
    "mysql_user_id": "root",
    "mysql_user_password": "!!! REPLACE WITH MYSQL USER PASSWORD !!!",
    "mysql_database": "thetadrive",
    "twoStepAuth": false,
    "smtp_email": "!!! REPLACE WITH SMTPS EMAIL ADDRESS !!!",
    "smtp_password": "!!! REPLACE WITH SMTPS EMAIL PASSWORD !!!",
    "smtp_server": "smtp.gmail.com",
    "smtp_port": 587,
    "smtp_use_ssl": false,
    "WriteOperationsPerSecond": 200,
    "ConnectionTimeoutSeconds": 600,
    "enforce_https": true,
    "logs_expiration_days": 10,
    "validate_ssl_certificates": false,
    "validate_ssl_certificate_chain": false,
    "use_custom_ssl_certificate": false,
    "ensure_host_name_and_certificate_domain_name_match": false,
    "custom_server_certificate_path": "!!! REPLACE WITH THE PATH TO THE SERVER PRIVATE CERTIFICATE !!!",
    "custom_server_certificate_password": "!!! REPLACE WITH SERVER PRIVATE CERTIFICATE PASSWORD !!!",
    "hsts_max_age_days": 730,
    "max_concurent_connections": null,
    "max_request_buffer_size": null,
    "max_response_buffer_size": null,
    "use_file_encryption": false,
    "aes_encryption_key_location": "!!! REPLACE WITH THE PATH TO THE SERVER FILE ENCRYPTION KEY !!!",
    "google_oauth_client_id": "REPLACE WITH GOOGLE OAUTH CLIENT ID",
    "enable_google_login": false,
    "maximum_number_of_requests_per_minute": 10000,
    "maximum_number_of_queued_requests": 10000
  }
}
```

<br/>
<br/>

## 📚 Tech stack

<img width="2074" alt="Tech Stack Diagram (Copy) (1)" src="https://github.com/user-attachments/assets/d1d24f2e-935b-4204-bc79-bebca80d4f95">

<br/>
<br/>

# 🗂️ Ftp features

## 🔼 Upload: Upload files securely.
![Upload-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/634600b7-9ab9-436c-b561-46f0270cda46)

## 🔽 Download: Download files from your server.
![Download-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/9ac751f8-4dd6-484e-87b0-710d9ff1b0fb)

## 📦  Move: Move files within your directories.
![Move-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/75251690-b595-4958-a6d1-a1d87037a11c)

## ✏️ Rename: Rename files or directories.
![Rename-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/95a02480-a167-4ea8-a63b-00a8af8f368f)

## 🗂️ Create: Create directories
![CreateDirectory-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/3c91788b-9d32-4dca-be09-54bfb01e02b4)

## 🗑️ Delete: Delete files or directories securely.
![Delete-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/8c3c4951-89a5-40eb-a318-7ad6ee00ee14)


<br/>
<br/>
<br/>

# 🛡️ Security features

## 🔐 DDos protection
The application uses an IP based rate limiter to protect against DDos attacks, as well as ensuring a predictible CPU and RAM usage.  

## 🔐 Connection encryption
The application uses SSL/TLS encryption to secure the client-server connection, preventing unauthorized access. You can configure it with either self-signed certificates or trusted publisher certificates.

## 🔐 Local storage encryption
The application will encrypt data to be stored within the local storage on the server side to prevent mallicious actors from gaining access to session tokens.

## 🔐 File encryption
The application uses state of the art AES 256 (Advanced Encryption Standard) encryption algorithm to encrypt files.

## 🔐 Path traversal attack prevention

* Each user has a dedicated directory.
* The server validates any file path to ensure it is within the user’s directory.
* Invalid paths are rejected to prevent unauthorized file access.

## 🔐 SQL injection prevention

* The application uses parameterized queries to prevent SQL injection attacks.
* Special characters are escaped to ensure safe SQL command execution.

## 🧂➕#️⃣ Salting and hashing

All sensitive information stored in the database is hashed using the **SHA-512** algorithm and salted, for added security. This ensures that even if the database is compromised, the data remains unreadable

## 🪪 Log in session validation

For every operation requested by the user, such as uploading or downloading a file, the server will request for the client its **log in session key**. If the **log in session key** is expired or the **log in session key** is invalid, the server will log out the user. If the **log in session key** is valid, the server will only process request and information for the account associated with the **log in session key**, thus preventing mallicious attacks.

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
<br/>
<br/>

#  1️⃣0️⃣1️⃣1️⃣0️⃣ Binary manipulation mechanism

## 1️⃣0️⃣1️⃣ File upload binary manipulation mechanisms
![Memory management for write operations](https://github.com/user-attachments/assets/d83ef538-b0ab-42a0-81f6-09f3526db7d9)

## 1️⃣0️⃣1️⃣ MemoryPool characteristics
![Memory pool characteristics](https://github.com/user-attachments/assets/5237e2ab-647b-443e-972e-91dd96ad47dd)






