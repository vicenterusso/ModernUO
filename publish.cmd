:<<"::SHELLSCRIPT"
@ECHO OFF
GOTO :CMDSCRIPT

::SHELLSCRIPT
config=$1
os=$2
arch=${3:-$(uname -m)}

if [[ -n $os ]]; then
  os="-r $os"
elif [[ $(uname) = "Darwin" ]]; then
  os="-r osx"
else
  os="-r linux"
fi

if [[ $config ]]; then
  config="$(tr '[:lower:]' '[:upper:]' <<< ${1:0:1})${1:1}"
  config="-c $config"
else
  config="-c Release"
fi

if [[ $arch == *'aarch'* || $arch == *'arm'* ]]; then
  arch="arm64"
else
  arch="x64"
fi

if [[ $os == *'centos'* || $os == *'rhel'* ]]; then
  export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
fi

echo dotnet tool restore
dotnet tool restore

echo dotnet clean --verbosity quiet
dotnet clean --verbosity quiet
echo dotnet restore --force-evaluate --source https://api.nuget.org/v3/index.json
dotnet restore --force-evaluate --source https://api.nuget.org/v3/index.json

echo dotnet publish ${config} ${os}-${arch} --no-restore --self-contained=false -o Distribution/Assemblies Projects/UOContent/UOContent.csproj
dotnet publish ${config} ${os}-${arch} --no-restore --self-contained=false -o Distribution/Assemblies Projects/UOContent/UOContent.csproj

echo Generating serialization migration schema...
dotnet tool run ModernUOSchemaGenerator -- ModernUO.sln

exit $?

:CMDSCRIPT

IF "%~1" == "" (
  SET config=-c Release
) ELSE (
  IF "%~1" == "release" (
    SET config=-c Release
  ) ELSE (
    SET config=-c Debug
  )
)

IF "%~2" == "" (
  SET os=-r win
) ELSE (
  SET os=-r %~2
)

IF "%~3" == "" (
  SET arch=x64
) ELSE (
  SET arch=%~3
)

echo dotnet tool restore
dotnet tool restore

echo dotnet clean --verbosity quiet
dotnet clean --verbosity quiet
echo dotnet restore --force-evaluate --source https://api.nuget.org/v3/index.json
dotnet restore --force-evaluate --source https://api.nuget.org/v3/index.json

echo dotnet publish %config% %os%-%arch% --no-restore --self-contained=false -o Distribution\Assemblies Projects\UOContent\UOContent.csproj
dotnet publish %config% %os%-%arch% --no-restore --self-contained=false -o Distribution\Assemblies Projects\UOContent\UOContent.csproj

echo Generating serialization migration schema...
dotnet tool run ModernUOSchemaGenerator -- ModernUO.sln
