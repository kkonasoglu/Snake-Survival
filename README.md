# 🐍 Bite & Boom (Snake Survivor)

[![Unity](https://img.shields.io/badge/Unity-6000.x_(Unity_6)-black?logo=unity&style=for-the-badge)](https://unity.com/)
[![Render Pipeline](https://img.shields.io/badge/URP-2D_Pipeline-107C41?style=for-the-badge)](https://unity.com/)
[![Language](https://img.shields.io/badge/Language-C%23-blue?logo=csharp&style=for-the-badge)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Art](https://img.shields.io/badge/Pixel_Art-Aseprite-ff6b6b?logo=aseprite&style=for-the-badge)](https://www.aseprite.org/)
[![Status](https://img.shields.io/badge/Build-Production_Ready-brightgreen?style=for-the-badge)]()

> A modern, action-packed twist on the classic retro Snake game, featuring dynamic arena expansion, risk-reward powerups, and juicy arcade game feel. Built from scratch with **Unity 6 (URP 2D)** and **Aseprite**.

---

## 🎮 About The Game
**Bite & Boom** re-imagines Nokia-era Snake as a 3-Phase dynamic arcade survivor. As your score climbs, the arena physically expands, the camera pulls back seamlessly, and high-risk mechanics introduce strategic depth to classic grid navigation.

---

## 🌟 Key Features

### 🔄 3-Phase Dynamic Progression
* **Phase 1: The Pit (Score 0 – 25)**
  * Claustrophobic starting grid bounded by indestructible inner walls.
  * Collect classic **Apples (+1 pt)** to grow and survive.
* **Phase 2: Walls Down (Score 25 – 75)**
  * Inner walls blow up with a screen-shaking explosion.
  * The orthographic camera smoothly zooms out (`7.5` → `12.0`) to reveal the expanded perimeter.
  * **Bananas (+4 pts)** spawn, providing a rapid score boost and speed pacing.
* **Phase 3: Chaos Unleashed (Score 75+)**
  * Outer boundaries drop, unlocking the full map.
  * **Bombs** appear with ticking fuses, pulsing warning animations, and lethal explosions.
  * **Devil Fruits (+8 pts)** spawn, temporarily inverting controls for high-risk / high-reward plays.

---

### 🍎 Collectibles & Hazard Guide
| Item | Sprite | Points | Behavior / Effect |
| :--- | :---: | :---: | :--- |
| **Apple** | 🍎 | `+1` | Classic food. Increases snake length by 1. |
| **Banana** | 🍌 | `+4` | High-value bonus fruit introduced in Phase 2. |
| **Devil Fruit** | 🍇 | `+8` | High-score gamble. Temporarily inverts player movement controls. |
| **Bomb** | 💣 | `0` | Hazard. Pulses red, ticks down for 5s, and detonates. Touching or explosion = Game Over. |

---

### 🕹️ Controls
| Action | Keybinding |
| :--- | :--- |
| **Move** | `Arrow Keys` or `W`, `A`, `S`, `D` |
| **Pause / Resume** | `ESC` |
| **Quick Restart** | `SPACE` (on Game Over screen) |

---

## ⚙️ Architecture & Technical Highlights

1. **Decoupled Event-Driven Gameplay:**
   * Game states, score triggers, and phase transitions communicate through C# `Action` events (`OnPhase2Started`, `OnPhase3Started`, `OnSnakeDied`, `OnSnakeReset`).
   * Eliminates tight coupling between UI, Camera, Sound, and Snake logic.

2. **Input Buffering (Queue System):**
   * Uses an `inputQueue` to store rapid arrow keystrokes, completely preventing dropped turns when executing fast 90-degree corner maneuvers.

3. **Smooth Orthographic Camera Lerp & Screen Shake:**
   * `CameraExpender.cs` handles smooth dynamic zoom transitions during phase shifts and triggers micro-shakes on bomb detonations and player death.

4. **V-Sync & Rendering Optimization:**
   * Hardware V-Sync enforced across menus and gameplay to eliminate screen tearing.
   * Tilemaps rendered in `Individual` mode to eradicate 1px pixel-art seam cracks.
   * Viewport Rect enforced to `(0, 0, 1, 1)` to guarantee seamless scene transitions with zero leftover framebuffer artifacts.

5. **Shared Lightweight Audio Engine:**
   * Responsive UI button audio powered by a single persistent `AudioSource` with a non-repeating shuffle algorithm for hover beeps and click sounds.

---

## 🚀 Getting Started

### Prerequisites
* **Unity 6 (`6000.5.x` or newer)** with **Universal 2D Pipeline** support.
* Windows Build Support (Mono / IL2CPP) for standalone export.

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/kursatkonasoglu/Snake_survivor.git
   ```
2. Open **Unity Hub** and click **Add -> Add project from disk**.
3. Select the cloned project folder.
4. Open the project in Unity 6.
5. In Project Window, open:
   * `Assets/Scenes/MainMenu.unity`
6. Press **Play** in the Unity Editor or build via **File -> Build Profiles -> Windows**.

---

## 🎨 Visuals & Audio
* **Color Palette:** 1-Bit Game Boy / Nokia monochrome retro aesthetic (`#61683A` olive background).
* **Sprites:** Custom hand-crafted 16x16 pixel-art sprites and UI panels.
* **Audio:** Chiptune soundtrack (*The Snake's Sunday*) with arcade sound effects.

---

## 👨‍💻 Developer & Studio
* **Developer:** Kürşat Konaşoğlu
* **GitHub:** [@kursatkonasoglu](https://github.com/kursatkonasoglu)
* **Studio / Brand:** TurxInteractive
