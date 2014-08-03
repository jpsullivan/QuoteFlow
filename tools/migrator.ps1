[CmdletBinding()]
param(
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$PSScriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition

Write-Output $PSScriptRoot

#$FluentMigratorExePath = Join-Path $PSScriptRoot \ssl.pfx
$FluentMigratorExePath = (dir $$PSScriptRoot\..\..\packages\FluentMigrator.Tools.*\tools\AnyCPU\40\Migrate.exe | select -first 1 -expand FullName)
$QuoteFlowDLLPath = (dir $$PSScriptRoot\..\..\QuoteFlow\bin\QuoteFlow.dll | select -first 1 -expand FullName)

Write-Output $FluentMigratorExePath
Write-Output $QuoteFlowDLLPath

& $FluentMigratorExePath /connection "Data Source=(local);Initial Catalog=QuoteFlow;Integrated Security=SSPI;" /provider sqlserver2012 /assembly $QuoteFlowDLLPath
