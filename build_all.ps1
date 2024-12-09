$solutionDir = Split-Path $MyInvocation.MyCommand.Path
Set-Location $solutionDir
Get-ChildItem -Recurse -Filter *.csproj | ForEach-Object {
    $content = Get-Content $_.FullName
    if ($content -notmatch "<TargetFramework>net8.0</TargetFramework>") {
        $content = $content -replace "<TargetFramework>.*?</TargetFramework>", "<TargetFramework>net8.0</TargetFramework>"
    }
    if ($content -notmatch "<RuntimeIdentifiers>win-x64</RuntimeIdentifiers>") {
        if ($content -match "<RuntimeIdentifiers>") {
            $content = $content -replace "<RuntimeIdentifiers>.*?</RuntimeIdentifiers>", "<RuntimeIdentifiers>win-x64</RuntimeIdentifiers>"
        } else {
            $content = $content -replace "</PropertyGroup>", "<RuntimeIdentifiers>win-x64</RuntimeIdentifiers></PropertyGroup>"
        }
    }
    if ($content -notmatch "<PublishTrimmed>true</PublishTrimmed>") {
        $content = $content -replace "</PropertyGroup>", "<PublishTrimmed>true</PublishTrimmed></PropertyGroup>"
    }
    if ($content -notmatch "<PublishSingleFile>true</PublishSingleFile>") {
        $content = $content -replace "</PropertyGroup>", "<PublishSingleFile>true</PublishSingleFile></PropertyGroup>"
    }
    if ($content -notmatch "<TrimMode>link</TrimMode>") {
        $content = $content -replace "</PropertyGroup>", "<TrimMode>link</TrimMode></PropertyGroup>"
    }
    Set-Content $_.FullName $content
}
dotnet restore
dotnet build --configuration Release
Get-ChildItem -Recurse -Filter *.csproj | ForEach-Object {
    $projectPath = $_.FullName
    $projectName = Split-Path $projectPath -LeafBase
    if ($projectName -match "Tests") {
        dotnet test $projectPath --configuration Release
    } else {
        dotnet publish $projectPath --configuration Release --output "$($solutionDir)\publish\$projectName" --no-build -r win-x64
    }
}
