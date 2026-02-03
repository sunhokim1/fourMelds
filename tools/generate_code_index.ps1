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

$baseRaw = "https://raw.githubusercontent.com/$Owner/$Repo/$Branch/"

# Header
@"
# CODE_INDEX.md
### fourMelds – Runtime Code Index (auto-generated)

> Do not edit manually. Run `tools/generate_code_index.ps1` to regenerate.

- Runtime root: `$RuntimePath`
- Raw base: $baseRaw

---

## Runtime (.cs)

"@ | Out-File $OutFile -Encoding utf8

# Collect .cs files
if (!(Test-Path $RuntimePath)) {
  throw "RuntimePath not found: $RuntimePath"
}

$files = Get-ChildItem $RuntimePath -Recurse -Filter *.cs | Sort-Object FullName

foreach ($f in $files) {
  $rel = NormalizePath($f.FullName.Substring((Get-Location).Path.Length + 1))
  $raw = $baseRaw + $rel
  "- $rel`n  $raw`n" | Out-File $OutFile -Append -Encoding utf8
}

Write-Host "Generated $OutFile with $($files.Count) files."