using Rhino;
using Rhino.PlugIns;
using System;
using Rhino.Geometry;
using System.Collections.Generic;
using Rhino.DocObjects;

namespace CustomCurveTest
{
    ///<summary>
    /// <para>Every RhinoCommon .rhp assembly must have one and only one PlugIn-derived
    /// class. DO NOT create instances of this class yourself. It is the
    /// responsibility of Rhino to create an instance of this class.</para>
    /// <para>To complete plug-in information, please also see all PlugInDescription
    /// attributes in AssemblyInfo.cs (you might need to click "Project" ->
    /// "Show All Files" to see it in the "Solution Explorer" window).</para>
    ///</summary>
    public class CustomCurveTestPlugin : Rhino.PlugIns.PlugIn
    {
        public CustomCurveTestPlugin()
        {
            Instance = this;
        }

        ///<summary>Gets the only instance of the CustomCurveTestPlugin plug-in.</summary>
        public static CustomCurveTestPlugin Instance { get; private set; }

        // You can override methods here to change the plug-in behavior on
        // loading and shut down, add options pages to the Rhino _Option command
        // and maintain plug-in wide options in a document.
        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {
            RhinoDoc.AddRhinoObject += OnAddRhinoObject;
            RhinoDoc.ReplaceRhinoObject += OnReplaceRhinoObject;
            RhinoDoc.DeleteRhinoObject += OnDeleteRhinoObject;
            return LoadReturnCode.Success;

        }
        public static void OnAddRhinoObject(object sender, Rhino.DocObjects.RhinoObjectEventArgs e)
        {
            RhinoApp.WriteLine("> AddRhinoObject ({0})", e.ObjectId);

        }

        public static void OnReplaceRhinoObject(object sender, Rhino.DocObjects.RhinoReplaceObjectEventArgs e)
        {
            RhinoApp.WriteLine("> ReplaceRhinoObject ({0})", e.ObjectId);
            RhinoApp.WriteLine("     UndoActive = {0}", e.Document.UndoActive);
            RhinoApp.WriteLine("     RedoActive = {0}", e.Document.RedoActive);
            var obj1 = e.OldRhinoObject;
            ObjectAttributes objectAttribute = obj1.Attributes.Duplicate();
            var obj2 = e.NewRhinoObject;
            CSUserData ud = (CSUserData)obj2.Attributes.UserData.Find(typeof(CSUserData));
            if (ud != null)
            {
                var doc = e.Document;
                List<Point3d> pts = GetDiscontiuityPoints(obj2.Geometry as Curve);
                CustomCurve cs = new CustomCurve(pts.ToArray(), objectAttribute);
                doc.Objects.AddRhinoObject(cs, new PolylineCurve(pts));
                doc.Objects.Delete(e.OldRhinoObject);
                doc.Objects.Delete(e.NewRhinoObject);



            }
        }

        public static void OnDeleteRhinoObject(object sender, Rhino.DocObjects.RhinoObjectEventArgs e)
        {
            RhinoApp.WriteLine("> DeleteRhinoObject ({0})", e.ObjectId);
        }
        public static List<double> GetDiscontinuityParams(Curve c)
        {
            List<double> parameter = new List<double>();
            parameter.Add(c.Domain.Min);
            double st = c.Domain.Min;
            while (st >= 0)
            {
                double t = 0;
                c.GetNextDiscontinuity(Continuity.C2_continuous, st, c.Domain.Max, out t);
                if (t < 0) { break; }
                st = t;
                parameter.Add(t);
            }
            parameter.Add(c.Domain.Max);
            return parameter;
        }


        public static List<Point3d> GetDiscontiuityPoints(Curve c)
        {
            List<double> param = GetDiscontinuityParams(c);
            List<Point3d> points = new List<Point3d>();
            param.ForEach(x => points.Add(c.PointAt(x)));
            return points;
        }
    }
}