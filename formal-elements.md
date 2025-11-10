# Formal Elements – *Woke Alone*

## High Concept
A 2D narrative survival game. The player awakens in a deserted hospital after a zombie-like outbreak, explores a semi-open city block, uncovers what happened to their family, and survives via stealth, scavenging, and smart use of sound and light.

## Target Audience
Players who enjoy story-first survival (13+) and tense stealth (e.g., “This War of Mine”, “Darkwood”, “Project Zomboid”).

## Platform
PC (Windows). Optional Web build for demos.

## Player Role & Objective
- **Role:** An amnesiac survivor.
- **Short-term goals:** Find basic supplies, avoid infected, unlock exits, decode clues.
- **Long-term goal:** Reconstruct the protagonist’s family storyline and reach a safe rendezvous.

## Core Loop
1) **Scout quietly** → 2) **Collect clues/resources** → 3) **Avoid/redirect threats** (sound/light) →  
4) **Unlock new areas/story beats** → 5) **Craft/prepare for the next push** → repeat.

## Mechanics (Rules & Systems)
- **Stealth:**  
  - Vision cones for enemies; cover & line-of-sight.  
  - Noise meter (footsteps, doors, thrown items, alarms).  
  - Light level affects detection; player can toggle flashlight and use shadows.
- **Inventory & Crafting (lightweight):** limited slots; craft distractions (bottle, timer), simple meds.
- **Health & Stamina:** sprint drains stamina; injuries reduce speed and noise control.
- **Time:** day/night cycle in demo (shortened); nights increase enemy senses.
- **World Interaction:** doors (locked/shut), cabinets, terminals, notes, keycards, fuses.
- **Failure/Success:** getting caught or overwhelmed → respawn at last safe point; success = objective completion and story progression.

## Entities
- **Player** – movement, crouch, sprint, interact, throw, flashlight.  
- **Infected** – hearing/vision parameters; simple patrol → investigate → chase states.  
- **World Props** – containers, noise-makers, locks, light sources, hiding spots.  
- **Narrative Items** – notes, recordings, photos, wristband IDs.

## Aesthetics (MDA)
- **Emotion:** tension, loneliness, fragile hope.  
- **Visuals:** hand-drawn/tileset 2D, muted palette, readable silhouettes.  
- **Audio:** diegetic hums, distant groans, footstep variations; audio cues communicate risk.

## Camera & Controls
- **Camera:** 2D top-down or slightly tilted 
- **Controls:** WASD move, Shift sprint, Ctrl crouch, E interact, F flashlight, G throw, I inventory.

## Narrative Structure (Acted beats for MVP)
- **Act 1 – Ward Awakening:** Tutorial beats through the hospital; find keycard & bandage; first infected encounter (escape, not kill).  
- **Act 2 – Street Crossing:** Reach pharmacy; learn about patient transfer involving the protagonist’s family.  
- **Act 3 – The House:** Search the apartment; reveal a message that sets the long-term goal (a rendezvous location beyond the demo).

## Level Plan (MVP)
- **Hospital (3 floors + basement)** → **Service alley** → **Small street hub** → **Pharmacy** → **Apartment block**.  
- Each area teaches one mechanic (noise, light, patrols, alternate paths).

## Accessibility (MVP)
- Subtitles, rebindable keys, color-contrast safe UI, difficulty presets (Story/Standard/Hard).

## Production Scope (MVP Backlog – first pass)
- **Gameplay:** movement/crouch/sprint, interact system, inventory, throwables, stealth AI, save/load.  
- **Content:** 5–7 rooms (hospital), 1–2 exterior blocks, 2 interior locations, 3 quests, 2 puzzles.
- **UI:** pause, options, inventory, minimal HUD (noise/light indicators).  
- **Audio:** SFX pass for footsteps/doors/alerts; simple music stingers.

---

# Market Survey (Comparable Games & Differentiation)

| Title | Why comparable | Key takeaways | Our differentiators |
|---|---|---|---|
| **Darkwood** (top-down survival-horror) | 2D tension, scarce resources, strong atmosphere | Sound & light as core tools; slow reveal of story | We lean more on investigative narrative and hospital start; stealth readability with explicit vision/noise UI |
| **This War of Mine** (2D survival narrative) | Scavenging under risk, moral choices | Human vulnerability & meaningful scarcity | Focus on stealth vs infected; micro-puzzles; semi-open traversal rather than day-phase management |
| **Project Zomboid** (isometric zombie survival) | Systems-driven survival, sound attracts hordes | Deep simulation; noise-driven AI | Smaller scope, story-first, curated levels with “simulation-lite” |
| **Death Road to Canada** (2D roguelite zombies) | 2D apocalypse tone | Readable crowds, fast feedback | Slower, stealth-forward, investigative pacing |
| **Cataclysm: DDA** / **Don’t Starve** (survival) | Crafting/foraging pressure | Long-tail survival hooks | Narrative closure and guided acts in a compact space |

**Positioning:** “A story-driven, stealth-first 2D survival experience with readable sound/light systems and a semi-open micro-city—beginning in a hospital and culminating in a personal family mystery.”

