using System;
using HalconDotNet;

namespace Algorithm
{
    /// <summary>
    /// There are four algorithms to calculate contrast
    /// </summary>
    public enum eAlgorithm
    {
        Origin = 0,
        Median,
        Mean,
        Gauss
    }
    public class AlgorithmExcutor
    {
        /// <summary>
        /// Execute the algorithm to smooth the image
        /// </summary>
        /// <param name="mode">algorithm mode</param>
        /// <param name="par">algorithm parameter,more bigger then more smooth</param>
        /// <param name="inimg">input image</param>
        /// <returns></returns>
        public int SmoothImage(eAlgorithm mode,int par, HObject inimg,out HObject outimg)
        {
            outimg = new HObject();
            if (inimg == null)
            {
                outimg.Dispose();
                return -2;
            }

            try
            {

                //Do smooth
                switch (mode)
                {
                    case eAlgorithm.Origin:
                        outimg = inimg.CopyObj(1, -1);
                        break;
                    case eAlgorithm.Median:
                        Median(par, inimg,out outimg);
                        break;
                    case eAlgorithm.Mean:
                        Mean(par, inimg, out outimg);
                        break;
                    case eAlgorithm.Gauss:
                        Gauss(par, inimg, out outimg);
                        break;
                }
                return 0;
            }
            catch (Exception ex)
            {
                outimg.Dispose();
                return -1;
            }
        }

        /// <summary>
        /// Calculate the quality of image
        /// </summary>
        /// <param name="inimg">input image</param>
        /// <param name="quality">output quality value</param>
        /// <returns></returns>
        public int CalQuality(HObject inimg,out double quality)
        {
            quality = 0;
            HObject rect = new HObject();
            HObject sobelimg = new HObject();

            try
            {
                //local variable initial
                HTuple width = 0, height = 0,mean = 0,dev = 0;
                HOperatorSet.GetImageSize(inimg,out width,out height);
                HOperatorSet.GenRectangle1(out rect,0,0,height,width);
                HOperatorSet.SobelAmp(inimg, out sobelimg, "sum_abs", 3);
                HOperatorSet.Intensity(rect, sobelimg, out mean, out dev);
                quality = mean;

                return 0;
            }
            catch (Exception ex)
            {
                return -1;
            }
            finally
            {
                sobelimg.Dispose();
                rect.Dispose();
            }
        }

        /// <summary>
        /// Median algorithm to smooth image
        /// </summary>
        /// <param name="par"></param>
        /// <param name="inimg"></param>
        /// <param name="outimg"></param>
        private void Median(int par, HObject inimg, out HObject outimg)
        {
            HOperatorSet.MedianImage(inimg, out outimg, "circle", par, "mirrored");
        }

        /// <summary>
        /// Mean algorithm to smooth image
        /// </summary>
        /// <param name="par"></param>
        /// <param name="inimg"></param>
        /// <param name="outimg"></param>
        private void Mean(int par, HObject inimg, out HObject outimg)
        {
            HOperatorSet.MeanImage(inimg, out outimg, par, par);
        }

        /// <summary>
        /// Gauss algorithm to smooth image
        /// </summary>
        /// <param name="par"></param>
        /// <param name="inimg"></param>
        /// <param name="outimg"></param>
        private void Gauss(int par, HObject inimg, out HObject outimg)
        {
            HOperatorSet.GaussImage(inimg, out outimg,par);
        }

         
    }
}
