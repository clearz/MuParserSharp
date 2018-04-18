@echo off & setlocal EnableDelayedExpansion
set row=
for /F "delims=" %%j in (test.txt) do (
  if  defined row echo."|"!row!"|">> newfile.txt
  set row=%%j
)