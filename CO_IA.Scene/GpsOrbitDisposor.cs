using CO_IA.Data;
using CO_IA.Data.Gps;
using CO_IA.GpsLib;
using I_CO_IA.Gps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Scene
{
    internal class GpsOrbitDisposor : IGpsDataInvoker
    {
        public AT_BC.Data.IAsyncExceptionReceiver ExceptionReceiver
        {
            get;
            set;
        }

        private string gpsIdentificationNumber;
        private string _activityId;
        private GpsOrbitValidator validator;
        private object syncObj = new object();

        private GpsOrbitDisposor(string identificationNumber, string pActivityId)
        {
            this.gpsIdentificationNumber = identificationNumber;
            this._activityId = pActivityId;
            var gpsConfig = CO_IA.Client.RiasPortal.GetTypeSessionParam<GpsDataAnalyseConfig>();
            if (gpsConfig == null)
            {
                gpsConfig = new GpsDataAnalyseConfig();
            }
            this.validator = GpsOrbitValidator.CreateGpsOrbitValidator(gpsConfig);
        }

        public static GpsOrbitDisposor CreateDisposor(string identificationNumber,string pActivityId)
        {
            return new GpsOrbitDisposor(identificationNumber, pActivityId);
        }

        private System.Threading.Thread disposeThread;
        private Queue<CO_IA.Data.Gps.GpsOrbit> queueOrbit = new Queue<Data.Gps.GpsOrbit>();
        public void Start()
        {
            this.disposeThread = new System.Threading.Thread(GpsOrbitDisposeThread);
            this.disposeThread.IsBackground = true;
            this.disposeThread.Start();
        }

        Action<GpsOrbit> saveGpsOrbit = new Action<GpsOrbit>(data =>
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Gps>(channel =>
                {
                    channel.SaveGpsOrbit(data);
                });
            });

        private void GpsOrbitDisposeThread()
        {
            try
            {
                //queueOrbit.Enqueue(this.CreateGpsOrbit(137.0125, 43.0002));
                //queueOrbit.Enqueue(this.CreateGpsOrbit(137.0135, 43.0002));
                //queueOrbit.Enqueue(this.CreateGpsOrbit(137.0145, 43.0002));
                //queueOrbit.Enqueue(this.CreateGpsOrbit(137.0155, 43.0002));
                //queueOrbit.Enqueue(this.CreateGpsOrbit(137.0165, 43.0002));
                GpsOrbit data;
                while (true)
                {
                    if (queueOrbit.Count > 0)
                    {
                        try
                        {
                            lock (syncObj)
                            {
                                data = queueOrbit.Dequeue();
                            }
                            if (this.validator.Filter(data))
                            {
                                this.saveGpsOrbit.BeginInvoke(data, asyncResult =>
                                {
                                    try
                                    {
                                        this.saveGpsOrbit.EndInvoke(asyncResult);
                                    }
                                    catch (System.Threading.ThreadAbortException)
                                    {
                                    }
                                    catch(Exception ex)
                                    {
                                        this.ThrowAsyncException(ex);
                                    }
                                }, null);
                                //写入数据库
                                //PT_BS_Service.Client.Framework.BeOperationInvoker.InvokeAsync<I_CO_IA_Gps>(channel =>
                                //    {
                                //        channel.SaveGpsOrbit(data);
                                //    }, null, null);
                            }
                        }
                        catch (System.Threading.ThreadAbortException)
                        {
                        }
                        catch(Exception ex)
                        {
                            this.ThrowAsyncException(ex);
                        }
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
        }

        private void ThrowAsyncException(Exception ex)
        {
            if (this.ExceptionReceiver != null)
            {
                this.ExceptionReceiver.SyncContext.Post(state =>
                {
                    this.ExceptionReceiver.Receive(ex);
                }, null);
            }
        }

        private GpsOrbit CreateGpsOrbit(double longitude,double latitude)
        {
            GpsOrbit data = new GpsOrbit();
            data.Height = 100;
            data.Latitude = latitude;
            data.Longitude = longitude;
            data.LongitudeHemisphere = "E";
            data.LatitudeHemisphere = "N";
            data.LocationState = "A";
            data.Guid = Guid.NewGuid().ToString();
            data.PlateNumber = "941dbcab-4cca-47d4-a992-9776174c5a68";
            data.RunDate = DateTime.Now;
            data.RunTime = double.Parse(DateTime.Now.ToString("HHmmss.fff"));
            data.Speed = 100;
            data.UsingStartNum = 10;
            return data;

        }

        public void Dispose(ref Gps.Decoder_Gps_Data gpsDataHeader)
        {
            CO_IA.Data.Gps.GpsOrbit gpsOrbit = new CO_IA.Data.Gps.GpsOrbit { PlateNumber = this.gpsIdentificationNumber, ActivityId = this._activityId };
            gpsOrbit.RunTime = gpsDataHeader.BTC_Time;
            gpsOrbit.LocationState = gpsDataHeader.LocationState.ToString();
            gpsOrbit.Latitude = gpsDataHeader.Latitude;
            gpsOrbit.Longitude = gpsDataHeader.Longitude;
            gpsOrbit.LatitudeHemisphere = gpsDataHeader.LatitudeHemisphere.ToString();
            gpsOrbit.LongitudeHemisphere = gpsDataHeader.LongitudeHemisphere.ToString();
            gpsOrbit.Height = gpsDataHeader.Height;
            gpsOrbit.Speed = gpsDataHeader.Speed;
            gpsOrbit.UsingStartNum = gpsDataHeader.UsingStartNum;
            lock (syncObj)
            {
                this.queueOrbit.Enqueue(gpsOrbit);
            }

            foreach (var receiver in listGpsOrbitReceiver)
            {
                receiver.SyncContext.Post(state =>
                {
                    receiver.Receive(gpsOrbit);
                }, null);
            }
        }

        private List<IGpsOrbitReceiver> listGpsOrbitReceiver=new List<IGpsOrbitReceiver>();

        public void RegisterReceiver(IGpsOrbitReceiver receiver)
        {
            this.listGpsOrbitReceiver.Add(receiver);
        }

        public void Stop()
        {
            if (this.disposeThread != null)
            {
                this.disposeThread.Abort();
                this.disposeThread = null;
            }

        }
    }
}
