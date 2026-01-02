# 🏢 Conference Room Booking System

A full-stack web application for managing and reserving conference rooms, built with **ASP.NET Core MVC** and **Entity Framework Core**. The system features role-based access control, real-time availability checking, and an admin dashboard for managing reservations.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat&logo=c-sharp)
![SQLite](https://img.shields.io/badge/SQLite-3.0-003B57?style=flat&logo=sqlite)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.0-7952B3?style=flat&logo=bootstrap)

## 📋 Table of Contents
- [Features](#-features)
- [Technologies](#-technologies)
- [Architecture](#-architecture)
- [Getting Started](#-getting-started)
- [Usage](#-usage)
- [Video presentation](#-video-presentation)
- [Database Schema](#-database-schema)
- [API Endpoints](#-api-endpoints)
- [Contributing](#-contributing)
- [License](#-license)

## ✨ Features

### User Features
- 🔐 **Secure Authentication** - Register, login, and role-based authorization using ASP.NET Core Identity
- 🏢 **Browse Rooms** - View available conference rooms with detailed information (capacity, equipment, pricing)
- 📅 **Create Reservations** - Book rooms with date/time selection and conflict detection
- ✏️ **Manage Bookings** - Edit or cancel pending reservations
- 📊 **Reservation Dashboard** - Track all personal reservations with status indicators

### Admin Features
- 👨‍💼 **Admin Dashboard** - Comprehensive overview with statistics and pending approvals
- ✅ **Approve/Reject Reservations** - Review and manage all booking requests
- 🏗️ **Room Management** - Full CRUD operations for conference rooms
- 📈 **Analytics** - View booking statistics and room popularity metrics
- 🔍 **Advanced Filtering** - Filter reservations by status (Pending, Approved, Rejected, Cancelled)

### Technical Features
- ⚡ **Real-time Validation** - Client and server-side validation with conflict detection
- 🔒 **Security** - CSRF protection, password hashing, role-based access control
- 📱 **Responsive Design** - Mobile-first design with modern gradient UI
- 🎨 **Modern UI/UX** - Custom CSS with animations, hover effects, and intuitive navigation
- 🗄️ **Database Seeding** - Automatic initialization with sample data

## 🛠 Technologies

### Backend
- **ASP.NET Core 8.0** - Web framework
- **Entity Framework Core** - ORM for database operations
- **ASP.NET Core Identity** - Authentication and authorization
- **SQLite** - Lightweight database
- **C# 12** - Programming language

### Frontend
- **Razor Pages** - Server-side rendering
- **Bootstrap 5** - UI framework
- **Custom CSS** - Gradient designs, animations
- **JavaScript** - Client-side validation and interactivity

### Development Tools
- **Visual Studio Code** - IDE
- **Git** - Version control
- **.NET CLI** - Build and migration tools

## 🏗 Architecture

This project follows the **Model-View-Controller (MVC)** architectural pattern:
```
ConferenceRoomBooking/
├── Controllers/          # Business logic and request handling
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── ConferenceRoomsController.cs
│   ├── ReservationsController.cs
│   └── HomeController.cs
├── Models/              # Data entities
│   ├── AppUser.cs
│   ├── ConferenceRoom.cs
│   └── Reservation.cs
├── Views/               # Razor UI templates
│   ├── Account/
│   ├── Admin/
│   ├── ConferenceRooms/
│   ├── Reservations/
│   ├── Home/
│   └── Shared/
├── ViewModels/          # Form models
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   └── ...
├── Data/                # Database context
│   └── ApplicationDbContext.cs
└── wwwroot/             # Static files (CSS, JS)
```

### Database Schema
```
┌─────────────────┐       ┌─────────────────┐       ┌─────────────────┐
│   AspNetUsers   │       │  Reservations   │       │ ConferenceRooms │
├─────────────────┤       ├─────────────────┤       ├─────────────────┤
│ Id (PK)         │◄──────┤ UserId (FK)     │       │ Id (PK)         │
│ Email           │   1:N │ RoomId (FK)     ├──────►│ Name            │
│ PasswordHash    │       │ StartTime       │   N:1 │ Description     │
│ ...             │       │ EndTime         │       │ Capacity        │
└─────────────────┘       │ Status          │       │ PricePerHour    │
                          │ Purpose         │       │ Equipment       │
                          │ CreatedAt       │       │ IsAvailable     │
                          └─────────────────┘       └─────────────────┘
```

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Git](https://git-scm.com/downloads)
- [Visual Studio Code](https://code.visualstudio.com/) (recommended)

### Installation

1. **Clone the repository**
```bash
   git clone https://github.com/pawelszojda/ConferenceRoomBooking.git
   cd ConferenceRoomBooking
```

2. **Restore dependencies**
```bash
   dotnet restore
```

3. **Apply database migrations**
```bash
   dotnet ef database update
```

4. **Run the application**
```bash
   dotnet run
```

5. **Open in browser**
```
   http://localhost:5050
```

### Default Credentials

**Admin Account:**
- Email: `admin@conference.com`
- Password: `Admin123!`

The database is seeded with 3 sample conference rooms.

## 💻 Usage

### For Users

1. **Register** a new account at `/Account/Register`
2. **Browse** available rooms at `/ConferenceRooms`
3. **Create** a reservation by selecting a room and time slot
4. **Manage** your bookings at `/Reservations`
5. **Wait** for admin approval

### For Administrators

1. **Login** with admin credentials
2. **Access** admin dashboard at `/Admin/Dashboard`
3. **Review** pending reservations
4. **Approve/Reject** booking requests
5. **Manage** conference rooms (add, edit, delete)
6. **View** statistics and analytics

## 📸 Video presentation

### Video presenting basic functionality of application

[Youtube video link](https://youtu.be/U8eSDG145qU)


## 🗄️ Database Schema

### Entities

**ConferenceRoom**
- Id, Name, Description, Capacity
- PricePerHour, Equipment, IsAvailable
- Navigation: `List<Reservation>` Reservations

**Reservation**
- Id, StartTime, EndTime, Purpose
- Status (Pending/Approved/Rejected/Cancelled)
- CreatedAt, UserId, RoomId
- Navigation: `AppUser` User, `ConferenceRoom` Room

**AppUser** (extends IdentityUser)
- Standard Identity fields (Email, PasswordHash, etc.)
- Navigation: `List<Reservation>` Reservations

### Relationships
- **User → Reservations** (1:N with Cascade Delete)
- **Room → Reservations** (1:N with Restrict Delete)
- **User ↔ Roles** (M:N via AspNetUserRoles)

## 🔌 API Endpoints

### Public Routes
```
GET  /                              # Home page
GET  /Account/Register              # Registration form
POST /Account/Register              # Create account
GET  /Account/Login                 # Login form
POST /Account/Login                 # Authenticate
GET  /ConferenceRooms               # List rooms
GET  /ConferenceRooms/Details/{id}  # Room details
```

### User Routes (Requires Authentication)
```
GET  /Reservations                  # My reservations
GET  /Reservations/Create           # New reservation form
POST /Reservations/Create           # Create reservation
GET  /Reservations/Edit/{id}        # Edit form
POST /Reservations/Edit/{id}        # Update reservation
POST /Reservations/Cancel/{id}      # Cancel reservation
GET  /Reservations/Details/{id}     # Reservation details
POST /Account/Logout                # Logout
```

### Admin Routes (Requires Admin Role)
```
GET  /Admin/Dashboard               # Admin dashboard
GET  /Admin/Reservations            # All reservations
POST /Admin/ApproveReservation/{id} # Approve booking
POST /Admin/RejectReservation/{id}  # Reject booking
GET  /Admin/Statistics              # Analytics
GET  /ConferenceRooms/Create        # Add room form
POST /ConferenceRooms/Create        # Create room
GET  /ConferenceRooms/Edit/{id}     # Edit room form
POST /ConferenceRooms/Edit/{id}     # Update room
GET  /ConferenceRooms/Delete/{id}   # Delete confirmation
POST /ConferenceRooms/Delete/{id}   # Delete room
```

## 🎓 Key Learning Outcomes

This project demonstrates proficiency in:

- ✅ **MVC Pattern** - Clean separation of concerns
- ✅ **Entity Framework Core** - Database operations, migrations, relationships
- ✅ **ASP.NET Core Identity** - Authentication, authorization, role management
- ✅ **CRUD Operations** - Full create, read, update, delete functionality
- ✅ **Data Validation** - Client and server-side validation
- ✅ **Security** - CSRF protection, password hashing, role-based access
- ✅ **Responsive Design** - Mobile-first UI with Bootstrap
- ✅ **Git Version Control** - Proper commit history and .gitignore
- ✅ **Database Design** - Normalized schema with proper relationships
- ✅ **Business Logic** - Conflict detection, status management

## 📝 Project Structure
```
ConferenceRoomBooking/
├── Controllers/              # MVC Controllers
├── Data/                     # DbContext and migrations
├── Migrations/               # EF Core migrations
├── Models/                   # Domain models
├── ViewModels/               # Form models
├── Views/                    # Razor views
│   ├── Account/
│   ├── Admin/
│   ├── ConferenceRooms/
│   ├── Home/
│   ├── Reservations/
│   └── Shared/
│       ├── _Layout.cshtml
│       └── _ValidationScriptsPartial.cshtml
├── wwwroot/                  # Static files
│   ├── css/
│   │   └── site.css         # Custom styling
│   ├── js/
│   └── lib/                 # Bootstrap, jQuery
├── appsettings.json          # Configuration
├── Program.cs                # Application entry point
└── README.md                 # This file
```

## 🔧 Configuration

### Database
The application uses SQLite by default. Connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=conferenceroom.db"
  }
}
```

To switch to SQL Server, update the connection string and change the provider in `Program.cs`.

### Identity Settings
Password requirements (configurable in `Program.cs`):
- Minimum length: 6 characters
- Requires uppercase, lowercase, and digit
- Unique email required
- Lockout: 5 failed attempts = 5 minutes

## 🧪 Testing

### Manual Testing Scenarios

**User Flow:**
1. Register new account → Should create user with "User" role
2. Browse rooms → Should see 3 seeded rooms
3. Create reservation → Should validate dates and conflicts
4. Edit pending reservation → Should allow changes
5. Cancel reservation → Should change status to "Cancelled"

**Admin Flow:**
1. Login as admin → Should see admin panel link
2. View dashboard → Should show pending reservations count
3. Approve reservation → Should change status and prevent conflicts
4. View statistics → Should show monthly data
5. Add new room → Should appear in listings

**Security Tests:**
- Try accessing `/Admin/Dashboard` as user → Should redirect to AccessDenied
- Try editing another user's reservation → Should return Forbid
- Try creating reservation in past → Should show validation error

## 🐛 Known Issues

- Date picker format may vary by browser locale
- Mobile menu requires tap to close (no auto-close)

## 🚀 Future Enhancements

- [ ] Email notifications for reservation status changes
- [ ] Calendar view for room availability
- [ ] Recurring reservations
- [ ] File attachments for meeting agendas
- [ ] Room availability API
- [ ] Payment integration
- [ ] Multi-language support
- [ ] Advanced search and filters
- [ ] Export reservations to PDF/CSV
- [ ] Real-time availability using SignalR

## 👤 Author

**Paweł Szojda**
- GitHub: [@pawelszojda](https://github.com/pawelszojda)
- LinkedIn: [Paweł Szojda](https://linkedin.com/in/paweł-szojda-1a627526b)
- Email: pawelszojda@icloud.com

## 🙏 Acknowledgments

- ASP.NET Core documentation
- Bootstrap team for the UI framework
- Entity Framework Core team
- Stack Overflow community
