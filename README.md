# Appointment Scheduling and Reservation System

Appointment Scheduling and Reservation System (A.S.R.) is a web application that allows staff to book a room and open up appointment slots for students to book for appointment or consultations with the relevant staff. 

## Use guide
This website provide three different web interfaces for three types of users, namely staffs, students and the web administrators. With the exception of the admin, staff and student who want to use this website needs to register themselves first. Registration and login can be made locally or externally using service provider such as Google and Facebook. Some restrictions apply, as student and staff can only register using their RMIT University email, given that this website will be used within RMIT only. 

### Staff
Staff can only create booking four booking slots per day and each room only allows two slots each day. Through this website, staff can create or remove slots when needed.

### Student
Student can book into any slots that are made available by staffs. However, each students are limited to only one appointment per day. Through this website, student can make or cancel booking when needed.

### Admin
Website administrator can help staff and students with using their account. As such, admin web interface have the functionality of both staff and student. In addition, admin is able to open new room for staffs to create their appointment slots.

## Web API
Web API is used to expose some of the resources available in our database to perform CRUD operation to our database. The benefit of using API to read and make changes to the database instead of making the changes directly is to ensure the extensibility of our application. In the event that the database structure has been changed, the web server do not need to be modified. It also allow multiple applications to communicate information with one another faster, as they do not need to establish connection with the databases. Lastly, it improves data security. Some of the information in the database, such as passwords or social security number should not be exposed to anybody, including the developer. API helps to hide sensitive informations but still allow our application to make the relevant changes in the database.

## Angular
Angular is using component-based architecture, we build website piece by piece from the component. This component can be reused again and each of the component is decoupled from each other. This concept makes the website easy to maintain and update. And also the component concept makes it easy to do unit test to the website.s

## Social media login
In this website, we implement Facebook and Google login for both student and staff. The advantage of using these social media login is so that users can ensure that their passwords can be stored more securely. Big companies such as Google and Facebook invest more in user security, making it a much better choice to store the password than in a local database. The additional benefit for user is that they do not need to create a new password for each and every new websites. This helps to lighten users' mental load and reduce the possibility of the users being locked out of their account permanently.

## Contributor
Callista Kitaro (S3446271)
Martina Yulianti (S3625730)
