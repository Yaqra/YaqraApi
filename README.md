# YaqraApi

YaqraApi is a robust API designed for Yaqra platform, a social media website tailored for book enthusiasts. It provides a rich set of features allowing users to review books, create playlists, engage in discussions, and stay updated with articles and news. This API facilitates a comprehensive social media experience for book geeks.

## Tech Stack

- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core (EFCore)**
- **SQL Server**
- **SignalR**
- **AutoMapper**
- **Worker Service**
- **Dapper**

## Features

### User Authentication and Profile Management
- **User Registration & Login**: Secure user registration and login with JWT authentication.
- **Token Management**: Refresh and revoke access tokens; manage expired tokens.
- **Profile Customization**: Update bio, username, password, profile picture, and cover photo.
- **Follow/Unfollow**: Follow or unfollow other users.
- **View Followers & Following**: Access and manage your followers and following lists.
- **Preferences**: Manage your favorite genres, authors, and reading goals.
- **Book Lists Management**: Keep track of books you want to read, are currently reading, or have already read.
- **User Profile**: Retrieve user profile details.

### Community Features
- **Post Interactions**: Like and comment on reviews, playlists, discussions, articles, and news.
- **Content Management**: Manage your own reviews, playlists, discussions, articles, and news.
- **Content Discovery**: Browse posts with various filtering and sorting options.
- **Main Page/Timeline**: See a personalized feed of book recommendations and user activities.

### Author Management
- **Authors**: Manage author details.
- **Author Ratings**: Retrieve ratings for authors based on their book ratings.
- **Author’s Books**: View books authored by a specific author.

### Book Management
- **Books**: Add and manage book details, including images, reviews, authors, genres, number of pages, and descriptions.
- **Book Details**: Access detailed information about books including image, reviews, author, genre, number of pages, and description.
- **Upcoming Books**: Retrieve information about upcoming books.
- **Trending Books**: Discover and receive recommendations for current popular books.

### Recommendation Engine
- **Content Suggestions**: Receive recommendations for books, reviews, playlists, discussions, articles, and news based on your activity.
- **Content Recommendations**: Recommend reviews, playlists, discussions, articles, and news based on user activity.

### Notifications
- **Real-Time Alerts**: Receive instant notifications for likes, comments, and other interactions on your posts.
- **Notification Management**: Easily view and manage your notifications.

### Book Finder
- **Book Search**: Quickly locate books by searching through genres, authors, or ratings.
- **Advanced Filtering**: Use detailed filters to refine your search and discover books that match your specific interests and criteria.

## Architecture

YaqraApi project is organized into the following key components:

- **Controllers**: Handle incoming HTTP requests and return responses.
- **DTOs**: Define data transfer objects for interacting with the API.
- **Helpers**: Utility classes that assist in various operations.
- **Hubs**: Include SignalR hubs for real-time notifications.
- **Models**: Define data structures, enums, and entities.
- **Repositories**: Manage data access with context, repository interfaces, and implementations.
- **Services**: Implement business logic and service interfaces.
- **Worker Service**: Background tasks and processing.

## Concepts

- **Dependency Injection**: Facilitates the injection of dependencies throughout the application.
- **Repository Pattern**: Abstracts data access, allowing for better separation of concerns.
- **DTOs**: Used for data transfer between client and server.
- **JWT Authentication**: Provides secure authentication using JSON Web Tokens.
- **AutoMapper**: Maps between objects and DTOs.
- **Caching**: Enhances performance by storing frequently accessed data.
- **Proxy Design Pattern**: Controls access to objects and can add additional behavior.
- **Multithreading**: Handles multiple operations concurrently.
- **Extension Methods**: Adds functionality to existing types.
- **Efficient Data Structures**: Utilizes data structures like hashtables for in-memory storage.
- **Efficient Data Queries**: Uses `IQueryable` for optimized data queries.
- **SOLID Principles**: Ensures the code adheres to the principles of object-oriented design.
## Worker Service

- **Purpose**: The worker service is a background service designed to clean up unwanted items from the database, such as revoked or expired tokens, to maintain database efficiency and integrity.
- **Implementation**: This service uses Dapper for efficient database operations, leveraging its lightweight and high-performance capabilities for direct SQL execution.

## Unit Testing

Unit tests are implemented using:
- **NUnit**
- **Moq**
- **SQL Server InMemory**

## Demo

_Coming soon!_

---

Thank you for exploring YaqraApi! We hope it enhances your book-related social interactions.
