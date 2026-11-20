# Boss Arena Architecture Research Plan

> **For agentic workers:** This document records the research and decision gate for architecture only. It does not authorize implementation of gameplay systems.

**Goal:** Select an arena-isolation architecture that preserves Calamity compatibility and supports authoritative multiplayer before any gameplay code is written.

**Architecture:** Compare native main-world regions, Subworld Library and separate-world approaches against the installed tModLoader and Calamity environment. Adopt a temporary main-world region only after documenting its limitations; defer its implementation to a separately approved plan.

**Tech Stack:** C#, tModLoader 2026.09 stable / Terraria 1.4.4.9, CalamityMod 2.0.4.67, Windows 11.

**Spec:** `docs/ARCHITECTURE_RESEARCH.md`

## Global Constraints

- CalamityMod is an obligatory `modReferences` dependency; do not redistribute its files.
- No arena, teleportation, portal, boss-detection or multiplayer gameplay code belongs to this stage.
- Future state must be server-authoritative and compatible with dedicated servers.
- A new stage requires explicit user approval.

## Review Focus

- A boss whose AI requires a special location must remain excluded until its mechanics are individually verified.
- Arena-region edits must never overwrite player construction without an explicit safe-site rule.
- A server restart during a fight must restore the modified region before players can use it.
- A client joining during a fight must receive authoritative state and relevant tile data.
- A second summon attempt must not start a competing arena session.

---

### Task 1: Record platform capabilities and alternatives

**Files:**
- Create: `docs/ARCHITECTURE_RESEARCH.md`
- Modify: `docs/ARCHITECTURE.md`

- [x] Inspect the installed tModLoader version and its `ModSystem` persistence/network documentation.
- [x] Inspect the installed availability and public implementation model of Subworld Library.
- [x] Inspect Calamity public sources only to establish that its bosses use normal world state and must retain their original mechanics.
- [x] Compare the four alternatives against compatibility, dedicated servers, synchronization, state persistence, return safety and performance.
- [x] Record evidence, confidence levels, rejected approaches and unverified assumptions.

### Task 2: Establish the architecture gate

**Files:**
- Modify: `README.md`
- Modify: `AGENTS.md`

- [x] Recommend temporary isolated regions in the primary world, with a server-owned snapshot and restore lifecycle.
- [x] Explicitly defer gameplay implementation and request approval before the Stage 3 plan.
