Param(
  [string]$RuntimePath = "Assets/_Project/01_Scripts/Runtime",
  [string]$OutFile = "SYMBOL_INDEX.md",
  [string]$Branch = "main",
  [string]$Owner = "sunhokim1",
  [string]$Repo = "fourMelds"
)

$ErrorActionPreference = "Stop"

function NormalizePath([string]$p) { $p.Replace("\","/") }

if (!(Test-Path $RuntimePath)) {
  throw "RuntimePath not found: $RuntimePath"
}

$baseRaw = "https://raw.githubusercontent.com/$Owner/$Repo/$Branch/"

@"
# SYMBOL_INDEX.md
### fourMelds – Symbol Index (auto-generated)

> Do not edit manually. Run `tools/generate_symbol_index.ps1` to regenerate.

- Runtime root: `$RuntimePath`
- Raw base: $baseRaw

---

This file is a lightweight index for:
- namespaces
- class/struct/interface/enum declarations
- method/property signatures (best-effort)

"@ | Out-File $OutFile -Encoding utf8

$files = Get-ChildItem $RuntimePath -Recurse -Filter *.cs | Sort-Object FullName

foreach ($f in $files) {
  $rel = NormalizePath($f.FullName.Substring((Get-Location).Path.Length + 1))
  $raw = $baseRaw + $rel

  # Read all lines safely
  $lines = Get-Content $f.FullName -ErrorAction Stop

  # Best-effort namespace
  $ns = ($lines | Where-Object { $_ -match '^\s*namespace\s+([A-Za-z0-9_.]+)\s*' } | Select-Object -First 1)
  if ($ns -match '^\s*namespace\s+([A-Za-z0-9_.]+)\s*') {
    $nsName = $Matches[1]
  } else {
    $nsName = "(global)"
  }

  # Types (class/struct/interface/enum)
  $typeLines = $lines | Where-Object {
    $_ -match '^\s*(public|internal|protected|private)?\s*(abstract|static|sealed|partial)?\s*(class|struct|interface|enum)\s+[A-Za-z0-9_]+'
  }

  # Members (very lightweight heuristics; avoids full parsing)
  $memberLines = $lines | Where-Object {
    # methods (avoid control keywords)
    ($_ -match '^\s*(public|internal|protected|private)\s+.*\)\s*(\{|=>)') -and
    ($_ -notmatch '^\s*(if|for|foreach|while|switch|catch|using)\b')
  }

  $propLines = $lines | Where-Object {
    # properties with { get; ... } or expression-bodied
    ($_ -match '^\s*(public|internal|protected|private)\s+.*\s+[A-Za-z0-9_]+\s*\{\s*(get;|set;|init;|get\s*=>|set\s*=>)') -or
    ($_ -match '^\s*(public|internal|protected|private)\s+.*\s+[A-Za-z0-9_]+\s*=>')
  }

  # Only write sections that have something meaningful
  if ($typeLines.Count -gt 0 -or $memberLines.Count -gt 0 -or $propLines.Count -gt 0) {
@"
---

## $rel
- Namespace: `$nsName`
- Raw: $raw

### Types
"@ | Out-File $OutFile -Append -Encoding utf8

    if ($typeLines.Count -eq 0) {
      "- (none)`n" | Out-File $OutFile -Append -Encoding utf8
    } else {
      foreach ($t in $typeLines) {
        ("- " + $t.Trim()) | Out-File $OutFile -Append -Encoding utf8
      }
      "" | Out-File $OutFile -Append -Encoding utf8
    }

"### Members (methods)`n" | Out-File $OutFile -Append -Encoding utf8
    if ($memberLines.Count -eq 0) {
      "- (none)`n" | Out-File $OutFile -Append -Encoding utf8
    } else {
      foreach ($m in $memberLines | Select-Object -First 60) {
        ("- " + ($m.Trim() -replace '\s+\{.*$',' { ... }')) | Out-File $OutFile -Append -Encoding utf8
      }
      if ($memberLines.Count -gt 60) { "- ...(truncated)`n" | Out-File $OutFile -Append -Encoding utf8 }
    }

"### Members (properties)`n" | Out-File $OutFile -Append -Encoding utf8
    if ($propLines.Count -eq 0) {
      "- (none)`n" | Out-File $OutFile -Append -Encoding utf8
    } else {
      foreach ($p in $propLines | Select-Object -First 60) {
        ("- " + ($p.Trim() -replace '\s+\{.*$',' { ... }')) | Out-File $OutFile -Append -Encoding utf8
      }
      if ($propLines.Count -gt 60) { "- ...(truncated)`n" | Out-File $OutFile -Append -Encoding utf8 }
    }
  }
}

Write-Host "Generated $OutFile with best-effort symbols."
