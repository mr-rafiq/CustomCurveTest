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
   public  class CustomCurve : CustomCurveObject
    {

        //    public MyPoint(){}
       public  ObjectAttributes attributes { get; set; }
       public Point3d[] m_active{ get; set; }
        public CustomCurve() : base() { }
        public CustomCurve(Point3d[] m_active, ObjectAttributes attributes) : base()
        {
            this.m_active = m_active;
            this.Attributes = attributes;
            this.attributes = attributes;
            this.GripsOn = true;
            CommitChanges();
        }



        public override string ShortDescription(bool plural)
        {
            return plural ? "CustomCurves" : "CustomCurve";
        }

        protected override void OnAddToDocument(RhinoDoc doc)
        {

            this.Attributes = this.attributes;
            RhinoApp.WriteLine("OnAddToDocument Rhino");
            Curve cr = new PolylineCurve(this.m_active);

            //doc.Objects.AddRhinoObject(this, cr);
            doc.Views.Redraw();
            //base.OnAddToDocument(doc);


        }


        protected override void OnSelectionChanged()
        {
            RhinoApp.WriteLine("OnSelectionChanged");
            //if (IsSelected(false) == 2)
            //{
            //    this.GripsOn = true;
            //    this.Document.Views.Redraw();
            //}
            
            base.OnSelectionChanged();
        }
        protected override void OnDuplicate(RhinoObject source)
        {
            RhinoApp.WriteLine("OnDuplicate");
            base.OnDuplicate(source);

        }



        protected override void OnSpaceMorph(SpaceMorph morph)
        {
            RhinoApp.WriteLine("OnSpaceMorph");
            base.OnSpaceMorph(morph);
            //this.GripsOn = true;
        }
        protected override void OnPicked(PickContext context, IEnumerable<ObjRef> pickedItems)
        {
            base.OnPicked(context, pickedItems);
        }

        protected override void OnTransform(Transform transform)
        {
            RhinoApp.WriteLine("OnTransform");
            base.OnTransform(transform);
        }

        protected override void OnDeleteFromDocument(RhinoDoc doc)
        {

            base.OnDeleteFromDocument(doc);
            RhinoApp.WriteLine("OnDeleteFromDocument");

        }
        protected override void OnDraw(DrawEventArgs e)
        {
            //      e.Display.DrawCircle(new Circle(this.PointGeometry.Location, 10.0), Color.Crimson);
            System.Drawing.Color color = this.Attributes.DrawColor(e.RhinoDoc);
            var off = this.CurveGeometry.Offset(Plane.WorldXY, 10, 0.001, CurveOffsetCornerStyle.Sharp);
            e.Display.DrawCurve(off[0], System.Drawing.Color.Red);
            //this.GripsOn = true;
            base.OnDraw(e);
        }




    }
}
