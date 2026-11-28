param(
    [string]$AssemblyPath = "$(Join-Path $PSScriptRoot '..\bin\Debug\net8.0\HenriksBossArenas.dll')",
    [string]$TModLoaderInstallDir = 'C:\Program Files (x86)\Steam\steamapps\common\tModLoader'
)

$resolvedAssemblyPath = (Resolve-Path -LiteralPath $AssemblyPath).Path
$resolvedTModLoaderInstallDir = (Resolve-Path -LiteralPath $TModLoaderInstallDir).Path
$buildPropertiesPath = Join-Path $PSScriptRoot '..\build.txt'
$backgroundPath = Join-Path $PSScriptRoot '..\Assets\Backgrounds\DesertScourgeArena.png'

if (-not (Select-String -LiteralPath $buildPropertiesPath -Pattern '^buildIgnore\s*=.*(^|,)tests/\*' -Quiet)) {
    throw 'build.txt must exclude tests/* so tModLoader does not compile test programs into the mod assembly.'
}

if (-not (Test-Path -LiteralPath $backgroundPath)) {
    throw 'The Desert Scourge arena background texture is missing.'
}

[System.Runtime.Loader.AssemblyLoadContext]::Default.add_Resolving({
    param($context, $assemblyName)

    $candidatePath = Join-Path $resolvedTModLoaderInstallDir "$($assemblyName.Name).dll"
    if (Test-Path -LiteralPath $candidatePath) {
        return $context.LoadFromAssemblyPath($candidatePath)
    }

    return $null
})

$assembly = [System.Reflection.Assembly]::LoadFrom($resolvedAssemblyPath)
$modType = $assembly.GetType('HenriksBossArenas.HenriksBossArenas')

if ($null -eq $modType) {
    throw 'The required HenriksBossArenas.HenriksBossArenas mod entry point was not found.'
}

$modBaseType = [System.Type]::GetType('Terraria.ModLoader.Mod, tModLoader', $true)
if (-not $modBaseType.IsAssignableFrom($modType)) {
    throw 'The mod entry point must inherit Terraria.ModLoader.Mod.'
}

Write-Output 'Mod entry point verification passed.'
