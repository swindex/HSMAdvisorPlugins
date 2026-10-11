# Lens and Corner-Rounding Endmill Import/Export (ExchangeHSMWorks)

Date: 2026-10-10
Project: `ExchangeHSMWorks`

## Summary

Added support for lens (`circle segment lens`) and corner-rounding (`radius mill`) endmills in the HSMWorks (.hsmlib) converter, mapping them to HSMAdvisor's dedicated `LensEndMill` and `CornerRounding` tool types so their feed/speed calculations engage the specialized engagement models. `bull nose end mill` handling was intentionally left unchanged (it already imports as `SolidEndMill` with `Corner_rad`).

## Behavior changes

### Import (`Converter.ToTool`)

- `radius mill` now maps to `Enums.ToolTypes.CornerRounding` (previously fell through to the plain `SolidEndMill` default branch). Body `diameter` is the pilot/core cutting width and `corner-radius` maps to `Corner_rad` - the same reference HSMAdvisor's corner-rounding calculators use (`ApplyCornerRoundingGeometryDefaults`, `CornerRounding.EffectiveDiameter`).
- `circle segment lens` (Fusion circle-segment milling tool) now maps to `Enums.ToolTypes.LensEndMill`. Body `diameter` is the maximum cutting diameter and `corner-radius` maps to `Corner_rad` as the profile arc radius, matching `Calc_Mill.CalculateLensEffectiveDiameter`/`CalculateLensFluteLength`. Previously this type was "unknown" (`Tool_type_id = 0`, no presets).
- Both types were added to `Converter.IsKnownHSMWorksType()` so geometry validation and preset handling treat them as cutting tools.

### Export (`Converter.FromTool`)

New-type branch (no Aux_data, or tool type changed):

- `CornerRounding` exports as `type="radius mill"`; body `diameter` and `corner-radius` round-trip unchanged via the generic `SetIfChanged` sync.
- `LensEndMill` exports as `type="circle segment lens"`.
- `SolidEndMill` with `Corner_rad > 0` now exports as `"bull nose end mill"` instead of `"flat end mill"`, preserving corner geometry in Fusion for tools created in HSMAdvisor; flat endmills are unchanged. Dovetail handling (negative taper) takes precedence as before.

Existing-type branch (`ApplyTypeSpecificChanges`):

- Corner-radius edits can move a tool between shapes: adding a radius to an imported flat end mill promotes it to `bull nose end mill`; clearing the radius demotes a bull nose back to `flat end mill`; a lens whose arc radius is cleared demotes to `flat end mill`. Geometry values themselves are synced by the existing common `SetIfChanged` calls.

### Preserved-unknown types

Unmapped/non-cutting HSMWorks types (probes, and circle segment `barrel`/`oval` variants which have no HSMAdvisor equivalent yet) continue to import with `Tool_type_id = 0` and round-trip losslessly via the full source XML kept in `Aux_data`. See swindex/HSMAdvisorPlugins#6.

## Files changed

- `ExchangeHSMWorks/Converter.cs` - `IsKnownHSMWorksType`, `ToTool` switch (`radius mill`, `circle segment lens`), `FromTool` new-type switch (`CornerRounding`, `LensEndMill`, corner-radius-aware `SolidEndMill`), `ApplyTypeSpecificChanges` shape-promotion rules.
- `ExchangeHSMWorks.Tests/test-data/LensAndCornerRounding.hsmlib` - new fixture containing a `circle segment lens`, a `radius mill`, and a `bull nose end mill`.
- `ExchangeHSMWorks.Tests/SimpleConverterTest.cs` - new `TestLensAndCornerRoundingHandling` test covering known-type detection, import type/geometry assertions, lossless Aux_data round-trip, and fresh (no-Aux_data) re-export to the original hsmlib type.
- `ExchangeHSMWorks/README.md` - updated milling type-mapping table and test-data list.

## Verification

MSBuild Release build of `HSMAdvisor-Plugins.sln` succeeds; all ExchangeHSMWorks tests pass, including:

```
Testing lens and corner-rounding handling... PASS (lens -> LensEndMill, radius mill -> CornerRounding, bull nose unchanged)
```

Existing suites (lossless round-trip across 24k tools, taper-angle, reamer/probe) remain green.
