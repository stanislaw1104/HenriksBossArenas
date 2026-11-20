param(
    [string]$AssemblyPath = "$(Join-Path $PSScriptRoot '..\bin\Debug\net8.0\HenriksBossArenas.dll')",
    [string]$TModLoaderInstallDir = 'C:\Program Files (x86)\Steam\steamapps\common\tModLoader'
)

$resolvedAssemblyPath = (Resolve-Path -LiteralPath $AssemblyPath).Path
$resolvedTModLoaderInstallDir = (Resolve-Path -LiteralPath $TModLoaderInstallDir).Path

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
