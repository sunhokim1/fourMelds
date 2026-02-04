Param(
  [string]$RuntimePath = "Assets/_Project/01_Scripts/Runtime",
  [string]$OutFile = "CODE_INDEX.md",
  [string]$Branch = "main",
  [string]$Owner = "sunhokim1",
  [string]$Repo = "fourMelds"
)

$ErrorActionPreference = "Stop"

function NormalizePath([string]$p) { return $p.Replace("\", "/") }

# Repo root = one level above /tools
$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")

# Absolute paths
$RuntimeAbs = Join-Path $RepoRoot $RuntimePath
if (!(Test-Path $RuntimeAbs)) { throw "RuntimePath not found: $RuntimeAbs" }

$OutAbs = Join-Path $RepoRoot $OutFile
$baseRaw = "https://raw.githubusercontent.com/$Owner/$Repo/$Branch/"

# Collect files
$files = Get-ChildItem $RuntimeAbs -Recurse -File -Filter *.cs | Sort-Object FullName

# Build lines (개행을 '줄 단위'로 확실히 보장)
$lines = New-Object System.Collections.Generic.List[string]
$lines.Add("# CODE_INDEX.md")
$lines.Add("### fourMelds Runtime Code Index (auto-generated)")
$lines.Add("")
$lines.Add("> Do not edit manually. Run `tools/generate_code_index.ps1` to regenerate.")
$lines.Add("")
$lines.Add("- Runtime root: $RuntimePath")
$lines.Add("- Raw base: $baseRaw")
$lines.Add("")
$lines.Add("---")
$lines.Add("")
$lines.Add("## Runtime (.cs)")
$lines.Add("")

foreach ($f in $files) {
  $rel = NormalizePath($f.FullName.Substring($RepoRoot.Path.Length + 1))
  $raw = $baseRaw + $rel
  $lines.Add("- $rel")
  $lines.Add("  $raw")
  $lines.Add("")
}

# Write UTF-8 (NO BOM) + 정상 개행
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllLines($OutAbs, $lines, $utf8NoBom)

Write-Host "Generated $OutAbs with $($files.Count) files."
