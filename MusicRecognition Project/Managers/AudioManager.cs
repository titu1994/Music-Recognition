using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NAudio.Dsp;
using NAudio.Wave;

namespace MusicRecognition_Project
{
    class AudioManager
    {
        private WaveIn waveSource;
        private byte[] dataBuffer;
        private const int UPPER_LIMIT = 300;
        private const int LOWER_LIMIT = 40;

        private DatabaseHandler db;

        private readonly int[] RANGE = new int[] { 40, 80, 120, 180, UPPER_LIMIT + 1 };

        public void InitWaveSource()
        {
            if (waveSource == null)
            {
                waveSource = new WaveIn();
                waveSource.WaveFormat = new WaveFormat(44100, 8, 1);

                waveSource.DataAvailable += WaveSourceOnDataAvailable;
                waveSource.RecordingStopped += WaveSourceOnRecordingStopped;

                dataBuffer = new byte[4096];
                db = new DatabaseHandler();
                db.InitDatabase();
            }
        }

        public void StartRecording()
        {
            waveSource?.StartRecording();
        }

        public void StopRecording()
        {
            waveSource?.StopRecording();
            waveSource = null;
        }

        private void WaveSourceOnDataAvailable(object sender, WaveInEventArgs e)
        {
            // Do important calculation work here
            // To get data, e.Buffer and e.BytesRecorded must be used
        }

        private void WaveSourceOnRecordingStopped(object sender, StoppedEventArgs stoppedEventArgs)
        {
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }
        }

        // Find out in which range
        private int GetIndex(int freq)
        {
            int i = 0;
            while (RANGE[i] < freq)
                i++;
            return i;
        }

        private int FUZ_FACTOR = 2;

        private long hash(long p1, long p2, long p3, long p4)
        {
            return (p4 - (p4 % FUZ_FACTOR)) * 100000000 + (p3 - (p3 % FUZ_FACTOR))
                    * 100000 + (p2 - (p2 % FUZ_FACTOR)) * 100
                    + (p1 - (p1 % FUZ_FACTOR));
        }

        void makeSpectrum(byte[] output, long songId, bool isMatching)
        {
            byte[] audio = output;
            int totalSize = audio.Length;

            int amountPossible = totalSize / 4096;
            
            Complex[][] results = new Complex[amountPossible][];

            // For all the chunks:
            int m = 12;

            for (int times = 0; times < amountPossible; times++)
            {
                Complex[] complex = new Complex[4096];
                for (int i = 0; i < 4096; i++)
                {
                    // Put the time domain data into a complex number with imaginary
                    // part as 0:
                    complex[i] = new Complex() {X = audio[(times * 4096) + i] , Y = 0};
                }
                // Perform FFT analysis on the chunk:
                FastFourierTransform.FFT(true, m, complex);
                results[times] = complex;
            }
            DetermineKeyPoints(results, songId, isMatching);
           /* JFrame spectrumView = new SpectrumView(results, 4096, highscores,
                    recordPoints);
            spectrumView.setVisible(true);*/
        }

        private void DetermineKeyPoints(Complex[][] results, long songId, bool isMatching)
        {
            StreamWriter outFile = null;
            try
            {
                outFile = new StreamWriter("result.txt");
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1);
            }

            double[,] highscores = new double[results.Length, 5];
            for (int i = 0; i < results.Length; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    highscores[i,j] = 0;
                }
            }

            double[,] recordPoints = new double[results.Length, UPPER_LIMIT];
            for (int i = 0; i < results.Length; i++)
            {
                for (int j = 0; j < UPPER_LIMIT; j++)
                {
                    recordPoints[i,j] = 0;
                }
            }

            long[,] points = new long[results.Length,5];
            for (int i = 0; i < results.Length; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    points[i,j] = 0;
                }
            }
            float x, y;
            double abs;

            for (int t = 0; t < results.Length; t++)
            {
                for (int freq = LOWER_LIMIT; freq < UPPER_LIMIT - 1; freq++)
                {
                    // Get the magnitude:
                    x = results[t][freq].X;
                    y = results[t][freq].Y;
                    abs = Math.Sqrt(x*x + y*y);
                    double mag = Math.Log(abs + 1);

                    // Find out which range we are in:
                    int index = GetIndex(freq);

                    // Save the highest magnitude and corresponding frequency:
                    if (mag > highscores[t,index])
                    {
                        highscores[t,index] = mag;
                        recordPoints[t,freq] = 1;
                        points[t,index] = freq;
                    }
                }

                try
                {
                    for (int k = 0; k < 5; k++)
                    {
                        outFile.Write("" + highscores[t,k] + ";"
                                + recordPoints[t,k] + "\t");
                    }
                    outFile.Write("\n");
                    outFile.Flush();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                long h = hash(points[t,0], points[t,1], points[t,2],
                        points[t,3]);

                if (isMatching)
                {
                    List<DataPoint> listPoints;

                    if ((listPoints = db.GetDataPoints(h)) != null)
                    {
                        foreach (DataPoint dP in listPoints)
                        {
                            int offset = Math.Abs(dP.time - t);
                            Dictionary<int, int> tmpMap = null;
                            if ((tmpMap = db.GetSong(dP.songId)) == null)
                            {
                                tmpMap = new Dictionary<int, int>();
                                tmpMap[offset] = 1;
                                db.InsertSong(dP.songId, tmpMap);
                            }
                            else
                            {
                                int count = tmpMap[offset];
                                if (count == null)
                                {
                                    tmpMap[offset] = 1;
                                }
                                else
                                {
                                    tmpMap[offset] = count + 1;
                                }
                            }
                        }
                    }
                }
                else
                {
                    List<DataPoint> listPoints = null;
                    if ((listPoints = db.GetDataPoints(h)) == null)
                    {
                        listPoints = new List<DataPoint>();
                        DataPoint point = new DataPoint() {songId = (int)songId, time = t };
                        listPoints.Add(point);
                        db.InsertDataPoint(h, listPoints);
                    }
                    else
                    {
                        DataPoint point = new DataPoint() { songId = (int)songId, time = t };
                        listPoints.Add(point);
                    }
                }
            }
            try
            {
                outFile?.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }

        }



    }
}
