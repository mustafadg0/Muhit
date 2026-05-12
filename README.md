# Muhit360 API

AI-powered neighborhood intelligence platform backend for the Muhit360 mobile application.

Muhit360 helps users discover, compare, and analyze neighborhoods using community-driven reviews, location-based insights, scoring systems, and AI-supported analysis.

---

# Tech Stack

- .NET 8 Web API
- Entity Framework Core
- SQL Server
- Azure App Service
- Azure Blob Storage
- Swagger / OpenAPI
- JWT Authentication
- Clean Architecture
- RESTful API

---

# Core Features

## Authentication & User Management
- User registration
- User login
- JWT-based authentication
- Profile management
- Profile image upload
- Membership system

## Neighborhood System
- City / District / Neighborhood hierarchy
- Neighborhood search
- Neighborhood detail pages
- Neighborhood scoring system
- AI-generated neighborhood analysis
- Community-driven insights

## Reviews & Community
- User reviews
- Neighborhood ratings
- Review comments
- Score aggregation
- Community feedback system

## AI Features
- AI-powered neighborhood summaries
- Safety analysis
- Transportation analysis
- Quietness analysis
- Social life analysis
- Cost analysis
- AI recommendation engine (planned)

## Media & Storage
- Azure Blob Storage integration
- Profile image upload
- Secure file handling
- Image validation

## Future Features
- Map-based neighborhood discovery
- Heatmaps
- Live crowd density estimation
- Favorite neighborhoods
- Neighborhood comparison
- Personalized recommendations
- Premium membership
- Notification system
- Admin panel
- Analytics dashboard

---

# Architecture

Project structure follows Clean Architecture principles:

```text
Muhit.Api
Muhit.Application
Muhit.Domain
Muhit.Persistence
Muhit.Infrastructure
```


# API Documentation

Swagger UI:

```text
/api/swagger
```

---


# Deployment

Production environment is hosted on:

- Azure App Service
- Azure Blob Storage


# Vision

Muhit360 aims to become a modern AI-powered neighborhood intelligence platform helping people make smarter decisions about where to live, work, travel, and invest.
