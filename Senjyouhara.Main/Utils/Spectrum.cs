using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Senjyouhara.Main.Utils
{
    internal class Spectrum
    {
        private static readonly int maxColums = 128;
        private static readonly int Y0 = 1 << ((FastFourierTransform.FFT_N_LOG + 3) << 1);
        private static readonly double logY0 = Math.Log10(Y0); //lg((8*FFT_N)^2)
        private readonly int band;
        private readonly int width, height;
        private int index;
        private readonly int[] xplot, lastPeak, lastY;
        private int deltax;
        private long lastTimeMillis;
        private readonly Bitmap spectrumImage, barImage;
        private readonly Graphics spectrumGraphics;
        private bool isAlive;

        public Spectrum()
        {

            isAlive = true;
            band = 64;      //64段
            width = 1920;    //频谱窗口 383x124
            height = 400;
            lastTimeMillis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            xplot = new int[maxColums + 1];
            lastPeak = new int[maxColums];
            lastY = new int[maxColums];
            spectrumImage = new Bitmap(width, height);
            spectrumGraphics = Graphics.FromImage(spectrumImage);
            //setPreferredSize(new Dimension(width, height));
            setPlot();
            barImage = new Bitmap(deltax - 1, height);

            setColor(0x7f7f7f, 0xff0000, 0xffff00, 0x7f7fff);
        }

        public void setColor(int rgbPeak, int rgbTop, int rgbMid, int rgbBot)
        {
            
            Color crPeak =  ColorTranslator.FromWin32(rgbPeak);
            //spectrumGraphics.Clear(crPeak);
            spectrumGraphics.Clear(Color.Gray);

            Graphics g = Graphics.FromImage(barImage);
            Color crTop = ColorTranslator.FromWin32(rgbTop);
            Color crMid = ColorTranslator.FromWin32(rgbMid);
            Color crBot = ColorTranslator.FromWin32(rgbBot);

            LinearGradientBrush gp1 = new(new(0, 0, deltax - 1, height), crTop, crMid, LinearGradientMode.Vertical);

            // 定义用于在多色渐变中以内插值取代颜色混合的颜色和位置的数组
           ColorBlend _colorBlend = new();
            //定义多种颜色
             _colorBlend.Colors = new[] {
                 crTop,
                 crMid,
                 crBot };
            _colorBlend.Positions = new float[] { 0 / 3f, 1 / 2f, 3 / 3f };
                         //设置多色渐变颜色
             gp1.InterpolationColors = _colorBlend;

            g.FillRectangle(gp1, new(0, 0, deltax - 1, height));

            //barImage.Save($@"D:\bar.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        private void setPlot()
        {
            deltax = (width - band + 1) / band + 1;

            // 0-16kHz分划为band个频段，各频段宽度非线性划分。
            for (int i = 0; i <= band; i++)
            {
                xplot[i] = 0;
                xplot[i] = (int)(0.5 + Math.Pow(FastFourierTransform.FFT_N >> 1, (double)i / band));
                if (i > 0 && xplot[i] <= xplot[i - 1])
                    xplot[i] = xplot[i - 1] + 1;
            }
        }

        /**
	 * 绘制"频率-幅值"直方图并显示到屏幕。
	 * @param amp amp[0..FFT.FFT_N/2-1]为频谱"幅值"(用复数模的平方)。
	 */
        private void drawHistogram(float[] amp)
        {
            spectrumGraphics.Clear(Color.Gray);

            long t = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            int speed = (int)(t - lastTimeMillis) / 30; //峰值下落速度
            lastTimeMillis = t;

            //int speed = (int)(100) / 30; //峰值下落速度


            int i = 0, x = 0, y, xi, peaki, w = deltax - 1;
            float maxAmp;
            for (; i != band; i++, x += deltax)
            {
                // 查找当前频段的最大"幅值"
                maxAmp = 0; xi = xplot[i]; y = xplot[i + 1];
                for (; xi < y; xi++)
                {
                    if (amp[xi] > maxAmp)
                        maxAmp = amp[xi];
                }

                /*
                 * maxAmp转换为用对数表示的"分贝数"y:
                 * y = (int) Math.sqrt(maxAmp);
                 * y /= FFT.FFT_N; //幅值
                 * y /= 8;	//调整
                 * if(y > 0) y = (int)(Math.log10(y) * 20 * 2);
                 * 
                 * 为了突出幅值y显示时强弱的"对比度"，计算时作了调整。未作等响度修正。
                 */
                y = (maxAmp > Y0) ? (int)((Math.Log10(maxAmp) - logY0) * 20) : 0;

                // 使幅值匀速度下落
                lastY[i] -= speed << 2;
                if (y < lastY[i])
                {
                    y = lastY[i];
                    if (y < 0) y = 0;
                }
                lastY[i] = y;

                if (y >= lastPeak[i])
                {
                    lastPeak[i] = y;
                }
                else
                {
                    // 使峰值匀速度下落
                    peaki = lastPeak[i] - speed;
                    if (peaki < 0)
                        peaki = 0;
                    lastPeak[i] = peaki;
                    peaki = height - peaki;
                    var color = barImage.GetPixel(0, peaki);
                    spectrumGraphics.DrawLine(new Pen(color), x, peaki, x+w-1, peaki);
                }
                // 画当前频段的直方图
                y = height - y;
                spectrumGraphics.DrawImage(barImage, x,y, new RectangleF(0, y, w, height), GraphicsUnit.Pixel);
            }

            // 刷新到屏幕
            //repaint(0, 0, width, height);
            spectrumImage.Save($@"e:\img\test-{index+1}.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        public void run(string filename)
        {
            WaveFileReader wi = new(filename);

            if (!wi.Success)
            {
                return;
            }

            FastFourierTransform fft = new();
            byte[] b = new byte[FastFourierTransform.FFT_N << 1];
            //int i, j;
            var readIndex = 0;
            //try
            //{
            //while (isAlive)
            //{

            var m = 10;

                    // 从混音器录制数据并转换为short类型的PCM
                    //wi.read(b, FastFourierTransform.FFT_N << 1);
                    //wi.getWave264(b, FFT.FFT_N << 1);//debug
                    //for (i = j = 0; i != FastFourierTransform.FFT_N; i++, j += 2)
                        //realIO[i] = (b[j + 1] << 8) | (b[j] & 0xff); //signed short
                        for(int s=0;s<m;s++)
            {
                Thread.Sleep(80);// 延时不准确,这不重要

                float[] realIO = new float[512];


                for (int i = 0; i != 512; i++)
                {
                    //realIO[i * 2] = wi.Data[0][readIndex++];
                    realIO[i] = wi.Data[1][readIndex++];
                }

                // 时域PCM数据变换到频域,取回频域幅值
                fft.calculate(realIO);

                // 绘制
                drawHistogram(realIO);

                index++;
            }

                //}

            //}
            //catch (ThreadInterruptedException e)
            //{
            //    throw e;
            //}
        }

        public void stop()
        {
            isAlive = false;
        }

    }
}
