using HSMAdvisorPlugin;
using HSMAdvisorDatabase;
using HSMAdvisorDatabase.ToolDataBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace ExchangeHSMWorks
{
    public class Converter : ToolsPluginInterface
    {
        public static Enums.ToolMaterials ToToolMaterial(string materialname)
        {
            switch (materialname)
            {
                case "carbide":
                    return Enums.ToolMaterials.Carbide;
                case "ceramics":
                    return Enums.ToolMaterials.Ceramic;
                case "cobalt":
                    return Enums.ToolMaterials.HSCobalt;
                case "hss":
                default:
                    return Enums.ToolMaterials.HSS;
            }
        }
        public static string FromToolMaterial(Enums.ToolMaterials material_id)
        {
            switch (material_id)
            {
                case Enums.ToolMaterials.Carbide:
                    return "carbide";
                case Enums.ToolMaterials.Ceramic:
                    return "ceramics";
                case Enums.ToolMaterials.HSCobalt:
                    return "cobalt";
                case Enums.ToolMaterials.HSS:
                default:
                    return "hss";
            }
        }
        public static Tool ToTool(toollibraryTool t)
        {
            var unitsMetric = t.unit == "millimeters";

            Tool ret = new Tool(true)
            {
                Tool_type_id = Enums.ToolTypes.SolidEndMill,

                Coating_id = Enums.ToolCoatings.None,
                Tool_material_id = ToToolMaterial(t.material?.name),

                Guid = t.guid,
                Comment = t.description,

                Library = "",

                Brand_name = t.manufacturer,
                Series_name = t.productid,
                Product_link = t.productlink,

                Number = Parse.ToInteger(t.nc?.number),

                Offset_Diameter = Parse.ToInteger(t.nc?.diameteroffset),
                Offset_Length = Parse.ToInteger(t.nc?.lengthoffset),

                Aux_data = Serializer.ToXML(t, "UTF-16"),

                Circle_dia_m = unitsMetric,
                Depth_m = unitsMetric,
                Diameter_m = unitsMetric,
                Corner_rad_m = unitsMetric,
                Doc_m = unitsMetric,
                Feed_m = unitsMetric,
                Flute_len_m = unitsMetric,
                Input_units_m = unitsMetric,
                Ipt_m = unitsMetric,
                Peck_m = unitsMetric,
                Result_units_m = unitsMetric,
                Pilot_Hole_m = unitsMetric,
                Sfm_m = unitsMetric,
                Shank_Dia_m = unitsMetric,
                Shoulder_Dia_m = unitsMetric,
                Shoulder_len_m = unitsMetric,
                Stickout_m = unitsMetric,
                Thread_drill_dia_m = unitsMetric,
                Thread_pitch_m = unitsMetric,
                Woc_m = unitsMetric


            };

            if (t.body != null)
            {
                ret.Diameter = Parse.ToDouble(t.body.diameter);
                ret.Corner_rad = Parse.ToDouble(t.body.cornerradius);
                ret.Stickout = Parse.ToDouble(t.body.bodylength);
                ret.Flute_Len = Parse.ToDouble(t.body.flutelength);
                ret.Shoulder_Len = Parse.ToDouble(t.body.shoulderlength);
                ret.Shank_Dia = Parse.ToDouble(t.body.shaftdiameter);
                ret.Flute_N = Parse.ToInteger(t.body.numberofflutes);
                ret.Helix_angle = -1;
                ret.Toolangle_mode = Enums.ToolAngleModes.Taper;
                ret.Leadangle = Parse.ToDouble(t.body.taperangle);
            }

            //set default values
            ret.Maxdeflection_pc = -1;
            ret.Maxtorque_pc = -1;
            ret.Productivity = -1;

            //override type
            switch (t.type)
            {
                case "flat end mill":
                case "bull nose end mill":
                case "tapered mill":
                case "radius mill":
                case "form mill":
                    //Tool_type_id = Convert.ToInt32(Enums.ToolTypes.SolidEndMill),
                    break;
                case "thread mill":
                    ret.Thread_pitch = Parse.ToDouble(t.body?.threadpitch);
                    ret.Tool_type_id = Enums.ToolTypes.ThreadMill;
                    break;
                case "ball end mill":
                case "lollipop mill":
                    ret.Tool_type_id = Enums.ToolTypes.SolidBallMill;
                    break;
                case "face mill":
                    ret.Tool_type_id = Enums.ToolTypes.IndexedFaceMill;
                    break;
                case "slot mill":
                    ret.Tool_type_id = Enums.ToolTypes.WoodRuff;
                    break;
                case "chamfer mill":
                    if (t.description != null && t.description.ToUpper().Contains("ENGR"))
                        ret.Tool_type_id = Enums.ToolTypes.VbitEngraver;
                    else
                        ret.Tool_type_id = Enums.ToolTypes.ChamferMill;

                    ret.Diameter = Parse.ToDouble(t.body?.tipdiameter);
                    if (ret.Diameter <= 0)
                        if (ret.Diameter_m)
                        {
                            ret.Diameter = 0.0010 * 25.4;
                        }
                        else
                        {
                            ret.Diameter = 0.001;
                        }
                    ret.Diameter = ret.Shank_Dia;
                    ret.Shank_Dia = Parse.ToDouble(t.body?.diameter);
                    ret.Toolangle_mode = Enums.ToolAngleModes.Tip;
                    ret.Leadangle = Parse.ToDouble(t.body?.taperangle);
                    break;

                case "center drill":
                case "drill":
                    ret.Tool_type_id = Enums.ToolTypes.JobberTwistDrill;
                    ret.Toolangle_mode = Enums.ToolAngleModes.Tip;
                    ret.Leadangle = Parse.ToDouble(t.body?.taperangle);

                    ret.Flute_N = 2;
                    break;
                case "spot drill":
                    ret.Tool_type_id = Enums.ToolTypes.SpotDrill;
                    ret.Toolangle_mode = Enums.ToolAngleModes.Tip;
                    ret.Leadangle = Parse.ToDouble(t.body?.taperangle);
                    ret.Flute_N = 2;
                    break;
                case "counter bore":
                    ret.Tool_type_id = Enums.ToolTypes.Counterbore;
                    ret.Toolangle_mode = Enums.ToolAngleModes.Tip;
                    ret.Leadangle = Parse.ToDouble(t.body?.taperangle);
                    break;
                case "counter sink":
                    ret.Tool_type_id = Enums.ToolTypes.CounterSink;
                    ret.Diameter = Parse.ToDouble(t.body?.tipdiameter);
                    if (ret.Diameter <= 0)
                        if (ret.Diameter_m)
                        {
                            ret.Diameter = 0.0010 * 25.4;
                        }
                        else
                        {
                            ret.Diameter = 0.001;
                        }
                    ret.Diameter = ret.Shank_Dia;
                    ret.Toolangle_mode = Enums.ToolAngleModes.Tip;
                    ret.Leadangle = Parse.ToDouble(t.body?.taperangle);
                    break;
                case "tap right hand":
                    ret.Tool_type_id = Enums.ToolTypes.Tap;
                    ret.Thread_pitch = Parse.ToDouble(t.body?.threadpitch);
                    ret.Flute_N = Parse.ToInteger(t.body?.numberoffteeth);
                    break;
                case "tap left hand":
                    ret.Tool_type_id = Enums.ToolTypes.Tap;
                    ret.Thread_pitch = Parse.ToDouble(t.body?.threadpitch);
                    ret.Flute_N = Parse.ToInteger(t.body?.numberoffteeth);
                    break;
                case "boring bar":
                    ret.Tool_type_id = Enums.ToolTypes.BoringHead;

                    break;
                case "turning threading":
                case "turning general":
                    ret.Tool_type_id = Enums.ToolTypes.TurningProfiling;
                    //ret.Thread_pitch = t.body.threadpitch;
                    ret.Shank_Dia = Parse.ToDouble(t.turningholder?.shankheight);
                    ret.Stickout = Parse.ToDouble(t.turningholder?.headlength);
                    ret.Corner_rad = Parse.ToDouble(t.insert?.cornerradius);

                    ret.Flute_N = 1;
                    break;
                case "turning boring":
                    ret.Tool_type_id = Enums.ToolTypes.BoringBar;
                    //ret.Thread_pitch = t.body.threadpitch;
                    ret.Shank_Dia = Parse.ToDouble(t.turningholder?.shankheight);
                    ret.Stickout = Parse.ToDouble(t.turningholder?.headlength);
                    ret.Corner_rad = Parse.ToDouble(t.insert?.cornerradius);

                    ret.Flute_N = 1;
                    break;
                case "turning grooving":
                    ret.Tool_type_id = Enums.ToolTypes.TurningProfiling;

                    //ret.Thread_pitch = t.body.threadpitch;
                    ret.Shank_Dia = Parse.ToDouble(t.turningholder?.shankheight);
                    ret.Stickout = Parse.ToDouble(t.turningholder?.headlength);
                    ret.Corner_rad = Parse.ToDouble(t.insert?.cornerradius);

                    ret.Flute_N = 1;
                    break;
                // These types are not supported in HSMAdvisor yet. Set to endmill by default
                case "dovetail mill":
                    ret.Tool_type_id = Enums.ToolTypes.SolidEndMill;
                    ret.Toolangle_mode = Enums.ToolAngleModes.Taper;
                    // Fusions's dovetail mills specify taperangle as always positive, but in HSMAdvisor it's negative for dovetails
                    ret.Toolangle = (-Parse.ToDouble(t.body?.taperangle));

                    break;
                default:
                    ret.Tool_type_id = 0;
                    break;

            }
            return ret;
        }

        /// <summary>
        /// Provide File Filter for OpenFile dialog
        /// </summary>
        /// <returns></returns>
        public string GetReadFileFilter()
        {
            return "HSMWorks Tool Database Files (*.hsmlib)|*.hsmlib|XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
        }

        /// <summary>
        /// Read HSMWorks Tool Database file
        /// </summary>
        /// <returns></returns>
        public override DataBase ImportTools()
        {
            var FileName = ShowOpenFileDialog();

            if (FileName == null)
            {
                return null;
            }
            //read xml from our source file
            var xml = File.ReadAllText(FileName);

            //use OTB's serializer to read xml into src
            toollibrary src = Serializer.FromXML<toollibrary>(xml, false);

            //Create a new database
            var targetDB = new DataBase();

            var libname = Path.GetFileNameWithoutExtension(FileName);


            //filename is our library name
            targetDB.AddLibrary(libname);

            //Add tools one by one
            src.tool.ForEach(srcTool =>
            {
                var tool = ToTool(srcTool);
                tool.Library = libname;
                targetDB.Tools.Add(tool);

                //add holder if it has one
                if (srcTool.holder != null)
                {
                    var holder = targetDB.Holders.FirstOrDefault(e => e.Comment == srcTool.holder.description && e.Library == tool.Library);
                    if (holder != null)
                        targetDB.Holders.Remove(holder);

                    targetDB.Holders.Add(new Holder()
                    {
                        Library = tool.Library,
                        Units_m = srcTool.unit == "millimeters",
                        Comment = srcTool.holder.description,
                        Brand_name = srcTool.holder.vendor,
                        Series_name = srcTool.holder.productid,
                        Shank_Dia = Parse.ToDouble(srcTool.body?.shaftdiameter)
                    });
                }
            });

            Debug.WriteLine(targetDB.Tools[0].Aux_data);

            var tt = Serializer.FromXML<toollibraryTool>(targetDB.Tools[0].Aux_data, false);

            Debug.WriteLine(Serializer.ToXML(tt));

            //return new database. HSMAdvisor will SAFELY merge it into it's current database
            return targetDB;
        }

        /// <summary>
        /// Export DataBase.
        /// </summary>
        /// <param name="db">Copy of the HSMAdvisor's database that you can dump or save</param>
        /// <returns></returns>
        public override void ExportTools(DataBase src)
        {
            if (src == null)
            {
                throw new Exception("Source DataBase is not specified!");
            }


            var FileName = ShowSaveAsFileDialog();

            if (FileName == null)
            {
                return;
            }
            //read xml from our source file
            //var xml = File.ReadAllText(FileName);

            //Create a new database
            toollibrary targetDB = new toollibrary();


            //Add tools one by one
            src.Tools.ToList().ForEach(srcTool =>
            {
                var tool = FromTool(srcTool);

                targetDB.tool.Add(tool);

                //add holder if it has one
                /*if (srcTool.holder != null)
                {
                    var holder = targetDB.Holders.FirstOrDefault(e => e.Comment == srcTool.holder.description && e.Library == tool.Library);
                    if (holder != null)
                        targetDB.Holders.Remove(holder);

                    targetDB.Holders.Add(new Holder()
                    {
                        Library = tool.Library,
                        Units_m = srcTool.unit == "millimeters",
                        Comment = srcTool.holder.description,
                        Brand_name = srcTool.holder.vendor,
                        Series_name = srcTool.holder.productid,
                        Shank_Dia = Parse.ToDouble(srcTool.body.shaftdiameter)
                    });
                }*/
            });
            File.WriteAllText(FileName, Serializer.ToXML(targetDB, "UTF-16"));
        }

        public static toollibraryTool FromTool(Tool srcTool)
        {
            Tool originalTool = null;
            toollibraryTool ret = new toollibraryTool();
            if (!string.IsNullOrEmpty(srcTool.Aux_data))
            {
                try
                {
                    ret = Serializer.FromXML<toollibraryTool>(srcTool.Aux_data, false);
                    originalTool = ToTool(ret);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            SetIfChanged(originalTool, srcTool.Tool_material_id, originalTool?.Tool_material_id ?? default(Enums.ToolMaterials), value => EnsureMaterial(ret).name = FromToolMaterial(value));
            SetIfChanged(originalTool, srcTool.Guid, originalTool?.Guid, value => ret.guid = value);

            SetIfChanged(originalTool, srcTool.Comment, originalTool?.Comment, value => ret.description = value);


            SetIfChanged(originalTool, srcTool.Brand_name, originalTool?.Brand_name, value => ret.manufacturer = value);
            SetIfChanged(originalTool, srcTool.Series_name, originalTool?.Series_name, value => ret.productid = value);
            SetIfChanged(originalTool, srcTool.Product_link, originalTool?.Product_link, value => ret.productlink = value);


            SetIfChanged(originalTool, srcTool.Number, originalTool?.Number ?? 0, value => EnsureNC(ret).number = Parse.ToString(value));

            SetIfChanged(originalTool, srcTool.Offset_Diameter, originalTool?.Offset_Diameter ?? 0, value => EnsureNC(ret).diameteroffset = Parse.ToString(value));
            SetIfChanged(originalTool, srcTool.Offset_Length, originalTool?.Offset_Length ?? 0, value => EnsureNC(ret).lengthoffset = Parse.ToString(value));

            SetIfChanged(originalTool, srcTool.Input_units_m, originalTool?.Input_units_m ?? false, value => ret.unit = value ? "millimeters" : "inches");

            SetIfChanged(originalTool, srcTool.Diameter, originalTool?.Diameter ?? 0, value => EnsureBody(ret).diameter = Parse.ToString(value));
            SetIfChanged(originalTool, srcTool.Corner_rad, originalTool?.Corner_rad ?? 0, value => EnsureBody(ret).cornerradius = Parse.ToString(value));
            SetIfChanged(originalTool, srcTool.Stickout, originalTool?.Stickout ?? 0, value => EnsureBody(ret).bodylength = Parse.ToString(value));
            SetIfChanged(originalTool, srcTool.Flute_Len, originalTool?.Flute_Len ?? 0, value => EnsureBody(ret).flutelength = Parse.ToString(value));
            SetIfChanged(originalTool, srcTool.Shoulder_Len, originalTool?.Shoulder_Len ?? 0, value => EnsureBody(ret).shoulderlength = Parse.ToString(value));
            SetIfChanged(originalTool, srcTool.Shank_Dia, originalTool?.Shank_Dia ?? 0, value => EnsureBody(ret).shaftdiameter = Parse.ToString(value));
            SetIfChanged(originalTool, srcTool.Flute_N, originalTool?.Flute_N ?? 0, value => EnsureBody(ret).numberofflutes = Parse.ToString(value));
            SetIfChanged(originalTool, srcTool.Leadangle, originalTool?.Leadangle ?? 0, value => EnsureBody(ret).taperangle = Parse.ToString(90 - value));

            if (originalTool == null || srcTool.Tool_type_id != originalTool.Tool_type_id || string.IsNullOrEmpty(ret.type))
            {
                switch ((Enums.ToolTypes)srcTool.Tool_type_id)
                {
                    case Enums.ToolTypes.SolidEndMill:
                        ret.type = "flat end mill";

                        break;
                    case Enums.ToolTypes.ThreadMill:
                        ret.type = "form mill";
                        EnsureBody(ret).threadpitch = Parse.ToString(srcTool.Thread_pitch);
                        EnsureBody(ret).numberoffteeth = Parse.ToString((int)(srcTool.Flute_Len / srcTool.Thread_pitch));
                        break;
                    case Enums.ToolTypes.SolidBallMill:
                        ret.type = "ball end mill";
                        break;
                    case Enums.ToolTypes.IndexedFaceMill:
                        ret.type = "face mill";
                        break;
                    case Enums.ToolTypes.WoodRuff:
                        ret.type = "slot mill";
                        break;

                    case Enums.ToolTypes.VbitEngraver:
                    case Enums.ToolTypes.ChamferMill:
                        ret.type = "chamfer mill";
                        EnsureBody(ret).tipdiameter = Parse.ToString(srcTool.Diameter);
                        EnsureBody(ret).diameter = Parse.ToString(srcTool.Shank_Dia);

                        break;
                    //case "center drill":
                    case Enums.ToolTypes.JobberTwistDrill:
                        ret.type = "drill";
                        break;
                    case Enums.ToolTypes.SpotDrill:
                        ret.type = "spot drill";
                        break;
                    case Enums.ToolTypes.CounterSink:
                        ret.type = "counter sink";
                        EnsureBody(ret).tipdiameter = Parse.ToString(srcTool.Diameter);
                        EnsureBody(ret).diameter = Parse.ToString(srcTool.Shank_Dia);
                        break;
                    case Enums.ToolTypes.Counterbore:
                        ret.type = "counter bore";
                        break;
                    //"tap left hand":
                    case Enums.ToolTypes.Tap:
                        ret.type = "tap right hand"; //"tap left hand":
                        EnsureBody(ret).threadpitch = Parse.ToString(srcTool.Thread_pitch);
                        EnsureBody(ret).numberoffteeth = Parse.ToString(srcTool.Flute_N);
                        break;
                    case Enums.ToolTypes.BoringHead:
                        ret.type = "boring bar";
                        break;
                    //case "turning threading":
                    case Enums.ToolTypes.TurningProfiling:
                        ret.type = "turning general";

                        EnsureTurningHolder(ret).shankheight = Parse.ToString(srcTool.Shank_Dia);
                        EnsureTurningHolder(ret).headlength = Parse.ToString(srcTool.Stickout);
                        EnsureInsert(ret).cornerradius = Parse.ToString(srcTool.Corner_rad);

                        break;
                    case Enums.ToolTypes.BoringBar:
                        ret.type = "turning boring";

                        EnsureTurningHolder(ret).shankheight = Parse.ToString(srcTool.Shank_Dia);
                        EnsureTurningHolder(ret).headlength = Parse.ToString(srcTool.Stickout);
                        EnsureInsert(ret).cornerradius = Parse.ToString(srcTool.Corner_rad);

                        break;
                    case Enums.ToolTypes.TurningGrooving:
                        ret.type = "turning grooving";

                        EnsureTurningHolder(ret).shankheight = Parse.ToString(srcTool.Shank_Dia);
                        EnsureTurningHolder(ret).headlength = Parse.ToString(srcTool.Stickout);

                        EnsureInsert(ret).cornerradius = Parse.ToString(srcTool.Corner_rad);

                        break;

                }
            }
            else
            {
                ApplyTypeSpecificChanges(ret, srcTool, originalTool);
            }
            return ret;
        }

        private static void ApplyTypeSpecificChanges(toollibraryTool ret, Tool srcTool, Tool originalTool)
        {
            switch ((Enums.ToolTypes)srcTool.Tool_type_id)
            {
                case Enums.ToolTypes.ThreadMill:
                    SetIfChanged(originalTool, srcTool.Thread_pitch, originalTool.Thread_pitch, value => EnsureBody(ret).threadpitch = Parse.ToString(value));
                    if (!AreEqual(srcTool.Thread_pitch, originalTool.Thread_pitch) || !AreEqual(srcTool.Flute_Len, originalTool.Flute_Len))
                    {
                        EnsureBody(ret).numberoffteeth = Parse.ToString((int)(srcTool.Flute_Len / srcTool.Thread_pitch));
                    }
                    break;

                case Enums.ToolTypes.VbitEngraver:
                case Enums.ToolTypes.ChamferMill:
                case Enums.ToolTypes.CounterSink:
                    if (!AreEqual(srcTool.Diameter, originalTool.Diameter) || !AreEqual(srcTool.Shank_Dia, originalTool.Shank_Dia))
                    {
                        EnsureBody(ret).tipdiameter = Parse.ToString(srcTool.Diameter);
                        EnsureBody(ret).diameter = Parse.ToString(srcTool.Shank_Dia);
                    }
                    break;

                case Enums.ToolTypes.Tap:
                    SetIfChanged(originalTool, srcTool.Thread_pitch, originalTool.Thread_pitch, value => EnsureBody(ret).threadpitch = Parse.ToString(value));
                    break;

                case Enums.ToolTypes.TurningProfiling:
                case Enums.ToolTypes.BoringBar:
                case Enums.ToolTypes.TurningGrooving:
                    SetIfChanged(originalTool, srcTool.Shank_Dia, originalTool.Shank_Dia, value => EnsureTurningHolder(ret).shankheight = Parse.ToString(value));
                    SetIfChanged(originalTool, srcTool.Stickout, originalTool.Stickout, value => EnsureTurningHolder(ret).headlength = Parse.ToString(value));
                    SetIfChanged(originalTool, srcTool.Corner_rad, originalTool.Corner_rad, value => EnsureInsert(ret).cornerradius = Parse.ToString(value));
                    break;
            }
        }

        private static void SetIfChanged<T>(Tool originalTool, T currentValue, T originalValue, Action<T> setValue)
        {
            if (originalTool == null || !EqualityComparer<T>.Default.Equals(currentValue, originalValue))
            {
                setValue(currentValue);
            }
        }

        private static void SetIfChanged(Tool originalTool, double currentValue, double originalValue, Action<double> setValue)
        {
            if (originalTool == null || !AreEqual(currentValue, originalValue))
            {
                setValue(currentValue);
            }
        }

        private static bool AreEqual(double value1, double value2)
        {
            return Math.Abs(value1 - value2) < 0.000000000001;
        }

        private static toollibraryToolMaterial EnsureMaterial(toollibraryTool tool)
        {
            if (tool.material == null)
                tool.material = new toollibraryToolMaterial();
            return tool.material;
        }

        private static toollibraryToolNC EnsureNC(toollibraryTool tool)
        {
            if (tool.nc == null)
                tool.nc = new toollibraryToolNC();
            return tool.nc;
        }

        private static toollibraryToolBody EnsureBody(toollibraryTool tool)
        {
            if (tool.body == null)
                tool.body = new toollibraryToolBody();
            return tool.body;
        }

        private static toollibraryToolTurningholder EnsureTurningHolder(toollibraryTool tool)
        {
            if (tool.turningholder == null)
                tool.turningholder = new toollibraryToolTurningholder();
            return tool.turningholder;
        }

        private static toollibraryToolInsert EnsureInsert(toollibraryTool tool)
        {
            if (tool.insert == null)
                tool.insert = new toollibraryToolInsert();
            return tool.insert;
        }

        public string ShowOpenFileDialog()
        {
            var OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();

            OpenFileDialog1.FileName = "";
            OpenFileDialog1.Title = "Select an a HSMWorks tool database file";
            OpenFileDialog1.Filter = this.GetReadFileFilter();

            OpenFileDialog1.AddExtension = true;
            OpenFileDialog1.SupportMultiDottedExtensions = true;
            OpenFileDialog1.CheckFileExists = true;

            var ret = OpenFileDialog1.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK && File.Exists(OpenFileDialog1.FileName))
            {
                return OpenFileDialog1.FileName;
            }
            return null;
        }

        public string ShowSaveAsFileDialog()
        {
            var OpenFileDialog1 = new System.Windows.Forms.SaveFileDialog();

            OpenFileDialog1.FileName = "";
            OpenFileDialog1.Title = "Save HSMWorks tool database into file";
            OpenFileDialog1.Filter = this.GetReadFileFilter();

            OpenFileDialog1.AddExtension = true;
            OpenFileDialog1.SupportMultiDottedExtensions = true;
            //OpenFileDialog1.Exi = true;

            var ret = OpenFileDialog1.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                return OpenFileDialog1.FileName;
            }
            return null;
        }

        public override void ModifyTools(DataBase db)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tell HSMAdvisor which methods are implemented and the method titles in the UI
        /// </summary>
        /// <returns></returns>
        public override List<Capability> GetCapabilities()
        {
            var caps = new List<Capability>();
            caps.Add(new Capability("Import HSMWorks Tool Database", (int)ToolsPluginCapabilityMethod.ImportTools));
            caps.Add(new Capability("Export HSMWorks Tool Database", (int)ToolsPluginCapabilityMethod.ExportTools));

            return caps;
        }
    }
}
