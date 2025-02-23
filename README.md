# University API

## Overview
The **University API** is my first RESTful web service, designed to manage students, subjects, and exams within a university system. It provides basic CRUD operations and additional analytical endpoints to retrieve useful insights about students and subjects.

## Technologies Used
- **.NET Core 9.0**
- **Entity Framework Core**
- **ASP.NET Core Web API**
- **Microsoft SQL Server**

## Endpoints

### **Student Controller** (`api/student`)
#### **GET**
- `GET /api/student` - Retrieves all students.
- `GET /api/student/{id}` - Retrieves a single student by ID.
- `GET /api/student/top3` - Retrieves the top 3 students with the highest weighted average grade.
- `GET /api/student/worst3` - Retrieves the bottom 3 subjects with the lowest weighted average grade.
- `GET /api/student/future-average/{id}` - Calculates the required average grade for a student to reach a final GPA of 28.

#### **POST**
- `POST /api/student/get-many` - Retrieves specific students based on a list of IDs.

#### **PUT**
- `PUT /api/student{id}}` - Updates an existing student's information.

#### **DELETE**
- `DELETE /api/student/{id}` - Deletes a student and their associated exams.

### **Subject Controller** (`api/subject`)
#### **GET**
- `GET /api/subject` - Retrieves all subjects.
- `GET /api/subject/{id}` - Retrieves a single subject by ID.
- `GET /api/subject/worst3` - Retrieves the 3 subjects with the lowest average grades.

#### **POST**
- `POST /api/subject` - Creates a new subject.
- `POST /api/subject/get-many` - Retrieves specific subjects based on a list of IDs.

#### **PUT**
- `PUT /api/subject/{id}` - Updates an existing subject.

#### **DELETE**
- `DELETE /api/subject/{id}` - Deletes a subject and its associated exams.

### **Exam Controller** (`api/exam`)
#### **GET**
- `GET /api/exam` - Retrieves all exams.
- `GET /api/exam/{studentId}/{subjectId}` - Retrieves a single exam by ID.
- `GET /api/exam/top3` - Retrieves the top 3 exams with the highest grades.
- `GET /api/exam/worst3` - Retrieves the bottom 3 exams with the lowest grades.

#### **POST**
- `POST /api/exam` - Creates a new exam.
- `POST /api/exam/get-many` - Retrieves aggregated exam data grouped by subject.

#### **PUT**
- `PUT /api/exam/{id}` - Updates an existing exam.

#### **DELETE**
- `DELETE /api/exam/{id}` - Deletes an exam.

## Database Relationships
- **Student** (1) ↔ (N) **Exam** (Many exams per student)
- **Subject** (1) ↔ (N) **Exam** (Many exams per subject)
- **Exam** acts as a **junction table** between **Student** and **Subject**

## Installation and Setup
1. Clone the repository:
   ```sh
   git clone https://github.com/isid123/UniversityApi.git
   cd UniversityApi
   ```
2. Configure the database in `appsettings.json`:
   ```json
   "ConnectionStrings": {
       "Default": "Server=YOUR_SERVER;Database=UniversityDb;Trusted_Connection=True;"
   }
   ```
3. Apply migrations and seed data (if applicable):
   ```sh
   dotnet ef database update
   ```
4. Run the application:
   ```sh
   dotnet run
   ```
## License
This project is licensed under the MIT License.

