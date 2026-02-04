Param(
  [string]$RuntimePath = "Assets/_Project/01_Scripts/Runtime",
  [string]$OutFile = "SYMBOL_INDEX.md",
  [string]$Branch = "main",
  [string]$Owner = "sunhokim1",
  [string]$Repo = "fourMelds"
)

$ErrorActionPreference = "Stop"

function NormalizePath([string]$p) { return $p.Replace("\", "/") }

$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$RuntimeAbs = Join-Path $RepoRoot $RuntimePath
if (!(Test-Path $RuntimeAbs)) { throw "RuntimePath not found: $RuntimeAbs" }

$OutAbs = Join-Path $RepoRoot $OutFile
$baseRaw = "https://raw.githubusercontent.com/$Owner/$Repo/$Branch/"

# Helpers: quick parse
function Get-NamespaceName([string[]]$lines) {
  foreach ($ln in $lines) {
    if ($ln -match '^\s*namespace\s+([A-Za-z0-9_.]+)') { return $Matches[1] }
  }
  return "(global)"
}

function Get-TypeDecls([string[]]$lines) {
  $types = New-Object System.Collections.Generic.List[string]
  foreach ($ln in $lines) {
    # class/struct/interface/enum/record (C#9 compatible)
    if ($ln -match '^\s*(public|internal|private|protected)?\s*(sealed\s+|static\s+|abstract\s+)?(partial\s+)?(class|struct|interface|enum|record)\s+([A-Za-z0-9_]+)') {
      $kind = $Matches[4]
      $name = $Matches[5]
      $types.Add("public $kind $name")
    }
  }
  if ($types.Count -eq 0) { $types.Add("(none)") }
  return $types
}

function Get-PublicMembers([string[]]$lines) {
  $methods = New-Object System.Collections.Generic.List[string]
  $props   = New-Object System.Collections.Generic.List[string]

  foreach ($ln in $lines) {
    # property (auto or block) best-effort
    if ($ln -match '^\s*public\s+([A-Za-z0-9_<>\[\]\?\.]+)\s+([A-Za-z0-9_]+)\s*\{\s*get') {
      $props.Add("public $($Matches[1]) $($Matches[2]) { ... }")
      continue
    }

    # method signature best-effort (skip constructors by allowing any name)
    if ($ln -match '^\s*public\s+([A-Za-z0-9_<>\[\]\?\.]+)\s+([A-Za-z0-9_]+)\s*\(([^)]*)\)') {
      $ret = $Matches[1]
      $name = $Matches[2]
      $args = $Matches[3].Trim()
      $methods.Add("public $ret $name($args)")
      continue
    }
  }

  if ($methods.Count -eq 0) { $methods.Add("(none)") }
  if ($props.Count   -eq 0) { $props.Add("(none)") }

  return @{ Methods = $methods; Props = $props }
}

# Collect files
$files = Get-ChildItem $RuntimeAbs -Recurse -File -Filter *.cs | Sort-Object FullName

# Header lines
$linesOut = New-Object System.Collections.Generic.List[string]
$linesOut.Add("# SYMBOL_INDEX.md")
$linesOut.Add("### fourMelds Symbol Index (auto-generated)")
$linesOut.Add("")
$linesOut.Add("> Do not edit manually. Run `tools/generate_symbol_index.ps1` to regenerate.")
$linesOut.Add("")
$linesOut.Add("- Runtime root: $RuntimePath")
$linesOut.Add("- Raw base: $baseRaw")
$linesOut.Add("")
$linesOut.Add("---")
$linesOut.Add("This file is a lightweight index for:")
$linesOut.Add("- namespaces")
$linesOut.Add("- class/struct/interface/enum declarations")
$linesOut.Add("- method/property signatures (best-effort)")
$linesOut.Add("---")
$linesOut.Add("")

foreach ($f in $files) {
  $rel = NormalizePath($f.FullName.Substring($RepoRoot.Path.Length + 1))
  $raw = $baseRaw + $rel

  # Read file (safe)
  $content = Get-Content -LiteralPath $f.FullName -ErrorAction Stop

  $ns = Get-NamespaceName $content
  $types = Get-TypeDecls $content
  $members = Get-PublicMembers $content

  $linesOut.Add("## $rel")
  $linesOut.Add("- Namespace: $ns")
  $linesOut.Add("- Raw: $raw")
  $linesOut.Add("")
  $linesOut.Add("### Types")
  foreach ($t in $types) { $linesOut.Add("- $t") }
  $linesOut.Add("")
  $linesOut.Add("### Members (methods)")
  foreach ($m in $members.Methods) { $linesOut.Add("- $m") }
  $linesOut.Add("")
  $linesOut.Add("### Members (properties)")
  foreach ($p in $members.Props) { $linesOut.Add("- $p") }
  $linesOut.Add("")
  $linesOut.Add("---")
  $linesOut.Add("")
}

# Write UTF-8 (NO BOM)
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllLines($OutAbs, $linesOut, $utf8NoBom)

Write-Host "Generated $OutAbs with $($files.Count) files."

