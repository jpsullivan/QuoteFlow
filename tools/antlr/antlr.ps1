[CmdletBinding()]
param(
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$PSScriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition

Write-Output $PSScriptRoot

$GrammarFilePath = (dir $$PSScriptRoot\..\..\..\QuoteFlow.Core\Jql\Parser\Antlr\Jql.g | select -first 1 -expand FullName)
$AntlrGenPath = (dir $$PSScriptRoot\..\..\..\QuoteFlow.Core\Jql\AntlrGen | select -first 1 -expand FullName)

Write-Output $GrammarFilePath
Write-Output $AntlrGenPath

& ./Antlr3.exe $GrammarFilePath -o $AntlrGenPath
