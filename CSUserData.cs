using Rhino;
using Rhino.Collections;
using Rhino.DocObjects.Custom;
using Rhino.FileIO;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace CustomCurveTest
{
     [Guid("51E53240-A5D7-4DF8-B579-14ED3426223D")]
    public class CSUserData : UserData
    {
        #region Private constants

        /// <summary>
        /// The major and minor version number of this data.
        /// </summary>
        private const int MAJOR_VERSION = 1;
        private const int MINOR_VERSION = 0;

        #endregion

        #region Public properties
        /// <summary>
        /// The notes field
        /// </summary>
        public string Name { get; set; }
        public Curve BaseCurve { get; set; }
        //public Transform UpdateTransformManually { get; set; }

        /// <summary>
        /// Returns true of our user data is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(this.Name) && this.BaseCurve.GetLength() > 0  ;

            }
        }
        #endregion

        #region Userdata overrides

        /// <summary>
        /// Descriptive name of the user data.
        /// </summary>
        public override string Description => this.Name;

        public override string ToString() => String.Format("BaseCurve={0}, Name={1}", this.BaseCurve.ToString(), this.Name);

        protected override void OnDuplicate(UserData source)
        {
            if (source is CSUserData src)
            {
                this.Name = src.Name;
                this.BaseCurve = src.BaseCurve;

            }

        }


        protected override void OnTransform(Transform transformation)
        {


            if (this.BaseCurve != null)
            {

              Rhino.RhinoApp.WriteLine(transformation.ToString());

                Curve basecrv = this.BaseCurve.DuplicateCurve();
                basecrv.Transform(transformation);
                this.BaseCurve = basecrv;
                base.OnTransform(transformation);                
                Rhino.RhinoDoc.ActiveDoc.Objects.UnselectAll();
            }

        }

        public override bool ShouldWrite
        {
            get
            {
                return IsValid;
            }
        }
        protected override bool Read(BinaryArchiveReader archive)
        {
            // Read the chuck version
            archive.Read3dmChunkVersion(out int major, out int minor);
            if (major == MAJOR_VERSION)
            {
                // Read 1.0 fields  here
                if (minor >= MINOR_VERSION)
                {
                    this.Name = archive.ReadString();

                    this.BaseCurve = (Curve)archive.ReadGeometry();

                }
                // Note, if you every roll the minor version number,
                // then read those fields here.
            }
            return !archive.ReadErrorOccured;
        }

        /// <summary>
        /// Writes the content of this data to a stream archive.
        /// </summary>
        protected override bool Write(BinaryArchiveWriter archive)
        {
            // Write the chuck version
            archive.Write3dmChunkVersion(MAJOR_VERSION, MINOR_VERSION);
            // Write 1.0 fields
            archive.WriteString(this.Name);
            // defined somewhere else
            archive.WriteGeometry(this.BaseCurve);
            // Note, if you every roll the minor version number,
            // then write those fields here.
            return !archive.WriteErrorOccured;
        }

        #endregion
    }
}
