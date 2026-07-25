# MusicSpace

A simple Music Playlist web application developed using **ASP.NET Core MVC**. Users can log in, create their own playlist by adding YouTube videos, play songs directly from the application, and view the Top 5 most-played songs based on the total number of plays across all users.

---

## 📌 Features

- 🔐 User Login Authentication
- 👤 User Profile
- ➕ Add YouTube Songs
- 🎵 Personal Playlist
- ▶️ Play Songs using Embedded YouTube Player
- 🗑️ Remove Individual Songs
- 🧹 Clear Playlist
- 📊 Top 5 Trending Songs
- 📈 Total Plays Counter
- 📱 Responsive User Interface

---

## 🛠️ Built With

- ASP.NET Core MVC
- C#
- Razor Views
- Bootstrap 5
- HTML5
- CSS3
- JavaScript
- YouTube Embedded Player

---

## 📂 Project Structure

```
MusicPlaylist
│
├── Controllers
│   ├── AccountController
│   ├── HomeController
│   ├── PlaylistController
│   └── ProfileController
│
├── Models
│   ├── UserProfile
│   ├── LoginViewModel
│   ├── PlaylistItem
│   ├── MusicVideo
│   └── HomeViewModel
│
├── Services
│   └── AppDataService
│
├── Views
│   ├── Account
│   ├── Home
│   ├── Playlist
│   ├── Profile
│   └── Shared
│
└── wwwroot
```

---

## 🚀 How It Works

1. Log in using one of the available user accounts.
2. Add a song by entering its title and YouTube URL.
3. The application extracts the YouTube Video ID automatically.
4. Songs are saved to your personal playlist.
5. Click **Play** to watch the video using the embedded YouTube player.
6. Every play increases the song's total play count.
7. The Home page displays the **Top 5 Most Played Songs** across all users.

---

## 👥 Contributors

- **Alliyah De Vera**
- **Sophia Solis**
- **Kevin Roque**

---

## 📝 Notes

- This project does **not** use a database.
- Data is stored in memory using `AppDataService`.
- YouTube videos are embedded using the standard YouTube Embed URL.
- Play counts are used to generate the Trending Songs ranking.

---

## 📄 License

This project is intended for educational purposes only.
