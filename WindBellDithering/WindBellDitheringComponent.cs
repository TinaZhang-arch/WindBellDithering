using AForge.Imaging.Filters;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace WindBellDithering
{
    public class WindBellDitheringComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public WindBellDitheringComponent()
          : base("WindBellDithering", "Nickname",
              "Description",
              "Category", "Subcategory")
        {

        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("path", "P", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("column", "C", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("row", "R", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Pattern", "P", "", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            String path = "";
            int C = 0;
            int R = 0;
            DA.GetData("path", ref path);
            DA.GetData("row", ref R);
            DA.GetData("column", ref C);

            Bitmap bm = new Bitmap(path);
            ResizeBilinear resizer = new ResizeBilinear(C, R);
            var bm2 = resizer.Apply(bm);
            AForge.Imaging.ColorReduction.FloydSteinbergColorDithering dithering = new AForge.Imaging.ColorReduction.FloydSteinbergColorDithering();
            dithering.ColorTable = new Color[] { Color.White, Color.Black };
            var bm3 = dithering.Apply(bm2);

            DataTree<bool> results = new DataTree<bool>();
            for (int i = 0; i < C; i++)
            {
                for (int j = 0; j < R; j++)
                {
                    bool result;
                    
                    Color color = bm3.GetPixel(i, j);
                    if (color.GetBrightness() < 0.5)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }

                    GH_Path gH_Path = new GH_Path(i, R - j -1);
                    results.Add(result, gH_Path);
                }
            }

            DA.SetDataTree(0, results);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e0e5916b-3574-4d4f-b0d1-5c4335045c5e"); }
        }
    }
}
