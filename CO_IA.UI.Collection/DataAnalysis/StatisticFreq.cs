using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using CO_IA.UI.Collection.Model;
using System.Windows.Forms;
using CO_IA.UI.Collection.DbEntity;
using CO_IA.Data.Collection;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Charts;
using ZedGraph;
using System.Windows.Media;
using System.Reflection;
using CO_IA_Data;
using DevExpress.Xpf.NavBar;

namespace CO_IA.UI.Collection.DataAnalysis
{
    public class StatisticFreq
    {
        public SaveAanalysisDataDelegate saveAanalysisDataDelegate;
        public RefreshAnalysisResultDelete refreshAnalysisResultDelete;
        public RefreshChartResultDelete refreshChartResultDelete;
        public RefreshChartFreqStatDelete refreshChartFreqStatDelete;
        public ReminderBoxDelegate reminderBoxDelegate;
        public ArrayList fileNameList;
        public int freqCount;
        public string MeasureId;
        public FreqNavBar freqNavBar;

        public List<short> AmplitudeMaxValue = null;
        public List<short> AmplitudeAverageValue = null;
        public double bandWidth;
        public bool isRefresh;

        public bool ShowUI = false;

        public StatisticFreq()
        {

        }

        public void initStatisticData()
        {
            //getFileNames();
            readFileData();
        }

        /// <summary>
        /// 读取分析文件
        /// </summary>
        /// <param name="fileAddr"></param>
        public void readAnalysisFile(string fileAddr)
        {
            Dictionary<double, FreqStatModel> dicFreqStatModel = new Dictionary<double, FreqStatModel>();
            StreamReader read = new StreamReader(fileAddr);
            string str;
            while (read.Peek() != -1)
            {
                str = read.ReadLine();
                FreqStatModel freqStatModel = new FreqStatModel();
                freqStatModel.DicAmplitudeCount = new Dictionary<int, int>();
                string[] arrFreqStat = str.TrimEnd(' ').Split(' ');
                freqStatModel.AmplitudeValue = Convert.ToDouble(arrFreqStat[0]);
                for (int i = 1; i < arrFreqStat.Length; i += 2)
                {
                    freqStatModel.DicAmplitudeCount.Add(Convert.ToInt32(arrFreqStat[i]), Convert.ToInt32(arrFreqStat[i + 1]));
                }
                dicFreqStatModel.Add(Convert.ToDouble(arrFreqStat[0]), freqStatModel);
                //temp
            }
            read.Close();
            Dictionary<double, FreqStatModel> dictionaryAsc = dicFreqStatModel.Where(x => x.Key >= Convert.ToDouble(freqNavBar.FreqStart) * 1000 && x.Key <= Convert.ToDouble(freqNavBar.FreqStop) * 1000).ToDictionary(o => o.Key, p => p.Value);
            ctrlStatData(dictionaryAsc, freqNavBar);
        }
        /// <summary>
        /// 读取分析文件
        /// </summary>
        /// <param name="fileAddr"></param>
        public void readAnalysisFileForAll(string fileAddr, List<FreqNavBar> freqnavbarList)
        {
            Dictionary<double, FreqStatModel> dicFreqStatModel = new Dictionary<double, FreqStatModel>();
            StreamReader read = new StreamReader(fileAddr);
            string str;
            while (read.Peek() != -1)
            {
                str = read.ReadLine();
                FreqStatModel freqStatModel = new FreqStatModel();
                freqStatModel.DicAmplitudeCount = new Dictionary<int, int>();
                string[] arrFreqStat = str.TrimEnd(' ').Split(' ');
                freqStatModel.AmplitudeValue = Convert.ToDouble(arrFreqStat[0]);
                for (int i = 1; i < arrFreqStat.Length; i += 2)
                {
                    freqStatModel.DicAmplitudeCount.Add(Convert.ToInt32(arrFreqStat[i]), Convert.ToInt32(arrFreqStat[i + 1]));
                }
                dicFreqStatModel.Add(Convert.ToDouble(arrFreqStat[0]), freqStatModel);
                //temp
            }
            read.Close();
            foreach (var fnr in freqnavbarList)
            {
                Dictionary<double, FreqStatModel> dictionaryAsc = dicFreqStatModel.Where(x => x.Key >= Convert.ToDouble(fnr.FreqStart) * 1000 && x.Key <= Convert.ToDouble(fnr.FreqStop) * 1000).ToDictionary(o => o.Key, p => p.Value);
                ctrlStatDataForAll(dictionaryAsc, fnr);
            }
        }

        //private void getFileNames() 
        //{
        //    fileNameList = new ArrayList();
        //    CollectionDataSave cds = new CollectionDataSave();
        //    cds.openSQLiteConnection();
        //    fileNameList = cds.getFileNames();
        //}

        /// <summary>
        /// 读取原始文件进行分析
        /// </summary>
        private void readFileData()
        {
            Dictionary<double, FreqStatModel> temp = new Dictionary<double, FreqStatModel>();
            FreqStatModel freqStatModel = null;
            for (int i = 0; i < fileNameList.Count; i++)
            {
                FreqCollectionIndex fci = (FreqCollectionIndex)fileNameList[i];
                double frequency = fci.StartFreq;
                double step = fci.FreqStep / 1000;
                using (FileStream fsRead = new FileStream(@fci.FileAddr, FileMode.Open))
                {
                    //long fsLen = (long)fsRead.Length;
                    //byte[] heByte = new byte[fsLen];
                    //int r = fsRead.Read(heByte, 0, heByte.Length);
                    //float[] targetData = new float[r / 4];
                    int countTemp = 0;
                    for (int k = 0; k < fsRead.Length; k += 4)
                    {
                        byte[] heBytetemp = new byte[4];
                        fsRead.Read(heBytetemp, 0, 4);
                        float targetData = BitConverter.ToSingle(heBytetemp, 0);
                        if (countTemp == freqCount)
                        {
                            countTemp = 0;
                            frequency = fci.StartFreq;
                        }
                        if (targetData + 107 < -20 || targetData + 107 > 100)
                        {
                            frequency += step;
                            countTemp++;
                            continue;
                        }
                        int AmplitudeValue = Convert.ToInt32(targetData) + 107;
                        if (!temp.ContainsKey(frequency))
                        {
                            //if (freqStatModel.DicAmplitudeCount != null)
                            //{
                            //    freqStatModel.DicAmplitudeCount.Clear();
                            //}
                            freqStatModel = new FreqStatModel();
                            freqStatModel.AmplitudeValue = frequency;
                            freqStatModel.DicAmplitudeCount = new Dictionary<int, int>();
                            freqStatModel.DicAmplitudeCount.Add(AmplitudeValue, 1);
                            temp.Add(frequency, freqStatModel);
                        }
                        else
                        {
                            //freqStatModel = null;
                            freqStatModel = temp[frequency];
                            if (freqStatModel.DicAmplitudeCount.ContainsKey(AmplitudeValue))
                            {
                                freqStatModel.DicAmplitudeCount[AmplitudeValue] = freqStatModel.DicAmplitudeCount[AmplitudeValue] + 1;
                            }
                            else
                            {
                                freqStatModel.DicAmplitudeCount.Add(AmplitudeValue, 1);
                            }
                        }
                        frequency += step;
                        countTemp++;
                    }
                    //if (DataConvert(heByte, ref targetData))
                    //{
                    //    heByte = null;
                    //    for (int j = 0; j < targetData.Length; j++)
                    //    {
                    //        if (frequency > fci.EndFreq)
                    //        {
                    //            frequency = fci.StartFreq;
                    //        }
                    //        if (targetData[j] + 107 < -20 || targetData[j] + 107 > 100)
                    //        {
                    //            frequency += 25;
                    //            continue;
                    //        }
                    //        string AmplitudeValue = (Convert.ToInt32(targetData[j]) + 107).ToString();
                    //        if (!temp.ContainsKey(frequency))
                    //        {
                    //            //if (freqStatModel.DicAmplitudeCount != null)
                    //            //{
                    //            //    freqStatModel.DicAmplitudeCount.Clear();
                    //            //}
                    //            freqStatModel = new FreqStatModel();
                    //            freqStatModel.AmplitudeValue = frequency;
                    //            freqStatModel.DicAmplitudeCount = new Dictionary<string, int>();
                    //            freqStatModel.DicAmplitudeCount.Add(AmplitudeValue, 1);
                    //            temp.Add(frequency, freqStatModel);
                    //        }
                    //        else
                    //        {
                    //            freqStatModel = null;
                    //            freqStatModel = temp[frequency];
                    //            if (freqStatModel.DicAmplitudeCount.ContainsKey(AmplitudeValue))
                    //            {
                    //                freqStatModel.DicAmplitudeCount[AmplitudeValue] = freqStatModel.DicAmplitudeCount[AmplitudeValue] + 1;
                    //            }
                    //            else
                    //            {
                    //                freqStatModel.DicAmplitudeCount.Add(AmplitudeValue, 1);
                    //            }
                    //        }
                    //        frequency += 25;
                    //    }
                    //    targetData = null;
                    //}
                    fsRead.Close();
                    fsRead.Dispose();
                }
            }
            Dictionary<double, FreqStatModel> dictionaryAsc = temp.Where(x => x.Key >= Convert.ToDouble(freqNavBar.FreqStart) * 1000 && x.Key <= Convert.ToDouble(freqNavBar.FreqStop) * 1000).OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            if (dictionaryAsc.Count == 0)
            {
                reminderBoxDelegate("选中频段超出数据范围！");
            }
            else
            {
                streamStatFile(dictionaryAsc);
                ctrlStatData(dictionaryAsc, freqNavBar);
            }


        }

        /// <summary>
        /// 根据起止频率获取列表
        /// </summary>
        /// <param name="startFreq">开始频率</param>
        /// <param name="endFreq">结束频率</param>
        /// <param name="step">带宽</param>
        /// <returns></returns>
        private List<AnalysisResult> GetAnalysList(decimal startFreq, decimal endFreq, decimal step)
        {
            List<AnalysisResult> list = new List<AnalysisResult>();
            decimal freq = startFreq;
            while (freq <= endFreq)
            {
                AnalysisResult analy = new AnalysisResult();
                analy.Id = Guid.NewGuid().ToString();
                analy.StartFreq = (double)(freq - step / 2);
                analy.EndFreq = (double)(freq + step / 2);
                analy.Frequency = (double)(freq);
                //analy.FreqGuid =;
                freq += step;
                list.Add(analy);
            }
            return list;
        }

        /// <summary>
        /// 保存统计数据存储成后缀.STAT文件
        /// </summary>
        /// <param name="dictionaryAsc"></param>
        private void streamStatFile(Dictionary<double, FreqStatModel> dictionaryAsc)
        {
            int count = dictionaryAsc.Count;
            string path = "freqAnalysis" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".stat";
            saveAanalysisDataDelegate(path);
            path = System.Windows.Forms.Application.StartupPath + "/SqliteData/" + path;
            foreach (KeyValuePair<double, FreqStatModel> kvp in dictionaryAsc)
            {
                string lineValue = kvp.Key.ToString() + " ";
                foreach (KeyValuePair<int, int> kvpFreq in kvp.Value.DicAmplitudeCount)
                {
                    lineValue += kvpFreq.Key + " " + kvpFreq.Value + " ";
                }
                lineValue += "\r\n";
                using (FileStream fsWrite = new FileStream(path, FileMode.Append))
                {
                    StreamWriter sw = new StreamWriter(fsWrite);
                    sw.Write(lineValue);
                    sw.Close();
                    fsWrite.Close();
                };
            }
        }

        /// <summary>
        /// 生成占用度和最大值、均值数据
        /// </summary>
        /// <param name="dicFreqStatModel">原始数据分析后统计数据</param>
        /// <param name="bandwidth"></param>
        private void ctrlStatData(Dictionary<double, FreqStatModel> dicFreqStatModel, FreqNavBar freqNavBar)
        {
            var listCreat = GetAnalysList(decimal.Parse(freqNavBar.FreqStart), decimal.Parse(freqNavBar.FreqStop), decimal.Parse(freqNavBar.BandWidth) / 1000);

            ChartSeriesPoints csps = new ChartSeriesPoints();
            List<AnalysisResult> freqOccupancyDegreeDic = new List<AnalysisResult>();
            int signalLimit = string.IsNullOrEmpty(freqNavBar.SignalLimit) ? 45 : Convert.ToInt16(freqNavBar.SignalLimit);
            int occuDegreeLimit = string.IsNullOrEmpty(freqNavBar.OccuDegreeLimit) ? 90 : Convert.ToInt16(freqNavBar.OccuDegreeLimit);

            if (listCreat != null)
            {
                //PointPairList maxValues = new PointPairList();
                //PointPairList midValues = new PointPairList();
                PointPairList occupyValues = new PointPairList();

                foreach (var item in listCreat)
                {
                    double trueFreq = item.Frequency;
                    double tempFreq;
                    int occupyValueCount = 0;

                    var list = dicFreqStatModel.Where((obj) => obj.Key >= item.StartFreq * 1000 && obj.Key < item.EndFreq * 1000).ToList();
                    List<AnalysisResult> tmpList = new List<AnalysisResult>();
                    PointPairList tmpOccupyValues = new PointPairList();
                    foreach (KeyValuePair<double, FreqStatModel> kvp in list)
                    {
                        PointPair spOccupy = new PointPair();
                        tempFreq = kvp.Key;
                        int overSignalLimit = kvp.Value.DicAmplitudeCount.Where(x => x.Key > signalLimit).Select(x => x.Value).Sum();
                        int sum = kvp.Value.DicAmplitudeCount.Values.Sum();
                        double occpy = ((double)overSignalLimit / (double)sum) * 100;

                        double maxValue = kvp.Value.DicAmplitudeCount.Keys.Max();
                        double AverageValue = kvp.Value.DicAmplitudeCount.Select(x => x.Key).Average();
                        spOccupy.X = tempFreq / 1000;
                        spOccupy.Y = Convert.ToInt16(occpy);

                        AnalysisResult tmpItem = Mapper<AnalysisResult, AnalysisResult>(item);

                        tmpItem.Occupy = Convert.ToInt16(occpy);
                        tmpItem.AmplitudeMidValue = Convert.ToInt16(AverageValue);
                        tmpItem.AmplitudeMaxValue = Convert.ToInt16(maxValue);
                        //如果不是第一次
                        if (occupyValueCount >= 1)
                        {
                            //如果占用度列表的上一个频率 + 带宽大于等于当前频率
                            if (tmpOccupyValues[occupyValueCount - 1].X + Convert.ToDouble(freqNavBar.BandWidth) / 1000 >= tempFreq / 1000)
                            {
                                //如果占用度列表的上一个占用度小于当前占用度
                                if (tmpOccupyValues[occupyValueCount - 1].Y < occpy)
                                {
                                    //占用度列表的上一个占用度重新赋值
                                    tmpOccupyValues[occupyValueCount - 1].X = tempFreq / 1000;
                                    tmpOccupyValues[occupyValueCount - 1].Y = occpy;

                                    //返回结果列表重新赋值
                                    tmpList[occupyValueCount - 1].Frequency = tempFreq / 1000;
                                    tmpList[occupyValueCount - 1].Occupy = Convert.ToInt16(occpy);
                                    tmpList[occupyValueCount - 1].AmplitudeMidValue = Convert.ToInt16(AverageValue);
                                    tmpList[occupyValueCount - 1].AmplitudeMaxValue = Convert.ToInt16(maxValue);
                                }
                            }
                            else
                            {
                                tmpOccupyValues.Add(spOccupy);
                                tmpList.Add(tmpItem);
                                occupyValueCount++;
                            }
                        }
                        else
                        {
                            tmpOccupyValues.Add(spOccupy);
                            tmpList.Add(tmpItem);
                            occupyValueCount++;
                        }
                    }

                    if (tmpList.Count > 0)
                    {
                        int occupyMax = tmpList.Max(obj => obj.Occupy);
                        int amplitudeMax = tmpList.Max(obj => obj.AmplitudeMaxValue);
                        int amplitudeMid = tmpList.Max(obj => obj.AmplitudeMidValue);
                        item.Occupy = occupyMax;
                        item.Frequency = trueFreq;
                        item.AmplitudeMaxValue = amplitudeMax;
                        item.AmplitudeMidValue = amplitudeMid;
                    }
                    freqOccupancyDegreeDic.Add(item);

                    if (tmpOccupyValues.Count > 0)
                    {
                        var spOcp = new PointPair();
                        spOcp.X = trueFreq;
                        spOcp.Y = tmpOccupyValues[0].Y;
                        occupyValues.Add(spOcp);
                    }

                    //occupyValues.Add(tmpOccupyValues);
                }
                csps.OccupyPointPairList = occupyValues;
            }



            #region 2016年10月26日修改
            //if (dicFreqStatModel != null)
            //{
            //    //*****  1
            //    PointPairList maxValues = new PointPairList();
            //    PointPairList midValues = new PointPairList();
            //    PointPairList occupyValues = new PointPairList();
            //    //******  2
            //    //SeriesPointCollection maxValues = new SeriesPointCollection();
            //    //SeriesPointCollection midValues = new SeriesPointCollection();
            //    //AmplitudeMaxValue = new List<short>();
            //    //AmplitudeAverageValue = new List<short>();

            //    double tempFreq;
            //    int occupyValueCount = 0;
            //    int Prec = GetPrec(decimal.Parse(freqNavBar.BandWidth) / 1000) + 1;
            //    foreach (KeyValuePair<double, FreqStatModel> kvp in dicFreqStatModel)
            //    {
            //        //PointPair spMax = new PointPair();
            //        //PointPair spMid = new PointPair();
            //        PointPair spOccupy = new PointPair();
            //        //Visifire.Charts.DataPoint dpMax = new Visifire.Charts.DataPoint();

            //        //SeriesPoint spMax = new SeriesPoint();
            //        //SeriesPoint spMid = new SeriesPoint();

            //        tempFreq = kvp.Key;
            //        int overSignalLimit = kvp.Value.DicAmplitudeCount.Where(x => x.Key > signalLimit).Select(x => x.Value).Sum();
            //        int sum = kvp.Value.DicAmplitudeCount.Values.Sum();
            //        double occpy = ((double)overSignalLimit / (double)sum) * 100;

            //        double maxValue = kvp.Value.DicAmplitudeCount.Keys.Max();
            //        //AmplitudeMaxValue.Add(Convert.ToInt16(maxValue));
            //        //dpMax.XValue = tempFreq / 1000;
            //        //dpMax.YValue = Convert.ToInt16(maxValue);

            //        //spMax.X = tempFreq / 1000;
            //        //spMax.Y = Convert.ToInt16(maxValue);
            //        //maxValues.Add(spMax);
            //        //maxPoints.Add(dpMax);
            //        double AverageValue = kvp.Value.DicAmplitudeCount.Select(x => x.Key).Average();
            //        //spMid.X = tempFreq / 1000;
            //        //spMid.Y = Convert.ToInt16(AverageValue);
            //        //AmplitudeAverageValue.Add(Convert.ToInt16(AverageValue));
            //        //midValues.Add(spMid);


            //        //if (occpy > occuDegreeLimit)
            //        //{
            //        spOccupy.X = tempFreq / 1000;
            //        spOccupy.Y = occpy;

            //        AnalysisResult ar = new AnalysisResult();
            //        ar.Frequency = Math.Round(tempFreq / 1000, Prec);
            //        ar.Occupy = Convert.ToInt16(occpy);
            //        ar.AmplitudeMidValue = Convert.ToInt16(AverageValue);
            //        ar.AmplitudeMaxValue = Convert.ToInt16(maxValue);
            //        if (occupyValueCount >= 1)
            //        {
            //            if (occupyValues[occupyValueCount - 1].X + Convert.ToDouble(freqNavBar.BandWidth) / 1000 >= tempFreq / 1000)
            //            {
            //                if (occupyValues[occupyValueCount - 1].Y < occpy)
            //                {
            //                    occupyValues[occupyValueCount - 1].X = tempFreq / 1000;
            //                    occupyValues[occupyValueCount - 1].Y = occpy;

            //                    freqOccupancyDegreeDic[occupyValueCount - 1].Frequency = tempFreq / 1000;
            //                    freqOccupancyDegreeDic[occupyValueCount - 1].Occupy = Convert.ToInt16(occpy);
            //                    freqOccupancyDegreeDic[occupyValueCount - 1].AmplitudeMidValue = Convert.ToInt16(AverageValue);
            //                    freqOccupancyDegreeDic[occupyValueCount - 1].AmplitudeMaxValue = Convert.ToInt16(maxValue);
            //                }
            //            }
            //            else
            //            {
            //                occupyValues.Add(spOccupy);
            //                freqOccupancyDegreeDic.Add(ar);
            //                occupyValueCount++;
            //            }
            //        }
            //        else
            //        {
            //            occupyValues.Add(spOccupy);
            //            freqOccupancyDegreeDic.Add(ar);
            //            occupyValueCount++;
            //        }

            //    }
            //    //}
            //    //csps.MaxPointPairList = maxValues;
            //    //csps.MidPointPairList = midValues;
            //    csps.OccupyPointPairList = occupyValues;
            //} 
            #endregion

            csps.MinFreq = Convert.ToDouble(freqNavBar.FreqStart);
            csps.MaxFreq = Convert.ToDouble(freqNavBar.FreqStop);
            csps.SignalLimit = signalLimit;
            csps.OccuDegreeLimit = occuDegreeLimit;
            //refreshChartResultDelete(csps);
            refreshChartFreqStatDelete(dicFreqStatModel, csps);
            compareStationInfo(freqOccupancyDegreeDic);
        }
        /// <summary>
        /// 生成占用度和最大值、均值数据
        /// </summary>
        /// <param name="dicFreqStatModel">原始数据分析后统计数据</param>
        /// <param name="bandwidth"></param>
        private void ctrlStatDataForAll(Dictionary<double, FreqStatModel> dicFreqStatModel, FreqNavBar freqNavBar)
        {
            var listCreat = GetAnalysList(decimal.Parse(freqNavBar.FreqStart), decimal.Parse(freqNavBar.FreqStop), decimal.Parse(freqNavBar.BandWidth) / 1000);

            ChartSeriesPoints csps = new ChartSeriesPoints();
            List<AnalysisResult> freqOccupancyDegreeDic = new List<AnalysisResult>();
            int signalLimit = string.IsNullOrEmpty(freqNavBar.SignalLimit) ? 45 : Convert.ToInt16(freqNavBar.SignalLimit);
            int occuDegreeLimit = string.IsNullOrEmpty(freqNavBar.OccuDegreeLimit) ? 90 : Convert.ToInt16(freqNavBar.OccuDegreeLimit);

            if (listCreat != null)
            {
                PointPairList occupyValues = new PointPairList();

                foreach (var item in listCreat)
                {
                    double trueFreq = item.Frequency;
                    double tempFreq;
                    int occupyValueCount = 0;

                    var list = dicFreqStatModel.Where((obj) => obj.Key >= item.StartFreq * 1000 && obj.Key < item.EndFreq * 1000).ToList();
                    List<AnalysisResult> tmpList = new List<AnalysisResult>();
                    PointPairList tmpOccupyValues = new PointPairList();
                    foreach (KeyValuePair<double, FreqStatModel> kvp in list)
                    {
                        PointPair spOccupy = new PointPair();
                        tempFreq = kvp.Key;
                        int overSignalLimit = kvp.Value.DicAmplitudeCount.Where(x => x.Key > signalLimit).Select(x => x.Value).Sum();
                        int sum = kvp.Value.DicAmplitudeCount.Values.Sum();
                        double occpy = ((double)overSignalLimit / (double)sum) * 100;

                        double maxValue = kvp.Value.DicAmplitudeCount.Keys.Max();
                        double AverageValue = kvp.Value.DicAmplitudeCount.Select(x => x.Key).Average();
                        spOccupy.X = tempFreq / 1000;
                        spOccupy.Y = Convert.ToInt16(occpy);

                        AnalysisResult tmpItem = Mapper<AnalysisResult, AnalysisResult>(item);

                        tmpItem.Occupy = Convert.ToInt16(occpy);
                        tmpItem.AmplitudeMidValue = Convert.ToInt16(AverageValue);
                        tmpItem.AmplitudeMaxValue = Convert.ToInt16(maxValue);
                        //如果不是第一次
                        if (occupyValueCount >= 1)
                        {
                            //如果占用度列表的上一个频率 + 带宽大于等于当前频率
                            if (tmpOccupyValues[occupyValueCount - 1].X + Convert.ToDouble(freqNavBar.BandWidth) / 1000 >= tempFreq / 1000)
                            {
                                //如果占用度列表的上一个占用度小于当前占用度
                                if (tmpOccupyValues[occupyValueCount - 1].Y < occpy)
                                {
                                    //占用度列表的上一个占用度重新赋值
                                    tmpOccupyValues[occupyValueCount - 1].X = tempFreq / 1000;
                                    tmpOccupyValues[occupyValueCount - 1].Y = occpy;

                                    //返回结果列表重新赋值
                                    tmpList[occupyValueCount - 1].Frequency = tempFreq / 1000;
                                    tmpList[occupyValueCount - 1].Occupy = Convert.ToInt16(occpy);
                                    tmpList[occupyValueCount - 1].AmplitudeMidValue = Convert.ToInt16(AverageValue);
                                    tmpList[occupyValueCount - 1].AmplitudeMaxValue = Convert.ToInt16(maxValue);
                                }
                            }
                            else
                            {
                                tmpOccupyValues.Add(spOccupy);
                                tmpList.Add(tmpItem);
                                occupyValueCount++;
                            }
                        }
                        else
                        {
                            tmpOccupyValues.Add(spOccupy);
                            tmpList.Add(tmpItem);
                            occupyValueCount++;
                        }
                    }

                    if (tmpList.Count > 0)
                    {
                        int occupyMax = tmpList.Max(obj => obj.Occupy);
                        int amplitudeMax = tmpList.Max(obj => obj.AmplitudeMaxValue);
                        int amplitudeMid = tmpList.Max(obj => obj.AmplitudeMidValue);
                        item.Occupy = occupyMax;
                        item.Frequency = trueFreq;
                        item.AmplitudeMaxValue = amplitudeMax;
                        item.AmplitudeMidValue = amplitudeMid;
                    }
                    freqOccupancyDegreeDic.Add(item);

                    if (tmpOccupyValues.Count > 0)
                    {
                        var spOcp = new PointPair();
                        spOcp.X = trueFreq;
                        spOcp.Y = tmpOccupyValues[0].Y;
                        occupyValues.Add(spOcp);
                    }

                }
                csps.OccupyPointPairList = occupyValues;
            }

            csps.MinFreq = Convert.ToDouble(freqNavBar.FreqStart);
            csps.MaxFreq = Convert.ToDouble(freqNavBar.FreqStop);
            csps.SignalLimit = signalLimit;
            csps.OccuDegreeLimit = occuDegreeLimit;
            //refreshChartFreqStatDelete(dicFreqStatModel, csps);//绘制统计图形

            compareStationInfoForAll(freqOccupancyDegreeDic, freqNavBar);
            
        }
        private int GetPrec(decimal number)
        {
            int count = 0;
            decimal num = number * 10;
            if (num != Math.Round(num))
            {
                count += 1;
                count += GetPrec(num);
            }
            return count;
        }
        /// <summary>
        /// 电磁信号与台站信息比对
        /// </summary>
        /// <param name="dicOccupy"></param>
        private void compareStationInfo(List<AnalysisResult> dicOccupy)
        {
            //List<StationBaseExt> stationBaseList = SQLiteDataService.QueryAroundStationWithEmits(LoginService.CurrentActivityPlace.Guid);

            List<ActivitySurroundStation> stationBaseList = SQLiteDataService.QueryStatBaseByPlaceID(LoginService.CurrentActivityPlace.Guid); //modify by michael 17.07.20
            ObservableCollection<AnalysisResult> freqList = new ObservableCollection<AnalysisResult>();
            Dictionary<string, ActivitySurroundStation> stationDic = new Dictionary<string, ActivitySurroundStation>();
            int occuDegreeLimit = Convert.ToInt16(freqNavBar.OccuDegreeLimit);
            int signalLimit = Convert.ToInt16(freqNavBar.SignalLimit);
            foreach (AnalysisResult kv in dicOccupy)
            {

                AnalysisResult analysisResult = new AnalysisResult();
                analysisResult.Id = Guid.NewGuid().ToString();
                analysisResult.Frequency = Convert.ToDouble(kv.Frequency.ToString("0.0000"));
                //analysisResult.BandWidth = this.bandWidth;
                analysisResult.BandWidth = Double.Parse(freqNavBar.BandWidth); ;
                analysisResult.AnalysisBandWidth = Double.Parse(freqNavBar.BandWidth); ;
                analysisResult.AmplitudeMidValue = kv.AmplitudeMidValue;
                analysisResult.AmplitudeMaxValue = kv.AmplitudeMaxValue;
                analysisResult.Occupy = Convert.ToInt32(kv.Occupy);
                List<StationEmitInfo> tempList = new List<StationEmitInfo>();
                foreach (ActivitySurroundStation sbe in stationBaseList)
                {
                    tempList = sbe.EmitInfo.Where(x => (x.FreqEC - x.FreqBand / 1000) <= kv.Frequency && (x.FreqEC + x.FreqBand / 1000) >= kv.Frequency).ToList();
                    if (tempList.Count() > 0)
                    {
                        //analysisResult.FreqType = SignalTypeEnum.已占;
                        analysisResult.StationName = sbe.STAT_NAME;
                        analysisResult.StationGuid = sbe.STATGUID;
                        analysisResult.NeedClear = (int)sbe.EmitInfo[0].NeedClear;
                        analysisResult.FreqGuid = sbe.EmitInfo[0].Guid;
                        analysisResult.ClearResult = (int)sbe.EmitInfo[0].ClearResult;
                        if (!stationDic.Keys.Contains(sbe.STATGUID))
                            stationDic.Add(sbe.STATGUID, sbe);
                        break;
                    }
                    else
                    {
                        //analysisResult.FreqType = SignalTypeEnum.空闲;
                    }
                }
                if (analysisResult.Occupy >= occuDegreeLimit && analysisResult.AmplitudeMaxValue > signalLimit)
                {
                    analysisResult.FreqType = SignalTypeEnum.已占;
                }
                else
                {
                    analysisResult.FreqType = SignalTypeEnum.空闲;
                }
                analysisResult.MeasureId = MeasureId;
                analysisResult.StartFreq = Convert.ToDouble(freqNavBar.FreqStart);
                analysisResult.EndFreq = Convert.ToDouble(freqNavBar.FreqStop);
                analysisResult.FreqGuid = freqNavBar.FreqGuid;
                freqList.Add(analysisResult);
            }
            foreach (ActivitySurroundStation sbe in stationBaseList)
            {
                if (stationDic.Keys.Contains(sbe.STATGUID))
                {
                    continue;
                }
                else
                {
                    if (sbe.EmitInfo != null && sbe.EmitInfo.Count > 0)
                    {
                        AnalysisResult analysisResult = new AnalysisResult();
                        analysisResult.Id = Guid.NewGuid().ToString();
                        analysisResult.Frequency = Convert.ToDouble(sbe.EmitInfo[0].FreqEC.ToString());
                        //analysisResult.BandWidth = Convert.ToDouble((sbe.EmitInfo[0].FreqBand * 1000).ToString());
                        analysisResult.BandWidth = Convert.ToDouble((sbe.EmitInfo[0].FreqBand).ToString());
                        analysisResult.FreqGuid = sbe.EmitInfo[0].Guid;
                        analysisResult.NeedClear = (int)sbe.EmitInfo[0].NeedClear;
                        analysisResult.ClearResult = (int)sbe.EmitInfo[0].ClearResult;
                        analysisResult.StationName = sbe.STAT_NAME;
                        analysisResult.StationGuid = sbe.STATGUID;
                        analysisResult.FreqType = SignalTypeEnum.已占;
                        analysisResult.MeasureId = MeasureId;
                        analysisResult.AnalysisBandWidth = Double.Parse(freqNavBar.BandWidth);
                        analysisResult.StartFreq = Convert.ToDouble(freqNavBar.FreqStart);
                        analysisResult.EndFreq = Convert.ToDouble(freqNavBar.FreqStop);
                        analysisResult.FreqGuid = freqNavBar.FreqGuid;
                        freqList.Add(analysisResult);
                    }
                }
            }
            if (freqList.Count == 0)
            {               
                initdata(freqList);
            }
            freqNavBar.freqList = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<ObservableCollection<AnalysisResult>>(freqList);
            freqNavBar.MeasureId = MeasureId;
            if (!ShowUI)
                return;
            if (isRefresh)
            {
                refreshAnalysisResultDelete(freqList);
            }
            else
            {
                List<AnalysisResult> analysisList = SQLiteDataService.QueryAnalysisResultByMeasureIDAndFreq(freqNavBar);
                if (analysisList.Count > 0)
                {
                    freqList = new ObservableCollection<AnalysisResult>(analysisList);
                    refreshAnalysisResultDelete(freqList);
                }
                else
                {
                    refreshAnalysisResultDelete(freqList);
                }
            }
        }
        /// <summary>
        /// 电磁信号与台站信息比对
        /// </summary>
        /// <param name="dicOccupy"></param>
        private void compareStationInfoForAll(List<AnalysisResult> dicOccupy,FreqNavBar navBar)
        {
            List<ActivitySurroundStation> stationBaseList = SQLiteDataService.QueryStatBaseByPlaceID(LoginService.CurrentActivityPlace.Guid); //modify by michael 17.07.20
            ObservableCollection<AnalysisResult> freqList = new ObservableCollection<AnalysisResult>();
            Dictionary<string, ActivitySurroundStation> stationDic = new Dictionary<string, ActivitySurroundStation>();
            int occuDegreeLimit = Convert.ToInt16(navBar.OccuDegreeLimit);
            int signalLimit = Convert.ToInt16(navBar.SignalLimit);
            foreach (AnalysisResult kv in dicOccupy)
            {
                AnalysisResult analysisResult = new AnalysisResult();
                analysisResult.Id = Guid.NewGuid().ToString();
                analysisResult.Frequency = Convert.ToDouble(kv.Frequency.ToString("0.0000"));
                //analysisResult.BandWidth = this.bandWidth;
                analysisResult.BandWidth = Double.Parse(navBar.BandWidth); ;
                analysisResult.AnalysisBandWidth = Double.Parse(navBar.BandWidth); ;
                analysisResult.AmplitudeMidValue = kv.AmplitudeMidValue;
                analysisResult.AmplitudeMaxValue = kv.AmplitudeMaxValue;
                analysisResult.Occupy = Convert.ToInt32(kv.Occupy);
                List<StationEmitInfo> tempList = new List<StationEmitInfo>();
                foreach (ActivitySurroundStation sbe in stationBaseList)
                {
                    tempList = sbe.EmitInfo.Where(x => (x.FreqEC - x.FreqBand / 1000) <= kv.Frequency && (x.FreqEC + x.FreqBand / 1000) >= kv.Frequency).ToList();
                    if (tempList.Count() > 0)
                    {
                        //analysisResult.FreqType = SignalTypeEnum.已占;
                        analysisResult.StationName = sbe.STAT_NAME;
                        analysisResult.StationGuid = sbe.STATGUID;
                        analysisResult.NeedClear = (int)sbe.EmitInfo[0].NeedClear;
                        analysisResult.FreqGuid = sbe.EmitInfo[0].Guid;
                        analysisResult.ClearResult = (int)sbe.EmitInfo[0].ClearResult;
                        if (!stationDic.Keys.Contains(sbe.STATGUID))
                            stationDic.Add(sbe.STATGUID, sbe);
                        break;
                    }
                    else
                    {
                        //analysisResult.FreqType = SignalTypeEnum.空闲;
                    }
                }
                if (analysisResult.Occupy >= occuDegreeLimit && analysisResult.AmplitudeMaxValue > signalLimit)
                {
                    analysisResult.FreqType = SignalTypeEnum.已占;
                }
                else
                {
                    analysisResult.FreqType = SignalTypeEnum.空闲;
                }
                analysisResult.MeasureId = MeasureId;
                analysisResult.StartFreq = Convert.ToDouble(navBar.FreqStart);
                analysisResult.EndFreq = Convert.ToDouble(navBar.FreqStop);
                analysisResult.FreqGuid = navBar.FreqGuid;
                freqList.Add(analysisResult);
            }
            foreach (ActivitySurroundStation sbe in stationBaseList)
            {
                if (stationDic.Keys.Contains(sbe.STATGUID))
                {
                    continue;
                }
                else
                {
                    if (sbe.EmitInfo != null && sbe.EmitInfo.Count > 0)
                    {
                        AnalysisResult analysisResult = new AnalysisResult();
                        analysisResult.Id = Guid.NewGuid().ToString();
                        analysisResult.Frequency = Convert.ToDouble(sbe.EmitInfo[0].FreqEC.ToString());
                        //analysisResult.BandWidth = Convert.ToDouble((sbe.EmitInfo[0].FreqBand * 1000).ToString());
                        analysisResult.BandWidth = Convert.ToDouble((sbe.EmitInfo[0].FreqBand).ToString());
                        analysisResult.FreqGuid = sbe.EmitInfo[0].Guid;
                        analysisResult.NeedClear = (int)sbe.EmitInfo[0].NeedClear;
                        analysisResult.ClearResult = (int)sbe.EmitInfo[0].ClearResult;
                        analysisResult.StationName = sbe.STAT_NAME;
                        analysisResult.StationGuid = sbe.STATGUID;
                        analysisResult.FreqType = SignalTypeEnum.已占;
                        analysisResult.MeasureId = MeasureId;
                        analysisResult.AnalysisBandWidth = Double.Parse(navBar.BandWidth);
                        analysisResult.StartFreq = Convert.ToDouble(navBar.FreqStart);
                        analysisResult.EndFreq = Convert.ToDouble(navBar.FreqStop);
                        analysisResult.FreqGuid = navBar.FreqGuid;
                        freqList.Add(analysisResult);
                    }
                }
            }
            if (freqList.Count == 0)
            {
                initdata(freqList);
            }
            navBar.MeasureId = MeasureId;
            navBar.freqList = freqList;
            
        }
        private void initdata(ObservableCollection<AnalysisResult> freqList)
        {
            AnalysisResult temp = new AnalysisResult();
            temp.Frequency = 85;
            temp.BandWidth = 200;
            temp.AmplitudeMidValue = 69;
            temp.AmplitudeMaxValue = 87;
            temp.Occupy = 95;
            temp.StationName = "交通广播";
            temp.FreqType = SignalTypeEnum.空闲;
            temp.MeasureId = MeasureId;
            temp.StartFreq = 30;
            temp.EndFreq = 3000;
            temp.IsCheck = false;

            AnalysisResult temp1 = new AnalysisResult();
            temp1.Frequency = 85;
            temp1.BandWidth = 200;
            temp1.AmplitudeMidValue = 69;
            temp1.AmplitudeMaxValue = 87;
            temp1.Occupy = 95;
            temp1.StationName = "人民广播";
            temp1.FreqType = SignalTypeEnum.清理;
            temp1.MeasureId = MeasureId;
            temp1.StartFreq = 30;
            temp1.EndFreq = 3000;
            temp1.IsCheck = false;

            AnalysisResult temp2 = new AnalysisResult();
            temp2.Frequency = 85;
            temp2.BandWidth = 200;
            temp2.AmplitudeMidValue = 69;
            temp2.AmplitudeMaxValue = 87;
            temp2.Occupy = 95;
            temp2.StationName = "音乐广播";
            temp2.FreqType = SignalTypeEnum.已占;
            temp2.IsCheck = false;
            temp2.MeasureId = MeasureId;
            temp2.StartFreq = 30;
            temp2.EndFreq = 3000;

            freqList.Add(temp);
            freqList.Add(temp1);
            freqList.Add(temp2);
        }

        /// <summary>
        /// byte数组转成float数组
        /// </summary>
        /// <param name="sourceData"></param>
        /// <param name="targetData"></param>
        /// <returns></returns>
        private bool DataConvert(byte[] sourceData, ref float[] targetData)
        {

            if (sourceData == null)
                return false;

            int dataLength = targetData.Length;

            if (sourceData.Length < dataLength * 4)
                return false;

            for (int i = 0; i < dataLength; i++)
            {
                targetData[i] = BitConverter.ToSingle(sourceData, i * 4);
            }

            return true;
        }

        public static D Mapper<D, S>(S s)
        {
            D d = Activator.CreateInstance<D>();
            try
            {
                var sType = s.GetType(); var dType = typeof(D);
                foreach (PropertyInfo sP in sType.GetProperties())
                {
                    int index = 0;
                    foreach (PropertyInfo dP in dType.GetProperties())
                    {
                        if (dP.Name == sP.Name)
                        {
                            dP.SetValue(d, sP.GetValue(s, null), null);
                        }
                        index++;
                    }
                }
            }
            catch (Exception ex) { }
            return d;
        }
    }

}
