# 🎵 AR Musical Application

An augmented reality application designed to enhance user music experiences through real-time audio visualization, interactive stage effects, virtual singer animation, and gesture-controlled performance switching.

---

## 🌈 Features

### 🔊 1. Audio Visualization Cube
- A set of cubes visualizes the audio in real time.
- Cubes bounce based on audio frequencies using **FFT (Fast Fourier Transform)**.
- Provides users with a visual rhythm feedback synchronized with the music.

### 🕺 2. Virtual Singer Animation
- Avatars are created using **Ready Player Me**.
- Animations such as singing and dancing are imported from **Mixamo**.
- The animation can be switched via gesture controls (e.g., changing dance or song actions).

### 🎇 3. Stage Effects
- The stage is composed of a grid of cubes.
- Cube textures and colors change dynamically according to the music beat.
- Effects are **driven purely by rhythm**, not affected by gesture input.

### ✋ 4. Gesture Recognition (Interaction)
- Implemented using a hand tracking solution (e.g., MediaPipe or Leap Motion).
- Gestures do **not** control the stage.
- Instead, they trigger changes in the **singer’s animation** to provide a sense of performance control.

---

## 🛠️ Technologies Used

- **Unity 3D**
- **Ready Player Me** (avatar generation)
- **Mixamo** (animation rigging)
- **FFT-based audio analysis**
- **Gesture Recognition System** (e.g., MediaPipe / Leap Motion)
- **AR Foundation / Vuforia** (depending on AR platform)

---

## 🚀 Getting Started

1. Clone this repository
2. Open the project in Unity (recommended Unity version: `202x.x.x`)
3. Import required plugins:
   - Audio Visualization Package
   - Ready Player Me SDK
   - Mixamo animations
   - Hand Tracking (e.g., MediaPipe Unity Plugin)
4. Play in Editor or build to your AR-supported device

---

## 📁 Project Structure (Example)

