# Desert Scourge Singleplayer Arena Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Завершить проверяемый одиночный сценарий Desert Scourge: пустая пустынная арена, бой, респавн и возврат после победы.

**Architecture:** Subworld остаётся временной отдельной картой. Генератор создаёт пустую коробку и песчаное основание для vanilla/Calamity пустынного биома. Серверная сессия различает переход из основного мира и босса, появившегося внутри арены; после смерти босса она вызывает документированный `SubworldLibrary.Call("Exit")` в одиночной игре.

**Tech Stack:** C#, tModLoader 1.4.4, Calamity Mod, Subworld Library, .NET 8, PowerShell.

**Spec:** `docs/superpowers/specs/2026-11-21-desert-scourge-subworld-design.md`

## Global Constraints

- Реализуется и проверяется только одиночная игра.
- `CalamityMod` и `SubworldLibrary` — обязательные зависимости; их исходники и бинарные файлы не входят в репозиторий.
- Основной мир не получает временных тайлов арены.
- Один Desert Scourge-сеанс на мир; другие боссы и параллельные арены вне объёма.
- Не заявлять поддержку multiplayer или dedicated server без отдельного игрового прогона.

## Review Focus

- Внутри коробки игрок видит воздух, пол, боковые и верхнюю границы; песок не должен заполнять игровое пространство.
- Песчаное основание должно сохранять `ZoneDesert`; это подтверждается ручным игровым прогоном до заявления совместимости с Calamity.
- NPC Desert Scourge, созданный внутри subworld, не должен пытаться открыть subworld повторно.
- Исходный босс удаляется только после принятого перехода, иначе остаётся в основном мире.
- Победа возвращает игрока только из активной арены; неактивная сессия не вызывает `Exit`.

---

### Task 1: Пустая коробка и пустынный биом

**Files:**
- Modify: `Systems/Arenas/ArenaSubworldLayout.cs`
- Modify: `Systems/Arenas/DesertScourgeArenaSubworld.cs`
- Modify: `tests/ArenaLogicTests/Program.cs`
- Modify: `README.md`

**Interfaces:**
- Produces: константы границ коробки и песчаного основания в `ArenaSubworldLayout`.
- Produces: `GenerateArena` с воздухом в игровой области, границами из `SandstoneBrick` и песком ниже пола.

- [ ] Написать failing-тесты для координат пола/потолка и правила: spawn находится строго между ними.
- [ ] Запустить `dotnet run --project tests/ArenaLogicTests/ArenaLogicTests.csproj`; убедиться, что новый тест падает.
- [ ] Добавить константы layout и переписать `GenerateArena`: очистить каждый тайл, создать пол/стены/потолок из `SandstoneBrick`, заполнить область под полом `Sand`.
- [ ] Запустить тесты, `pwsh -File tests/verify-mod-entrypoint.ps1` и MSBuild.
- [ ] Ручной одиночный прогон: после призыва убедиться, что игрок внутри пустой коробки и видит пустынный фон; не заявлять совместимость с Calamity до отдельного боя.
- [ ] Обновить README фактическим результатом прогона.

### Task 2: Босс в subworld и возврат после победы

**Files:**
- Modify: `Systems/Arenas/ArenaSessionState.cs`
- Modify: `Systems/Arenas/ArenaSessionSystem.cs`
- Modify: `Systems/Arenas/DesertScourgeArenaTrigger.cs`
- Modify: `Systems/Arenas/DesertScourgeArenaSubworld.cs`
- Modify: `tests/ArenaLogicTests/Program.cs`

**Interfaces:**
- Consumes: layout и subworld Task 1.
- Produces: `ArenaSessionSystem.TryTrackArenaBoss(NPC boss)` и `ArenaSessionSystem.IsParticipant(string playerName)`.
- Produces: одиночный `ExitArena()` через `SubworldLibrary.Call("Exit")` после смерти отслеживаемого босса.

- [ ] Написать failing-тесты: сессия заменяет привязку исходного босса на аренового и очищается после завершения.
- [ ] Запустить тесты и увидеть ожидаемый RED.
- [ ] Разделить trigger: исходный Desert Scourge начинает сессию только вне subworld; созданный в subworld привязывается к активной сессии и не вызывает повторный вход.
- [ ] Реализовать проверку смерти аренового босса и одиночный выход из subworld. Исходный босс деактивируется только после успешного `Enter`.
- [ ] Запустить тесты, entrypoint-проверку и MSBuild.
- [ ] Ручной одиночный прогон: призыв, бой с Desert Scourge, победа и возврат в основной мир.

### Task 3: Респавн участника на арене

**Files:**
- Create: `Players/ArenaParticipantPlayer.cs`
- Modify: `Systems/Arenas/ArenaSessionSystem.cs`
- Modify: `tests/ArenaLogicTests/Program.cs`
- Modify: `README.md`

**Interfaces:**
- Consumes: `ArenaSessionSystem.IsParticipant(string playerName)` и spawn-константы Task 1.
- Produces: `ArenaParticipantPlayer.OnRespawn()` с переносом только живого участника активной одиночной сессии к spawn arena.

- [ ] Написать failing-тесты: после очистки участник больше не считается участником; spawn остаётся внутри коробки.
- [ ] Запустить тесты и увидеть RED.
- [ ] Добавить `ModPlayer`; в `OnRespawn` перенести игрока к spawn только при активной сессии и активном subworld.
- [ ] Запустить тесты, entrypoint-проверку и MSBuild.
- [ ] Ручной одиночный прогон: смерть в арене, респавн внутри коробки, победа и возврат.

### Task 4: Итоговая документация и границы поддержки

**Files:**
- Modify: `README.md`
- Modify: `docs/ARCHITECTURE.md`
- Modify: `docs/ROADMAP.md`

- [ ] Зафиксировать реальные результаты одиночных прогонов и явно оставить multiplayer/dedicated server вне поддержки.
- [ ] Запустить полный набор: `dotnet run --project tests/ArenaLogicTests/ArenaLogicTests.csproj`, `pwsh -File tests/verify-mod-entrypoint.ps1`, MSBuild и `git diff --check`.
- [ ] Перед коммитом просмотреть изменения, проверить отсутствие секретов и не коммитить непроверенный код.

## Самопроверка плана

- Пустая коробка и пустынный фон покрыты Task 1.
- Отслеживание босса и возврат после победы покрыты Task 2.
- Респавн внутри арены покрыт Task 3.
- Multiplayer и dedicated server исключены из задач и явно остаются неподтверждёнными.
