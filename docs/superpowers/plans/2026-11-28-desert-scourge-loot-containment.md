# Desert Scourge Loot Containment Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Удерживать все выпавшие предметы внутри доступной игроку части Desert Scourge arena.

**Architecture:** Чистый `ArenaItemBounds` вычисляет допустимый центр item hitbox по layout арены и сообщает, по каким осям произошла коррекция. `GlobalItem` применяет результат после физики только в активном subworld и гасит соответствующие компоненты скорости.

**Tech Stack:** C#, tModLoader 1.4.4, .NET 8.

**Spec:** `docs/superpowers/specs/2026-11-28-desert-scourge-loot-containment-design.md`

## Global Constraints

- Ограничение действует только в активном `DesertScourgeArenaSubworld`.
- Предметы остаются в месте выпадения настолько, насколько это позволяют границы; не удалять и не переносить их в общую точку.
- Учитывать полный hitbox item, а не только позицию origin.
- Одиночная игра — единственный проверяемый режим; multiplayer/dedicated server не заявляются поддержанными.

## Review Focus

- Предмет внутри области не должен менять позицию или скорость — тест в Task 1.
- Предметы у левой, правой стены, пола и потолка должны оставаться полностью внутри — тесты в Task 1.
- При упоре в боковую границу не должна гаситься вертикальная скорость, и наоборот — тесты в Task 1.
- Предметы в основном мире не должны затрагиваться — ручная проверка в Task 2.
- Нестандартные размеры hitbox должны использоваться в расчёте — тест Task 1.

---

### Task 1: Чистые границы предмета

**Files:**
- Create: `Systems/Arenas/ArenaItemBounds.cs`
- Modify: `tests/ArenaLogicTests/ArenaLogicTests.csproj`
- Modify: `tests/ArenaLogicTests/Program.cs`

**Interfaces:**
- Produces: `ArenaItemBounds.ClampCenter(float centerX, float centerY, int width, int height) -> ArenaItemClampResult`.
- `ArenaItemClampResult` содержит `CenterX`, `CenterY`, `WasClampedX`, `WasClampedY`.

- [ ] Написать failing-тесты для неизменной позиции внутри области, всех четырёх границ, независимости осей и широкого hitbox.
- [ ] Запустить `dotnet run --project tests/ArenaLogicTests/ArenaLogicTests.csproj`; увидеть RED из-за отсутствующих типов.
- [ ] Реализовать `ArenaItemBounds` через существующие tile-константы `ArenaSubworldLayout`; допустимая область — одна клетка внутри стен, над полом и под потолком.
- [ ] Запустить `dotnet run --project tests/ArenaLogicTests/ArenaLogicTests.csproj`; увидеть GREEN.

### Task 2: Применение к world-item

**Files:**
- Create: `Systems/Arenas/ArenaLootContainment.cs`
- Modify: `README.md`

**Interfaces:**
- Consumes: `ArenaItemBounds.ClampCenter` и `DesertScourgeArenaSubworld.IsActive()`.
- Produces: `GlobalItem.PostUpdate(Item item)`; при коррекции обновляет `item.Center`, гасит `item.velocity.X` и/или `item.velocity.Y` только по исправленным осям.

- [ ] Реализовать `ArenaLootContainment : GlobalItem` с ранним выходом для неактивных предметов и неактивного subworld.
- [ ] Добавить в README ограничение: дроп удерживается внутри арены в одиночной игре.
- [ ] Выполнить `dotnet run --project tests/ArenaLogicTests/ArenaLogicTests.csproj`, `pwsh -File tests/verify-mod-entrypoint.ps1`, `dotnet msbuild HenriksBossArenas.csproj /p:TModLoaderInstallDir="<путь-к-tModLoader>"` и `git diff --check`.
- [ ] Ручной одиночный прогон: победить босса у каждой стены и над полом; подтвердить, что весь дроп остаётся доступным; затем выбросить предмет в основном мире и подтвердить отсутствие коррекции.
