using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera
{
    /// <summary>
    /// Fro Camera singleton
    /// </summary>
    public class SingletonCam
    {
        private static SingletonCam mSingletonCamera;
        private static Cam mCameara;

        public SingletonCam()
        {
            mCameara = new Cam();
        }

        public static SingletonCam getInstance()
        {
            if (mSingletonCamera == null)
                mSingletonCamera = new SingletonCam();
            return mSingletonCamera;
        }

        public Cam CamModel { get { return mCameara; } }
    }
}
