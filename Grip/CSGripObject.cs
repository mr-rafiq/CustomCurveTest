using Rhino.DocObjects.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomRhinoObjects
{
    public class CSGripObject : CustomGripObject
    {

        /// <summary>
        /// True if grip motion can change dimension
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CSGripObject()
        {
            
            Active = true;
            
        }

        /// <summary>
        /// RhinoObject override
        /// </summary>
        public override string ShortDescription(bool plural)
        {
            return plural ? "CurveGrips" : "CurveGrip";
        }
        
    }
}
