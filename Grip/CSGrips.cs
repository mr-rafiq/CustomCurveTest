using CustomCurveTest;
using Rhino;
using Rhino.DocObjects;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomRhinoObjects
{
    public class CSGrips : CustomObjectGrips
    {

        private CSGripObject[] cs_polylinegrip;
        private Point3d[] m_active;
        private Point3d[] m_original;
        public ObjectAttributes abs { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public CSGrips()
        {


           
        }

        /// <summary>
        /// Creates grips
        /// </summary>
        public bool CreateGrips(PolylineCurve polylineCurve)
        {

            List<Point3d> pts = GetDiscontiuityPoints(polylineCurve);

 

          
            cs_polylinegrip = new CSGripObject[pts.Count];
            m_active = new Point3d[pts.Count];
            m_original = new Point3d[pts.Count];
            for (int i = 0; i < pts.Count; i++)
            {

                cs_polylinegrip[i].OriginalLocation = pts[i];
                m_active[i] = pts[i];

                cs_polylinegrip[i].Active = true;
                AddGrip(cs_polylinegrip[i]);
            }
            Array.Copy(m_active, m_original, pts.Count);


            return true;
        }
        public  List<double> GetDiscontinuityParams(Curve c)
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


        public  List<Point3d> GetDiscontiuityPoints(Curve c)
        {
            List<double> param = GetDiscontinuityParams(c);
            List<Point3d> points = new List<Point3d>();
            param.ForEach(x => points.Add(c.PointAt(x)));
            return points;
        }

        protected override void OnReset()
        {
            Array.Copy(m_original, m_active, m_active.Length);
            RhinoApp.WriteLine("Onreset");
            base.OnReset();
        }

        /// <summary>
        /// CustomObjectGrips override
        /// </summary>
        protected override GeometryBase NewGeometry()
        {
            RhinoApp.WriteLine("NewGeometry");
            CustomCurve cs = new CustomCurve(m_active,this.abs);
            //UpdateGrips();
            
            if (GripsMoved)
                return cs.Geometry;

            return null;
        }

        /// <summary>
        /// CustomObjectGrips override
        /// </summary>
        protected override void OnDraw(GripsDrawEventArgs args)
        {
            RhinoApp.WriteLine("OnDraw");


            base.OnDraw(args);
        }
    }

}
