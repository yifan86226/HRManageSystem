using Best.VWPlatform.Common.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Best.VWPlatform.Controls.Freq
{
    /// <summary>
    /// 谱图绘制基础控件
    /// </summary>
    /// <remarks>
    /// 提供谱图的基本绘制，包络图、柱状图、瀑布图
    /// </remarks>
    [TemplatePartAttribute(Name = "x_wbTargetSource", Type = typeof(Image))]
    [TemplatePartAttribute(Name = "x_colorPanel", Type = typeof(Rectangle))]
    public sealed class SpectrumDiagramEx : Control
    {
        #region 变量
        private readonly List<WaterfallItem> _waterfallHistory = new List<WaterfallItem>();
        private const byte WaterBackgroundAlpha = 0x55;
        private WriteableBitmap _wbTargetBitmap;
        private WriteableBitmap _wbBackgroundBitmap;
        private WriteableBitmap _wbNoiseBitmap;
        private WriteableBitmap _wbMaxBitmap;
        private WriteableBitmap _wbMinBitmap;
        private Image _wbTargetSource;
        private Image _wbBackground;
        private Image _wbNoise;
        private Image _wbMax;
        private Image _wbMin;
        private WriteableBitmap _wbColorPanel; //彩色y轴，画瀑布图时使用

        /// <summary>
        /// 绘制中频荧光谱图
        /// </summary>
        private WriteableBitmap _wbFluorogramBitmap;
        private Image _wbFluorogramSource;
        /// <summary>
        /// 记录像素点出现频次及颜色等信息
        /// </summary>
        private Dictionary<Tuple<int, int>, FluoroPixelInfo> _FluoroPixelInfo = new Dictionary<Tuple<int, int>, FluoroPixelInfo>();
        
        /// <summary>
        /// 记录荧光谱图像素点颜色
        /// </summary>
        private Color[] _FluoroColorCache;
        private System.Drawing.Color _drawColor;
        private System.Windows.Media.Color _mediaColor;
        #endregion

        #region 构造函数

        static SpectrumDiagramEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpectrumDiagramEx), new FrameworkPropertyMetadata(typeof(SpectrumDiagramEx)));
        }
        public SpectrumDiagramEx()
        {
            SizeChanged += OnSizeChanged;
            Property = new SpectrumDiagramProperty
            {
                UpperLimitValueColor = Colors.Red,
                CenterLimitValueColor = Color.FromArgb(0xff, 0x7c, 0xfc, 0x00),
                LowerLimitValueColor = Colors.Blue
            };
            _drawColor = System.Drawing.ColorTranslator.FromHtml("#333333");
            _mediaColor = System.Windows.Media.Color.FromRgb(_drawColor.R, _drawColor.G, _drawColor.B);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateSize((int)e.NewSize.Width, (int)e.NewSize.Height);
        }

        #endregion

        #region 重写
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _wbTargetSource = GetTemplateChild("x_wbTargetSource") as Image;
            _wbBackground = GetTemplateChild("x_wbBackground") as Image;
            _wbNoise = GetTemplateChild("x_wbNoise") as Image;
            _wbMax = GetTemplateChild("x_wbMax") as Image;
            _wbMin = GetTemplateChild("x_wbMin") as Image;
            Debug.Assert(_wbTargetSource != null, "_wbTargetSource!=null");
            Debug.Assert(_wbBackground != null, "x_wbBackground!=null");
            Debug.Assert(_wbNoise != null, "x_wbNoise!=null");
            Debug.Assert(_wbMax != null, "x_wbMax!=null");
            Debug.Assert(_wbMin != null, "x_wbMin!=null");

            _wbFluorogramSource = GetTemplateChild("x_wbFluorogramSource") as Image;
            Debug.Assert(_wbFluorogramSource != null, "x_wbFluorogramSource!=null");
        }

        #endregion

        #region 属性
        /// <summary>
        /// 谱图属性
        /// </summary>
        public SpectrumDiagramProperty Property { get; set; }

        public SpectrumZoomLevel CurrentZoomLevel { get; set; }
        #endregion

        #region 内部方法
        private void UpdateSize(int w, int h)
        {
            _waterfallHistory.Clear();
            if (_wbTargetBitmap != null)
                _wbTargetBitmap.Clear(Colors.Transparent);
            if (_wbTargetBitmap == null || _wbTargetBitmap.PixelWidth != w || _wbTargetBitmap.PixelHeight != h)
            {
                _wbTargetBitmap = new WriteableBitmap(w, h, 96, 96, PixelFormats.Pbgra32, null);
                _wbTargetSource.Source = _wbTargetBitmap;
            }

            if (_wbBackgroundBitmap != null)
                _wbBackgroundBitmap.Clear(Colors.Transparent);
            if (_wbBackgroundBitmap == null || _wbBackgroundBitmap.PixelWidth != w || _wbBackgroundBitmap.PixelHeight != h)
            {
                _wbBackgroundBitmap = new WriteableBitmap(w, h, 96, 96, PixelFormats.Pbgra32, null);
                _wbBackground.Source = _wbBackgroundBitmap;
            }

            if (_wbNoiseBitmap != null)
                _wbNoiseBitmap.Clear(Colors.Transparent);
            if (_wbNoiseBitmap == null || _wbNoiseBitmap.PixelWidth != w || _wbNoiseBitmap.PixelHeight != h)
            {
                _wbNoiseBitmap = new WriteableBitmap(w, h, 96, 96, PixelFormats.Pbgra32, null);
                _wbNoise.Source = _wbNoiseBitmap;
            }

            if (_wbMaxBitmap != null)
                _wbMaxBitmap.Clear(Colors.Transparent);
            if (_wbMaxBitmap == null || _wbMaxBitmap.PixelWidth != w || _wbMaxBitmap.PixelHeight != h)
            {
                _wbMaxBitmap = new WriteableBitmap(w, h, 96, 96, PixelFormats.Pbgra32, null);
                _wbMax.Source = _wbMaxBitmap;
            }

            if (_wbMinBitmap != null)
                _wbMinBitmap.Clear(Colors.Transparent);
            if (_wbMinBitmap == null || _wbMinBitmap.PixelWidth != w || _wbMinBitmap.PixelHeight != h)
            {
                _wbMinBitmap = new WriteableBitmap(w, h, 96, 96, PixelFormats.Pbgra32, null);
                _wbMin.Source = _wbMinBitmap;
            }

            if (_wbFluorogramBitmap != null)
                ClearFluorogram();
            if (_wbFluorogramBitmap == null || _wbFluorogramBitmap.PixelWidth != w || _wbFluorogramBitmap.PixelHeight != h)
            {
                _wbFluorogramBitmap = new WriteableBitmap(w, h, 96, 96, PixelFormats.Pbgra32, null);
                _wbFluorogramSource.Source = _wbFluorogramBitmap;
            }

            _FluoroColorCache = new Color[(int)ActualHeight * (int)ActualWidth];

            for (int i = 0; i < _FluoroColorCache.Length; i++)
            {
                _FluoroColorCache[i] = _mediaColor;
            }

            UpdateColorPanel();
        }

        private void UpdateColorPanel()
        {
            var rect = new Rectangle { Width = 5, Height = ActualHeight };
            var lgBrush = new LinearGradientBrush { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
            var gs1 = new GradientStop { Color = Property.UpperLimitValueColor, Offset = 0.0 };
            var gs2 = new GradientStop { Color = Property.CenterLimitValueColor, Offset = 0.5 };
            var gs3 = new GradientStop { Color = Property.LowerLimitValueColor, Offset = 1 };
            lgBrush.GradientStops.Add(gs1);
            lgBrush.GradientStops.Add(gs2);
            lgBrush.GradientStops.Add(gs3);
            rect.Fill = lgBrush;
            rect.Arrange(new Rect(0, 0, rect.Width, rect.Height));

            RenderTargetBitmap rtb = new RenderTargetBitmap(5, (int) ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(rect);
            _wbColorPanel = new WriteableBitmap(rtb);
        }
        private Color GetColorFromColorPanel(int pValue)
        {
            if (_wbColorPanel == null)
                return Colors.Transparent;
            if (pValue < 0)
                return _wbColorPanel.GetPixel(1, 0);
            if (pValue >= _wbColorPanel.PixelHeight)
                return _wbColorPanel.GetPixel(1, _wbColorPanel.PixelHeight - 1);
            return _wbColorPanel.GetPixel(1, pValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pBmp">图片</param>
        /// <param name="pCount">频率总点数</param>
        /// <param name="pFreqIndex">频率序号</param>
        /// <param name="pAmplitudeValues">数据值 short[]</param>
        /// <param name="pLineColor"></param>
        /// <param name="pLineType"></param>
        /// <param name="pPreFreqIndexAmplitude">缓存中的幅度值</param>
        private void DrawLine(WriteableBitmap pBmp, long pCount, long pFreqIndex, short[] pAmplitudeValues, Color pLineColor, SpectrumLineType pLineType, short pPreFreqIndexAmplitude = -12345)
        {
            if (pBmp == null || pAmplitudeValues == null)
                return;

            long index = pFreqIndex;
            int valueLen = pAmplitudeValues.Length;
            int startPixel = 0;
            int endPixel = 0;

            startPixel = (int)WMonitorUtile.ViewToScreen(index - 1, ActualWidth - 1, 0, pCount - 1, 0);
            endPixel = (int)WMonitorUtile.ViewToScreen(index + valueLen - 1, ActualWidth - 1, 0, pCount - 1, 0);

            if (startPixel <= 0)
            {
                pBmp.FillRectangle(0, 0, endPixel, (int)ActualHeight, Colors.Transparent);
            }
            else
            {
                pBmp.FillRectangle(startPixel, 0, endPixel - 1, (int)ActualHeight, Colors.Transparent);
            }

            int previousLeftX = -12345;
            int previousLeftY = 0;
            int pixelVal = 0;
            if (pFreqIndex != 0 && pPreFreqIndexAmplitude != -12345)
            {
                startPixel = (int)WMonitorUtile.ViewToScreen(--pFreqIndex, ActualWidth - 1, 0, pCount - 1, 0);
                pixelVal = (int)WMonitorUtile.ViewToScreen(pPreFreqIndexAmplitude, ActualHeight, 0, Property.UpperLimitValue, Property.LowerLimitValue);
                pixelVal = (int)ActualHeight - pixelVal;
                if (pLineType == SpectrumLineType.Wave)
                {
                    if (pPreFreqIndexAmplitude != -12345)
                    {
                        previousLeftX = startPixel;
                        previousLeftY = pixelVal;
                    }
                }
                else
                {
                    if (pPreFreqIndexAmplitude != 12345)
                    {
                        pBmp.DrawLine(startPixel, (int)ActualHeight, startPixel, pixelVal, pLineColor);
                    }
                }
            }

            int step = (int)Utile.MathNoRound(pCount / ActualWidth, 0);
            int ci = 0;
            short max = (short)Property.LowerLimitValue;
            short dbuvVal = -12345;

            for (int i = 0; i < valueLen; i++, index++)
            {
                dbuvVal = pAmplitudeValues[i];
                //if (valueLen > 1)
                //{
                //    if (step > 0 && ci < step)
                //    {
                //        ci++;
                //        if (dbuvVal > max)
                //        {
                //            max = dbuvVal;
                //        }
                //        if (ci < step)
                //            continue;
                //        dbuvVal = max;
                //    }
                //    ci = 0;
                //    max = (short)Property.LowerLimitValue;
                //}

                startPixel = (int)WMonitorUtile.ViewToScreen(index, ActualWidth - 1, 0, pCount - 1, 0);
                pixelVal = (int)WMonitorUtile.ViewToScreen(dbuvVal, ActualHeight, 0, Property.UpperLimitValue, Property.LowerLimitValue);
                pixelVal = (int)ActualHeight - pixelVal;

                if (pLineType == SpectrumLineType.Column)
                {
                    pBmp.DrawLine((int)startPixel, (int)ActualHeight, (int)startPixel, (int)pixelVal, pLineColor);
                }
                else if (pLineType == SpectrumLineType.Wave)
                {
                    if (previousLeftX == -12345)
                    {
                        previousLeftX = startPixel;
                        previousLeftY = pixelVal;
                        continue;
                    }

                    pBmp.DrawLine(previousLeftX, previousLeftY, startPixel, pixelVal, pLineColor);
                    previousLeftX = startPixel;
                    previousLeftY = pixelVal;
                }
            }                
        }
      
        private int _index = 0;
        private int _previousLeftX = -12345;
        private short _previousLeftMaxY = -12345;
        private int _maxX = -12345;
        private short _MaxY = -12345;
        private void DrawFscanLine(WriteableBitmap pBmp, long pCount, long pFreqIndex, short[] pAmplitudeValues, Color pLineColor, SpectrumLineType pLineType, short pPreFreqIndexAmplitude = -12345)
        {
            if (pBmp == null || pAmplitudeValues == null)
                return;
            if (pFreqIndex == 0 || pLineType == SpectrumLineType.Column)
            {
                _index = 0;
                _previousLeftX = -12345;
                _previousLeftMaxY = -12345;
                _maxX = -12345;
                _MaxY = -12345;
            }

            long index = pFreqIndex;
            int valueLen = pAmplitudeValues.Length;
            int pixelVal = 0;
            int startPixel = 0;
            int endPixel = 0;

            int afterpixelVal = 0;
            int afterstartPixel = 0;

            startPixel = (int)WMonitorUtile.ViewToScreen(index, ActualWidth - 1, 0, pCount - 1, 0);
            endPixel = (int)WMonitorUtile.ViewToScreen(index + valueLen - 1, ActualWidth - 1, 0, pCount - 1, 0);
            pBmp.FillRectangle(startPixel, 0, endPixel, (int)ActualHeight, Colors.Transparent);
            
            if (pFreqIndex != 0 && pPreFreqIndexAmplitude != -12345)
            {
                afterstartPixel = (int)WMonitorUtile.ViewToScreen(index, ActualWidth - 1, 0, pCount - 1, 0);
                if (pLineType == SpectrumLineType.Wave)
                {
                    if (Math.Abs(afterstartPixel - _maxX) > 2)
                    {
                        _index = 0;
                        _maxX = _previousLeftX = afterstartPixel;
                        _MaxY = _previousLeftMaxY = pAmplitudeValues[0];
                    }

                    if (pPreFreqIndexAmplitude != -12345)
                    {
                        if (afterstartPixel != _previousLeftX)
                        {
                            _index++;

                            if (_index == 2)
                            {
                                pixelVal = (int)WMonitorUtile.ViewToScreen(_MaxY, ActualHeight, 0, Property.UpperLimitValue, Property.LowerLimitValue);
                                pixelVal = (int)ActualHeight - pixelVal;

                                afterpixelVal = (int)WMonitorUtile.ViewToScreen(_previousLeftMaxY, ActualHeight, 0, Property.UpperLimitValue, Property.LowerLimitValue);
                                afterpixelVal = (int)ActualHeight - afterpixelVal;
                                pBmp.DrawLine(_maxX, pixelVal, _previousLeftX, afterpixelVal, pLineColor);
                                _index = 1;
                            }    

                            _maxX = _previousLeftX;
                            _MaxY = _previousLeftMaxY;
                            _previousLeftX = afterstartPixel;
                            _previousLeftMaxY = pAmplitudeValues[0];
                        }
                        else
                        {
                            if (pAmplitudeValues[0] > _previousLeftMaxY)
                            {
                                _previousLeftMaxY = pAmplitudeValues[0];
                            }
                        }
                    }
                }
                else
                {
                    if (pPreFreqIndexAmplitude != 12345)
                    {
                        afterpixelVal = (int)WMonitorUtile.ViewToScreen(pAmplitudeValues[0], ActualHeight, 0, Property.UpperLimitValue, Property.LowerLimitValue);
                        afterpixelVal = (int)ActualHeight - afterpixelVal;
                        pBmp.DrawLine(afterstartPixel, (int)ActualHeight, afterstartPixel, afterpixelVal, pLineColor);
                    }
                }
            }
            else
            {
                startPixel = (int)WMonitorUtile.ViewToScreen(index, ActualWidth - 1, 0, pCount - 1, 0);
                pixelVal = (int)WMonitorUtile.ViewToScreen(pAmplitudeValues[0], ActualHeight, 0, Property.UpperLimitValue, Property.LowerLimitValue);
                pixelVal = (int)ActualHeight - pixelVal;

                if (pLineType == SpectrumLineType.Column)
                {
                    pBmp.DrawLine((int)startPixel, (int)ActualHeight, (int)startPixel, (int)pixelVal, pLineColor);
                }
                else if (pLineType == SpectrumLineType.Wave)
                {
                    if (_previousLeftX == -12345)
                    {
                        if (startPixel != _previousLeftX)
                            _maxX = _previousLeftX = startPixel;
                        if (pAmplitudeValues[0] > _previousLeftMaxY)
                            _MaxY = _previousLeftMaxY = pAmplitudeValues[0];
                    }
                }   
            }
        }

        private void DrawWaveLine(WriteableBitmap pBmp, short[] pAmplitudeValues, Color pLineColor)
        {
            if (pBmp == null)
                return;
            pBmp.Clear();

            if (pAmplitudeValues == null)
                return;
            int len = pAmplitudeValues.Length;
            int step = (int)Utile.MathNoRound(len / ActualWidth, 0);
            int ci = 0;
            short max = (short)Property.LowerLimitValue;

            Property.PointCount = (uint)len;
            int previousLeftX = -12345;
            int previousLeftY = 0;
            double freqVal = Property.BeginLeftValue;
            for (int i = 0; i < len; i++)
            {
                short dbuvVal = pAmplitudeValues[i];
                if (dbuvVal == -999 || dbuvVal == 123)
                    continue;
                if (step > 0 && ci < step)
                {
                    ci++;
                    if (dbuvVal > max)
                    {
                        max = dbuvVal;
                    }
                    if (ci < step)
                        continue;
                    dbuvVal = max;
                }
                ci = 0;
                max = (short)Property.LowerLimitValue;

                var x = (int)WMonitorUtile.ViewToScreen(i, ActualWidth - 1, 0, len - 1, 0);
                var pixelVal = (int)WMonitorUtile.ViewToScreen(dbuvVal, ActualHeight, 0, Property.UpperLimitValue, Property.LowerLimitValue);
                pixelVal = (int)ActualHeight - pixelVal;
                freqVal += Property.Span;

                if (previousLeftX == -12345)
                {
                    previousLeftX = x;
                    previousLeftY = pixelVal;
                    continue;
                }

                pBmp.DrawLine(previousLeftX, previousLeftY, x, pixelVal, pLineColor);
                previousLeftX = x;
                previousLeftY = pixelVal;
            }
        }

        private void DrawColumnLine(WriteableBitmap pBmp, short[] pAmplitudeValues, Color pLineColor)
        {
            if (pBmp == null)
                return;
            pBmp.Clear();

            int len = pAmplitudeValues.Length;
            int step = (int)Utile.MathNoRound(len / ActualWidth, 0);
            int ci = 0;
            short max = (short)Property.LowerLimitValue;

            //Property.PointCount = (uint)len;
            double freqVal = Property.BeginLeftValue;
            for (int i = 0; i < len; i++)
            {
                short dbuvVal = pAmplitudeValues[i];
                if (dbuvVal == -999 || dbuvVal == 123)
                    continue;
                if (step > 0 && ci < step)
                {
                    ci++;
                    if (dbuvVal > max)
                    {
                        max = dbuvVal;
                    }
                    if (ci < step)
                        continue;
                    dbuvVal = max;
                }
                ci = 0;
                max = (short)Property.LowerLimitValue;

                double x = WMonitorUtile.ViewToScreen(i, ActualWidth - 1, 0, len - 1, 0);
                double pixelVal = WMonitorUtile.ViewToScreen(dbuvVal, ActualHeight, 0, Property.UpperLimitValue, Property.LowerLimitValue);
                pixelVal = ActualHeight - pixelVal;      
                double lowerpixelVal = WMonitorUtile.ViewToScreen(Property.LowerLimitValue, ActualHeight, 0, Property.UpperLimitValue, Property.LowerLimitValue);
                lowerpixelVal = ActualHeight - lowerpixelVal;
                //绘制柱状图
                pBmp.DrawLine((int)x, (int)lowerpixelVal, (int)x, (int)pixelVal, pLineColor);

                freqVal += Property.Span;
            }
        }

        internal int GetDbuvValue(double pY)
        {
            double vCount = Math.Abs(Property.UpperLimitValue - Property.LowerLimitValue);
            var v = (int)WMonitorUtile.ScreenToView(pY, vCount, 0, ActualHeight, 0);
            return (int)(Property.UpperLimitValue - v);
        }

        #endregion

        #region 方法
        /// <summary>
        /// 刷新谱图
        /// </summary>
        /// <param name="pClear">true - 清理画布，false - 刷新画布</param>
        internal void UpdateSpectrumDiagram(bool pClear = false)
        {
            _waterfallHistory.Clear();
            if (_wbTargetBitmap == null)
                return;
            if (pClear)
            {
                _wbTargetBitmap.Clear(Colors.Transparent);
                ClearBackground();
                ClearNoise();
                ClearMax();
                ClearMin();
                ClearFluorogram();

                _index = 0;
                _previousLeftX = -12345;
                _previousLeftMaxY = -12345;
                _maxX = -12345;
                _MaxY = -12345;
            }
        }
        internal void ClearBackground()
        {
            _wbBackgroundBitmap.Clear(Colors.Transparent);
        }
        internal void ClearNoise()
        {
            _wbNoiseBitmap.Clear(Colors.Transparent);
        }
        internal void ClearMax()
        {
            _wbMaxBitmap.Clear(Colors.Transparent);
        }
        internal void ClearMin()
        {
            _wbMinBitmap.Clear(Colors.Transparent);
        }
        
        /// <summary>
        /// 绘制包络线图
        /// </summary>
        /// <param name="pAmplitudeValues">从开始到结束范围内的上下幅度值集合</param>
        /// <param name="pLineColor">线颜色</param>
        internal void DrawWaveLine(short[] pAmplitudeValues, Color pLineColor)
        {
            DrawWaveLine(_wbTargetBitmap, pAmplitudeValues, pLineColor);
        }
        internal void DrawWaveLineToBackground(short[] pAmplitudeValues, Color pLineColor)
        {
            DrawWaveLine(_wbBackgroundBitmap, pAmplitudeValues, pLineColor);
        }
        internal void DrawWaveLineToNoise(short[] pAmplitudeValues, Color pLineColor)
        {
            DrawWaveLine(_wbNoiseBitmap, pAmplitudeValues, pLineColor);
        }
        internal void DrawWaveLineToMax(short[] pAmplitudeValues, Color pLineColor)
        {
            DrawWaveLine(_wbMaxBitmap, pAmplitudeValues, pLineColor);
        }
        internal void DrawWaveLineToMin(short[] pAmplitudeValues, Color pLineColor)
        {
            DrawWaveLine(_wbMinBitmap, pAmplitudeValues, pLineColor);
        }
        /// <summary>
        /// 绘制柱状线图
        /// </summary>
        /// <param name="pAmplitudeValues">从开始到结束范围内的上下幅度值集合</param>
        /// <param name="pLineColor">线颜色</param>
        internal void DrawColumnLine(short[] pAmplitudeValues, Color pLineColor)
        {
            DrawColumnLine(_wbTargetBitmap, pAmplitudeValues, pLineColor);
        }
        internal void DrawColumnLineToBackground(short[] pAmplitudeValues, Color pLineColor)
        {
            DrawColumnLine(_wbBackgroundBitmap, pAmplitudeValues, pLineColor);
        }
        internal void DrawColumnLineToMax(short[] pAmplitudeValues, Color pLineColor)
        {
            DrawColumnLine(_wbMaxBitmap, pAmplitudeValues, pLineColor);
        }
        internal void DrawColumnLineToMin(short[] pAmplitudeValues, Color pLineColor)
        {
            DrawColumnLine(_wbMinBitmap, pAmplitudeValues, pLineColor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pCount">频率总点数</param>
        /// <param name="pFreqIndex">频率序号</param>
        /// <param name="pAmplitudeValues">数据值 short[]</param>
        /// <param name="pLineColor"></param>
        /// <param name="pLineType"></param>
        /// <param name="pPreFreqIndexAmplitude">缓存中的幅度值</param>
        internal void DrawLine(long pCount, long pFreqIndex, short[] pAmplitudeValues, Color pLineColor, SpectrumLineType pLineType, short pPreFreqIndexAmplitude = -12345)
        {
            if (pAmplitudeValues.Length == 1 && (int)Utile.MathNoRound(pCount / ActualWidth, 0) > 0)
                DrawFscanLine(_wbTargetBitmap, pCount, pFreqIndex, pAmplitudeValues, pLineColor, pLineType, pPreFreqIndexAmplitude);
            else
                DrawLine(_wbTargetBitmap, pCount, pFreqIndex, pAmplitudeValues, pLineColor, pLineType, pPreFreqIndexAmplitude);
        }
        internal void DrawLineToBackground(long pCount, long pFreqIndex, short[] pAmplitudeValues, Color pLineColor, SpectrumLineType pLineType, short pPreFreqIndexAmplitude = -12345)
        {
            DrawLine(_wbBackgroundBitmap, pCount, pFreqIndex, pAmplitudeValues, pLineColor, pLineType, pPreFreqIndexAmplitude);
        }
        internal void DrawLineToNoise(long pCount, long pFreqIndex, short[] pAmplitudeValues, Color pLineColor, SpectrumLineType pLineType, short pPreFreqIndexAmplitude = -12345)
        {
            DrawLine(_wbNoiseBitmap, pCount, pFreqIndex, pAmplitudeValues, pLineColor, pLineType, pPreFreqIndexAmplitude);
        }
        internal void DrawLineToMax(long pCount, long pFreqIndex, short[] pAmplitudeValues, Color pLineColor, SpectrumLineType pLineType, short pPreFreqIndexAmplitude = -12345)
        {
            DrawLine(_wbMaxBitmap, pCount, pFreqIndex, pAmplitudeValues, pLineColor, pLineType, pPreFreqIndexAmplitude);
        }
        internal void DrawLineToMin(long pCount, long pFreqIndex, short[] pAmplitudeValues, Color pLineColor, SpectrumLineType pLineType, short pPreFreqIndexAmplitude = -12345)
        {
            DrawLine(_wbMinBitmap, pCount, pFreqIndex, pAmplitudeValues, pLineColor, pLineType, pPreFreqIndexAmplitude);
        }

        /// <summary>
        /// 绘制瀑布图
        /// </summary>
        /// <param name="pAmplitudeValues">从开始到结束范围内的上下幅度值集合</param>
        /// <param name="pTime"> </param>
        internal void DrawWaterfall(short[] pAmplitudeValues, DateTime? pTime = null)
        {
            if (_wbTargetBitmap == null || pAmplitudeValues == null) return;

            int len = pAmplitudeValues.Length;
            int step = (int)Utile.MathNoRound(len / ActualWidth, 0);
            int ci = 0;
            short max = (short)Property.LowerLimitValue;

            if (_waterfallHistory.Count > ActualHeight)
                _waterfallHistory.RemoveAt(_waterfallHistory.Count - 1);

            int rowIndex = 0;
            foreach (var pLineColor in _waterfallHistory)
            {
                if (rowIndex >= _wbTargetBitmap.PixelHeight)
                    break;
                byte[] pixelData = new byte[_wbTargetBitmap.BackBufferStride];

                for (int i = 0; i < pLineColor.Colors.Length; i++)
                {
                    if ((i + 3) >= pixelData.Length)
                    {
                        break;
                    }
                    int index = i * 4;
                    var color = pLineColor.Colors[i];
                    pixelData[index] = color.B;
                    pixelData[index + 1] = color.G;
                    pixelData[index + 2] = color.R;
                    pixelData[index + 3] = color.A;
                }

                _wbTargetBitmap.WritePixels(new Int32Rect(0, rowIndex, _wbTargetBitmap.PixelWidth, 1), pixelData, _wbTargetBitmap.BackBufferStride, 0);
                rowIndex++;
            }

            var cs = new Color[_wbTargetBitmap.PixelWidth];
            int prevX = 0;
            int prePixelVal = 0;

            for (int i = 0; i < len; i++)
            {
                short dbuvVal = pAmplitudeValues[i];
                if (step > 0 && ci < step)
                {
                    ci++;
                    if (dbuvVal > max)
                    {
                        max = dbuvVal;
                    }
                    if (ci < step)
                        continue;
                    dbuvVal = max;
                }
                ci = 0;
                max = (short)Property.LowerLimitValue;

                var x = (int)WMonitorUtile.ViewToScreen(i, ActualWidth - 1, 0, len - 1, 0);
                if (x >= ActualWidth)
                    x = x - 1;
                if (x < 0) return;
                var pixelVal = (int)WMonitorUtile.ViewToScreen(dbuvVal, ActualHeight, 0, Property.UpperLimitValue, Property.LowerLimitValue);
                pixelVal = (int)ActualHeight - pixelVal;
                if (x >= cs.Length)
                    x = cs.Length - 1;
                cs[x] = GetColorFromColorPanel(pixelVal);
                int nx = x - prevX;
                if (nx > 1)
                {
                    //中间有空隙时
                    int cpixel = (prePixelVal - (prePixelVal - pixelVal) / 2);
                    //Color scolor = GetColorFromColorPanel(prePixelVal);
                    //Color ccolor = GetColorFromColorPanel(cpixel);
                    //Color ecolor = GetColorFromColorPanel(pixelVal);
                    //var rect = new Rectangle { Width = nx, Height = 2 };
                    //var lgBrush = new LinearGradientBrush { StartPoint = new Point(1, 0), EndPoint = new Point(0, 1) };
                    //var gs1 = new GradientStop { Color = scolor, Offset = 0.0 };
                    //var gs2 = new GradientStop { Color = ccolor, Offset = 0.5 };
                    //var gs3 = new GradientStop { Color = ecolor, Offset = 1 };
                    //lgBrush.GradientStops.Add(gs1);
                    //lgBrush.GradientStops.Add(gs2);
                    //lgBrush.GradientStops.Add(gs3);
                    //rect.Fill = lgBrush;

                    //rect.Arrange(new Rect(0, 0, rect.Width, rect.Height));

                    //RenderTargetBitmap rtb = new RenderTargetBitmap(nx, 2, 96, 96, PixelFormats.Pbgra32);
                    //rtb.Render(rect);
                    //var rectBmp = new WriteableBitmap(rtb);
                    for (int nxIndex = 1; nxIndex < nx; nxIndex++)
                    {
                        //cs[x - nxIndex] = rectBmp.GetPixel(nxIndex, 0);
                        cs[x - nxIndex] = GetColorFromColorPanel(cpixel);
                    }
                }
                prevX = x;
                prePixelVal = pixelVal;
            }

            var waterItem = new WaterfallItem();
            int cslen = cs.Length;
            waterItem.Colors = new Color[cslen];
            Array.Copy(cs, waterItem.Colors, cslen);
            waterItem.DrawTime = pTime;
            _waterfallHistory.Insert(0, waterItem);
        }

        internal WaterfallItem? GetWaterfallItem(int pRowIndex)
        {
            if (pRowIndex < 0 || pRowIndex >= _waterfallHistory.Count)
                return null;
            return _waterfallHistory[pRowIndex];
        }

        public IList<DateTime> WaterfallTimes
        {
            get
            {
                return (from w in _waterfallHistory let drawTime = w.DrawTime where drawTime != null where drawTime != null select drawTime.Value).ToList();
            }
        }
        #endregion

        #region 中频荧光谱图
        private WriteableBitmap _wbFluoroColorPanel;
        private double _zoombeforeleftfreq = 0;
        private double _zoombeforerightfreq = 0;
        private int _durationtime = 0;
        private FluoroScanInfo _scanInfo = new FluoroScanInfo();
        private DispatcherTimer _fluoroTimer = new DispatcherTimer(DispatcherPriority.Send);
        private int _timecount = 0;
        private int _maxcolorradio = 1;
        private int _mincolorradio = 10;
        private int _pretime = 1;
        public void SetFluoro(int duration)
        {
            _durationtime = duration;
            SetFluoroColorPanel();
        }

        /// <summary>
        /// 清除荧光谱
        /// </summary>
        internal void ClearFluorogram()
        {
            _FluoroPixelInfo.Clear();
            _wbFluorogramBitmap.Clear(Colors.Transparent);
            _scanInfo = new FluoroScanInfo();

            _FluoroColorCache = new Color[(int)ActualHeight * (int)ActualWidth];

            for (int i = 0; i < _FluoroColorCache.Length; i++)
            {
                _FluoroColorCache[i] = _mediaColor;
            }
        }

        internal void StopFluoroTimer()
        {
            if (_fluoroTimer.IsEnabled)
            {
                _fluoroTimer.Stop();
                _fluoroTimer.IsEnabled = false;
            }
        }

        internal void ClearCacheForZoom(double leftfreq, double rightfreq)
        {
            if (_zoombeforeleftfreq != leftfreq || _zoombeforerightfreq != rightfreq)
            {
                _zoombeforeleftfreq = leftfreq;
                _zoombeforerightfreq = rightfreq;

                ClearFluorogram();
            }
        }
        /// <summary>
        /// 绘制中频荧光谱图
        /// </summary>
        /// <param name="pAmplitudeValues">从开始到结束范围内的上下幅度值集合</param>
        /// <param name="ptime">时间</param>
        /// <param name="durationtime">持续时间</param>
        internal void DrawFluoroPoint(short[] pAmplitudeValues, DateTime ptime, int durationtime)
        {
            if (_scanInfo.ScanAllCount == 0)
            {
                _fluoroTimer.IsEnabled = true;
                _fluoroTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                _fluoroTimer.Tick -= _fluoroTimer_Tick;
                _fluoroTimer.Tick += _fluoroTimer_Tick;
                _fluoroTimer.Start();
                _timecount = 1;
            }

            _scanInfo.ScanAllCount++;
            if (!_scanInfo.ScanCountForTime.ContainsKey(_timecount))
                _scanInfo.ScanCountForTime.Add(_timecount, 0);
            _scanInfo.ScanCountForTime[_timecount]++;

            DrawFluoroPoint(_wbFluorogramBitmap, pAmplitudeValues, _timecount);
        }

        void _fluoroTimer_Tick(object sender, EventArgs e)
        {
            _timecount++;

            //if (_FluoroPixelInfo.Count > 1)
            //{
            //    int mintime = _FluoroPixelInfo.First().Value.ShowCountForTime.Keys.Min();
            //    if (_timecount - mintime >= 2)
            //        ReDrawPixel(_timecount - 1);
            //}

            //if (_timecount * 2 > _durationtime)
            //{
            //    if (_FluoroPixelInfo.Count < 1)
            //        return;
            //    int mintime = _FluoroPixelInfo.First().Value.ShowCountForTime.Keys.Min();

            //    var listkey = _FluoroPixelInfo.Keys.ToList();
            //    for (int i = 0; i < listkey.Count; i++)
            //    {
            //        SetFluoroPixelInfos(listkey[i].Item1, listkey[i].Item2, mintime, -1);
            //        if (_FluoroPixelInfo.ContainsKey(listkey[i]) && _FluoroPixelInfo[listkey[i]].PixelShowCount == 0)
            //            _FluoroPixelInfo.Remove(listkey[i]);
            //    }

            //    if (_scanInfo.ScanCountForTime.ContainsKey(mintime))
            //    {
            //        _scanInfo.ScanAllCount = _scanInfo.ScanAllCount - _scanInfo.ScanCountForTime[mintime];
            //        _scanInfo.ScanCountForTime.Remove(mintime);
            //    }
            //}
        }
        private void DrawFluoroPoint(WriteableBitmap pBmp, short[] pAmplitudeValues, int seconds)
        {
            if (pBmp == null)
                return;

            if (pAmplitudeValues == null)
                return;

            int len = pAmplitudeValues.Length;
            int step = (int)Utile.MathNoRound(len / ActualWidth, 0);
            int ci = 0;
            short max = (short)Property.LowerLimitValue;

            Property.PointCount = (uint)len;

            int previousLeftX = -12345;
            int previousLeftY = 0;
            for (int i = 0; i < len; i++)
            {
                short dbuvVal = pAmplitudeValues[i];
                if (step > 0 && ci < step)
                {
                    ci++;
                    if (dbuvVal > max)
                    {
                        max = dbuvVal;
                    }
                    if (ci < step)
                        continue;
                    dbuvVal = max;
                }
                ci = 0;
                max = (short)Property.LowerLimitValue;

                var x = (int)WMonitorUtile.ViewToScreen(i, ActualWidth - 1, 0, len - 1, 0);
                var pixelVal = (int)WMonitorUtile.ViewToScreen(dbuvVal, ActualHeight, 0, Property.UpperLimitValue, Property.LowerLimitValue);
                pixelVal = (int)ActualHeight - pixelVal;

                if (x >= (int)ActualWidth)
                    x = (int)ActualWidth - 1;

                if (pixelVal >= (int)ActualHeight)
                    pixelVal = (int)ActualHeight - 1;

                if (x < 0)
                    x = 0;

                if (pixelVal < 0)
                    pixelVal = 0;

                if (previousLeftX == -12345)
                {
                    previousLeftX = x;
                    previousLeftY = pixelVal;
                    continue;
                }

                SetFluoroPixelInfos(x, pixelVal, seconds, 1);
                //Color cl = Colors.Blue;
                //var pixel = new Tuple<int, int>(x, pixelVal);
                //if (_FluoroPixelInfo[pixel].PixelShowCount != _scanInfo.ScanAllCount)
                //    cl = _FluoroColorCache[pixelVal * (int)ActualWidth + x];

                //pBmp.DrawLine(previousLeftX, previousLeftY, x, pixelVal, cl);
                //previousLeftX = x;
                //previousLeftY = pixelVal;    
            }

           DrawPixels();
        }

        /// <summary>
        /// 设置像素信息
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="time"></param>
        /// <param name="countchange">根据该值设置像素出现次数。1，表示像素出现，0，表示time时间像素没有出现，-1删除该time像素</param>
        private void SetFluoroPixelInfos(int x, int y, int time, int countchange)
        {
            var pixel = new Tuple<int, int>(x, y);
            FluoroPixelInfo pixelinfo = new FluoroPixelInfo();
            Color newcolor = GetColorFromFluoroColorPanel(0);
            int allcount = 0;
            int percount = 0;
            int colorradio = 0;

            Dictionary<int, int> dicShowCountTime = new Dictionary<int, int>();

            if (_FluoroPixelInfo.Count != 0 && _FluoroPixelInfo.ContainsKey(pixel))
            {
                allcount = _FluoroPixelInfo[pixel].PixelShowCount;
                newcolor = _FluoroPixelInfo[pixel].PixelColor;
                dicShowCountTime = _FluoroPixelInfo[pixel].ShowCountForTime;
            }

            if (countchange == 1)
            {
                allcount++;
                if (dicShowCountTime.ContainsKey(time))
                    percount = dicShowCountTime[time];
                percount++;

                if (!dicShowCountTime.ContainsKey(time))
                    dicShowCountTime.Add(time, percount);
                dicShowCountTime[time] = percount;
            }
            else if (countchange == -1)
            {
                if (dicShowCountTime.ContainsKey(time))
                {
                    allcount = allcount - dicShowCountTime[time];
                    dicShowCountTime.Remove(time);
                }
            }
            else
            {
                if (!dicShowCountTime.ContainsKey(time))
                    dicShowCountTime.Add(time, 0);
            }

            if (allcount < 0)
                allcount = 0;

            if (countchange != -1)
            {
                int mintimecount = dicShowCountTime[dicShowCountTime.Keys.Min()];
                if (_durationtime == dicShowCountTime.Count)
                    colorradio = (allcount - mintimecount) * 10 / _scanInfo.ScanAllCount;
                else
                    colorradio = allcount * 10 / _scanInfo.ScanAllCount;

                if (_pretime != time)
                {
                    _maxcolorradio = 1;
                    _mincolorradio = 10;
                }

                if (colorradio > _maxcolorradio)
                {
                    _maxcolorradio = colorradio;
                }

                if (_maxcolorradio > 10)
                    _maxcolorradio = 10;

                if (colorradio < _mincolorradio)
                {
                    _mincolorradio = colorradio;
                }

                if (_mincolorradio < 0)
                    _mincolorradio = 0;

               //newcolor = GetColorFromFluoroColorPanel((dicShowCountTime[time] - _mincolorradio) * 10 / _maxcolorradio);
                 newcolor = GetColorFromFluoroColorPanel(allcount * 10 * 5/ _scanInfo.ScanAllCount);

                if ((y * (int)ActualWidth + x) >= (int)(ActualHeight * ActualWidth))
                    _FluoroColorCache[(int)(ActualHeight * ActualWidth) - 1] = newcolor;
                else
                    _FluoroColorCache[y * (int)ActualWidth + x] = newcolor;
            }

            pixelinfo.PixelShowCount = allcount;
            pixelinfo.PixelColor = newcolor;
            pixelinfo.ShowCountForTime = dicShowCountTime;

            if (!_FluoroPixelInfo.ContainsKey(pixel))
                _FluoroPixelInfo.Add(pixel, pixelinfo);
            _FluoroPixelInfo[pixel] = pixelinfo;

            _pretime = time;
        }
        /// <summary>
        /// 绘制像素区域
        /// </summary>
        /// <param name="pBmp"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void DrawPixels()
        {
            byte[] pixelData = new byte[_wbFluorogramBitmap.BackBufferStride * (int)ActualHeight];

            for (int i = 0; i < _FluoroColorCache.Length; i++)
            {
                if ((i + 3) >= pixelData.Length)
                {
                    break;
                }
                int index = i * 4;
                var color = _FluoroColorCache[i];
                pixelData[index] = color.B;
                pixelData[index + 1] = color.G;
                pixelData[index + 2] = color.R;
                pixelData[index + 3] = color.A;
            }

            _wbFluorogramBitmap.WritePixels(new Int32Rect(0, 0, _wbFluorogramBitmap.PixelWidth, _wbFluorogramBitmap.PixelHeight), pixelData, _wbFluorogramBitmap.BackBufferStride, 0);
        }

        /// <summary>
        /// 对没有出现的像素重绘
        /// </summary>
        /// <param name="time"></param>
        private void ReDrawPixel(int time)
        {
            var listkey = _FluoroPixelInfo.Keys.ToList();
            for (int i = 0; i < listkey.Count; i++)
            {
                if (!_FluoroPixelInfo[listkey[i]].ShowCountForTime.ContainsKey(time))
                {
                    SetFluoroPixelInfos(listkey[i].Item1, listkey[i].Item2, time, 0);
                }
            }
        }
        /// <summary>
        /// 设置荧光谱图颜色面板
        /// </summary>
        public void SetFluoroColorPanel()
        {
            var rect = new Rectangle { Width = 5, Height = 10 };
            var lgBrush = new LinearGradientBrush { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
            var gs1 = new GradientStop { Color = Colors.Red, Offset = 1 };
            var gs2 = new GradientStop { Color = Color.FromArgb(0xff, 0x7c, 0xfc, 0x00), Offset = 0.5 };
            var gs3 = new GradientStop { Color = Colors.Blue, Offset = 0.0 };
            lgBrush.GradientStops.Add(gs1);
            lgBrush.GradientStops.Add(gs2);
            lgBrush.GradientStops.Add(gs3);

            rect.Fill = lgBrush;
            rect.Arrange(new Rect(0, 0, rect.Width, rect.Height));

            RenderTargetBitmap rtb = new RenderTargetBitmap(5, (int)10, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(rect);
            _wbFluoroColorPanel = new WriteableBitmap(rtb);
        }
        /// <summary>
        /// 根据显示次数比例获取颜色
        /// </summary>
        /// <param name="pValue"></param>
        /// <returns></returns>
        private Color GetColorFromFluoroColorPanel(int pValue)
        {
            if (_wbFluoroColorPanel == null)
                return Colors.Transparent;

            if (pValue < 0)
                return _wbFluoroColorPanel.GetPixel(1, 0);
            if (pValue >= _wbFluoroColorPanel.PixelHeight)
                return _wbFluoroColorPanel.GetPixel(1, _wbFluoroColorPanel.PixelHeight - 1);
            return _wbFluoroColorPanel.GetPixel(1, pValue);
        }

        #endregion
    }

    /// <summary>
    /// 谱线类型
    /// </summary>
    public enum SpectrumLineType
    {
        /// <summary>
        /// 波形图
        /// </summary>
        Wave,
        /// <summary>
        /// 柱状图
        /// </summary>
        Column,
        /// <summary>
        /// 瀑布图
        /// </summary>
        Waterfall
    }

    /// <summary>
    /// 谱图属性
    /// </summary>
    public class SpectrumDiagramProperty : INotifyPropertyChanged
    {
        private Color _upperLimitValueColor;
        private Color _centerLimitValueColor;
        private Color _lowerLimitValueColor;
        private double _upperLimitValue;
        private double _lowerLimitValue;
        private double _beginLeftValue;
        private double _endRightValue;
        private double _span = 1;

        public Color UpperLimitValueColor
        {
            get { return _upperLimitValueColor; }
            set
            {
                _upperLimitValueColor = value;
                OnPropertyChanged("UpperLimitValueColor");
            }
        }

        public Color CenterLimitValueColor
        {
            get { return _centerLimitValueColor; }
            set { _centerLimitValueColor = value; OnPropertyChanged("CenterLimitValueColor"); }
        }

        public Color LowerLimitValueColor
        {
            get { return _lowerLimitValueColor; }
            set { _lowerLimitValueColor = value; OnPropertyChanged("LowerLimitValueColor"); }
        }

        /// <summary>
        /// 上限值
        /// </summary>
        public double UpperLimitValue
        {
            get { return _upperLimitValue; }
            set { _upperLimitValue = value; OnPropertyChanged("UpperLimitValue"); }
        }

        /// <summary>
        /// 下限值
        /// </summary>
        public double LowerLimitValue
        {
            get { return _lowerLimitValue; }
            set { _lowerLimitValue = value; OnPropertyChanged("LowerLimitValue"); }
        }

        /// <summary>
        /// 左侧开始值
        /// </summary>
        public double BeginLeftValue
        {
            get { return _beginLeftValue; }
            set { _beginLeftValue = value; OnPropertyChanged("BeginLeftValue"); }
        }

        /// <summary>
        /// 右侧结束值，要求值始终大于左侧开始值
        /// </summary>
        public double EndRightValue
        {
            get { return _endRightValue; }
            set { _endRightValue = value; OnPropertyChanged("EndRightValue"); }
        }
        /// <summary>
        /// 获取或设置频点之间的跨距
        /// </summary>
        public double Span
        {
            get { return _span; }
            set
            {
                _span = value; OnPropertyChanged("Span");
            }
        }

        /// <summary>
        /// 获取或设置频点总数量
        /// </summary>
        public long PointCount { get; set; }

        protected void OnPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>
    /// 谱图缩放级别
    /// </summary>
    public class SpectrumZoomLevel
    {
        public double? UpperLimitValue { get; set; }
        public double? LowerLimitValue { get; set; }
        public double? BeginLeftValue { get; set; }
        public double? EndRightValue { get; set; }
        public uint PointCount { get; set; }
    }
    /// <summary>
    /// 每帧瀑布图数据
    /// </summary>
    public struct WaterfallItem
    {
        public Color[] Colors { get; set; }
        public DateTime? DrawTime { get; set; }
    }
    /// <summary>
    /// 记录像素点信息，包括出现次数，显示颜色等
    /// </summary>
    public class FluoroPixelInfo
    {
        private Dictionary<int, int> _diccountfortime = new Dictionary<int, int>();
        public int NewColor { get; set; }
        public Color PixelColor { get; set; }
        /// <summary>
        /// 像素点出现总次数
        /// </summary>
        public int PixelShowCount { get; set; }
        /// <summary>
        /// 不同时间像素点出现的次数
        /// </summary>
        public Dictionary<int, int> ShowCountForTime
        {
            get { return _diccountfortime; }
            set { _diccountfortime = value; }
        }
    }
    /// <summary>
    /// 记录每秒扫描帧数及总帧数
    /// </summary>
    public class FluoroScanInfo
    {
        private Dictionary<int, int> _diccountfortime = new Dictionary<int, int>();
        /// <summary>
        /// 扫描总次数
        /// </summary>
        public int ScanAllCount { get; set; }
        /// <summary>
        /// 不同时间扫描次数
        /// </summary>
        public Dictionary<int, int> ScanCountForTime
        {
            get { return _diccountfortime; }
            set { _diccountfortime = value; }
        }
    }
}
