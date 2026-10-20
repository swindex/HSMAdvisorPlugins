# Tool fields `Hsm`/`Hss` renamed to `Chip_Thinning`/`HSM`

HSMAdvisor 3.2.1 renamed the `Tool`/`Cut` properties to match what they mean:

| Old `ToolField` | New `ToolField` | Meaning | Stored as (XML/JSON) |
|---|---|---|---|
| `Hsm` | `Chip_Thinning` | Radial chip thinning compensation | `chip_thinning` (legacy `hsm`) |
| `Hss` | `HSM` | High speed machining | `high_speed_machining` (legacy `hss`) |

The old docs described `Hsm` as "High speed machining" and `Hss` as "High speed steel"; both were wrong.

## Impact
- ImportCsvTools resolves `ToolField` by reflection, so saved mappings that use `"ToolField": "Hsm"` or
  `"Hss"` now report "Tool field ... was not found on Tool" for that column. Change them to
  `Chip_Thinning` / `HSM`. The shipped `HSMAdvisor Native CSV Format.json` (HSMAdvisor repo) is updated.
- No ImportCsvTools or ExchangeHSMWorks code changes. `HSMadvisorDlls/` refreshed from the HSMAdvisor 3.2.1 build.

## Changes
- `TOOL_FIELD_REFERENCE.md`, `ImportCsvTools/CSV_Tool_Library_Import_Mapping.md`, `ImportCsvTools/README.md`.
- ImportCsvTools version 1.2.0 → 1.2.1.
