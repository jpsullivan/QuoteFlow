[CmdletBinding()]
param(
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$PSScriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition

Write-Output $PSScriptRoot

$GrammarRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$GrammarFilePath = Join-Path $GrammarRoot "..\..\QuoteFlow.Core\Jql\Parser\Antlr\Jql.g"
$GrammarFilePath = Convert-Path $GrammarFilePath
$AntlrGenPath = Join-Path $PSScriptRoot "..\..\QuoteFlow.Core\Jql\AntlrGen"
$AntlrGenPath = Convert-Path $AntlrGenPath

Write-Output "Grammar File Path: " $GrammarFilePath
Write-Output "AntlrGen Path: " $AntlrGenPath

& "./Antlr3.exe" $GrammarFilePath -o $AntlrGenPath
