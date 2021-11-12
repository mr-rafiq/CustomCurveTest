using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CustomCurveTest
{
    public class CustomCurveTestCommand : Command
    {
        public CustomCurveTestCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static CustomCurveTestCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "CustomCurveTestCommand";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);



            Polyline pt1;
            using (GetPolyline getPointAction = new GetPolyline())
            {
                //getPointAction.SetCommandPrompt("Please select the end point");
                //getPointAction.SetBasePoint(pt0, true);
                //getPointAction.DynamicDraw +=
                //  (sender, e) => e.Display.DrawLine(pt0, e.CurrentPoint, System.Drawing.Color.DarkRed);
                //if (getPointAction.Get() != GetResult.Point)
                //{
                //    RhinoApp.WriteLine("No end point was selected.");
                //    return getPointAction.CommandResult();
                //}
                //pt1 = getPointAction.Point();
                getPointAction.FirstPointPrompt = "Please place the end point";
                getPointAction.Get(out pt1);

            }
            ObjectAttributes attributes = new ObjectAttributes();
            attributes.UserDictionary.Set("first", "CustomCurveObjectdata");
            attributes.ObjectColor = Color.Red;
            attributes.Name = "CustomCurveRhino";
            List<Point3d> pts = GetDiscontiuityPoints(pt1.ToPolylineCurve());

            CustomCurve cs = new CustomCurve(pts.ToArray(), attributes);
            doc.Objects.AddRhinoObject(cs,pt1.ToPolylineCurve());
            //doc.Objects.AddLine(pt0, pt1);
            doc.Views.Redraw();
            RhinoApp.WriteLine("The {0} command added one line to the document.", EnglishName);

            // ---
            return Result.Success;
        }
        public List<double> GetDiscontinuityParams(Curve c)
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


        public List<Point3d> GetDiscontiuityPoints(Curve c)
        {
            List<double> param = GetDiscontinuityParams(c);
            List<Point3d> points = new List<Point3d>();
            param.ForEach(x => points.Add(c.PointAt(x)));
            return points;
        }
    }
}
