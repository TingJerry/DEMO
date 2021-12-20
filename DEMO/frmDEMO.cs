using System;

using System.Windows.Forms;
using HalconDotNet;
using Algorithm;
using Camera;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DEMO
{
    public partial class frmDEMO : Form
    {
        private MainController mController = new MainController();
        private int mLiveAlg = 0;
        private int mLivePar = 0;
        private Stopwatch mSt = new Stopwatch();  //For FPS
        public frmDEMO()
        {
            InitializeComponent();
            //Get algorithm name to Viewer
            var algorithmarr =  (string[])Enum.GetNames(typeof(eAlgorithm));
            cbAlgorithm.Items.Clear();
            for (int i = 0; i < algorithmarr.Length; i++)
            {
                cbAlgorithm.Items.Add(algorithmarr[i]);
            }
            algorithmarr = null;

            cbAlgorithm.SelectedIndex = 0;

            SingletonCam.getInstance().CamModel.Connect();
            SingletonCam.getInstance().CamModel.ImageRecieveEvent += CamModel_ImageRecieveEvent;

            mLiveAlg = cbAlgorithm.SelectedIndex;
            mLivePar = Convert.ToInt32(cbSmoothPar.Items[cbSmoothPar.SelectedIndex]);
        }

        /// <summary>
        /// Get Image event
        /// </summary>
        /// <param name="img"></param>
        private void CamModel_ImageRecieveEvent(HObject img)
        {
            
            double quality = 0;
            HObject smoothimage;

            //Execute
            mController.Execute(img, mLivePar, mLiveAlg, out quality, out smoothimage);

            //showimage
            UpdateShowimg(smoothimage);


            Updatelabel(lbImageQuality, quality);

            img.Dispose();

            ////Get FPS
            mSt.Stop();
            double fps = 1000.0 / mSt.ElapsedMilliseconds;
            Updatelabel(lbFPS, fps);
            mSt.Restart();
        }

        /// <summary>
        /// For offline to load image to calculate contrast
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            if(OFD.ShowDialog()== DialogResult.OK)
            {
                HObject img = new HObject();
                HOperatorSet.ReadImage(out img,OFD.FileName);  //Get image in menory


                //Get parameter
                int algorithmmode = cbAlgorithm.SelectedIndex;
                int par = Convert.ToInt32(cbSmoothPar.Text);
                double quality = 0;
                HObject smoothimage;

                //Execute
                mController.Execute(img,par,algorithmmode,out quality,out smoothimage);

                //showimage
                showimg(smoothimage);


                lbImageQuality.Text = quality.ToString(".0000");

                img.Dispose();
                smoothimage.Dispose();

            }
        }

        

        /// <summary>
        /// Show image with fix the window
        /// </summary>
        /// <param name="img">inputimage</param>
        private void showimg(HObject img)
        {
            try
            {
                HTuple width = 0, height = 0;
                HOperatorSet.GetImageSize(img, out width, out height);
                int winWidth = hWindowControl1.Width;
                int winHeight = hWindowControl1.Height;

                int dispWidth = 0;
                int dispHeight = 0;


                double picWHRatio = 1.0 * width / height;
                double winWHRatio = 1.0 * winWidth / winHeight;
                if (width > winWidth || height > winHeight)
                {              
                    //For width bigger 
                    if (picWHRatio >= winWHRatio)
                    {
                        dispWidth = width.I;
                        dispHeight = (int)(width.D / winWHRatio);                        
                    }

                    //For height bigger 
                    if (picWHRatio < winWHRatio)
                    {
                        dispWidth  = (int)(height.D * winWHRatio);
                        dispHeight = height.I;                        
                    }
                    hWindowControl1.HalconWindow.SetPart(0, 0, dispHeight, dispWidth);
                }

                else
                {

                    //image smaller than window
                    hWindowControl1.HalconWindow.SetPart(0, 0, winHeight, winWidth);
                }
                hWindowControl1.HalconWindow.DispObj(img);

            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Start or Stop the camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "Start")
            {

                Task tLive = Task.Run(() =>
                {
                    SingletonCam.getInstance().CamModel.Live();
                });

                btnStart.Text = "Stop";
            }
            else
            {
                SingletonCam.getInstance().CamModel.Stop();
                btnStart.Text = "Start";
            }

            
          
        }

        /// <summary>
        /// Disconnect Camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmDEMO_FormClosed(object sender, FormClosedEventArgs e)
        {
            SingletonCam.getInstance().CamModel.Disconnect();
        }



        private delegate void dUpdateShowimg(HObject img);
        /// <summary>
        /// Begininvoke the UI
        /// </summary>
        private void UpdateShowimg(HObject img)
        {
            if (hWindowControl1.InvokeRequired)
            {
                hWindowControl1.BeginInvoke(new dUpdateShowimg(UpdateShowimg), new object[] { img });
            }
            else
            {
                showimg(img);
            }
        }

        private delegate void dUpdatelabel(Label lb, double value);
        /// <summary>
        /// Begininvoke the UI
        /// </summary>
        private void Updatelabel(Label lb, double value)
        {
            if (lb.InvokeRequired)
            {
                lb.BeginInvoke(new dUpdatelabel(Updatelabel), new object[] { lb,value });

            }
            else
            {
                lb.Text = value.ToString(".0000");

            }
        }

        /// <summary>
        /// Set Live algorithm mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            mLiveAlg = cbAlgorithm.SelectedIndex;
        }

        /// <summary>
        /// Set Live pamareter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSmoothPar_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                mLivePar = Convert.ToInt32(cbSmoothPar.Items[cbSmoothPar.SelectedIndex]);
            }
            catch (Exception)
            {

            }
        }
    }
}
