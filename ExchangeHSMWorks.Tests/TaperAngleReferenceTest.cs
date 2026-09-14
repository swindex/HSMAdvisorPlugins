using System;
using System.IO;
using System.Linq;
using HSMAdvisorDatabase;
using HSMAdvisorDatabase.ToolDataBase;
using ExchangeHSMWorks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExchangeHSMWorks.Tests
{
    /// <summary>
    /// Regression tests for swindex/HSMAdvisor#140:
    /// HSMLib import must interpret body/@taper-angle against the correct reference.
    /// In HSMWorks libraries a tapered mill's taper-angle is measured from the
    /// vertical (tool axis) reference, and a chamfer mill / counter sink taper-angle
    /// is the included (apex) angle of the cone.
    ///
    /// These tests do NOT assert the imported angle constant directly. They parse
    /// the synthetic TaperAngleReference.hsmlib fixture through the real import code
    /// (Serializer.FromXML &lt;toollibrary&gt; + Converter.ToTool), rebuild the tool's
    /// cone geometry from the imported data (derived small-end diameter, cone apex
    /// position, surface point) and compare it with the cone geometry that the
    /// fixture describes independently. A horizontal/vertical reference mix-up in
    /// the import makes the reconstructed cone diverge from the fixture cone and
    /// the tests fail.
    /// </summary>
    [TestClass]
    public class TaperAngleReferenceTest
    {
        private const double Tolerance = 1e-6;

        private static double TanRad(double deg)
        {
            return Math.Tan(deg * Math.PI / 180.0);
        }

        /// <summary>
        /// Reconstructs the diameter the imported tool's cone has at the far end
        /// of the flute (small end for taper/Tip tools). The stored Leadangle in
        /// HSMAdvisor is always referenced from the horizontal plane, so the cone
        /// radius slope per 1mm of axial length is tan(90 - Leadangle) for Taper
        /// mode tools and tan(90 - Leadangle) = tan(half included angle) for Tip
        /// mode tools.
        /// </summary>
        private static double DerivedSmallEndDiameter(Tool tool)
        {
            double slope = TanRad(90.0 - tool.Leadangle);
            return tool.Diameter - 2.0 * tool.Flute_Len * slope;
        }

        [TestMethod]
        public void TaperedMill_ConeSurface_MatchesFixtureGeometry()
        {
            // Fixture describes: Ø10 top end, 20 mm flute, 4 deg taper from the vertical axis.
            const double TopDiameter = 10.0;
            const double FluteLength = 20.0;
            const double TaperAngle = 4.0;

            var tool = FindTool("4deg Tapered Mill");

            // Expected cone, computed independently from the fixture's raw dimensions:
            double expectedTipDiameter = TopDiameter - 2.0 * FluteLength * TanRad(TaperAngle);
            double expectedApexDistance = (TopDiameter / 2.0) / TanRad(TaperAngle); // flute top -> cone apex
            const double z = 12.0; // 12 mm below the flute top
            double expectedRadiusAtZ = (TopDiameter / 2.0) - z * TanRad(TaperAngle);

            // Same cone rebuilt from the imported tool's stored angle:
            double slope = TanRad(90.0 - tool.Leadangle);
            double actualTipDiameter = tool.Diameter - 2.0 * tool.Flute_Len * slope;
            double actualApexDistance = (tool.Diameter / 2.0) / slope;
            double actualRadiusAtZ = (tool.Diameter / 2.0) - z * slope;

            Assert.AreEqual(expectedTipDiameter, actualTipDiameter, Tolerance,
                "Tapered mill tip diameter does not match the fixture cone (wrong angle reference).");
            Assert.AreEqual(expectedApexDistance, actualApexDistance, Tolerance,
                "Tapered mill cone apex position does not match the fixture cone (wrong angle reference).");
            Assert.AreEqual(expectedRadiusAtZ, actualRadiusAtZ, Tolerance,
                "Radius on the tapered mill surface at z=12mm does not match the fixture cone (wrong angle reference).");
        }

        [TestMethod]
        public void ChamferMill_90degIncluded_ConeMatchesFixture()
        {
            // Fixture describes a 90 deg included-angle chamfer: Ø10 large end,
            // 2.5 mm flute, small (tip) end Ø5.
            const double LargeDiameter = 10.0;
            const double TipDiameter = 5.0;

            var tool = FindTool("90deg Chamfer Mill");

            Assert.AreEqual(LargeDiameter, tool.Diameter, Tolerance,
                "Chamfer mill large end diameter was not imported from the fixture.");

            double derivedSmallEnd = DerivedSmallEndDiameter(tool);
            Assert.AreEqual(TipDiameter, derivedSmallEnd, Tolerance,
                "Chamfer mill cone does not reach the fixture tip diameter over the flute length (wrong angle reference).");
        }

        [TestMethod]
        public void CounterSink_120degIncluded_ConeMatchesFixture()
        {
            // Fixture describes a 120 deg included-angle counter sink: Ø10 large end,
            // 1 mm flute, small end Ø(10 - 2*1*tan(60deg)).
            const double LargeDiameter = 10.0;
            const double FluteLength = 1.0;
            const double HalfIncludedAngle = 60.0;

            var tool = FindTool("120deg Counter Sink");

            Assert.AreEqual(LargeDiameter, tool.Diameter, Tolerance,
                "Counter sink large end diameter was not imported from the fixture.");

            double expectedSmallEnd = LargeDiameter - 2.0 * FluteLength * TanRad(HalfIncludedAngle);
            double derivedSmallEnd = DerivedSmallEndDiameter(tool);

            Assert.AreEqual(expectedSmallEnd, derivedSmallEnd, Tolerance,
                "Counter sink cone does not match the fixture small-end diameter (wrong angle reference).");
        }

        [TestMethod]
        public void FlatEndMill_WithoutTaperAngle_ImportsAsCylinder()
        {
            // A flat end mill has no taper-angle attribute in the fixture;
            // the imported tool must resolve to a zero-degree taper (cylinder).
            var tool = FindTool("Flat End Mill");

            double derivedSmallEnd = DerivedSmallEndDiameter(tool);
            Assert.AreEqual(tool.Diameter, derivedSmallEnd, Tolerance,
                "Flat end mill imported with a non-zero taper (angle reference not resolved to the vertical reference).");
        }

        [TestMethod]
        public void RoundTrip_TaperAngleAttribute_Preserved()
        {
            // Importing and re-exporting an unmodified tool must not alter the
            // original taper-angle attribute written by HSMWorks.
            var lib = LoadFixture();
            var src = lib.tool?.FirstOrDefault(t => t.description != null && t.description.Contains("4deg Tapered Mill"));
            Assert.IsNotNull(src, "Fixture tool '4deg Tapered Mill' not found.");

            var tool = Converter.ToTool(src);
            var exported = Converter.FromTool(tool);

            Assert.AreEqual("4", exported?.body?.taperangle,
                "Round trip through the converter altered the original taper-angle attribute.");
        }

        private static Tool FindTool(string descriptionPart)
        {
            var lib = LoadFixture();
            var src = lib.tool?.FirstOrDefault(t => t.description != null && t.description.Contains(descriptionPart));
            if (src == null)
                throw new InvalidOperationException($"Fixture tool '{descriptionPart}' not found.");
            return Converter.ToTool(src);
        }

        private static toollibrary LoadFixture()
        {
            var xml = File.ReadAllText(ResolveFixturePath());
            return Serializer.FromXML<toollibrary>(xml, false);
        }

        private static string ResolveFixturePath()
        {
            string[] startDirs =
            {
                Directory.GetCurrentDirectory(),
                AppDomain.CurrentDomain.BaseDirectory,
                new FileInfo(typeof(TaperAngleReferenceTest).Assembly.Location).DirectoryName
            };

            foreach (var start in startDirs)
            {
                if (string.IsNullOrEmpty(start))
                    continue;

                var dir = new DirectoryInfo(start);
                while (dir != null)
                {
                    // Skip build output directories so a copied fixture shadowing
                    // the source test-data folder can never be picked up.
                    bool isBuildOutput = dir.Name == "bin" || dir.Name == "debug" || dir.Name == "release";
                    if (!isBuildOutput)
                    {
                        string[] candidates =
                        {
                            Path.Combine(dir.FullName, @"ExchangeHSMWorks.Tests\test-data\TaperAngleReference.hsmlib"),
                            Path.Combine(dir.FullName, "test-data", "TaperAngleReference.hsmlib")
                        };
                        foreach (var candidate in candidates)
                        {
                            if (File.Exists(candidate))
                                return candidate;
                        }
                    }
                    dir = dir.Parent;
                }
            }

            throw new FileNotFoundException("TaperAngleReference.hsmlib fixture not found.");
        }
    }
}
