# University API

## Overview
This is my first API project, designed to manage students, subjects, and exams in a university system. The API allows basic CRUD operations and provides insights into student performance.

## Features
- Manage students, subjects, and exams.
- Retrieve top-performing students.
- Calculate required future GPA.
- Identify subjects with the lowest average grades.

## Endpoints
### Student Endpoints
- `GET /api/student` - Get all students
- `GET /api/student/{id}` - Get a specific student
- `GET /api/student/top3` - Get the top 3 students by average grade
- `GET /api/student/future-average/{id}` - Calculate the required future GPA for a student
- `POST /api/student/get-many` - Retrieve multiple students by ID
- `POST /api/student` - Create a new student
- `PUT /api/student` - Update student details
- `DELETE /api/student/{id}` - Delete a student and their exams

### Subject Endpoints
- `GET /api/subject` - Get all subjects
- `GET /api/subject/{id}` - Get a specific subject
- `GET /api/subject/worst-subject` - Get subjects with the lowest average grades
- `POST /api/subject` - Create a new subject
- `PUT /api/subject/{id}` - Update a subject
- `DELETE /api/subject/{id}` - Delete a subject and its exams

## Setup
1. Clone the repository
2. Install dependencies
3. Configure the database connection
4. Run database migrations
5. Start the application

## Technologies Used
- .NET Core (C#)
- Entity Framework Core
- SQL Server

This project is for learning purposes. Feedback is welcome!💪
