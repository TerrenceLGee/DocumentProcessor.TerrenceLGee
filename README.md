# Document Processor
A C# 14/.Net 10 application that reads contacts from an Excel spread sheet and seeds a SQLite database with these contacts.

Created following the curriculm of [C# Academy](https://www.thecsharpacademy.com/)
[Document Processor](https://www.thecsharpacademy.com/project/20/document-processor)

## Features
- This is basically a rewrite of the previous [Phonebook project](https://www.thecsharpacademy.com/project/16/phonebook). But instead of being a console application. I decided to make a small [AvaloniaUI](https://docs.avaloniaui.net/docs/welcome?utm_source=avaloniaui&utm_medium=referral&utm_content=nav) application.
- On the first run of the project if there is no database it will be created and it will be seeded with contacts from an Excel spreadsheet (included in the project root directory). The database connection string is in appsettings.json if you want to change the database name before running this program.
- Allows the user to add, update, delete, and retrieve contacts from the database. 
- Allows the user to send email to a contact (the contacts that are seeded are not 'real' contacts with 'real' email addresses so please add a contact with a legitimate email address to test this functionality).
- Allows the user to generate a 'report' of the contacts in either, .txt, .xlsx, .csv, or .pdf format. 
- Also gives the user upon generating the report the option to email the report to their email address (The one specified in appsettings.json see below in the instructions on sending email).
- Allows the user to add contacts to the database from a file in either .txt, .xlsx, or .csv format.
- Implements retry functionality for sending emails in case of smtp errors.
- Implements logging
- Unit testing of the email service.
- Integration testing of the CRUD operations of the database.

## Challenges Faced When Implementing This Project
- No real challenges because the external libraries used for .xlsx and .pdf functionality had great easy to understand documentation.
- No real challenges with AvaloniaUI because it was a simple project.

## What I Learned Implementing This Project
- Learned the importance of using the examples in documentation to put together my own usage of the methods in the library documented.
- Learned how to work with files on a basic level which will allow me in the future to implement more complex operations with files.

## Instructions To Send Email From This Application
- First off you will need a gmail account. Once inside of your gmail account click on the Google Account icon.
- Click on the button that says 'Manage your Google Account'.
- You will be taken to your Account options. Click on the option 'Security & sign-in' Click on and enable Two-Factor Authentication if it is not already enabled.
- After you have enabled Two-Factor Authentication, go to search at the top of the page and type in 'App Password'
- Click on 'App Password' in the results that are displayed from the drop down. From there you will be taken to a page to generate an app password.
- Simply type in the name for the App Password and click create and from there a pop up will display with your app password. Please copy this password so that you will have it for the next step.
- Next you need to set up the configuration files with the relevant information that will allow email to be sent from this application.
- The way the project is set up all information needed to send an email except for the subject and body (will be entered when you send the email) and the receiver's name and email address (which will be populated when you select a contact to email) are to be configured in the appsettings.json file (you can also use user secrets). In appsettings.json just fill in the relevant information:
```
"EmailConfiguration": {
    "SenderName": "Sender's name",
    "SenderEmail": "Sender's email (your gmail email address)",
    "Password": "Your app password here",
    "Host": "smtp.gmail.com",
    "Port": 587
  }
```
- Then you can simply run the program:
- On Visual Studio and JetBrains Rider you can simply build and run the program from the IDE.
- With Visual Studio Code you can enter the following .NET CLI commands from the command line of your choice from the project directory:

```
dotnet build && dotnet run
```

## Instructions for adding contact from a file.
- Please make sure if you are adding contacts from a file to place the contact information in the following order per line:
```
Contact First Name  Contact Middle Initial  Contact Last Name  Contact Email Address  Contact Telephone Number
```
- For adding contacts from a .xlsx file just put each item it's own cell in each row.
- For adding contacts from a .csv file just separate each item with a comma.
- For adding contacts from a .txt file just separate each item with a space.

## Areas To Improve Upon
- Everything. With each project done from The C# Academy I feel I get better and learn more, but also realize there is always more to learn, more improvements to make and that is one of the most rewarding parts of this "journey".

## Technologies Used
- [.NET10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [ClosedXML](https://github.com/closedxml/closedxml)
- [QuestPDF](https://www.questpdf.com/quick-start.html)
- [CsvHelper](https://joshclose.github.io/CsvHelper/)
- [Avalonia](https://avaloniaui.net/)
- [MessageBox.Avalonia](https://github.com/AvaloniaCommunity/MessageBox.Avalonia)
- [Serilog](https://serilog.net/)
- [Microsoft.EntityFrameworkCore](https://learn.microsoft.com/en-us/ef/)
- [SQLite](https://sqlite.org/)
- [XUnit](https://xunit.net/?tabs=cs)
- [Moq](https://github.com/devlooped/moq)
- [MailKit](https://github.com/jstedfast/MailKit)

## Helpful Resources Used
- [Create Excels with C# and ClosedXML: A tutorial](https://itenium.be/blog/dotnet/create-xlsx-excel-with-closedxml-csharp/)
- [How to EASILY Create a PDF in C# .NET with QuestPDF!](https://www.youtube.com/watch?v=3qEPv-67iRg)
- [How to Work with CSV Files - Creating and Reading With Different Options](https://www.youtube.com/watch?v=OguO5TpF-AA)
- [How to Use Factory Pattern With Dependency Injection in .NET](https://www.youtube.com/watch?v=7HXGmHseQRQ)
- [Splitting string based on variable number of white spaces](https://stackoverflow.com/questions/12387577/splitting-string-based-on-variable-number-of-white-spaces)
