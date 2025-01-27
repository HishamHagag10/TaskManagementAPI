# Task Management API

## Overview
The Task Management API is a web-based application built using ASP.NET Core that allows users to manage tasks, projects, comments, and notifications. It provides endpoints for creating, updating, deleting, and retrieving tasks, as well as assigning users to tasks and managing task statuses.

## Features
- User authentication and authorization using JWT.
- CRUD operations for tasks, projects, comments, and tags.
- Task assignment and status management.
- Notification system for task updates.
- Dashboard for overview and statistics.
- Background job processing for sending notifications.

## Technologies Used
- ASP.NET Core 8
- Entity Framework Core
- Hangfire for background job processing
- FluentValidation for DTO validation
- JWT for authentication and authorization

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server or any other supported database
- Visual Studio or any other preferred IDE

### Installation

1. Clone the repository:
    ```bash 
           git clone https://github.com/HishamHagag10/TaskManagementAPI.git
           cd TaskManagementAPI
    ```
    
2. Set up the database:
    - Update the connection string in `appsettings.json` to point to your database.
    - Run the following command to apply migrations:
       ```bash
           dotnet ef database update
       ```
    
3. Run the application:
       ```bash
           dotnet run
       ```

### Configuration
- Update `appsettings.json` for any configuration changes such as database connection strings, JWT settings, etc.

## API Endpoints

### Task Endpoints
- `GET /api/Task/GetTasks` - Retrieve a list of tasks.
- `GET /api/Task/GetTaskById/{id}` - Retrieve a task by its ID.
- `GET /api/Task/MyTasks` - Retrieve tasks assigned to the authenticated user.
- `GET /api/Task/TasksDueSoon` - Retrieve tasks that are due soon.
- `PUT /api/Task/UpdateTaskStatus/{id}` - Update the status of a task.
- `PUT /api/Task/AssignUser/{id}` - Assign a user to a task.
- `PUT /api/Task/UnassignUser/{id}` - Unassign a user from a task.
- `POST /api/Task/AddTask` - Add a new task.
- `PUT /api/Task/UpdateTask/{id}` - Update an existing task.
- `DELETE /api/Task/DeleteTask/{id}` - Delete a task.

### Project Endpoints
- `GET /api/Project/GetProjects` - Retrieve a list of projects with optional filters for user email, status, pagination, and sorting.
- `GET /api/Project/GetProjectById/{id}` - Retrieve a project by its ID.
- `GET /api/Project/MyProjects` - Retrieve projects assigned to the authenticated user with optional pagination and sorting.
- `PUT /api/Project/UpdateProjectStatus/{id}` - Update the status of a project.
- `PUT /api/Project/AssignManager/{id}` - Assign a manager to a project.
- `PUT /api/Project/UnassignManager/{id}` - Unassign a manager from a project.
- `PUT /api/Project/AssignUser/{id}` - Assign a user to a project.
- `PUT /api/Project/UnassignUser/{id}` - Unassign a user from a project.
- `POST /api/Project/AddProject` - Add a new project.
- `PUT /api/Project/UpdateProject/{id}` - Update an existing project.
- `DELETE /api/Project/DeleteProject/{id}` - Delete a project.

### Tag Endpoints
- `GET /api/Tag/GetTags` - Retrieve a list of tags.
- `GET /api/Tag/GetTagById/{id}` - Retrieve a tag by its ID.
- `POST /api/Tag/AddTag` - Add a new tag.
- `PUT /api/Tag/UpdateTag/{id}` - Update an existing tag.
- `DELETE /api/Tag/DeleteTag/{id}` - Delete a tag.

### Comment Endpoints
- `GET /api/Comment/GetComment` - Retrieve comments with optional filters for task ID, user email, pagination, and sorting.
- `GET /api/Comment/GetMyComment` - Retrieve comments made by the authenticated user with optional filters for task ID, pagination, and sorting.
- `POST /api/Comment/AddComment` - Add a new comment to a task.
- `PUT /api/Comment/UpdateComment/{id}` - Update an existing comment.
- `DELETE /api/Comment/DeleteComment/{id}` - Delete a comment.

### Dashboard Endpoints
- `GET /api/Dashboard/UserDashBoard` - Retrieve the dashboard data for the authenticated user.
- `GET /api/Dashboard/AdminDashBoard` - Retrieve the dashboard data for the authenticated admin user.

### Notification Endpoints
- `GET /api/Notification/GetNotifications` - Retrieve a list of notifications.
- `POST /api/Notification/MarkAsRead` - Mark a notification as read.
- `POST /api/Notification/MarkAllAsRead` - Mark all notifications as read.
- `POST /api/Notification/SendNotification` - Send a new notification.
- `POST /api/Notification/DeleteNotification` - Delete a notification.

## Contributing
1. Fork the repository.
2. Create a new branch (`git checkout -b feature-branch`).
3. Make your changes and commit them (`git commit -m 'Add new feature'`).
4. Push to the branch (`git push origin feature-branch`).
5. Create a new Pull Request.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact
For any questions or feedback, please contact [hishamhagag18@gmail.com].
