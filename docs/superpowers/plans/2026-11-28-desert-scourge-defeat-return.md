# Desert Scourge Defeat Return Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** В одиночной игре вернуть погибшего участника после поражения и дать победителю портал для ручного возврата после сбора лута.

**Architecture:** `ArenaSessionState` хранит фазу ожидания arena-босса, но решение о поражении принимает `ArenaSessionSystem` только в активном subworld: отслеживаемый босс неактивен и нет живых участников. Перед `SubworldLibrary.Call("Exit")` система помечает отложенный возврат; `OnWorldLoad` переносит игрока на `Main.spawnTileX/Main.spawnTileY` после восстановления основного мира.

**Tech Stack:** C#, tModLoader 1.4.4, Subworld Library, .NET 8.

**Spec:** `docs/superpowers/specs/2026-11-28-desert-scourge-defeat-return-design.md`

## Global Constraints

- Поддерживается только `NetmodeID.SinglePlayer`.
- Multiplayer и dedicated server не реализуются и не объявляются поддержанными.
- Не менять тайлы основного мира.
- Не копировать Calamity или Subworld Library в репозиторий.

## Review Focus

- Не завершать сессию, пока arena-босс ожидается после перехода.
- Не считать поражением живого участника или активного босса.
- Не вызывать `Exit` за пределами активного subworld.
- Переносить только после загрузки основного мира, когда `Main.spawnTileX/Y` восстановлены.
- Не затрагивать multiplayer-ветки.
- Не вызывать автоматический выход после победы: игроки должны собрать дроп и вернуться вручную.

---

### Task 1: Чистое правило поражения

**Files:**
- Create: `Systems/Arenas/ArenaDefeatEvaluator.cs`
- Modify: `tests/ArenaLogicTests/Program.cs`
- Modify: `tests/ArenaLogicTests/ArenaLogicTests.csproj`

**Interfaces:**
- Produces: `ArenaDefeatEvaluator.IsDefeat(bool bossIsActive, bool hasLivingParticipants) -> bool`.

- [ ] Написать failing-тесты: `false/false` возвращает `true`; активный босс или живой участник возвращает `false`.
- [ ] Запустить `dotnet run --project tests/ArenaLogicTests/ArenaLogicTests.csproj`; убедиться в RED.
- [ ] Реализовать минимальное правило в `ArenaDefeatEvaluator`.
- [ ] Запустить тесты и убедиться в GREEN.

### Task 2: Возврат после поражения

**Files:**
- Modify: `Systems/Arenas/ArenaSessionSystem.cs`
- Modify: `Systems/Arenas/DesertScourgeArenaSubworld.cs`
- Modify: `tests/ArenaLogicTests/Program.cs`
- Modify: `README.md`

**Interfaces:**
- Consumes: `ArenaDefeatEvaluator.IsDefeat`.
- Produces: `ArenaSessionSystem.OnWorldLoad()` с отложенным одиночным переносом на основной spawn.

- [ ] Написать failing-тест для evaluator, закрепляющий что уход босса при живом участнике не является поражением.
- [ ] В `PostUpdateWorld` вычислять наличие живых `Main.ActivePlayers`; при поражении в активном subworld отметить отложенный перенос, очистить session state и вызвать `SubworldLibrary.Call("Exit")`.
- [ ] В `OnWorldLoad` после возврата перенести `Main.LocalPlayer` к `Main.spawnTileX * 16, Main.spawnTileY * 16` только при отмеченном одиночном возврате.
- [ ] Запустить тесты, `pwsh -File tests/verify-mod-entrypoint.ps1`, MSBuild и `git diff --check`.
- [ ] Ручной одиночный прогон: погибнуть, дождаться исчезновения Desert Scourge, подтвердить возврат в основной мир на мировом spawn.
- [ ] Обновить README фактическим результатом; явно сохранить ограничение multiplayer/dedicated server.

### Task 3: Портал ручного выхода после победы

**Files:**
- Create: `Projectiles/ArenaReturnPortal.cs`
- Modify: `Systems/Arenas/ArenaSessionSystem.cs`
- Modify: `Systems/Arenas/DesertScourgeArenaSubworld.cs`
- Modify: `tests/ArenaLogicTests/Program.cs`

**Interfaces:**
- Produces: `ArenaReturnPortal` — одиночный визуальный портал, использующий ванильную текстуру и вызывающий `DesertScourgeArenaSubworld.Exit()` при касании локального живого игрока.
- Consumes: конец активной сессии с неактивным боссом и хотя бы одним живым участником.

- [ ] Написать failing-тесты правила исхода: неактивный босс и живой участник не являются поражением.
- [ ] Запустить `dotnet run --project tests/ArenaLogicTests/ArenaLogicTests.csproj`; увидеть RED.
- [ ] Добавить portal projectile без PNG-ресурса; разместить его над полом после победы. Он не должен автоматически выводить игрока и не должен появляться при поражении.
- [ ] При касании живого локального игрока вызвать `Exit`, деактивировать projectile и не менять основной мир.
- [ ] Запустить тесты, entrypoint-проверку, MSBuild и `git diff --check`.
- [ ] Ручной одиночный прогон: убить босса, подобрать дроп, войти в портал и оказаться в основном мире.

## Самопроверка

- Спецификация покрыта задачами 1–2.
- Решение о поражении тестируется изолированно.
- Перенос намеренно выполняется только после загрузки основного мира.
- Сетевая логика не добавляется.
