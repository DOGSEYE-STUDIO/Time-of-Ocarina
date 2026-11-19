---
name: sounds-good-inspector-components
description: "Use this skill whenever the user wants audio driven by scene objects rather than by code in a project with Sounds Good — e.g. 'play a sound when the player enters this room', 'make this torch crackle', 'add ambient bird sounds to the forest', 'play a sound when these crates collide', 'change the music in the tavern', 'add a sound to this object without scripting', 'put music on this area'. Covers the emitter components, Audio Trigger, Music Zone, Random Ambience and Collision Sound: which one fits the request, how to add it from the GameObject menu, and which fields to set. Do NOT use for playing audio in C# (see sounds-good-play-audio), registering clips (see sounds-good-register-audio), or effect zones and occlusion (see sounds-good-audio-effects, sounds-good-audio-occlusion). When in doubt whether a scene-driven audio request involves Sounds Good, use this skill — Prerequisites shows how to confirm the asset is installed."
metadata:
  asset: "Sounds Good - Easy & Optimized Audio Manager"
  publisher: "Melenitas Dev"
  asset-version: "2.3.0"
  skill-version: "1.0.0"
  unity: "2022.3+"
  render-pipelines: "Built-in, URP"
  category: "tools/audio"
  asset-store-url: "https://assetstore.unity.com/packages/tools/audio/sounds-good-easy-optimized-audio-manager-266130"
  documentation-url: "https://melenitass-organization.gitbook.io/sounds-good-docs"
  support-url: "https://www.melenitas.dev"
  last-verified: "2026-09-28"
---

# Drive Audio from the Inspector with Sounds Good

Sounds Good ships components that do from the scene what the API does from code. This skill covers
choosing the right one for a request and adding it correctly, so a room, a torch or a collision
makes sound without a line of script.

## When to use this skill

- "play a sound when the player enters this room"
- "make this torch crackle", "add bird sounds to the forest"
- "play a sound when these crates collide"
- "change the music in the tavern"
- The user asks for audio behaviour attached to an object, or says they do not want to write code.

Not for:

- Audio triggered by game logic in C# — see `sounds-good-play-audio`.
- A clip that is not registered yet — see `sounds-good-register-audio` first.
- Reverb zones and occlusion — see `sounds-good-audio-effects` and `sounds-good-audio-occlusion`.

## Prerequisites

- Sounds Good installed, and the tags the component will reference already registered.
- An `AudioListener` in the scene.

**Install check.** Confirm the menu `GameObject > Sounds Good > Emitters > Sound Emitter` exists, or
that the type `MelenitasDev.SoundsGood.SoundEmitter` resolves. If not, Sounds Good is not installed.

Audio Trigger, Music Zone, Random Ambience and Collision Sound are full-version components. If
`MelenitasDev.SoundsGood.AudioTrigger` does not resolve, the project has Sounds Good Lite, which has
the three emitters only — say so rather than substituting something else.

## Quick start

Add a sound emitter that plays on start:

1. Menu: `GameObject > Sounds Good > Emitters > Sound Emitter`. This instantiates a configured
   prefab, which is why it is preferred over adding the component by hand.
2. In the Inspector, set **Sfx** to the registered tag.
3. Tick **Play On Start**.

Expected observable result: the Hierarchy contains a `Sound Emitter` object, and entering Play Mode
plays the sound.

## Workflows

### Workflow: Choose the right component

**Goal.** Match the request to the component that already does it.

| The user wants | Component | Menu |
|---|---|---|
| An object that plays a sound / music / a queue of tracks | Sound, Music or Playlist Emitter | `GameObject > Sounds Good > Emitters > …` |
| Layers of music blended at runtime | Dynamic Music Emitter | `GameObject > Sounds Good > Emitters > Dynamic Music Emitter` |
| Audio to start or stop on entering, leaving or an event | Audio Trigger | `GameObject > Sounds Good > Audio Trigger` |
| Different music per area, crossfading as you walk | Music Zone | `GameObject > Sounds Good > Music Zone` |
| Occasional random sounds around the player | Random Ambience | `GameObject > Sounds Good > Random Ambience` |
| A sound on impact, louder the harder the hit | Collision Sound | Add `CollisionSound` to an object with a Collider |
| A volume slider in an options menu | Output Volume Slider | `GameObject > Sounds Good > UI > Output Volume Slider` |

**Expected result.** One component chosen, rather than a custom script written to do the same job.

### Workflow: Add and configure an emitter

**Goal.** An object that plays a registered sound, configured from the Inspector.

**Steps.**

1. Create it from the `GameObject > Sounds Good > Emitters` menu so the prefab's defaults come with it.
2. Set the fields that matter. On `SoundEmitter` the serialized names are: `sfx`, `playOnStart`,
   `volume`, `loop`, `spatialSound`, `randomPitch`, `pitchRange`, `playProbability`,
   `hearDistanceMin`, `hearDistanceMax`, `output`.
3. From a script, set them through `SerializedObject` so the change is recorded and saved:

   ```csharp
   using UnityEditor;
   using UnityEngine;
   using MelenitasDev.SoundsGood;

   var emitter = Object.FindAnyObjectByType<SoundEmitter>();
   var so = new SerializedObject(emitter);
   so.FindProperty("playOnStart").boolValue = true;
   so.FindProperty("loop").boolValue = true;
   so.ApplyModifiedProperties();
   ```

4. Drive it at runtime through the component, which exposes `Play()`, `Stop()`, `Pause()`,
   `Resume()` and `IsPlaying`, plus `Sound` for the underlying object.

**Expected result.** The emitter plays as configured, and its Inspector shows the values you set.

### Workflow: Play a sound when something enters an area

**Goal.** Audio driven by physics or lifecycle, with no script.

**Steps.**

1. Put a `Sound Emitter` on the object and set its tag. Leave **Play On Start** off.
2. Add `GameObject > Sounds Good > Audio Trigger` to the same object.
3. In the Audio Trigger, choose what fires it and which action it takes: `TriggerAction.Play`,
   `Stop` or `TogglePlayStop`. Filter by layer or tag so only the player sets it off.
4. Ensure the object has a Collider marked **Is Trigger**, and that the entering object has a
   Rigidbody — Unity's requirement, not the asset's.

The trigger can also be fired from a UnityEvent or from code with `Play()`, `Stop()` or `Toggle()`.

**Expected result.** Walking into the area plays the emitter's sound; leaving it stops it if
configured that way.

### Workflow: Give an area its own music

**Goal.** The tavern has its theme, the forest another, crossfading as the listener moves.

**Steps.**

1. Create a `Music Emitter` for the area's track.
2. Add `GameObject > Sounds Good > Music Zone` and size its area around the region.
3. The zone fades the emitter by the listener's position, so the music never restarts when you
   cross back and forth.

**Expected result.** Music rises entering the area and falls leaving it, without a cut.

### Workflow: Make a place sound alive

**Goal.** Birds, drips or creaks at irregular intervals, from varying directions.

**Steps.**

1. Add `GameObject > Sounds Good > Random Ambience`.
2. Point it at one or more registered sounds and set the interval range.
3. Choose the emission mode: `FixedAtTransform` for a fixed point, `RandomWithinRadius` for sounds
   scattered around it, `FollowTransform` to move with an object.
4. Control it at runtime with `Play()` and `Stop()`.

**Expected result.** Sounds occur at irregular intervals from varying positions, with no audible
loop.

## Verification

- The component is present in the Hierarchy and its Inspector shows the intended values.
- Entering Play Mode produces the expected sound at the expected moment.
- For triggers and zones, the gizmo drawn in the Scene view covers the intended area.
- The Console shows no missing-reference or missing-script errors.
- For an Audio Trigger, the object has a Collider with **Is Trigger** ticked.

## API quick reference

| Entry point | Type | What it does |
|---|---|---|
| `SoundEmitter` / `MusicEmitter` / `PlaylistEmitter` / `DynamicMusicEmitter` | MonoBehaviour | The API in the Inspector; `Play/Stop/Pause/Resume`, `IsPlaying` |
| `AudioEmitter` | abstract MonoBehaviour | Base of all four, for code that treats them alike |
| `AudioTrigger` | MonoBehaviour | `Play()`, `Stop()`, `Toggle()`, `TriggerAction` |
| `RandomAmbience` | MonoBehaviour | `Play()`, `Stop()`, `EmissionMode` |
| `MusicZone` / `CollisionSound` | MonoBehaviour | Configured entirely in the Inspector |

All in the `MelenitasDev.SoundsGood` namespace.

## Common issues

| Symptom | Cause | Fix |
|---|---|---|
| The emitter does nothing at start | **Play On Start** is off | Tick it, or call `Play()` from a trigger or script |
| The Audio Trigger never fires | The Collider is not **Is Trigger**, or nothing has a Rigidbody | Fix the physics setup; Unity needs a Rigidbody on one of the two |
| The trigger fires for everything | No layer or tag filter | Set the filter to the player |
| The sound is heard everywhere at once | **Spatial Sound** is off on the emitter | Tick it; that is what makes position matter |
| Adding the component by hand leaves it unconfigured | `AddComponent` skips the prefab's defaults | Create it from the `GameObject > Sounds Good` menu instead |
| The component shows "missing script" | The project has Lite, where these components are absent | Use the emitters, or point the user at the full version |

## Boundaries

- These components cover common patterns. Behaviour beyond them is written in C# against the same
  API — see `sounds-good-play-audio`.
- Music Zone, Audio Trigger, Random Ambience and Collision Sound are full-version only.
- Zones use their own geometry for fading; they are not Unity Reverb Zones and do not interact with
  them.
- Collision Sound scales with impact force. It does not choose different sounds per material; for
  that, register several tags and pick in code.
