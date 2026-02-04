Param(
  [string]$RuntimePath = "Assets/_Project/01_Scripts/Runtime",
  [string]$OutFile = "CODE_INDEX.md",
  [string]$Branch = "main",
  [string]$Owner = "sunhokim1",
  [string]$Repo = "fourMelds"
)

$ErrorActionPreference = "Stop"

function NormalizePath([string]$p) {
  return $p.Replace("\", "/")
}

# Repo root = one level above /tools
$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")

# Absolute runtime path (always from repo root)
$RuntimeAbs = Join-Path $RepoRoot $RuntimePath
if (!(Test-Path $RuntimeAbs)) {
  throw "RuntimePath not found: $RuntimeAbs"
}

# Absolute output file path (always to repo root)
$OutAbs = Join-Path $RepoRoot $OutFile

$baseRaw = "https://raw.githubusercontent.com/$Owner/$Repo/$Branch/"

@"
# CODE_INDEX.md
### fourMelds – Runtime Code Index (auto-generated)

> Do not edit manually. Run `tools/generate_code_index.ps1` to regenerate.

- Runtime root: $RuntimePath
- Runtime abs : $(NormalizePath($RuntimeAbs))
- Out file    : $(NormalizePath($OutAbs))
- Raw base    : $baseRaw

---

## Runtime (.cs)

"@ | Out-File $OutAbs -Encoding utf8

$files = Get-ChildItem $RuntimeAbs -Recurse -File -Filter *.cs | Sort-Object FullName

foreach ($f in $files) {
  $rel = NormalizePath($f.FullName.Substring($RepoRoot.Path.Length + 1))
  $raw = $baseRaw + $rel
  "- $rel`n  $raw`n" | Out-File $OutAbs -Append -Encoding utf8
}

Write-Host "Generated $OutAbs with $($files.Count) files."
