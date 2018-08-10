# Chatovatko
A chatting C# application with end-to-end encryption.
The application is now under development. The works on front end hasn't started yet.

Chatovatko is currently tested under Ubuntu 18.04 and Windows 10.

## Important technical details
* **.NET Core 2.1** used for server-side service and for command-based console application (for testing porpuse)
* **.NET Standart 2.0** used for multiplatform client libraries and server-client shared libraries
* **Tls 1.2** is used for encrypting communication between server and client and for server verification
* **RSA** with key size **4096** is used for client verification and sending AES keys
* **AES265** used for encrypting messages
* **MySql** is required as server-side database.
* Sqlite is used as client side database
* Project is developed using **Visual Studio 2017** and **MySql Workbench 8.0**
* Also, project also benefits from Entity Framework Core, Newtonsoft.Json and Pomelo.EntityFrameworkCore.MySql
* SHA256 and SHA1 used for hashing
* Only little endian is supported
