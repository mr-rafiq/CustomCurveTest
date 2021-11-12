using Rhino;
using Rhino.Collections;
using Rhino.Display;
using Rhino.DocObjects;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCurveTest
{
    public class CustomCurve : CustomCurveObject
    {

        //    public MyPoint(){}
        public override ObjectAttributes Attributes { get; set; }
        public Point3d[] m_active { get; set; }

        public override GeometryBase Geometry { get { return this.geom(new PolylineCurve(m_active)); } }

        private CSGripEnabler m_grip_enabler;

        public CustomCurve() : base() { }
        public CustomCurve(Point3d[] m_active, ObjectAttributes attributes) : base()
        {
            this.m_active = m_active;
            this.Attributes = attributes;
            CommitChanges();
        }



        public override string ShortDescription(bool plural)
        {
            return plural ? "CustomCurves" : "CustomCurve";
        }

        protected override void OnAddToDocument(RhinoDoc doc)
        {

            RhinoApp.WriteLine("CustomCurve OnAddToDocument Rhino");
            Curve cr = new PolylineCurve(this.m_active);
            //doc.Objects.AddRhinoObject(this, cr);
            CSUserData cd = new CSUserData() { BaseCurve = base.CurveGeometry, Name = "CUstomCurve" };
            base.Attributes.UserData.Add(cd);


            //doc.Objects.Delete(this);
            //doc.Objects.Add(this.Geometry);
            doc.Views.Redraw();
            //base.OnAddToDocument(doc);


        }

        protected override void OnSelectionChanged()
        {
            RhinoApp.WriteLine("CustomCurve OnSelectionChanged");
            Enablegrip();
            base.OnSelectionChanged();
        }


        public void Enablegrip()
        {
            if (null == this.m_grip_enabler)
            {
                // Register once and only once...
                this.m_grip_enabler = new CSGripEnabler();
                CustomObjectGrips.RegisterGripsEnabler(m_grip_enabler.TurnOnGrips, typeof(CSGrips));
            }

            List<RhinoObject> rh_object = this.Document.Objects.GetSelectedObjects(false, false).ToList();
            if (rh_object.Count == 1)
            {
                if (rh_object[0].GripsOn)
                    rh_object[0].GripsOn = false;

                m_grip_enabler.TurnOnGrips(rh_object[0]);
                this.Document.Views.Redraw();
            }
            if (rh_object == null || rh_object.Count > 1)
            {
                if (rh_object[0].GripsOn)
                    rh_object[0].GripsOn = false;
            }
        }


        public GeometryBase geom(Curve curve)
        {
            List<Curve> crvs = new List<Curve>();

                Curve crv = curve;
                var off = crv.Offset(Plane.WorldXY, -2, this.Document.ModelAbsoluteTolerance, CurveOffsetCornerStyle.Sharp);
                var off1 = crv.Offset(Plane.WorldXY, 2, this.Document.ModelAbsoluteTolerance, CurveOffsetCornerStyle.Sharp);
                Line l1 = new Line(off[0].PointAtStart, off1[0].PointAtStart);
                Line l2 = new Line(off[0].PointAtEnd, off1[0].PointAtEnd);
                crvs.Add(off[0]);
                crvs.Add(l1.ToNurbsCurve());
                crvs.Add(l2.ToNurbsCurve());
                crvs.Add(off1[0]);
                var join = Curve.JoinCurves(crvs);
                return join[0];
            
            //return base.CurveGeometry;
        }


        protected override void OnDuplicate(RhinoObject source)
        {
            RhinoApp.WriteLine("CustomCurve OnDuplicate");
            base.OnDuplicate(source);

        }

        protected override void OnSpaceMorph(SpaceMorph morph)
        {
            RhinoApp.WriteLine("CustomCurve OnSpaceMorph");
            base.OnSpaceMorph(morph);
            //this.GripsOn = true;
        }
        protected override void OnPicked(PickContext context, IEnumerable<ObjRef> pickedItems)
        {
            base.OnPicked(context, pickedItems);
        }

        protected override void OnTransform(Transform transform)
        {
            RhinoApp.WriteLine("CustomCurve OnTransform");
            base.OnTransform(transform);
        }

        protected override void OnDeleteFromDocument(RhinoDoc doc)
        {

            base.OnDeleteFromDocument(doc);
            RhinoApp.WriteLine("CustomCurve OnDeleteFromDocument");

        }
        protected override void OnDraw(DrawEventArgs e)
        {
            System.Drawing.Color color = base.Attributes.DrawColor(e.RhinoDoc);
            PolylineCurve pl = new PolylineCurve(m_active);
            GeometryBase off = geom(pl);
            e.Display.DrawCurve(off as Curve, System.Drawing.Color.Red);
            base.OnDraw(e);
        }




    }
}
