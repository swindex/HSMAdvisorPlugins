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
            Tool ret = new Tool(true)
            {
                Tool_type_id = Enums.ToolTypes.SolidEndMill,
                
                Coating_id = Enums.ToolCoatings.None,
                Tool_material_id = ToToolMaterial(t.material.name),

                Guid = t.guid,
                Comment = t.description,

                Library = "",

                Brand_name = t.manufacturer,
                Series_name = t.productid,
                Product_link = t.productlink,

                Number = Parse.ToInteger(t.nc.number),

                Offset_Diameter = Parse.ToInteger(t.nc.diameteroffset),
                Offset_Length = Parse.ToInteger(t.nc.lengthoffset),

                Aux_data = Serializer.ToXML(t, "UTF-16"),

                Circle_dia_m = t.unit == "millimeters",
                Depth_m = t.unit == "millimeters",
                Diameter_m = t.unit == "millimeters",
                Corner_rad_m = t.unit == "millimeters",
                Doc_m = t.unit == "millimeters",
                Feed_m = t.unit == "millimeters",
                Flute_len_m = t.unit == "millimeters",
                Input_units_m = t.unit == "millimeters",
                Ipt_m = t.unit == "millimeters",
                Peck_m = t.unit == "millimeters",
                Result_units_m = t.unit == "millimeters",
                Pilot_Hole_m = t.unit == "millimeters",
                Sfm_m = t.unit == "millimeters",
                Shank_Dia_m = t.unit == "millimeters",
                Shoulder_Dia_m = t.unit == "millimeters",
                Shoulder_len_m = t.unit == "millimeters",
                Stickout_m = t.unit == "millimeters",
                Thread_drill_dia_m = t.unit == "millimeters",
                Thread_pitch_m = t.unit == "millimeters",
                Woc_m = t.unit == "millimeters"


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
                ret.Toolangle_mode = Enums.ToolAngleModes.Lead;
                ret.Leadangle = 90 - Parse.ToDouble(t.body.taperangle);
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
                    ret.Thread_pitch = Parse.ToDouble(t.body.threadpitch);
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
                    ret.Tool_type_id = Enums.ToolTypes.ChamferMill;
                    ret.Diameter = Parse.ToDouble(t.body.tipdiameter);
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
                    ret.Shank_Dia = Parse.ToDouble(t.body.diameter);
                    ret.Toolangle_mode = Enums.ToolAngleModes.Tip;

                    break;

                case "center drill":
                case "drill":
                    ret.Tool_type_id = Enums.ToolTypes.JobberTwistDrill;
                    ret.Toolangle_mode = Enums.ToolAngleModes.Tip;
                    ret.Flute_N = 2;
                    break;
                case "spot drill":
                    ret.Tool_type_id = Enums.ToolTypes.SpotDrill;
                    ret.Toolangle_mode = Enums.ToolAngleModes.Tip;
                    ret.Flute_N = 2;
                    break;
                case "counter bore":
                    ret.Tool_type_id = Enums.ToolTypes.Counterbore;
                    ret.Toolangle_mode = Enums.ToolAngleModes.Tip;
                    break;
                case "tap right hand":
                    ret.Tool_type_id = Enums.ToolTypes.Tap;
                    ret.Thread_pitch = Parse.ToDouble(t.body.threadpitch);
                    ret.Flute_N = 1;
                    break;
                case "tap left hand":
                    ret.Tool_type_id = Enums.ToolTypes.Tap;
                    ret.Thread_pitch = Parse.ToDouble(t.body.threadpitch);
                    ret.Flute_N = 1;
                    break;
                case "boring bar":
                    ret.Tool_type_id = Enums.ToolTypes.BoringHead;

                    break;
                case "turning threading":
                case "turning general":
                    ret.Tool_type_id = Enums.ToolTypes.TurningProfiling;
                    //ret.Thread_pitch = t.body.threadpitch;
                    ret.Shank_Dia = Parse.ToDouble(t.turningholder.shankheight);
                    ret.Stickout = Parse.ToDouble(t.turningholder.headlength);
                    ret.Corner_rad = Parse.ToDouble(t.insert.cornerradius);

                    ret.Flute_N = 1;
                    break;
                case "turning boring":
                    ret.Tool_type_id = Enums.ToolTypes.BoringBar;
                    //ret.Thread_pitch = t.body.threadpitch;
                    ret.Shank_Dia = Parse.ToDouble(t.turningholder.shankheight);
                    ret.Stickout = Parse.ToDouble(t.turningholder.headlength);
                    ret.Corner_rad = Parse.ToDouble(t.insert.cornerradius);

                    ret.Flute_N = 1;
                    break;
                case "turning grooving":
                    ret.Tool_type_id = Enums.ToolTypes.TurningProfiling;

                    //ret.Thread_pitch = t.body.threadpitch;
                    ret.Shank_Dia = Parse.ToDouble(t.turningholder.shankheight);
                    ret.Stickout = Parse.ToDouble(t.turningholder.headlength);
                    ret.Corner_rad = Parse.ToDouble(t.insert.cornerradius);

                    ret.Flute_N = 1;
                    break;
                // These types are not supported in HSMAdvisor yet. Set to endmill by default
                case "dovetail mill":
                    ret.Tool_type_id = Enums.ToolTypes.SolidEndMill;
                    ret.Toolangle_mode = Enums.ToolAngleModes.Taper;
                    // Fusions's dovetail mills specify taperangle as always positive, but in HSMAdvisor it's negative for dovetails
                    ret.Toolangle = (-Parse.ToDouble(t.body.taperangle));

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
                        Shank_Dia = Parse.ToDouble(srcTool.body.shaftdiameter)
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

        private toollibraryTool FromTool(Tool srcTool)
        {

            toollibraryTool ret = new toollibraryTool();
            if (!string.IsNullOrEmpty(srcTool.Aux_data))
            {
                try
                {
                    ret = Serializer.FromXML<toollibraryTool>(srcTool.Aux_data, false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            if (ret.material == null)
                ret.material = new toollibraryToolMaterial();

            ret.material.name = FromToolMaterial((Enums.ToolMaterials)srcTool.Tool_material_id);
            ret.guid = srcTool.Guid;

            ret.description = srcTool.Comment;


            ret.manufacturer = srcTool.Brand_name;
            ret.productid = srcTool.Series_name;
            ret.productlink = srcTool.Product_link;


            if (ret.nc == null)
                ret.nc = new toollibraryToolNC();
            ret.nc.number = Parse.ToString(srcTool.Number);

            ret.nc.diameteroffset = Parse.ToString(srcTool.Offset_Diameter);
            ret.nc.lengthoffset = Parse.ToString(srcTool.Offset_Length);

            ret.unit = srcTool.Input_units_m ? "millimeters" : "inches";

            if (ret.body == null)
                ret.body = new toollibraryToolBody();

            ret.body.diameter = Parse.ToString(srcTool.Diameter);
            ret.body.cornerradius = Parse.ToString(srcTool.Corner_rad);
            ret.body.bodylength = Parse.ToString(srcTool.Stickout);
            ret.body.flutelength = Parse.ToString(srcTool.Flute_Len);
            ret.body.shoulderlength = Parse.ToString(srcTool.Shoulder_Len);
            ret.body.shaftdiameter = Parse.ToString(srcTool.Shank_Dia);
            ret.body.numberofflutes = Parse.ToString(srcTool.Flute_N);
            ret.body.taperangle = Parse.ToString(90 - srcTool.Leadangle);

            //override type ONLY if none is specified by AUX_DATA
            if (string.IsNullOrEmpty(ret.type))
            {
                switch ((Enums.ToolTypes)srcTool.Tool_type_id)
                {
                    case Enums.ToolTypes.SolidEndMill:
                        ret.type = "flat end mill";

                        break;
                    case Enums.ToolTypes.ThreadMill:
                        ret.type = "form mill";
                        ret.body.threadpitch = Parse.ToString(srcTool.Thread_pitch);
                        ret.body.numberoffteeth = Parse.ToString((int)(srcTool.Flute_Len / srcTool.Thread_pitch));
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
                    case Enums.ToolTypes.ChamferMill:
                        ret.type = "chamfer mill";
                        ret.body.tipdiameter = Parse.ToString(srcTool.Diameter);
                        ret.body.diameter = Parse.ToString(srcTool.Shank_Dia);

                        break;
                    //case "center drill":
                    case Enums.ToolTypes.JobberTwistDrill:
                        ret.type = "drilll";
                        break;
                    case Enums.ToolTypes.SpotDrill:
                        ret.type = "spot drill";
                        break;
                    //"tap left hand":
                    case Enums.ToolTypes.Tap:
                        ret.type = "tap right hand"; //"tap left hand":
                        ret.body.threadpitch = Parse.ToString(srcTool.Thread_pitch);
                        break;
                    case Enums.ToolTypes.BoringHead:
                        ret.type = "boring bar";
                        break;
                    //case "turning threading":
                    case Enums.ToolTypes.TurningProfiling:
                        ret.type = "turning general";
                        if (ret.turningholder == null)
                            ret.turningholder = new toollibraryToolTurningholder();

                        ret.turningholder.shankheight = Parse.ToString(srcTool.Shank_Dia);
                        ret.turningholder.headlength = Parse.ToString(srcTool.Stickout);
                        if (ret.insert == null)
                            ret.insert = new toollibraryToolInsert();
                        ret.insert.cornerradius = Parse.ToString(srcTool.Corner_rad);

                        break;
                    case Enums.ToolTypes.BoringBar:
                        ret.type = "turning boring";

                        if (ret.turningholder == null)
                            ret.turningholder = new toollibraryToolTurningholder();
                        ret.turningholder.shankheight = Parse.ToString(srcTool.Shank_Dia);
                        ret.turningholder.headlength = Parse.ToString(srcTool.Stickout);
                        if (ret.insert == null)
                            ret.insert = new toollibraryToolInsert();
                        ret.insert.cornerradius = Parse.ToString(srcTool.Corner_rad);

                        break;
                    case Enums.ToolTypes.TurningGrooving:
                        ret.type = "turning grooving";

                        if (ret.turningholder == null)
                            ret.turningholder = new toollibraryToolTurningholder();
                        ret.turningholder.shankheight = Parse.ToString(srcTool.Shank_Dia);
                        ret.turningholder.headlength = Parse.ToString(srcTool.Stickout);

                        if (ret.insert == null)
                            ret.insert = new toollibraryToolInsert();
                        ret.insert.cornerradius = Parse.ToString(srcTool.Corner_rad);

                        break;

                }
            }
            return ret;
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
