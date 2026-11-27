# Desert Scourge Arena Protection Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Сделать arena-блоки неразрушаемыми и удержать игрока внутри коробки при blink/teleport.

**Architecture:** Чистый helper вычисляет допустимые границы hitbox. `GlobalTile` и `GlobalWall` отменяют разрушение/взрывы только в активном subworld; `ModPlayer` зажимает позицию локального игрока в границах и гасит скорость при попытке выхода.

**Tech Stack:** C#, tModLoader 1.4.4, .NET 8.

**Spec:** `docs/superpowers/specs/2026-11-28-desert-scourge-arena-protection-design.md`

## Global Constraints

- Только активный Desert Scourge subworld.
- Основной мир не изменяется.
- Multiplayer/dedicated server не заявляются поддержанными.

## Review Focus

- За пределами arena subworld обычная добыча не должна блокироваться.
- Границы должны учитывать размер hitbox игрока, а не только его origin.
- Blink не должен оставлять игрока за стеной даже на один полный тик.
- Портал возврата должен остаться достижимым внутри границ.

---

### Task 1: Границы движения

**Files:**
- Create: `Systems/Arenas/ArenaPlayerBounds.cs`
- Modify: `tests/ArenaLogicTests/Program.cs`
- Modify: `tests/ArenaLogicTests/ArenaLogicTests.csproj`

**Interfaces:**
- Produces: `ArenaPlayerBounds.ClampCenter(float centerX, float centerY, int width, int height) -> Vector2`.

- [ ] Написать failing-тесты: точка внутри остаётся прежней; точки за каждой стороной зажимаются внутрь с учётом половины hitbox.
- [ ] Запустить `dotnet run --project tests/ArenaLogicTests/ArenaLogicTests.csproj`; увидеть RED.
- [ ] Реализовать helper через `ArenaSubworldLayout`.
- [ ] Запустить тесты и убедиться в GREEN.

### Task 2: Защита мира и игрока

**Files:**
- Create: `Systems/Arenas/ArenaWorldProtection.cs`
- Create: `Players/ArenaBoundaryPlayer.cs`
- Modify: `README.md`

- [ ] Добавить `GlobalTile`/`GlobalWall`, возвращающие запрет для добычи, стен и взрывов только при `DesertScourgeArenaSubworld.IsActive()`.
- [ ] В `ModPlayer.PostUpdate` применить `ArenaPlayerBounds.ClampCenter`; при коррекции выставить `Player.velocity = Vector2.Zero`.
- [ ] Запустить тесты, entrypoint-проверку, MSBuild и `git diff --check`.
- [ ] Ручной одиночный прогон: кирка/бомба не меняют arena tiles; Rod of Discord и другой blink не выводят из коробки; основной мир не затронут.
