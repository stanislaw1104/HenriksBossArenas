# Henrik's Boss Arenas

Аддон для **Calamity Mod**, который в будущем добавит уникальные тематические арены для боёв с подходящими боссами Terraria и Calamity.

## Статус

**Первый прототип в разработке.** После штатного призыва Desert Scourge игрок в одиночной игре переходит на отдельную временную карту арены, созданную через Subworld Library. Основной мир временными тайлами не изменяется. Вход в карту вручную проверен в tModLoader; бой, возврат после победы, респавн и multiplayer/dedicated server пока не реализованы и не подтверждены.

## Требования

- Terraria 1.4.4 и совместимый стабильный tModLoader;
- .NET SDK, требуемый установленной версией tModLoader (для текущего локального окружения — .NET 8 SDK);
- [Calamity Mod](https://steamcommunity.com/sharedfiles/filedetails/?id=2824688072), включённый в tModLoader.
- [Subworld Library](https://steamcommunity.com/sharedfiles/filedetails/?id=2785100219), включённая в tModLoader.

## Зависимости

Calamity Mod и Subworld Library являются обязательными зависимостями и объявлены в `build.txt`. Их исходный код и бинарные файлы не входят в этот репозиторий.

## Локальная сборка

1. Установите tModLoader и Calamity Mod через Steam Workshop.
2. Убедитесь, что `CalamityMod.tmod` доступен в каталоге модов tModLoader.
3. В корне проекта выполните:

   ```powershell
   dotnet msbuild HenriksBossArenas.csproj /p:TModLoaderInstallDir="C:\Program Files (x86)\Steam\steamapps\common\tModLoader"
   ```

   Если tModLoader установлен в другом месте, передайте фактический путь в свойстве `TModLoaderInstallDir`.

## Запуск в tModLoader

После успешной сборки tModLoader создаёт файл `.tmod` в своём каталоге модов. В меню **Mods** включите Calamity Mod, Subworld Library и Henrik's Boss Arenas, затем перезагрузите моды.

## Планируемые возможности

- исследование архитектуры изолированных арен;
- серверно-авторитетное управление сражениями в одиночной игре и мультиплеере;
- расширяемые описания арен;
- тематические арены для подходящих боссов.

## Ограничения первого прототипа

- Поддерживается только Desert Scourge и одна сессия на мир.
- Арена — отдельный временный subworld, а не область основного мира.
- Подтверждён только одиночный вход в карту после призыва; сам бой, возврат, смерть/респавн, multiplayer и dedicated server требуют отдельной реализации и игрового прогона.

Подробности: [архитектура](docs/ARCHITECTURE.md), [исследование архитектуры](docs/ARCHITECTURE_RESEARCH.md), [дорожная карта](docs/ROADMAP.md), [разработка](docs/DEVELOPMENT.md).
