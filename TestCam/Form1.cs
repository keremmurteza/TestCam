using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.IO.Ports;

namespace TestCam
{
    public partial class Form1 : Form
    {
        private Capture capture;
        private Image<Bgr, Byte> IMG;
        private Image<Bgr, Byte> IMG_Post;
        private Image<Gray, Byte> BWImg;
        private Image<Gray, Byte> R_frame;
        private Image<Gray, Byte> G_frame;
        private Image<Gray, Byte> B_frame;
        private Image<Gray, Byte> GrayImg;

        private double myXScale = 100.0 / 640;
        private int Xpx, Ypx, N;
        private double Xcm, Ycm, d1 = 16.0, Px, Py, Pz;
        double Zcm = 83.0;

        

        private double Th1, Th2;

        static SerialPort _serialPort;
        public byte[] Buff = new byte[2];



        public Form1()
        {
            InitializeComponent();
            _serialPort = new SerialPort();
            _serialPort.PortName = "COM3";//Set your board COM
            _serialPort.BaudRate = 9600;
            _serialPort.Open();
        }
        
        private void processFrame(object sender, EventArgs e)
        {
            if (capture == null)
            {
                try
                {
                    capture = new Capture(); 
                }
                catch (NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }
            Xpx = 0; Ypx = 0; N = 0;
            IMG = capture.QueryFrame();
            GrayImg = IMG.Convert<Gray, Byte>();
            BWImg = GrayImg.ThresholdBinaryInv(new Gray(25), new Gray(275));
            IMG_Post = IMG.CopyBlank();
            
            
            R_frame = IMG[2].Copy();
            G_frame = IMG[1].Copy();
            B_frame = IMG[0].Copy();            
            GrayImg = IMG.Convert<Gray, Byte>();
      
            try
            {
                
                imageBox1.Image = IMG;
                imageBox2.Image = GrayImg;
                imageBox3.Image = BWImg;
                imageBox4.Image = G_frame;
                imageBox5.Image = B_frame;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            for (int i = 0; i < BWImg.Width; i++)
            {
                for (int j = 0; j < BWImg.Height; j++)
                {
                    if (BWImg[j, i].Intensity > 128)
                    {
                        N++;
                        Xpx += i;
                        Ypx += j;
                    }
                }
            }

            if (N > 0)// if there is an object
            {
                // the center point of the forground of object in pixels
                Xpx = Xpx / N;
                Ypx = Ypx / N;

                Xpx = Xpx - BWImg.Width/2;
                Ypx = BWImg.Height/2 - Ypx ;
                // the center point of the forground of object in centimeters

                Xcm = Xpx * myXScale;
                Ycm = Ypx * myXScale;

                textBox1.Text = Xcm.ToString();
                textBox2.Text = Ycm.ToString();
                textBox3.Text = N.ToString();
                
                Px = Zcm;
                Py = -Xcm;
                Pz =  Py-d1;

                // Inverse K. model
                double Th2r = Math.Atan(Py / Px);
                double Th1r = Math.Atan(Math.Sin(Th2r)*(Ycm-d1)/-Xcm);

                Th1 = (Th1r * 180) / Math.PI;
                Th2 = ((Th2r * 180) / Math.PI);

                Th1 = Th1 + 110;
                Th2 = Th2 + 90;


                textBox4.Text = Th1.ToString();
                textBox5.Text = Th2.ToString();
                textBox6.Text = Px.ToString();
                textBox7.Text = Py.ToString();



                Buff[0] = (byte)Th1;
                Buff[1] = (byte)Th2;
                _serialPort.Write(Buff, 0, 2);

            }
            else
            {
                textBox1.Text = Xpx.ToString();
                textBox2.Text = Ypx.ToString();
                textBox3.Text = N.ToString();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Application.Idle += processFrame;
           // timer1.Enabled = true;
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Idle -= processFrame;
            //timer1.Enabled = false;
            button1.Enabled = true;
            button2.Enabled = false;
        }    

        private void button3_Click(object sender, EventArgs e)
        {
            IMG.Save("D:\\Image" + ".jpg");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            processFrame(sender,e);
        }

    }
}
