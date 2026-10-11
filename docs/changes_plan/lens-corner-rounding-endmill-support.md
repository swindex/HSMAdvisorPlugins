# Plan: Lens and Corner-Rounding Endmill Import/Export (ExchangeHSMWorks)

**Status: Implemented 2026-10-10** - see [`docs/changes/2026-10-10-lens-corner-rounding-endmills.md`](../changes/2026-10-10-lens-corner-rounding-endmills.md) for the delivered behavior. Deviations from this plan, per user direction: `bull nose end mill` import was left untouched (it was already handled as SolidEndMill + Corner_rad); only its export gained the corner-radius-aware type selection. `circle segment barrel`/`oval` remain preserved-unknown (Aux_data round-trip).

## Background

HSMAdvisor already defines dedicated tool types in `HSMAdvisorDatabase/ToolDatabase/Enums.cs`:

- `ToolTypes.CornerRounding = 31` - corner-rounding mills; calculators treat the cutting profile as a torus: effective diameter = `Diameter + 2 * Corner_Rad` (`Calc_Mill.IsCornerRoundingMill`, `CornerRounding.EffectiveDiameter`).
- `ToolTypes.LensEndMill = 8` - lens (circle-segment) mills; geometry modeled with `Corner_Rad` as the profile arc radius and `Diameter` as the max cutting diameter (`Calc_Mill.CalculateLensEffectiveDiameter`, `CalculateLensFluteLength`, subtype `ToolSubTypeNames.lensmill`).

Fusion 360 / HSMWorks `.hsmlib` uses these tool type strings:

- Corner rounding family: `bull nose end mill`, `radius mill`
- Lens / circle segment family: `circle segment lens`, `circle segment barrel`, `circle segment oval` (newer Fusion schema; geometry stored on `<body>` via `diameter`, `corner-radius`, sometimes `tip-diameter`/`tip-length`)

## Current Gaps in ExchangeHSMWorks

1. **Import** (`Converter.ToTool`, `IsKnownHSMWorksType`):
   - `bull nose end mill` / `radius mill` import as plain `SolidEndMill`; HSMAdvisor's specialized `CornerRounding` calculations are never engaged.
   - `circle segment lens` (barrel/oval) is unknown -> imported with `Tool_type_id = 0`, no feed/speed presets, even though it is a cutting tool.
2. **Export** (`Converter.FromTool` new-tool branch):
   - `SolidEndMill` with `Corner_rad > 0` writes back as `"flat end mill"`, losing corner-rounding semantics in Fusion.
   - No mapping for HSMAdvisor `CornerRounding` or `LensEndMill` tool types at all (they would export with an empty `type`).

## Proposed Mapping

### Import (hsmlib -> HSMAdvisor)

| hsmlib type | HSMAdvisor Tool_type_id | Fields |
|---|---|---|
| `bull nose end mill` | `CornerRounding` | `Diameter`, `Corner_rad` (body `corner-radius`) |
| `radius mill` | `CornerRounding` | same; radius mills have `2*corner-radius > diameter` |
| `circle segment lens` | `LensEndMill` | `Diameter` = max cutting dia, `Corner_rad` = profile arc radius (`corner-radius`) |
| `circle segment barrel` / `circle segment oval` | out of scope initially (keep existing unknown-preserving Aux_data behavior) unless a fixture shows otherwise |

All three recognized types must be added to `IsKnownHSMWorksType()` so presets/diagnostics behave like other cutting tools. Round-trip via `Aux_data` remains unchanged for unmodified imported tools.

### Export (HSMAdvisor -> hsmlib)

New-tool branch (`FromTool`, when no Aux_data or type changed):

- `CornerRounding` -> `type="bull nose end mill"`, body `corner-radius = Corner_rad`.
- `LensEndMill` -> `type="circle segment lens"`, body `diameter = Diameter`, `corner-radius = Corner_rad`.
- `SolidEndMill` with `Corner_rad > 0` -> `type="bull nose end mill"` (so user-created HSMAdvisor tools with corner radius keep that geometry in Fusion); flat endmills (`Corner_rad == 0`) unchanged.

Existing-tool branch (`ApplyTypeSpecificChanges`): when `Corner_rad`, `Diameter`, or type changed on imported CornerRounding/LensEndMill/SolidEndMill-with-radius tools, sync body `corner-radius` (already handled generically) and re-evaluate the `type` string where needed.

## Implementation Steps

1. `ExchangeHSMWorks/Converter.cs`:
   - Add `"circle segment lens"` (+ barrel/oval) to `IsKnownHSMWorksType`.
   - In `ToTool()` switch: separate handling for `bull nose end mill`/`radius mill` -> `CornerRounding`; add `circle segment lens` -> `LensEndMill`.
   - In `FromTool()` type-generation switch: add `CornerRounding`, `LensEndMill` cases; adjust `SolidEndMill` case to emit `"bull nose end mill"` when `srcTool.Corner_rad > 0`.
   - Add corresponding sync cases in `ApplyTypeSpecificChanges()`.
2. Schema: verify `Schema.cs`/`XMLSchema.xsd` already cover the needed `<body>` attributes (`corner-radius`, `tip-diameter`, `tip-length`, `taper-angle2`) for circle-segment entries; extend only if a real-world sample requires attributes missing from the XSD.
3. Tests (`ExchangeHSMWorks.Tests/SimpleConverterTest.cs`):
   - New fixture file(s) with `bull nose end mill`, `radius mill`, and `circle segment lens` tools (or reuse `Milling Tools (Inch).hsmlib` which already contains bull nose + radius mills).
   - Import assertions: correct `Tool_type_id`, `Corner_rad` values.
   - Round-trip test with `Aux_data = null` mirroring `TestTaperAngleRoundTripNoAuxData`: corner-rounded tool exports as `bull nose end mill` with preserved `corner-radius`; lens tool exports as `circle segment lens`.
4. Docs: update `ExchangeHSMWorks/README.md` mapping table; after implementation, write `docs/changes/<YYYY-MM-DD>-lens-corner-rounding-endmills.md`.

## Open Questions

- Whether `bull nose end mill` should map to `CornerRounding` vs staying `SolidEndMill` with corner radius (proposal: CornerRounding, since it engages HSMAdvisor's dedicated engagement calculators; `FlatEndMill` path unchanged otherwise).
- Whether barrel/oval circle-segment mills should also be imported as `LensEndMill` approximations or remain preserved-unknown.
