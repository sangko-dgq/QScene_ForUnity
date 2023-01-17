@echo off
chcp 65001

:begin
color 1F & set /p choice=Are you sure you want to continue? (y/n)
if /i "%choice%"=="y" goto continue
if /i "%choice%"=="n" goto end
color 0A echo Invalid choice.
goto begin

:continue
echo Continuing...

git add .
git commit -m "feat: add Stylized Scene"

color 2F & echo Start upload to github.......
git push origin main


:end
echo Exiting...


color 1F & echo Press any key to exit..
pause

