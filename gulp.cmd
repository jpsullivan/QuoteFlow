@IF EXIST "%~dp0\node.exe" (
  "%~dp0\node.exe"  "%~dp0\.\node_modules\gulp\bin\gulp" %* --type production
) ELSE (
  node  "%~dp0\.\node_modules\gulp\bin\gulp" %* --type production
)
