🎮 Day 03 – 3D Roll-a-Ball (Unity 3D)

This is the third game in my 10 Days – 10 Games challenge, where I create one complete game per day to improve my Unity, C#, and overall game-development skills.

Day 03 is a modern, optimized version of the classic Roll-a-Ball game — featuring smoother physics, cleaner scripts, smarter pickups, full UI flow, and a performance-optimized WebGL build.

🌐 Play the Game

🔗 Standard WebGL Build:
https://momoshomo.itch.io/day-03-3d-roll-a-ball

⚡ Optimized WebGL Build (Super Fast):
https://momoshomo.itch.io/day-03-3d-roll-a-ball-optimized

🕹 Gameplay Overview

Roll the ball around the arena, collect all rotating pickups, and avoid falling off the platform. Movement is physics-based, responsive, and smooth.

The goal: Collect all pickups to win!

🎮 Controls

Keyboard

W / A / S / D – Move

Arrow Keys – Move

R – Restart level (only in certain builds)

Esc – Return to menu

Mouse

Used only for menu navigation

✨ Features

Smooth Rigidbody-based ball movement

Rotating, animating pickup objects

Win condition when all pickups are collected

UI flow (Start Screen → Game → Win Screen)

Restart + Quit buttons

Optimized for WebGL performance

Code cleanup + script optimization

Collectible counter

Improved collision detection and friction settings

🛠 Day 03 Optimization Changelog

✔ Rigidbody settings improvements

✔ Movement controller rewrite

✔ Pickup collision fixes

✔ UI restructuring

✔ WebGL performance enhancements

✔ Memory + GC optimizations

✔ Lighting & material changes for clarity

✔ Cleaner folder structure



📦 Project Structure
Assets/
│── Scripts/
│    ├── PlayerController.cs
│    ├── PickupRotator.cs
│    ├── GameManager.cs
│
│── Prefabs/
│    ├── Player
│    ├── Pickup
│
│── Scenes/
│    ├── MainMenu
│    ├── Game
│    ├── WinScreen


📈 What I Learned This Day

Smoother physics-based movement

Scene management workflow

Game loops (start → play → win)

Performance optimization for WebGL

Writing cleaner, reusable scripts

UI anchoring + resolution independence

🚀 Next Improvements

Add timer and best-time scoring

Add obstacles / moving platforms

Add particles when collecting pickups

Add sound effects

Add multiple levels

