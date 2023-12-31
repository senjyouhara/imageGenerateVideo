﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Main.Utils
{
    internal class FastFourierTransform
    {
        public static readonly int FFT_N_LOG = 9; // FFT_N_LOG <= 13
        public static readonly int FFT_N = 1 << FFT_N_LOG;
        private static readonly float MINY = (float)((FFT_N << 2) * Math.Sqrt(2)); //(*)
        private readonly float[] real, imag, sintable, costable;
        private readonly int[] bitReverse;

        public FastFourierTransform()
        {
            real = new float[FFT_N];
            imag = new float[FFT_N];
            sintable = new float[FFT_N >> 1];
            costable = new float[FFT_N >> 1];
            bitReverse = new int[FFT_N];

            int i, j, k, reve;
            for (i = 0; i < FFT_N; i++)
            {
                k = i;
                for (j = 0, reve = 0; j != FFT_N_LOG; j++)
                {
                    reve <<= 1;
                    reve |= (k & 1);
                    k = k >= 0 ? k >> 1 : int.MaxValue - Math.Abs(k-1);
                }
                bitReverse[i] = reve;
            }

            double theta, dt = 2 * 3.14159265358979323846 / FFT_N;
            for (i = 0; i < (FFT_N >> 1); i++)
            {
                theta = i * dt;
                costable[i] = (float)Math.Cos(theta);
                sintable[i] = (float)Math.Sin(theta);
            }
        }

        /**
         * 用于频谱显示的快速傅里叶变换
         * @param realIO 输入FFT_N个实数，也用它暂存fft后的FFT_N/2个输出值(复数模的平方)。
         */
        public void calculate(float[] realIO)
        {
            int i, j, k, ir, exchanges = 1, idx = FFT_N_LOG - 1;
            float cosv, sinv, tmpr, tmpi;
            for (i = 0; i != FFT_N; i++)
            {
                real[i] = realIO[bitReverse[i]];
                imag[i] = 0;
            }

            for (i = FFT_N_LOG; i != 0; i--)
            {
                for (j = 0; j != exchanges; j++)
                {
                    cosv = costable[j << idx];
                    sinv = sintable[j << idx];
                    for (k = j; k < FFT_N; k += exchanges << 1)
                    {
                        ir = k + exchanges;
                        tmpr = cosv * real[ir] - sinv * imag[ir];
                        tmpi = cosv * imag[ir] + sinv * real[ir];
                        real[ir] = real[k] - tmpr;
                        imag[ir] = imag[k] - tmpi;
                        real[k] += tmpr;
                        imag[k] += tmpi;
                    }
                }
                exchanges <<= 1;
                idx--;
            }

            j = FFT_N >> 1;
            /*
             * 输出模的平方(的FFT_N倍):
             * for(i = 1; i <= j; i++)
             * 	realIO[i-1] = real[i] * real[i] +  imag[i] * imag[i];
             * 
             * 如果FFT只用于频谱显示,可以"淘汰"幅值较小的而减少浮点乘法运算. MINY的值
             * 和Spectrum.Y0,Spectrum.logY0对应.
             */
            sinv = MINY;
            cosv = -MINY;
            for (i = j; i != 0; i--)
            {
                tmpr = real[i];
                tmpi = imag[i];
                if (tmpr > cosv && tmpr < sinv && tmpi > cosv && tmpi < sinv)
                    realIO[i - 1] = 0;
                else
                    realIO[i - 1] = tmpr * tmpr + tmpi * tmpi;
            }
        }
        }
    }
