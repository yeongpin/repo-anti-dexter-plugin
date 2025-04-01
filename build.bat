@echo off
chcp 65001 > nul
echo ===================================
echo Anti-Dexter 插件编译脚本
echo ===================================
echo.

:: 设置环境变量
set PROJECT_NAME=Anti-Dexter
set SOLUTION_FILE=%PROJECT_NAME%.sln
set OUTPUT_DIR=Build
set RELEASE_NAME=anti-dexter

:: 创建输出目录
if not exist %OUTPUT_DIR% mkdir %OUTPUT_DIR%

echo 正在清理旧的编译文件...
if exist %PROJECT_NAME%\bin rd /s /q %PROJECT_NAME%\bin
if exist %PROJECT_NAME%\obj rd /s /q %PROJECT_NAME%\obj
if exist %OUTPUT_DIR%\%PROJECT_NAME%.dll del %OUTPUT_DIR%\%PROJECT_NAME%.dll
if exist %OUTPUT_DIR%\%RELEASE_NAME%.zip del %OUTPUT_DIR%\%RELEASE_NAME%.zip

echo.
echo 正在编译项目...
dotnet build %SOLUTION_FILE% -c Release

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo 编译失败！请检查错误信息。
    goto :end
)

echo.
echo 正在复制文件到输出目录...
copy %PROJECT_NAME%\bin\Release\netstandard2.1\%PROJECT_NAME%.dll %OUTPUT_DIR%\%PROJECT_NAME%.dll

if exist %OUTPUT_DIR%\%PROJECT_NAME%.dll (
    echo.
    echo 编译成功！文件已保存到 %OUTPUT_DIR%\%PROJECT_NAME%.dll
) else (
    echo.
    echo 文件复制失败！请检查路径是否正确。
    goto :end
)

echo.
echo 正在复制附加文件...
if exist icon.png copy icon.png %OUTPUT_DIR%\icon.png
if exist manifest.json copy manifest.json %OUTPUT_DIR%\manifest.json
if exist README.md copy README.md %OUTPUT_DIR%\README.md
if exist CHANGELOG.md copy CHANGELOG.md %OUTPUT_DIR%\CHANGELOG.md

echo.
echo 正在创建发布包...
powershell -Command "Compress-Archive -Path '%OUTPUT_DIR%\*' -DestinationPath '%OUTPUT_DIR%\%RELEASE_NAME%.zip' -Force"

if exist %OUTPUT_DIR%\%RELEASE_NAME%.zip (
    echo.
    echo 发布包已创建: %OUTPUT_DIR%\%RELEASE_NAME%.zip
) else (
    echo.
    echo 创建发布包失败！
    goto :end
)

echo.
echo 是否要将插件复制到游戏目录？(Y/N)
set /p COPY_TO_GAME=

if /i "%COPY_TO_GAME%"=="Y" (
    echo.
    echo 请输入BepInEx插件目录的路径(例如: D:\Games\REPO\BepInEx\plugins):
    set /p GAME_PLUGINS_DIR=
    
    if exist "%GAME_PLUGINS_DIR%" (
        copy %OUTPUT_DIR%\%PROJECT_NAME%.dll "%GAME_PLUGINS_DIR%"
        echo 插件已复制到游戏目录！
    ) else (
        echo 指定的目录不存在，请手动复制插件。
    )
)

:end
echo.
echo 按任意键退出...
pause > nul 