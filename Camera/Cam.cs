using System;
using HalconDotNet;

namespace Camera
{
    public class Cam : ICamera
    {
        public delegate void dImageRecieve(HObject img);
        public event dImageRecieve ImageRecieveEvent;

        private HTuple mCamHandle;
        private bool mIsLive = false;

        /// <summary>
        /// Connect Camera(For microsoft cam)
        /// </summary>
        /// <returns></returns>
        public int Connect()
        {
            try
            {
                HOperatorSet.OpenFramegrabber("DirectShow", 1, 1, 0, 0, 0, 0, "default", 8, "rgb", -1, "false", "default", "[0] Microsoft® LifeCam HD-3000", 0, -1,out mCamHandle);
                HOperatorSet.GrabImageStart(mCamHandle, -1);
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Disconnect the camera
        /// </summary>
        /// <returns></returns>
        public int Disconnect()
        {
            try
            {
                HOperatorSet.CloseFramegrabber(mCamHandle);

                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Start Live
        /// </summary>
        /// <returns></returns>
        public int Live()
        {
            try
            {
                if (mIsLive)
                    return 0;


                mIsLive = true;
                while (mIsLive)
                {
                    HObject img = new HObject();
                    HOperatorSet.GrabImage(out img, mCamHandle);
                    ImageRecieveEvent?.Invoke(img);
                    
                }
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }

        }

        /// <summary>
        /// Grab one image
        /// </summary>
        /// <returns></returns>
        public int Grab()
        {
            try
            {
                HObject img = new HObject();
                HOperatorSet.GrabImage(out img, mCamHandle);
                ImageRecieveEvent?.Invoke(img);
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }

        }

        /// <summary>
        /// Stop Live
        /// </summary>
        public void Stop()
        {           
            mIsLive = false;
        }
    }
}
