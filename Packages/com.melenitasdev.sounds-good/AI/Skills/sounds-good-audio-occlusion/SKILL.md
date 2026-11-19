---
name: sounds-good-audio-occlusion
description: "Use this skill whenever the user wants geometry to affect what can be heard through it in a project with Sounds Good — e.g. 'sound should be muffled through walls', 'I can hear enemies through the floor', 'make the door block the noise', 'muffle audio behind obstacles', 'a window should let more sound through than concrete', 'why do I hear everything through solid objects'. Covers turning occlusion on and tuning it in Settings, creating occlusion materials from script so each surface blocks its own amount, and putting an Audio Occlusion Surface on colliders. Do NOT use for reverb or filters applied by choice (see sounds-good-audio-effects), for plain volume and distance (see sounds-good-play-audio), or for mixer channels (see sounds-good-audio-channels). When in doubt whether a 'muffled through walls' request involves Sounds Good, use this skill — Prerequisites shows how to confirm the asset is installed."
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

# Block Sound with Geometry using Sounds Good

Occlusion makes obstacles between a sound and the listener muffle it, in real time. It works from
the moment it is enabled, and each surface can be told how much it holds back, so a pane of glass
is not a concrete wall.

## When to use this skill

- "sound should be muffled through walls", "make the door block the noise"
- "I can hear enemies through the floor"
- "a window should let more sound through than concrete"
- "why do I hear everything through solid objects"

Not for:

- Reverb, echo or filters applied deliberately — see `sounds-good-audio-effects`.
- Sounds simply being too loud far away — that is `SetHearDistance`, see `sounds-good-play-audio`.

## Prerequisites

- Sounds Good **full version**. Occlusion does not exist in Lite.
- 3D sounds: occlusion applies to spatial audio, so the sound needs `SetSpatialSound()` or an
  emitter with **Spatial Sound** ticked.
- Colliders on the geometry meant to block, on a layer included in the occlusion layer mask.

**Install check.** Confirm `MelenitasDev.SoundsGood.AudioOcclusionSurface` resolves. If it does not,
the project has Lite; say occlusion is part of the full version and point at
<https://assetstore.unity.com/packages/tools/audio/sounds-good-easy-optimized-audio-manager-266130>.

## Quick start

1. Open `Tools > Sounds Good > Settings`.
2. Tick **Enable Occlusion**.
3. Set **Occlusion Layers** to the layers your walls and doors are on.
4. Make sure the sound is spatial:

   ```csharp
   new Sound(SFX.Alarm).SetSpatialSound().SetOcclusion().Play();
   ```

Expected observable result: in Play Mode, moving a wall between the listener and the sound makes it
audibly duller and quieter, and moving out from behind it restores it.

## Workflows

### Workflow: Turn occlusion on and tune it

**Goal.** Sensible muffling across the project.

**Steps.**

1. `Tools > Sounds Good > Settings`, Occlusion section.
2. The settings that matter most:
   - **Enable Occlusion** — the global switch.
   - **Occlusion Layers** — what counts as an obstacle. Leave out layers such as the player, or the
     player's own body will muffle everything.
   - **Max Distance** — beyond this, occlusion is not computed at all. Keep it near the range you
     actually hear things.
   - **Min Cutoff** / **Max Cutoff** — how dark a fully blocked sound gets, and its unblocked
     ceiling. Defaults are 1200 Hz and 22000 Hz.
   - **Min Volume Multiplier** — how quiet a fully blocked sound becomes.
   - **Check Interval** — how often each source re-evaluates. Larger is cheaper and less responsive.
   - **Lerp Duration** — how quickly it reacts, which is what stops it popping as you walk.
3. Stacked surfaces compound, bounded by **Max Occlusion Layers** and the extra-depth multipliers,
   so a room inside a room is duller than one wall.

**Expected result.** Sounds behind geometry are muffled and quieter, and the change is smooth as
the listener moves.

### Workflow: Give surfaces their own density

**Goal.** Glass lets more through than concrete.

**Steps.**

1. Create the materials from script:

   ```csharp
   using MelenitasDev.SoundsGood.Editor;

   SoundsGoodAuthoring.CreateOcclusionMaterial("Glass",    0.25f, out string error);
   SoundsGoodAuthoring.CreateOcclusionMaterial("Wood",     0.55f, out error);
   SoundsGoodAuthoring.CreateOcclusionMaterial("Concrete", 0.95f, out error);
   ```

   Density runs 0 (heard through unchanged) to 1 (blocks as much as the Settings allow).

2. After the reload, add an `Audio Occlusion Surface` component to each blocking object and set its
   material to the matching tag. Its `DensitySource` chooses between taking the value from the
   material or overriding it on that one object.

3. Objects with a collider but no surface component still occlude, at the global default.

**Expected result.** The same wall thickness muffles differently depending on its material, and
`Tools > Sounds Good > Occlusion Material Creator` lists all three.

### Workflow: Occlude some sounds but not others

**Goal.** Enemies are muffled through walls; the narrator is not.

**Steps.**

1. Occlusion is per audio object. Turn it on where it belongs:

   ```csharp
   new Sound(SFX.EnemyFootstep).SetSpatialSound().SetOcclusion(true).Play();
   new Sound(SFX.Narrator).Play();   // 2D, never occluded
   ```

2. On emitters, the same switch is the **Use Occlusion** tick box.
3. 2D sounds are never occluded, by design — music and UI stay clear.

**Expected result.** Diegetic audio reacts to geometry while narration and music do not.

## Verification

- `Tools > Sounds Good > Settings` shows **Enable Occlusion** ticked and the layer mask set.
- `Tools > Sounds Good > Occlusion Material Creator` lists the materials, and assets exist under the
  data root's `OcclusionMaterials` folder.
- `OcclusionMaterial.YourTag` compiles.
- In Play Mode, putting geometry between listener and source lowers the volume and dulls the tone,
  and the transition is gradual rather than instant.
- The Console is clean.

## API quick reference

| Entry point | Type | What it does |
|---|---|---|
| `SoundsGoodAuthoring.CreateOcclusionMaterial(tag, density, out error)` | static bool (Editor) | Creates and registers a material |
| `SoundsGoodAuthoring.GetOcclusionMaterialTags()` | static string[] | What already exists |
| `AudioOcclusionMaterial.Density` | float 0–1 | How much this surface holds back |
| `AudioOcclusionSurface` | MonoBehaviour | Puts a material on a collider; `Density`, `DensitySource` |
| `Sound/Music/Playlist.SetOcclusion(bool)` | instance | Per-audio switch |
| `Tools > Sounds Good > Settings` | window | Global switch, layers, cutoffs, interval |

## Common issues

| Symptom | Cause | Fix |
|---|---|---|
| Nothing is occluded | **Enable Occlusion** is off, or the sound is 2D | Tick it in Settings; call `SetSpatialSound()` |
| Still audible through walls | The wall's layer is not in **Occlusion Layers**, or it has no Collider | Fix the mask, add a Collider |
| Everything is muffled all the time | The player or the source itself is on an occluding layer | Exclude those layers |
| Muffling pops as the listener moves | **Lerp Duration** too short, or **Check Interval** too long | Raise the lerp, lower the interval |
| Frame rate drops with many sources | Occlusion raycasts per source | Raise **Check Interval**, lower **Max Distance**, reduce bounce rays |
| A thin window blocks like concrete | No occlusion material, so it uses the global default | Add an `Audio Occlusion Surface` with a low-density material |

## Boundaries

- Full version only. Absent in Sounds Good Lite.
- Occlusion is raycast-based and approximate. It is not an acoustic simulation and does not model
  diffraction exactly, though bounce rays approximate going around corners.
- Only 3D sounds are occluded. 2D audio is never affected.
- It needs Colliders. Renderers alone do not block sound.
