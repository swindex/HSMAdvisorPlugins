using HSMAdvisorPlugin;
using ObjectToolDatabase;
using ObjectToolDatabase.ToolDataBase;
using System;
using System.Collections.Generic;
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
        public static Tool ToTool( toollibraryTool t )
        {
            Tool ret = new Tool(true)
            {
                Type = Enums.ToolTypeNames.endmill,
                Name_id = Convert.ToInt32(Enums.ToolTypes.SolidEndMill),
                Coating_id = Convert.ToInt32(Enums.ToolCoatings.None),
                Tool_material_id = Convert.ToInt32(ToToolMaterial(t.material.name)),

                Guid = t.guid,
                Comment = t.description,

                Library = "",

                Brand_name = t.manufacturer,
                Series_name = t.productid,
                Product_link = t.productlink,

                Number = Parse.ToInteger(t.nc.number),

                Offset_Diameter = Parse.ToInteger(t.nc.diameteroffset),
                Offset_Length = Parse.ToInteger(t.nc.lengthoffset),

                Aux_data = Serializer.ToXML(t),

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
                ret.Leadangle = 90 - Parse.ToDouble(t.body.taperangle);
                ret.Leadangle_mode = Convert.ToInt32(Enums.ToolAngleModes.Lead);
            }

            //set default values
            ret.Maxdeflection_pc = -1;
            ret.Maxtorque_pc = -1;
            ret.Productivity = -1;

            switch (t.type) {
                case "flat end mill":
                case "bull nose end mill":
                case "tapered mill":
                case "radius mill":
                    break;
                case "ball end mill":
                case "lollipop mill":
                    ret.Name_id = Convert.ToInt32(Enums.ToolTypes.SolidBallMill);
                    break;
                case "face mill":
                    ret.Name_id = Convert.ToInt32(Enums.ToolTypes.IndexedFaceMill);
                    break;
                case "slot mill":
                    ret.Name_id = Convert.ToInt32(Enums.ToolTypes.WoodRuff);
                    break;
                case "chamfer mill":
                    ret.Name_id = Convert.ToInt32(Enums.ToolTypes.ChamferMill);
                    ret.Diameter = Parse.ToDouble(t.body.tipdiameter);
                    ret.Shank_Dia = Parse.ToDouble(t.body.diameter);

                    break;

                case "center drill":
                case "drill":
                    ret.Name_id = Convert.ToInt32(Enums.ToolTypes.JobberTwistDrill);
                    ret.Leadangle_mode = Convert.ToInt32(Enums.ToolAngleModes.Tip);
                    ret.Flute_N = 2;
                    break;
                case "spot drill":
                    ret.Name_id = Convert.ToInt32(Enums.ToolTypes.SpotDrill);
                    ret.Leadangle_mode = Convert.ToInt32(Enums.ToolAngleModes.Tip);
                    ret.Flute_N = 2;
                    break;
                case "tap right hand":
                    ret.Name_id = Convert.ToInt32(Enums.ToolTypes.Tap);
                    ret.Thread_pitch = Parse.ToDouble(t.body.threadpitch);
                    ret.Flute_N = 1;
                    break;
                case "tap left hand":
                    ret.Name_id = Convert.ToInt32(Enums.ToolTypes.Tap);
                    ret.Thread_pitch = Parse.ToDouble(t.body.threadpitch);
                    ret.Flute_N = 1;
                    break;
                case "boring bar":
                    ret.Name_id = Convert.ToInt32(Enums.ToolTypes.BoringHead);
                    ret.Thread_pitch = Parse.ToDouble(t.body.threadpitch);
                    break;
                case "turning threading":
                case "turning general":
                    ret.Type = Enums.ToolTypeNames.turn;
                    ret.Name_id = Convert.ToInt32(Enums.ToolTypes.TurningProfiling);
                    //ret.Thread_pitch = t.body.threadpitch;
                    ret.Shank_Dia = Parse.ToDouble(t.turningholder.shankheight);
                    ret.Stickout = Parse.ToDouble(t.turningholder.headlength);
                    ret.Corner_rad = Parse.ToDouble(t.insert.cornerradius);

                    ret.Flute_N = 1;
                    break;
                case "turning boring":
                    ret.Type = Enums.ToolTypeNames.turn;
                    ret.Name_id = Convert.ToInt32(Enums.ToolTypes.BoringBar);
                    //ret.Thread_pitch = t.body.threadpitch;
                    ret.Shank_Dia = Parse.ToDouble(t.turningholder.shankheight);
                    ret.Stickout = Parse.ToDouble(t.turningholder.headlength);
                    ret.Corner_rad = Parse.ToDouble(t.insert.cornerradius);

                    ret.Flute_N = 1;
                    break;
                case "turning grooving":
                    ret.Type = Enums.ToolTypeNames.turn;
                    ret.Name_id = Convert.ToInt32(Enums.ToolTypes.TurningProfiling);
                    
                    //ret.Thread_pitch = t.body.threadpitch;
                    ret.Shank_Dia = Parse.ToDouble(t.turningholder.shankheight);
                    ret.Stickout = Parse.ToDouble(t.turningholder.headlength);
                    ret.Corner_rad = Parse.ToDouble(t.insert.cornerradius);

                    ret.Flute_N = 1;
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
        public DataBase ImportTools() 
        {
            var FileName = ShowSelectFileDialog();

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

            //return new database. HSMAdvisor will SAFELY merge it into it's current database
            return targetDB;
        }

        //
        public object ExportTools(DataBase db)
        {
            throw new NotImplementedException();
           
        }

        public string ShowSelectFileDialog()
        {
            var OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();

            OpenFileDialog1.FileName = "";
            OpenFileDialog1.Title = "Select an a HSMWorks tool database file";
            OpenFileDialog1.Filter = this.GetReadFileFilter();
            
            OpenFileDialog1.AddExtension = true;
            OpenFileDialog1.SupportMultiDottedExtensions = true;
            OpenFileDialog1.CheckFileExists = true;

            var ret = OpenFileDialog1.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.OK && File.Exists(OpenFileDialog1.FileName)) {
                return OpenFileDialog1.FileName;
            }
            return null;
        }

        public object ModifyTools(DataBase db)
        {
            throw new NotImplementedException();
        }

        List<Capability> HSMAdvisorPluginInterface.GetCapabilities()
        {
            var caps = new List<Capability>();
            caps.Add(new Capability("Import HSMWords Tool Database", (int)ToolsPluginCapabilityMethod.ImportTools));

            return caps;
        }
    }
}
