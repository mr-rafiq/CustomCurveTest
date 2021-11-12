using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCurveTest
{
    public class CSGripEnabler
    {

        /// <summary>
        /// Delegate function used to turn custom grips on
        /// </summary>
        public void TurnOnGrips(RhinoObject rhObject)
        {
            if (rhObject == null)
                return;
            //CustomUserData ud = rhObject.Attributes.UserData.Find(typeof(CustomUserData)) as CustomUserData;
            //if (ud == null)
            //    return;
            //PolylineCurve polyline_curve = ud.BaseCurve.ToPolyline(0.001,0.001,ud.BaseCurve.Domain.Min,ud.BaseCurve.Domain.Max);
            //if (polyline_curve == null)
            //    return;
            if(typeof(CustomCurve) == rhObject.GetType())
            {
                CustomCurve obj = (CustomCurve)rhObject;
                PolylineCurve pl = new PolylineCurve(obj.m_active); 
                CSGrips rectangle_grips = new CSGrips();
                if (!rectangle_grips.CreateGrips(pl))
                    return;

                rhObject.EnableCustomGrips(rectangle_grips);
            }

        }

    }
}
