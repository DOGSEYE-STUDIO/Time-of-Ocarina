# 🎵 Sounds Good — Easy & Optimized Audio Manager for Unity

**Thank you for choosing _Sounds Good_**, a powerful and easy-to-use audio management plugin for Unity, developed with love by **Melenitas Dev**.
Your support means the world to me. 💖

This README is a quick, offline starting point. For the full guide (every method, component and window) see the [online documentation](https://melenitass-organization.gitbook.io/sounds-good-docs).

---

## 🧭 New here? Start with this

Everything lives under the **`Tools > Sounds Good`** menu and the **`GameObject > Sounds Good`** menu.

1. Open **`Tools > Sounds Good > Get Started!`** for a guided tour.
2. Open **`Tools > Sounds Good > Audio Creator`** to register your first sound.
3. Play it — from code (one line) or with a no-code component in the Inspector.

There are two ways to use Sounds Good, and you can mix them freely:

- **From code** — create audio objects and call `.Play()`.
- **No code** — drop an *Emitter* (or other component) onto a GameObject and configure it in the Inspector.

---

## 🎚️ Step 1 — Register your audio (Audio Creator)

Open **`Tools > Sounds Good > Audio Creator`**:

- Add one or more **AudioClips**.
- Give them a **tag** (this becomes an entry in an auto-generated enum, e.g. `SFX.Jump`).
- Pick a **compression preset** for performance.
- If you add several clips under one tag, one is chosen at random on each play (less repetition).

Sounds live in the **SFX** enum, music tracks in the **Track** enum.

---

## ⌨️ Step 2 — Play from code

```csharp
using MelenitasDev.SoundsGood;

// The simplest possible sound
new Sound(SFX.Jump).Play();
```

Configure it by chaining methods, then play:

```csharp
new Sound(SFX.Explosion)
    .SetVolume(0.7f)
    .SetRandomPitch()       // small pitch variation each play
    .SetSpatialSound()      // 3D positional audio
    .SetFollowTarget(transform)
    .Play();
```

### The 4 audio types

```csharp
new Sound(SFX.Coin).Play();                              // any one-shot / looping SFX
new Music(Track.MainTheme).SetLoop().Play();             // a single music track
new Playlist(Track.Song1, Track.Song2).SetLoop().Play(); // tracks one after another
new DynamicMusic(Track.Drums, Track.Bass, Track.Lead)    // layers played together…
    .Play();                                             // …blend them at runtime
```

> 💡 Dynamic music has two volumes. Each layer's own volume is the **mix** (`ChangeTrackVolume`), while `ChangeMasterVolume` is how loud the music is **as a whole** — it scales every layer at once, so ducking the music under dialogue never disturbs the blend underneath. The Dynamic Music Emitter exposes it as a *Master Volume* slider.

### The method naming convention

Once you know the prefixes, the whole API reads like a sentence:

| Prefix | When | Returns | Chainable |
|--------|------|---------|-----------|
| `Set…`  | Configure **before** playing | the same object | ✔️ |
| `Change…` | Mutate **while playing** (optional lerp time) | `void` | ❌ |
| `On…`  | Subscribe to a lifecycle event (a callback) | the same object | ✔️ |
| `Play` / `Pause` / `Resume` / `Stop` | Immediate action | `void` | ❌ |

```csharp
Music music = new Music(Track.Menu).SetVolume(1f).OnPlay(() => Debug.Log("Playing!"));
music.Play();
music.ChangeVolume(0.2f, 1.5f);   // duck over 1.5s while it keeps playing
```

> 💡 Keep a reference to control audio later, or give it an id with `.SetId("myId")` and control it through `SoundsGoodManager` without a direct reference.

---

## 🧩 Step 3 — Play without code (components)

Prefer the Inspector? Add a component from **`GameObject > Sounds Good`** (or *Add Component ▸ Sounds Good*). Each exposes the same options as the code API, with tooltips and gizmos.

- **Emitters** (Sound / Music / Playlist / Dynamic Music) — configure an audio object in the Inspector and drive it with Play/Pause/Resume/Stop. Reference the component to call any `Set*/Change*/On*` at runtime.
- **Audio Trigger** — play/stop the emitter on the same object from physics, lifecycle or UnityEvents.
- **Music Zone** — fade a music emitter in/out based on the listener's position.
- **Audio Effect Zone** — apply an effect to sounds inside a zone (underwater, a cave…).
- **Random Ambience** — play random sounds at random intervals (birds, drips…).
- **Collision Sound** — play a sound on impact, scaled by collision strength.
- **Occlusion Surface** — set how much an object's collider blocks sound, so a thin window muffles less than a concrete wall.
- **UI ▸ Output Volume Slider** — a ready-made volume slider for a channel (saves automatically).

---

## 🔊 Outputs (audio channels)

Group your audio into channels (SFX, Music, Voice…) routed through an Audio Mixer.

1. Open **`Tools > Sounds Good > Output Manager`**, type a name and press **CREATE**. The mixer group is created and its volume exposed for you — no Audio Mixer steps to follow. You can rename or delete outputs from the same window.
2. Route audio to it: `new Sound(SFX.Shot).SetOutput(Output.SFX).Play();`
3. Control the channel volume: `SoundsGoodManager.ChangeOutputVolume(Output.Music, 0.5f);` (saved to PlayerPrefs).

---

## 🌊 Effects

Design reusable audio effects (reverb, echo, low/high pass, distortion, pitch…) in
**`Tools > Sounds Good > Effect Creator`**, with a live preview. Then apply them:

```csharp
new Sound(SFX.Splash).SetEffect(Effect.Underwater, 1f).Play();  // intensity 0–1

// Muffle the whole game while paused (2D sounds included):
SoundsGoodManager.SetGlobalEffect(Effect.Underwater, 1f, 0.3f);
SoundsGoodManager.RemoveGlobalEffect(0.3f);
```

Effects can also be applied per-Output, or automatically in the world with an **Audio Effect Zone**.

---

## 🧱 Other built-in features

- **Occlusion** — sounds are muffled in real time when obstacles block the path to the listener. Tune it in **`Tools > Sounds Good > Settings`**, and vary it per surface with an **Occlusion Surface**.
- **Time Mode** — `SetTimeMode(TimeMode.Scaled)` follows `Time.timeScale` (default); `Unscaled` keeps playing at normal speed while the game is paused or in slow motion (menus, UI).
- **Object pooling** — audio sources are pooled internally, so playing many sounds stays cheap.
- **Latency** — does a sound feel like it plays *after* you asked for it? There are two causes and both live under `Tools > Sounds Good`. **Audio Silence Trimmer** finds the silence baked into your clips (an MP3 always carries some) and trims it away. **DSP Buffer Size**, in Settings, decides how long *any* sound takes to reach the speakers — Unity's default costs about 40 ms.

---

## 🤖 AI Skills — let your AI assistant use Sounds Good

Sounds Good ships **7 skills** at `Packages/com.melenitasdev.sounds-good/AI/Skills/`: written instructions that teach an AI coding assistant how to drive this asset properly. Without them an agent asked for audio tends to write its own `AudioSource` code from scratch, next to the audio manager you already installed.

They are plain Markdown, so they work with any assistant that can read your project.

### ✅ The easy way — ask your assistant to install them

Paste this into whichever AI you are using:

> Install the AI Skills that Sounds Good ships. They are in `Packages/com.melenitasdev.sounds-good/AI/Skills/`, one folder per skill. Put them wherever you read skills or project instructions from, using your own conventions.

Your assistant knows where its own instructions live better than any installer could, so this covers every tool. **Do it once per project.**

After that you never mention Sounds Good again. Say *"play a sound when the player jumps"* and it reaches for the right skill by itself, registers the clip and writes the one line that plays it.

### 🛠️ Or do it by hand

Each skill is a folder containing a `SKILL.md`. Where they go depends on your assistant:

- **If it supports skills** (Claude Code, for example, reads `<your project>/.claude/skills/`), copy the 7 folders in as they are. It then loads only the one the task needs.
- **If it reads a single instructions file** (a project-rules or custom-instructions file), do **not** paste all 7 in — together they are about 1,400 lines, and you would pay for all of them on every message. Add this pointer instead:

  ```
  This project uses Sounds Good for audio. Before writing any audio code, read the
  matching skill in Packages/com.melenitasdev.sounds-good/AI/Skills/:
    sounds-good-setup-and-overview   - read first; what is registered, how tags work
    sounds-good-register-audio       - adding clips as sounds or music tracks
    sounds-good-play-audio           - playing, stopping, fading, callbacks from code
    sounds-good-inspector-components - emitters, triggers and zones, no code
    sounds-good-audio-channels       - output channels and volume sliders
    sounds-good-audio-effects        - reverb, filters, effect zones
    sounds-good-audio-occlusion      - walls muffling sound, surface materials
  Do not write your own AudioSource wrapper.
  ```

### 📋 What they cover

| Skill | What it teaches |
|-------|-----------------|
| **setup-and-overview** | Confirming the install, the tag-and-enum model, where data lives, which skill to use next |
| **register-audio** | Registering clips under tags, compression presets, several clips per tag |
| **play-audio** | The 4 audio types, `Set`/`Change`/`On` conventions, 3D audio, fades, ids |
| **inspector-components** | Emitters, triggers, zones, random ambience, volume sliders |
| **audio-channels** | Creating outputs, routing audio, saved channel volumes |
| **audio-effects** | Effect presets, applying them per sound, per channel or globally, effect zones |
| **audio-occlusion** | Turning occlusion on, tuning it, authoring surface materials |

They also ship a scripted authoring API, `SoundsGoodAuthoring`, which is what lets an agent register audio and create channels without clicking through the editor windows. It is just as usable from your own editor tooling.

> ℹ️ The skills are kept in step with the asset: each one records the version it was verified against. If you upgrade Sounds Good, re-run the install line above to refresh your copies.

---

## 🛠️ Editor windows (`Tools > Sounds Good`)

| Window | What it does |
|--------|--------------|
| **Get Started!** | Guided introduction. |
| **Audio Creator** | Register sounds and music, assign tags and compression. |
| **Audio Collection** | Browse and manage all registered audio. |
| **Output Manager** | Create and manage audio output channels. |
| **Effect Creator** | Design reusable audio effect presets. |
| **Occlusion Material Creator** | Author reusable occlusion materials, with a live preview. |
| **Audio Silence Trimmer** | Measure and trim the silence that makes sounds play late. |
| **Settings** | Data path, audio latency, occlusion tuning and global options. |

---

## 📚 Full documentation

- [📘 English Documentation](https://melenitass-organization.gitbook.io/sounds-good-docs)
- [📙 Documentación en Español](https://melenitass-organization.gitbook.io/sounds-good-docs/spanish)

---

## 🙌 Credits

Special thanks to the creators and resources used in the demo:

- 🎨 **Kenney** ([kenney.nl](https://www.kenney.nl)) – UI Pack
- 🎶 **Nicky Case!** – Music from *"Cute Gay Nerd"*
- 🎼 **Sweet Symmetry** ([sweetsymmetry.com](https://www.sweetsymmetry.com)) – Dynamic music *"Gravity"*

Your contributions helped bring this plugin to life. 💛

---

## 📄 License

**Sounds Good** is © Melenitas Dev.
All rights reserved.

> ❗ Redistribution of the standalone asset is strictly prohibited.

---
