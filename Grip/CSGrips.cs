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

namespace CustomCurveTest
{
    public class CSGrips : CustomObjectGrips
    {

        private CSGripObject[] cs_polylinegrip;
        private Point3d[] m_active;
        private Plane m_plane;
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
            polylineCurve.TryGetPlane(out this.m_plane);
            List<Point3d> pts = GetDiscontiuityPoints(polylineCurve);

            cs_polylinegrip = new CSGripObject[pts.Count];
            m_active = new Point3d[pts.Count];
            for (int i = 0; i < m_active.Length; i++)
            {
                m_active[i] = pts[i];
                cs_polylinegrip[i] = new CSGripObject();
                cs_polylinegrip[i].OriginalLocation = m_active[i];
                cs_polylinegrip[i].Active = true;
                AddGrip(cs_polylinegrip[i]);
            }
            RhinoApp.WriteLine("CSGrips CreateGrips");
            return true;
        }


        private void UpdateGrips()
        {
            if (NewLocation)
            {
                var world_to_plane = Transform.ChangeBasis(Plane.WorldXY, m_plane);


                for (var i = 0; i < m_active.Length; i++)
                {
                    if (cs_polylinegrip[i].Active && cs_polylinegrip[i].Moved)
                    {
                        Point pt = (Point)cs_polylinegrip[i].Geometry;
                        m_active[i] = new Point3d(pt.Location);
                    }

                }

                RhinoApp.WriteLine("CSGrips UpdateGrips");
                NewLocation = false;

            }
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

        protected override void OnReset()
        {
            RhinoApp.WriteLine("CSGrips Onreset");
            base.OnReset();
        }

        /// <summary>
        /// CustomObjectGrips override
        /// </summary>
        protected override GeometryBase NewGeometry()
        {
            RhinoApp.WriteLine("CSGrips NewGeometry");
            //UpdateGrips();
            UpdateGrips();
            if (GripsMoved)
            {
                ObjectAttributes ob;
                ob = base.OwnerObject.Attributes.Duplicate();
                CustomCurve cs = new CustomCurve(this.m_active, ob);
                PolylineCurve pl = new PolylineCurve(m_active);
                return pl;
            }
            RhinoApp.WriteLine("CSGrips NullGeometry");

            return null;
        }

        /// <summary>
        /// CustomObjectGrips override
        /// </summary>
        protected override void OnDraw(GripsDrawEventArgs args)
        {
            RhinoApp.WriteLine("CSGrips OnDraw");
            UpdateGrips();
            if (args.DrawDynamicStuff)
            {
                //for (var i = 0; i < cs_polylinegrip.Length - 1; i++)
                //{
                //    if (cs_polylinegrip[i].Active && cs_polylinegrip[i].Moved)
                //    {

                //        var start = (i == 0) ? 0 : i - 1;
                //        var end = i;
                //        var pt1 = cs_polylinegrip[end].OriginalLocation;
                //        Line ln = new Line(cs_polylinegrip[start].CurrentLocation, pt1);
                //        args.DrawControlPolygonLine(ln,  start, start);
                //        //args.dr
                //    }

                //}
                foreach (var item in cs_polylinegrip)
                {
                    if (item.Active && item.Moved)
                    {
                        var pt1 = item.OriginalLocation;
                        Line ln = new Line(item.CurrentLocation, pt1);


                        args.DrawControlPolygonLine(ln, 0, 0);
                        //args.dr
                    }

                }
            }
            base.OnDraw(args);
        }
    }

}
