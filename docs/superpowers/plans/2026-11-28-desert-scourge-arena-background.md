# Desert Scourge Arena Background Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Показать пользовательскую пустынную панораму фоном только на арене Desert Scourge.

**Architecture:** Ресурс мода проверяется скриптом упаковки. `ModSceneEffect` активен только в subworld и выбирает `ModSurfaceBackgroundStyle`, который рисует один panorama texture вместо ванильного close background.

**Tech Stack:** C#, tModLoader 1.4.4, XNA SpriteBatch.

**Spec:** `docs/superpowers/specs/2026-11-28-desert-scourge-arena-background-design.md`

## Global Constraints

- Фон действует только в активном Desert Scourge subworld.
- Панорама находится позади тайлов и сущностей.
- Основной мир не изменяется.

## Review Focus

- Ресурс должен войти в `.tmod` — проверка Task 1.
- Фон не должен быть активен вне subworld — ручная проверка Task 2.
- Не растягивать панораму с искажением пропорций — ручная проверка Task 2.

---

### Task 1: Ресурс и его проверка

**Files:**
- Create: `Assets/Backgrounds/DesertScourgeArena.png`
- Modify: `tests/verify-mod-entrypoint.ps1`

- [ ] Добавить failing-проверку наличия `Assets/Backgrounds/DesertScourgeArena.png`.
- [ ] Запустить `pwsh -File tests/verify-mod-entrypoint.ps1`; увидеть RED.
- [ ] Скопировать предоставленный пользователем PNG под фиксированным именем.
- [ ] Запустить проверку; увидеть GREEN.

### Task 2: Отрисовка фона

**Files:**
- Create: `Systems/Arenas/DesertScourgeArenaSceneEffect.cs`
- Create: `Systems/Arenas/DesertScourgeArenaBackgroundStyle.cs`

- [ ] Реализовать scene effect с `IsSceneEffectActive`, зависящим от `DesertScourgeArenaSubworld.IsActive()`.
- [ ] Реализовать custom close background, рисующий панораму без искажения и только в активной арене.
- [ ] Выполнить entrypoint-проверку, тесты, source build tModLoader и `git diff --check`.
- [ ] Ручной прогон: зайти в arena, проверить фон; выйти в основной мир, подтвердить отсутствие фона.
