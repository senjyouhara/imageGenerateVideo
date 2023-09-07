using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Main.Utils
{

    public class WaveConstants
    {
        static public readonly int LENCHUNKDESCRIPTOR = 4;
        static public readonly int LENCHUNKSIZE = 4;
        static public readonly int LENWAVEFLAG = 4;
        static public readonly int LENFMTSUBCHUNK = 4;
        static public readonly int LENSUBCHUNK1SIZE = 4;
        static public readonly int LENAUDIOFORMAT = 2;
        static public readonly int LENNUMCHANNELS = 2;
        static public readonly int LENSAMPLERATE = 2;
        static public readonly int LENBYTERATE = 4;
        static public readonly int LENBLOCKLING = 2;
        static public readonly int LENBITSPERSAMPLE = 2;
        static public readonly int LENDATASUBCHUNK = 4;
        static public readonly int LENSUBCHUNK2SIZE = 4;

        public static readonly string CHUNKDESCRIPTOR = "RIFF";
        public static readonly string WAVEFLAG = "WAVE";
        public static readonly string FMTSUBCHUNK = "fmt ";
        public static readonly string DATASUBCHUNK = "data";
    }

    public class WaveFileReader
    {
        private string filename = null;
        private int[][] data = null;
        private byte[][][] originData = null;

        private int len = 0;

        private string chunkdescriptor = null;
        private long chunksize = 0;
        private string waveflag = null;
        private string fmtsubchunk = null;
        private long subchunk1size = 0;
        private int audioformat = 0;
        private int numchannels = 0;
        private long samplerate = 0;
        private long byterate = 0;
        private int blockalign = 0;
        private int bitspersample = 0;
        private string datasubchunk = null;
        private long subchunk2size = 0;
        private float timeTotal = 0;
        private FileStream fis = null;
        private BufferedStream bis = null;

        private bool issuccess = false;

        public WaveFileReader(string filename)
        {

            initReader(filename);
        }

        public float TimeTotal { get =>  timeTotal; }

        // 判断是否创建wav读取器成功
        public bool Success { get => issuccess; }
        public int BitPerSample { get => bitspersample; }
        // 获取采样率
        public long SampleRate { get => samplerate; }
        // 获取声道个数，1代表单声道 2代表立体声
        public int NumChannels { get => numchannels; }
        // 获取数据长度，也就是一共采样多少个
        public int Length { get => len; }
        // 获取数据
        // 数据是一个二维数组，[n][m]代表第n个声道的第m个采样值
        public int[][] Data { get => data; }
        public byte[][][] OriginData { get => originData; }


        private void initReader(string filename)
        {
            this.filename = filename;


            using (bis = new BufferedStream(new FileStream(this.filename, FileMode.Open)))
            {

                //4
                chunkdescriptor = readString(WaveConstants.LENCHUNKDESCRIPTOR);
                if (!chunkdescriptor.EndsWith("RIFF"))
                    throw new Exception("RIFF miss, " + filename + " is not a wave file.");

                //4
                chunksize = readLong() + 8;
                //4
                waveflag = readString(WaveConstants.LENWAVEFLAG);
                if (!waveflag.EndsWith("WAVE"))
                    throw new Exception("WAVE miss, " + filename + " is not a wave file.");
                //4
                fmtsubchunk = readString(WaveConstants.LENFMTSUBCHUNK);
                if (!fmtsubchunk.EndsWith("fmt "))
                    throw new Exception("fmt miss, " + filename + " is not a wave file.");
                //4
                subchunk1size = readLong();
                //2
                audioformat = readInt();
                //2
                numchannels = readInt();
                //4
                samplerate = readLong();
                byterate = readLong();
                blockalign = readInt();
                bitspersample = readInt();

                datasubchunk = readString(WaveConstants.LENDATASUBCHUNK);
                if (datasubchunk.EndsWith("LIST"))
                {
                    int listSize = readInt();
                    readBytes(listSize + 2);
                    datasubchunk = readString(WaveConstants.LENDATASUBCHUNK);
                }
                if (!datasubchunk.EndsWith("data"))
                    throw new Exception("data miss, " + filename + " is not a wave file.");
                subchunk2size = readLong();

                len = (int)(subchunk2size / (bitspersample / 8) / numchannels);

                data = new int[numchannels][];
                originData = new byte[numchannels][][];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = new int[len];
                    originData[i] = new byte[len][];
                }

                // 读取数据
                for (int i = 0; i < len; ++i)
                {
                    for (int n = 0; n < numchannels; ++n)
                    {
                        if (bitspersample == 8)
                        {
                            var b = new byte[1];
                            bis.Read(b, 0, b.Length);
                            data[n][i] = b[0];
                            originData[n][i] = b;
                        }
                        else if (bitspersample == 16)
                        {
                            byte[] buf = new byte[4];

                            if (bis.Read(buf, 0, 2) != 2)
                                throw new IOException("no more data!!!");

                            originData[n][i] = buf;
                            data[n][i] = (buf[0] & 0xFF) | (buf[1] << 8);
                        }
                    }
                }

                timeTotal = originData[0].Length / samplerate;

                issuccess = true;
            }
        }


        private string readString(int len)
        {
            byte[] buf = new byte[len];

                if (bis.Read(buf, 0, buf.Length) != len)
                    throw new IOException("no more data!!!");
            return Encoding.UTF8.GetString(buf);
        }

        private int readInt()
        {
            byte[] buf = new byte[2];
            int res;
                if (bis.Read(buf, 0, buf.Length) != 2)
                    throw new IOException("no more data!!!");
                res = (buf[0] & 0xFF) | ((buf[1]) << 8);
            return res;
        }

        private long readLong()
        {
            long res;
                long[] l = new long[4];
                for (int i = 0; i < 4; ++i)
                {
                    var buf = new byte[1];
                    l[i] = bis.Read(buf, 0, 1);
                    if (l[i] == -1)
                    {
                        throw new IOException("no more data!!!");
                    }
                l[i] = buf[0];
                }
                res = l[0] | (l[1] << 8) | (l[2] << 16) | (l[3] << 24);
            return res;
        }

        private byte[] readBytes(int len)
        {
            byte[] buf = new byte[len];

                if (bis.Read(buf, 0, buf.Length) != len)
                    throw new IOException("no more data!!!");
            return buf;
        }

        public static int[] readSingleChannel(string filename)
        {
            if (filename == null || filename.Length == 0)
            {
                return null;
            }

            WaveFileReader reader = new(filename);
            int[] res = reader.Data[0];
            return res;

        }

    }
}
