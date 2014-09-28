@IF EXIST "%~dp0\node.exe" (
  "%~dp0\node.exe"  "%~dp0\.\node_modules\gulp\bin\gulp" %* --type dev
) ELSE (
  node  "%~dp0\.\node_modules\gulp\bin\gulp" %* --type dev
)
