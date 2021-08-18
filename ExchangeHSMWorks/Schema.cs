using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeHSMWorks
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hsmworks.com/xml/2004/cnc/tool-library")]
    [System.Xml.Serialization.XmlRootAttribute("tool-library", Namespace = "http://www.hsmworks.com/xml/2004/cnc/tool-library", IsNullable = false)]
    public partial class toollibrary
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElement("tool")]
        public List<toollibraryTool> tool { get; set; } = new List<toollibraryTool>();

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public string version { get; set; }
    }

    /// <remarks/>
    //[System.Serializable()]
    public partial class toollibraryTool
    {

        /// <remarks/>
        public string description { get; set; }
        public string manufacturer { get; set; }
        [System.Xml.Serialization.XmlElementAttribute("product-id")]
        public string productid { get; set; }
        [System.Xml.Serialization.XmlElementAttribute("product-link")]
        public string productlink { get; set; }

        
        /// <remarks/>
        public toollibraryToolNC nc { get; set; }

        /// <remarks/>
        public toollibraryToolCoolant coolant { get; set; }

        /// <remarks/>
        public toollibraryToolMaterial material { get; set; }

        /// <remarks/>
        public toollibraryToolInsert insert { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("turning-holder")]
        public toollibraryToolTurningholder turningholder { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("turning-setup")]
        public toollibraryToolTurningsetup turningsetup { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement()]
        public toollibraryToolBody body { get; set; }

        /// <remarks/>
        public toollibraryToolHolder holder { get; set; }

        /// <remarks/>
        public toollibraryToolShaft shaft { get; set; }

        /// <remarks/>
        public toollibraryToolMotion motion { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlArray("presets")]
        [System.Xml.Serialization.XmlArrayItem("preset")]
        public List<toollibraryToolPreset> presets { get; set; } = new List<toollibraryToolPreset>();

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string guid { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string unit { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string version { get; set; }

    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hsmworks.com/xml/2004/cnc/tool-library")]
    public partial class toollibraryToolNC
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("break-control")]
        public string breakcontrol { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("diameter-offset")]
        public string diameteroffset { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool diameteroffsetSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("length-offset")]
        public string lengthoffset { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lengthoffsetSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("live-tool")]
        public string livetool { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("manual-tool-change")]
        public string manualtoolchange { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string number { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string turret { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("compensation-offset")]
        public string compensationoffset { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool compensationoffsetSpecified { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hsmworks.com/xml/2004/cnc/tool-library")]
    public partial class toollibraryToolCoolant
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string mode { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hsmworks.com/xml/2004/cnc/tool-library")]
    public partial class toollibraryToolMaterial
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hsmworks.com/xml/2004/cnc/tool-library")]
    public partial class toollibraryToolInsert
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("corner-radius")]
        public string cornerradius { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("cross-section")]
        public string crosssection { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string diameter { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("groove-width")]
        public string groovewidth { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("head-length")]
        public string headlength { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("insert-width")]
        public string insertwidth { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("internal-thread")]
        public string internalthread { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("overall-length")]
        public string overalllength { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("relief-angle")]
        public string reliefangle { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string shape { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string thickness { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("thread-pitch")]
        public string threadpitch { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("tip-angle")]
        public string tipangle { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string tolerance { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hsmworks.com/xml/2004/cnc/tool-library")]
    public partial class toollibraryToolTurningholder
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string clamping { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("cutting-width")]
        public string cuttingwidth { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string diameter { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string hand { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("head-length")]
        public string headlength { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("overall-length")]
        public string overalllength { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("relief-angle")]
        public string reliefangle { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("round-shank")]
        public string roundshank { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("shank-height")]
        public string shankheight { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("shank-width")]
        public string shankwidth { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string shape { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("side-angle")]
        public string sideangle { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string style { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hsmworks.com/xml/2004/cnc/tool-library")]
    public partial class toollibraryToolTurningsetup
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string compensation { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("right-spindle")]
        public string rightspindle { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("tool-angle")]
        public string toolangle { get; set; }
    }

    /// <remarks/>
    //[System.Serializable()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hsmworks.com/xml/2004/cnc/tool-library")]
    public partial class toollibraryToolBody
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("body-length")]
        public string bodylength { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("coolant-support")]
        public string coolantsupport { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string diameter { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("flute-length")]
        public string flutelength { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("number-of-flutes")]
        public string numberofflutes { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("number-of-teeth")]
        public string numberoffteeth { get; set; }


        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("overall-length")]
        public string overalllength { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("shaft-diameter")]
        public string shaftdiameter { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("shoulder-length")]
        public string shoulderlength { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("taper-angle")]
        public string taperangle { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool taperangleSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("thread-pitch")]
        public string threadpitch { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("thread-profile-angle")]
        public string threadprofileangle { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("corner-radius")]
        public string cornerradius { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cornerradiusSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("taper-angle2")]
        public string taperangle2 { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool taperangle2Specified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("tip-diameter")]
        public string tipdiameter { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tipdiameterSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("tip-length")]
        public string tiplength { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tiplengthSpecified { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hsmworks.com/xml/2004/cnc/tool-library")]
    public partial class toollibraryToolHolder
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("section")]
        public List<toollibraryToolHolderSection> section { get; set; } = new List<toollibraryToolHolderSection>();

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string description { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("product-id")]
        public string productid { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string vendor { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hsmworks.com/xml/2004/cnc/tool-library")]
    public partial class toollibraryToolShaft
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("section")]
        public List<toollibraryToolHolderSection> section { get; set; } = new List<toollibraryToolHolderSection>();
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hsmworks.com/xml/2004/cnc/tool-library")]
    public partial class toollibraryToolHolderSection
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string diameter { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string length { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hsmworks.com/xml/2004/cnc/tool-library")]
    public partial class toollibraryToolMotion
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string clockwise { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("cutting-feedrate")]
        public string cuttingfeedrate { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("entry-feedrate")]
        public string entryfeedrate { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("exit-feedrate")]
        public string exitfeedrate { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("feed-mode")]
        public string feedmode { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("plunge-feedrate")]
        public string plungefeedrate { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("ramp-feedrate")]
        public string rampfeedrate { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("ramp-spindle-rpm")]
        public string rampspindlerpm { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("retract-feedrate")]
        public string retractfeedrate { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("spindle-rpm")]
        public string spindlerpm { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool spindlerpmSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("surface-speed")]
        public string surfacespeed { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool surfacespeedSpecified { get; set; }
    }

    /*
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hsmworks.com/xml/2004/cnc/tool-library")]
    public partial class toollibraryToolPresets
    {

        /// <remarks/>
        public toollibraryToolPresetsPreset preset { get; set; }
    }*/

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.hsmworks.com/xml/2004/cnc/tool-library")]
    public partial class toollibraryToolPreset
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string description { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string tool_coolant { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string tool_feedPlunge { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tool_feedPlungeSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string tool_feedRetract { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tool_feedRetractSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string tool_spindleSpeed { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tool_spindleSpeedSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string tool_feedCutting { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tool_feedCuttingSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string tool_feedEntry { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tool_feedEntrySpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string tool_feedExit { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tool_feedExitSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string tool_feedRamp { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tool_feedRampSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string tool_rampSpindleSpeed { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tool_rampSpindleSpeedSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string tool_feedCuttingRel { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tool_feedCuttingRelSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string tool_feedEntryRel { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tool_feedEntryRelSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string tool_feedExitRel { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tool_feedExitRelSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string  tool_surfaceSpeed { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tool_surfaceSpeedSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string tool_useConstantSurfaceSpeed { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tool_useConstantSurfaceSpeedSpecified { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string tool_useFeedPerRevolution { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tool_useFeedPerRevolutionSpecified { get; set; }
    }
}
