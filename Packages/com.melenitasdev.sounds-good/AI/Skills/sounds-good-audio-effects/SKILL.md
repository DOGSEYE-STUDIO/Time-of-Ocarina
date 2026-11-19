---
name: sounds-good-audio-effects
description: "Use this skill whenever the user wants to colour how audio sounds in a project with Sounds Good — e.g. 'make it sound like a cave', 'add reverb', 'muffle everything when the game is paused', 'make the audio sound underwater', 'add an echo in this tunnel', 'make everything sound like a phone call', 'apply a filter to the music'. Covers creating reusable effect presets from script (reverb, echo, chorus, distortion, low and high pass, volume and pitch), applying them to one sound, to a whole output channel or to the entire game, and dropping Audio Effect Zones into a scene so an area sounds like itself. Do NOT use for sound blocked by walls, which is occlusion (see sounds-good-audio-occlusion), for plain playback (see sounds-good-play-audio), or for creating channels (see sounds-good-audio-channels). When in doubt whether a 'make it sound like…' request involves Sounds Good, use this skill — Prerequisites shows how to confirm the asset is installed."
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

# Create and Apply Audio Effects with Sounds Good

An effect in Sounds Good is a reusable preset combining reverb, echo, chorus, distortion, filters,
volume and pitch. Once registered it has a name, and can be applied to a single sound, to a whole
channel, to the entire game, or to everything inside an area of the scene.

## When to use this skill

- "make it sound like a cave", "add reverb", "add an echo in this tunnel"
- "make the audio sound underwater", "everything should sound like a phone call"
- "muffle everything when the game is paused"
- "apply a filter to the music"

Not for:

- Sound blocked by geometry between source and listener — that is occlusion, see
  `sounds-good-audio-occlusion`.
- Plain playback and volume — see `sounds-good-play-audio`.

## Prerequisites

- Sounds Good **full version**, 2.3.0 or newer for the scripted API. Effects do not exist in Lite.
- The calling script must be in an Editor assembly to create presets.

**Install check.** Confirm `MelenitasDev.SoundsGood.AudioEffectPreset` resolves. If it does not, the
project has Lite; tell the user effects are part of the full version and point at
<https://assetstore.unity.com/packages/tools/audio/sounds-good-easy-optimized-audio-manager-266130>.

## Quick start

Create a cave preset and use it:

```csharp
using MelenitasDev.SoundsGood.Editor;
using UnityEngine;
using UnityEngine.Audio;

SoundsGoodAuthoring.CreateEffect("Cave", preset =>
{
    preset.UseReverb = true;
    preset.ReverbPreset = AudioReverbPreset.Cave;
    preset.UseLowPass = true;
    preset.LowPassCutoff = 4000f;
}, out string error);
```

After the domain reload:

```csharp
new Sound(SFX.Footstep).SetEffect(Effect.Cave, 1f).Play();
```

Expected observable result: the effect appears in `Tools > Sounds Good > Effect Creator`, an asset
exists under the data root's `Effects` folder, and the sound is audibly reverberant.

## Workflows

### Workflow: Create an effect preset from script

**Goal.** A named, reusable effect.

**Steps.**

1. Call `CreateEffect` with a tag and a configuration callback.
2. In the callback, switch on the parts you want and set their values. Anything you do not touch
   keeps its default and contributes nothing, so a preset with no part enabled is silent in the
   chain rather than an error.
3. The parts, with their defaults:

| Switch | Parameters | Defaults |
|---|---|---|
| `UseReverb` | `ReverbPreset` | `AudioReverbPreset.Generic` |
| `UseEcho` | `EchoDelay` 10–5000 ms, `EchoDecayRatio`, `EchoWetMix`, `EchoDryMix` | 500, 0.5, 1, 1 |
| `UseChorus` | `ChorusDryMix`, `ChorusWetMix1/2/3`, `ChorusDelay`, `ChorusRate`, `ChorusDepth` | 0.5, 0.5, 40, 0.8, 0.03 |
| `UseDistortion` | `DistortionLevel` 0–1 | 0.5 |
| `UseLowPass` | `LowPassCutoff` 10–22000 Hz, `LowPassResonance` | 1000, 1 |
| `UseHighPass` | `HighPassCutoff` 10–22000 Hz, `HighPassResonance` | 3000, 1 |

Lower `LowPassCutoff` means darker and more muffled. A phone call is a high pass around 800 Hz plus
a low pass around 3000 Hz; underwater is a low pass near 700 Hz with some reverb.

**Expected result.** `SoundsGoodAuthoring.GetEffectTags()` includes the tag, and `Effect.Tag`
compiles after the reload.

### Workflow: Apply an effect to one sound

**Goal.** One sound reverberant, everything else dry.

**Steps.**

```csharp
new Sound(SFX.Shout).SetEffect(Effect.Cave, intensity: 1f).Play();
```

Intensity blends from 0 (none) to 1 (exactly as saved), so the same preset covers a hint and a
drenching. While it plays, `ChangeEffect(newIntensity, lerpTime)` fades it.

**Expected result.** Only that sound is affected.

### Workflow: Apply an effect to a channel or to the whole game

**Goal.** Duck the world under a pause menu, or colour one channel.

**Steps.**

```csharp
using MelenitasDev.SoundsGood;

// Everything, faded in over 0.3s. Keeps working while the game is paused.
SoundsGoodManager.SetGlobalEffect(Effect.Underwater, 1f, 0.3f);
SoundsGoodManager.RemoveGlobalEffect(0.3f);

// Only one output channel.
SoundsGoodManager.SetOutputEffect(Output.SFX, Effect.Cave, 1f, 0.3f);
SoundsGoodManager.RemoveOutputEffect(Output.SFX, 0.3f);
```

A global effect reaches 2D sounds too, which is what makes it right for a pause menu.

**Expected result.** The whole mix changes and returns without clicks.

### Workflow: Make an area sound like itself

**Goal.** Walk into a cave and it sounds like a cave.

**Steps.**

1. `GameObject > Sounds Good > Effects > Audio Effect Zone`.
2. Size its area over the region and pick the effect.
3. Choose what it reaches with the `Affect` setting:
   - **Sources** — sounds inside the zone carry the effect, heard from anywhere. A waterfall keeps
     its space when you listen from outside.
   - **Listener** — everything is affected while the listener is inside. This is the one for going
     underwater.
   - **Both**.
4. The zone fades the effect across its margin, so there is no hard boundary.

**Expected result.** Crossing the boundary changes the sound gradually, and reverb tails ring out
instead of cutting.

## Verification

- `Tools > Sounds Good > Effect Creator` lists the preset, and its asset exists under the data root's
  `Effects` folder.
- `Effect.YourTag` compiles.
- In Play Mode, the affected audio is audibly different and unaffected audio is not.
- For a zone, the Scene view gizmo covers the intended area and the change is gradual at the edge.
- The Console is clean.

## API quick reference

| Entry point | Type | What it does |
|---|---|---|
| `SoundsGoodAuthoring.CreateEffect(tag, configure, out error)` | static bool (Editor) | Creates and registers a preset |
| `SoundsGoodAuthoring.GetEffectTags()` | static string[] | What already exists |
| `AudioEffectPreset` | ScriptableObject | The preset's switches and parameters |
| `Sound/Music/Playlist.SetEffect(Effect, intensity)` | instance | Applies before playing |
| `…ChangeEffect(intensity, lerpTime)` | instance | Fades while playing |
| `SoundsGoodManager.SetGlobalEffect/RemoveGlobalEffect` | static | The entire game |
| `SoundsGoodManager.SetOutputEffect/RemoveOutputEffect` | static | One channel |
| `AudioEffectZone` | MonoBehaviour | An area, with its `Affect` mode |

## Common issues

| Symptom | Cause | Fix |
|---|---|---|
| The effect is inaudible | No part was switched on in the callback | Set at least one `Use…` to true |
| `Effect.MyTag` does not compile | The enum is a source file and Unity has not recompiled | Let the domain reload finish |
| The zone does nothing to 2D sounds | Zones affect 3D sounds by design, so music and UI stay dry | Use `SetGlobalEffect` for everything |
| The effect cuts abruptly when leaving a zone | Expected to be a tail | Tails ring out on their own; if it truly cuts, check that the sound is 3D and inside the zone |
| Everything sounds distorted | Several effects stacked, or `DistortionLevel` too high | Lower the intensity, or check for a global effect still applied |
| `AudioEffectPreset` does not resolve | The project has Lite | Effects are full-version only |

## Boundaries

- Effects are full-version only. Absent in Sounds Good Lite.
- Presets combine Unity's own audio filters. They are not a convolution reverb and do not take
  impulse responses.
- One override wins at a time: a global or output effect takes precedence over a zone, which takes
  precedence over a sound's own effect.
- The API creates and registers presets; editing an existing one afterwards is done in
  `Tools > Sounds Good > Effect Creator`.
