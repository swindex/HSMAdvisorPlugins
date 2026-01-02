# CSV Tool Library Import Mapping Rules for HSMAdvisor

This document outlines the mapping rules for importing CSV tool libraries into the HSMAdvisor database using the `ToolDatabase.Tool` class fields.

## Overview

The CSV import process uses a JSON file to define how CSV columns map to Tool properties.

**Required Fields:** Tool_type_id, Tool_material_id, Coating_id, Diameter - These must be mapped for successful import.

## CsvMappingConfig Structure

```csharp
{
  "LibraryName": "My Tool Library",
  "AllowInvalidToolImport": false,
  "CsvInputUnits": "in",
  "Mappings": [
    {
      "CsvColumn": "Tool Diameter",
      "ToolField": "Diameter",
      "ValueMap": {},
      "DefaultValue": "",
      "EnumType": "",
      "Expression": ""
    }
  ]
}
```

## Tool Fields and Mapping Rules

The following table lists all public fields from `ToolDatabase.Tool`, their types, and mapping guidelines:

| Field | Type | Required | Description | Possible Values (Enums) | Example Mapping |
|-------|------|----------|-------------|-------------------------|-----------------|
| Tool_material_id | ToolMaterials | Yes | Tool material | Unknown, HSS, HSCobalt, Carbide, PCD, Ceramic | `{"CsvColumn": "Material", "ToolField": "Tool_material_id", "EnumType": "ToolMaterials"}` |
| Tool_type_id | ToolTypes | Yes | Tool type | Unknown, SolidEndMill, SolidBallMill, HPEndMill, IndexedEndMill, IndexedBallMill, IndexedFaceMill, JobberTwistDrill, SpotDrill, CounterSink, Reamer, HighFeedMill, ParabolicDrill, StraightFluteDrill, SpadeDrill, ThreadMill, HelicalIndexedEndMill, VbitEngraver, TurningProfiling, TurningFacing, TurningGrooving, BoringBar, Tap, FormingTap, Counterbore, ChamferMill, IndexedDrill, BoringHead, WoodRuff | `{"CsvColumn": "Tool Type", "ToolField": "Tool_type_id", "EnumType": "ToolTypes"}` |
| Coating_id | ToolCoatings | Yes | Tool coating | Unknown, None, TiN, TiCN, TiAlN, AlTiN, AlCrN, ZrN, nACRo, nACo | `{"CsvColumn": "Coating", "ToolField": "Coating_id", "EnumType": "ToolCoatings"}` |
| Diameter | double | Yes | Tool diameter | | `{"CsvColumn": "Diameter", "ToolField": "Diameter"}` |
| Maxdeflection_pc | double | No | Maximum deflection percentage | | `{"CsvColumn": "Max Deflection %", "ToolField": "Maxdeflection_pc"}` |
| Maxtorque_pc | double | No | Maximum torque percentage | | `{"CsvColumn": "Max Torque %", "ToolField": "Maxtorque_pc"}` |
| Productivity | int | No | Productivity rating | | `{"CsvColumn": "Productivity", "ToolField": "Productivity"}` |
| Thread_drill_dia | double | No | Thread drill diameter | | `{"CsvColumn": "Thread Drill Dia", "ToolField": "Thread_drill_dia"}` |
| Thread_drill_dia_m | bool | No | Thread drill diameter in mm | | `{"CsvColumn": "Thread Drill Dia MM", "ToolField": "Thread_drill_dia_m"}` |
| Custom_SF | bool | No | Custom surface footage | | `{"CsvColumn": "Custom SF", "ToolField": "Custom_SF"}` |
| Generic | bool | No | Generic tool flag | | `{"CsvColumn": "Generic", "ToolField": "Generic"}` |
| Offset_Diameter | int | No | Offset diameter | | `{"CsvColumn": "Offset Dia", "ToolField": "Offset_Diameter"}` |
| Offset_Length | int | No | Offset length | | `{"CsvColumn": "Offset Len", "ToolField": "Offset_Length"}` |
| Coolant | int | No | Coolant type (integer) | | `{"CsvColumn": "Coolant", "ToolField": "Coolant"}` |
| IsInsert | bool | No | Is insert tool | | `{"CsvColumn": "Is Insert", "ToolField": "IsInsert"}` |
| Number | int | No | Tool number | | `{"CsvColumn": "Tool Number", "ToolField": "Number"}` |
| Tip_Diameter | double | No | Tip diameter | | `{"CsvColumn": "Tip Diameter", "ToolField": "Tip_Diameter"}` |
| Stickout | double | No | Tool stickout | | `{"CsvColumn": "Stickout", "ToolField": "Stickout"}` |
| Corner_rad | double | No | Corner radius | | `{"CsvColumn": "Corner Radius", "ToolField": "Corner_rad"}` |
| Instock | int | No | Quantity in stock | | `{"CsvColumn": "In Stock", "ToolField": "Instock"}` |
| Stock_OrderQty | int | No | Stock order quantity | | `{"CsvColumn": "Order Qty", "ToolField": "Stock_OrderQty"}` |
| Stock_MinQty | int | No | Minimum stock quantity | | `{"CsvColumn": "Min Qty", "ToolField": "Stock_MinQty"}` |
| Stock_MaxQty | int | No | Maximum stock quantity | | `{"CsvColumn": "Max Qty", "ToolField": "Stock_MaxQty"}` |
| Stock_OnOrderQty | int | No | Quantity on order | | `{"CsvColumn": "On Order Qty", "ToolField": "Stock_OnOrderQty"}` |
| Stock_Received | int | No | Quantity received | | `{"CsvColumn": "Received", "ToolField": "Stock_Received"}` |
| Stock_Price | double | No | Stock price | | `{"CsvColumn": "Price", "ToolField": "Stock_Price"}` |
| Tool_life | int | No | Tool life | | `{"CsvColumn": "Tool Life", "ToolField": "Tool_life"}` |
| Rating | int | No | Tool rating | | `{"CsvColumn": "Rating", "ToolField": "Rating"}` |
| Shank_Dia | double | No | Shank diameter | | `{"CsvColumn": "Shank Dia", "ToolField": "Shank_Dia"}` |
| Flute_N | int | No | Number of flutes | | `{"CsvColumn": "Flutes", "ToolField": "Flute_N"}` |
| Flute_Len | double | No | Flute length | | `{"CsvColumn": "Flute Length", "ToolField": "Flute_Len"}` |
| Shoulder_Len | double | No | Shoulder length | | `{"CsvColumn": "Shoulder Length", "ToolField": "Shoulder_Len"}` |
| Shoulder_Dia | double | No | Shoulder diameter | | `{"CsvColumn": "Shoulder Dia", "ToolField": "Shoulder_Dia"}` |
| Shoulder_Tapered | bool | No | Shoulder tapered | | `{"CsvColumn": "Shoulder Tapered", "ToolField": "Shoulder_Tapered"}` |
| Helix_angle | double | No | Helix angle | | `{"CsvColumn": "Helix Angle", "ToolField": "Helix_angle"}` |
| Leadangle | double | No | Lead angle | | `{"CsvColumn": "Lead Angle", "ToolField": "Leadangle"}` |
| Toolangle_mode | ToolAngleModes | No | Tool angle mode | Lead, Taper, Tip | `{"CsvColumn": "Angle Mode", "ToolField": "Toolangle_mode", "EnumType": "ToolAngleModes"}` |
| Toolangle | double | No | Tool angle (convenience) | | `{"CsvColumn": "Tool Angle", "ToolField": "Toolangle"}` |
| Rampmilling | bool | No | Ramp milling capable | | `{"CsvColumn": "Ramp Milling", "ToolField": "Rampmilling"}` |
| Ramp | double | No | Ramp angle | | `{"CsvColumn": "Ramp", "ToolField": "Ramp"}` |
| Peck | double | No | Peck depth | | `{"CsvColumn": "Peck", "ToolField": "Peck"}` |
| Doc | double | No | Depth of cut | | `{"CsvColumn": "DOC", "ToolField": "Doc"}` |
| Woc | double | No | Width of cut | | `{"CsvColumn": "WOC", "ToolField": "Woc"}` |
| Sfm_adj | double | No | SFM adjustment | | `{"CsvColumn": "SFM Adj", "ToolField": "Sfm_adj"}` |
| Ipt_adj | double | No | IPT adjustment | | `{"CsvColumn": "IPT Adj", "ToolField": "Ipt_adj"}` |
| Tool_maxrpm | int | No | Maximum RPM | | `{"CsvColumn": "Max RPM", "ToolField": "Tool_maxrpm"}` |
| Depth | double | No | Cutting depth | | `{"CsvColumn": "Depth", "ToolField": "Depth"}` |
| Circle_comp | bool | No | Circle compensation | | `{"CsvColumn": "Circle Comp", "ToolField": "Circle_comp"}` |
| Hsm | bool | No | High speed machining | | `{"CsvColumn": "HSM", "ToolField": "Hsm"}` |
| Hss | bool | No | High speed steel | | `{"CsvColumn": "HSS", "ToolField": "Hss"}` |
| Circle_dia | double | No | Circle diameter | | `{"CsvColumn": "Circle Dia", "ToolField": "Circle_dia"}` |
| Circle_external | int | No | Circle external flag | | `{"CsvColumn": "Circle External", "ToolField": "Circle_external"}` |
| Thread_pitch | double | No | Thread pitch | | `{"CsvColumn": "Thread Pitch", "ToolField": "Thread_pitch"}` |
| Pilot_Hole | double | No | Pilot hole diameter | | `{"CsvColumn": "Pilot Hole", "ToolField": "Pilot_Hole"}` |
| Diameter_m | bool | No | Diameter in mm | | `{"CsvColumn": "Diameter MM", "ToolField": "Diameter_m"}` |
| Tip_Diameter_m | bool | No | Tip diameter in mm | | `{"CsvColumn": "Tip Diameter MM", "ToolField": "Tip_Diameter_m"}` |
| Shank_Dia_m | bool | No | Shank diameter in mm | | `{"CsvColumn": "Shank Dia MM", "ToolField": "Shank_Dia_m"}` |
| Flute_len_m | bool | No | Flute length in mm | | `{"CsvColumn": "Flute Len MM", "ToolField": "Flute_len_m"}` |
| Stickout_m | bool | No | Stickout in mm | | `{"CsvColumn": "Stickout MM", "ToolField": "Stickout_m"}` |
| Corner_rad_m | bool | No | Corner radius in mm | | `{"CsvColumn": "Corner Rad MM", "ToolField": "Corner_rad_m"}` |
| Doc_m | bool | No | DOC in mm | | `{"CsvColumn": "DOC MM", "ToolField": "Doc_m"}` |
| Woc_m | bool | No | WOC in mm | | `{"CsvColumn": "WOC MM", "ToolField": "Woc_m"}` |
| Ipt_m | bool | No | IPT in mm | | `{"CsvColumn": "IPT MM", "ToolField": "Ipt_m"}` |
| Sfm_m | bool | No | SFM in mm | | `{"CsvColumn": "SFM MM", "ToolField": "Sfm_m"}` |
| Feed_m | bool | No | Feed in mm | | `{"CsvColumn": "Feed MM", "ToolField": "Feed_m"}` |
| Input_units_m | bool | No | Input units in mm | | `{"CsvColumn": "Input Units MM", "ToolField": "Input_units_m"}` |
| Result_units_m | bool | No | Result units in mm | | `{"CsvColumn": "Result Units MM", "ToolField": "Result_units_m"}` |
| Circle_dia_m | bool | No | Circle diameter in mm | | `{"CsvColumn": "Circle Dia MM", "ToolField": "Circle_dia_m"}` |
| Depth_m | bool | No | Depth in mm | | `{"CsvColumn": "Depth MM", "ToolField": "Depth_m"}` |
| Shoulder_len_m | bool | No | Shoulder length in mm | | `{"CsvColumn": "Shoulder Len MM", "ToolField": "Shoulder_len_m"}` |
| Peck_m | bool | No | Peck in mm | | `{"CsvColumn": "Peck MM", "ToolField": "Peck_m"}` |
| Thread_pitch_m | bool | No | Thread pitch in mm | | `{"CsvColumn": "Thread Pitch MM", "ToolField": "Thread_pitch_m"}` |
| Shoulder_Dia_m | bool | No | Shoulder diameter in mm | | `{"CsvColumn": "Shoulder Dia MM", "ToolField": "Shoulder_Dia_m"}` |
| Pilot_Hole_m | bool | No | Pilot hole in mm | | `{"CsvColumn": "Pilot Hole MM", "ToolField": "Pilot_Hole_m"}` |

## Enum Handling

For enum fields, the CSV column should contain the enum name as a string (e.g., "Carbide", "Lead"). The `EnumType` field in `CsvMapping` specifies the enum type for validation and conversion.

If your CSV uses different values, use `ValueMap` to translate them:

```json
{
  "CsvColumn": "Material",
  "ToolField": "Tool_material_id",
  "ValueMap": {
    "HSS Steel": "HSS",
    "Cobalt": "HSCobalt",
    "Carb": "Carbide"
  },
  "EnumType": "ToolMaterials"
}
```

## Notes

- Boolean fields expect "true"/"false", "1"/"0", or "yes"/"no" in CSV
- Numeric fields are parsed as double or int as appropriate
- The `Toolangle` field is a convenience property that depends on `Toolangle_mode`; map it separately if needed
- `CsvInputUnits` affects how dimensional values are interpreted (default "in" for inches)
- Use `DefaultValue` for missing CSV columns
- `Expression` can be used for calculated mappings if needed
- The `Input_units_m` field can be used to import an individual tool as a metric tool.
    you can use an expression to infer the units from a column value.
