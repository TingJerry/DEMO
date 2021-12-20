using System;
using HalconDotNet;
using Algorithm;

namespace DEMO
{
    public class MainController
    {

        public int Execute(HObject inputimg,int par,int algorithmmode,out double quality,out HObject smoothimg)
        {
            smoothimg = null;

            AlgorithmExcutor AE = new AlgorithmExcutor();
            try
            {
                AE.SmoothImage((eAlgorithm)algorithmmode, par, inputimg, out smoothimg);

                quality = 0;
                AE.CalQuality(smoothimg, out quality);

                //AE = null;

                return 0;
            }
            catch (Exception)
            {
                quality = 0;
                return -1;
            }
        }
    }
}
