using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AgilentDll
{

    public class Sensor
    {
        #region Agilent Struct

        public enum MeasurementMode
        {
            None = 0,      // No Measurement
            SpectrumMonitoring = 1,      // Spectrum Monitoring
            WaveformMonitoring = 2,      // Waveform Monitoring
            TdoaWaveformMonitoring = 3,      // TDOA Waveform
            Error = 65536   // Error Mode*/      
        };
        public enum GpsMode
        {
            Static,
            Mobile
        };

        public enum AntennaType
        {
            TestSignal = -4,  /**<  Connect input to internal comb generator (NOTE: due to the high signal level of the internal signal,
                                       the comb generator may cause detectable radiation from an antenna connected to a sensor input) */
            Auto = -3,  /**<  Select antenna as configured by the SMS */
            Unknown = -2,  /**<  Unknown antenna type */
            Termination = -1,  /**< Sensor internal 50 ohm termination */

            Antenna_1 = 0,            /**< Sensor Antenna 1 input */
            Antenna_2 = 1             /**< Sensor Antenna 2 input */
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct AntennaAuto
        {
            public double startFreq;        /**< Start frequency for this frequency band */
            public double stopFreq;         /**< Stop frequency for this frequency band */
            public double attenuation;      /**< attenuation for this frequency band */
            public double mixerLevel;       /**< mixerLevel for this frequency band */
            public Int32 preamp;            /**< preamp for this frequency band */
            public AntennaType antenna;     /**< antenna for this frequency band */
        };

        public enum DataType
        {
            None = -1,
            Complex32 = 0,
            Complex16,
            COMPLEX_FLOAT32,   /**< 32 bit float complex pairs */
            REAL_INT8,         /**< 8 bit integer real data */
            REAL_INT8_ALAW,    /**< 8 bit integer real data with A-law encoding*/
            REAL_INT8_ULAW,    /**< 8 bit integer real data with A-law encoding*/
            REAL_INT16,        /**< 16 bit integer real data */
            REAL_FLOAT32,       /**< 32 bit float real data */
            REAL_FLOAT32_DBM,   /**< 32 bit float real data in units of dBm */
            NumDataType
        };

        public enum SensorMode
        {
            None = 0,          /**< No Measurement             */
            Tdoa = 3,          /**< TDOA measurement           */
            Default = 100,     /**< E3238s or IQ measurement   */
            Error = -1         /**< Error mode                 */
        };

        public enum Service
        {
            None,   /**< Undefined mode */
            IQ,     /**< IQ data (may be streaming or block mode, see salMode) */
            Diagnostics,
            NumService
        };

        public enum TimeSyncAlarmBits
        {
            ClockSet = 1,
            TimeQuestionable = 2,
        };

        public enum StatusBits
        {
            StatusNotRead = 1,
            CommunicationDown = 2,
            PowerQuestionable = 4,
            FrequencyQuestionable = 8,
            TemperatureQuestionable = 16,
            CalibrationQuestionable = 32
        };

        public enum TriggerType
        {
            RelativeTime = 0,      /**< trigger after a specified time has elapsed */
            RelativeLevel = 1,      /**< trigger when signal exceeds a threshold by a specified amount */
            AbsoluteLevel = 2,      /**< trigger when signal exceeds specified level */
        };

        public enum SensorEventType
        {
            Shutdown,      /**< The sensor powered off */
            Disconnected,       /**< The connection to the sensor was terminated by another application */
            CommunicationError,    /**< The sensor failed to send or read a message */
            Unknown,       /**< An unknown event occurred */
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct Location
        {
            public double latitude;         /**< In fractional degrees, southern latitudes are negative numbers */
            public double longitude;        /**< In fractional degrees, western longitudes are negative numbers */
            public double elevation;        /**< In meters  */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_GEOGRAPHIC_DATUM)]
            public string geographicDatum; /**<  */
            public double x_offset;         /**< in meters  */
            public double y_offset;         /**< in meters  */
            public double z_offset;         /**< in meters  */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_UNIT)]
            public string xyz_unit;
            public UInt32 latitudeType;
            public UInt32 longitudeType;
            public double rotation;         /**< In degrees, counter-clockwise from Longitude */

            public override string ToString()
            {
                return latitude.ToString("0.000000000") + ", " + longitude.ToString("0.000000000");
            }
        }

        public const int MAX_SENSOR_NAME = 64;
        public const int MAX_ERROR_STRING = 64;
        public const int MAX_SENSOR_HOSTNAME = 64;
        public const int MAX_GEOGRAPHIC_DATUM = 64;
        public const int MAX_APPLICATION_NAME = 64;
        public const int MAX_FILENAME = 256;
        public const int MAX_COMMENT = 256;
        public const int MAX_UNIT = 32;

        public enum MiscellaneousConstants
        {
            SAL_DEFAULT_QUEUED_MSGS = 10, /**< Default number of data messages that will be queued */
            SAL_MIN_LOCATION_IMAGE_PIXELS = 20, /**< Minimum number of pixels for the height or width of the location image */
            SAL_MAX_LOCATION_IMAGE_PIXELS = 1000, /**< Maximum number of pixels for the height or width of the location image */
            SAL_LOCATION_SPECTRUM_POINTS = 401,  /**< Number of points in the location measurement spectra */
            SAL_LOCATION_CORRELATION_POINTS = 401, /**< Number of points in the location measurement correlation */

            SAL_MIN_AUDIO_SAMPLES = 64,  /**< Minimum blocksize when using demodulation */

        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct SensorStatus
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_NAME)]
            public string name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string hostName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string userHostName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_APPLICATION_NAME)]
            public string userApplicationName;
            public UInt32 lastHeardFrom;
            public Location location;
            public Int16 currentMode;
            public Int16 timeSyncAlarms;
            public Int16 systemAlarms;
            public Int16 integrityAlarms;

            public enum SystemAlarmBits
            {
                OperationSuspended = 1,
                LearnEnvironmentInProgress = 2,
                LearnEnvironmentFailed = 4,
                RfPoweredDown = 8,
                SmsCommunicationQuestionable = 16,
                FpgaUsingBackupImage = 32,
            };


            public enum TimeSyncBits
            {
                ClockNotSet = 1,
                TimeQuestionable = 2,
            };

            public enum IntegrityBits
            {
                FrequencyQuestionable = 1,
                TemperatureQuestionable = 2,
                TemperatureShutdownPending = 4,
                RfPoweredDown = 8,
                CalibrationQuestionable = 16,
                CaseOpen = 32,
                WatchDogDisabled = 64,
                GpxTxDisabled = 128
            };

            public bool IsFrequencyQuestionable
            {
                get { return (integrityAlarms & (int)IntegrityBits.FrequencyQuestionable) != 0; }
            }
        }

        public const string SMS_ENV_VARIABLE_NAME = "AGILENTSMS";

        private const int AgSalSensorStatusSize = 2048;

        public const int MAX_SAMPLES_PER_TRANSFER_TCP = 32768;
        public const int MAX_SAMPLES_PER_TRANSFER_UDP = 4096;

        public const int salRECOMMENDED_DATA_BUFFER_SIZE = 32768 * 2 * 4;

        public enum SalError
        {
            SAL_ERR_NONE = 0,	                    /**< No Error */
            SAL_ERR_NOT_IMPLEMENTED = -1,	        /**< This functionality is not implemented yet. */
            SAL_ERR_UNKNOWN = -2,	                /**< Error of unspecified type */
            SAL_ERR_BUSY = -3,	                    /**< The system is busy */
            SAL_ERR_TRUNCATED = -4,	                /**< Unspecified error */
            SAL_ERR_ABORTED = -5,	                /**< The measurement was aborted */
            SAL_ERR_RPC_NORESULT = -6,	            /**< The server accepted the call but returned no result */
            SAL_ERR_RPC_FAIL = -7,	                /**< The RPC call to the server failed completely */
            SAL_ERR_PARAM = -8,	                    /**< Incorrect parameter in call. */
            SAL_ERR_MEAS_IN_PROGRESS = -9,	        /**< Another measurement is currently in progress */
            SAL_ERR_NO_RESULT = -10,	            /**< No result was returned */
            SAL_ERR_SENSOR_NAME_EXISTS = -11,	    /**< The sensor name specified already exists */
            SAL_ERR_INVALID_CAL_FILE = -12,      	/**< The calibration file has an invalid format */
            SAL_ERR_NO_SUCH_ANTENNAPATH = -13,   	/**< The antenna path specified does not exist */
            SAL_ERR_INVALID_SENSOR_NAME = -14,	    /**< The sensor name specified does not exist */
            SAL_ERR_INVALID_MEASUREMENT_ID = -15,	/**< The given measurement ID is not valid */
            SAL_ERR_INVALID_REQUEST = -16,	        /**< Internal system error */
            SAL_ERR_MISSING_MAP_PARAMETERS = -17,	/**< You need to specify map coordinates */
            SAL_ERR_TOO_LATE = -18,	                /**< The measurement arrived at the sensor too late */
            SAL_ERR_HTTP_TRANSPORT = -19,	        /**< An HTTP error occurred when trying to talk to the sensors */
            SAL_ERR_NO_SENSORS = -20,	            /**< No sensors available for measurement */
            SAL_ERR_NOT_ENOUGH_TIMESERIES = -21,	/**< Not enough timeseries im measurement */
            SAL_ERR_NATIVE = -22,	                /**< Error in native code */
            SAL_ERR_BAD_SENSOR_LOCATION = -23,	    /**< Invalid sensor location */
            SAL_ERR_DATA_CHANNEL_OPEN = -24,        /**< Data Channel already open */
            SAL_ERR_DATA_CHANNEL_NOT_OPEN = -25,    /**< Data Channel not open */
            SAL_ERR_SOCKET_ERROR = -26,			    /**< Socket error */
            SAL_ERR_SENSOR_NOT_CONNECTED = -27,     /**< Sensor not connected */
            SAL_ERR_NO_DATA_AVAILABLE = -28,        /**< No data available */
            SAL_ERR_NO_SMS = -29,                   /**< NO SMS at that address */
            SAL_ERR_BUFFER_TOO_SMALL = -30,         /**< User data buffer too small for data > */
            SAL_ERR_DIAGNOSTIC = -31,				/**< Some Diagnostic Error */
            SAL_ERR_QUEUE_EMPTY = -32,              /**< No more msgs in the Error Queue */
            SAL_ERR_WRONG_MODE = -33,               /**< Sensor is in the wrong measurement mode */
            SAL_ERR_MEMORY = -34,                   /**< Could not allocate memory */
            SAL_ERR_INVALID_HANDLE = -35,           /**< User supplied handle was invalid */
            SAL_ERR_SENSOR_CONNECT = -36,           /**< Attempt to connect to sensor failed */
            SAL_ERR_SMS_NO_TOKEN = -37,             /**< SMS refused to issue token */
            SAL_ERR_COMMAND_FAILED = -38,           /**< Sensor command failed */
            SAL_ERR_NO_LOCATE_HISTORY = -39,        /**< Could not get locate result history */
            SAL_ERR_TIMEOUT = -40,                  /**< Measurement timed out */
            SAL_ERR_IMAGE_SIZE = -41,               /**< Requested location image size too big */
            SAL_ERR_INVALID_ANTENNA = -42,          /**< Requested antenna type not valid */
            SAL_ERR_STRING_TOO_LONG = -43,          /**< Input string too long */
            SAL_ERR_INVALID_TIMEOUT = -44,          /**< Requested timeout value not valid */
            SAL_ERR_INVALID_SENSOR_INDEX = -45,     /**< Sensor index not valid */
            SAL_ERR_INVALID_TRIGGER_TYPE = -46,     /**< Requested trigger type not valid */
            SAL_ERR_INVALID_DOPPLER_COMP = -47,     /**< Requested doppler compensation not valid */
            SAL_ERR_NUM_SENSORS = -48,              /**< Maximum number of sensors already added to group */
            SAL_ERR_EMPTY_GROUP = -49,              /**< Operation not valid on empty sensor group */
            SAL_ERR_HANDLE_IN_USE = -50,            /**< Handle can not be closed because it is in use */
            SAL_ERR_DATA_TYPE = -52,                /**< Requested salDataType not valid for measurement */
            SAL_ERR_SENSOR_SERVER = -53,            /**< Sensor measurement server communications error */
            SAL_ERR_TIME_NOT_IN_STREAM = -54,       /**< Request for time data that is not in sensor memory */
            SAL_ERR_FREQ_NOT_IN_STREAM = -55,       /**< Requested frequency is outside of current tuner range */
            SAL_ERR_NOT_IN_LOOKBACK = -56,          /**< Measurement requires sensor in lookback mode */
            SAL_ERR_AUTHORIZATION = -57,            /**< Error authorizing current application and user on the sensor */
            SAL_ERR_TUNER_LOCK = -58,               /**< Could not obtain a lock on tuner resource */
            SAL_ERR_FFT_LOCK = -59,                 /**< Could not obtain a lock on FFT resource */
            SAL_ERR_LOCK_FAILED = -60,              /**< Could not obtain a lock on requested resource */
            SAL_ERR_SENSOR_DATA_END = -61,          /**< RF Sensor data stream terminated unexpectedly */
            SAL_ERR_INVALID_SPAN = -62,             /**< Requested measurement span is not valid */
            SAL_ERR_INVALID_ALGORITHM = -63,        /**< Requested geolocation algorithm is not available */
            SAL_ERR_LICENSE = -64,                  /**< License error */
            SAL_ERR_LIST_END = -65,                 /**< End of list reached */
            SAL_ERR_MEAS_FAILED = -66,              /**< The measurement failed of timed out with no results */
            SAL_ERR_EMBEDDED = -67,                 /**< Function not supported in embedded apps. */
            SAL_ERR_SMS_EXCEPTION = -68,            /**< Exception in SMS processing */
            SAL_SDRAM_OVERFLOW = -69,               /**< SDRAM overflow in sensor */
            SAL_NO_DMA_BUFFER = -70,				/**< NO free DMA Buffers in sensor */
            SAL_DMA_FIFO_UNDERFLOW = -71,           /**< DMA FIFO Underflow in sensor */
            SAL_FFT_SETUP_ERROR = -72,              /**< FFT Setup Error */
            SAL_ERR_NUM = -72                       /** this should ALWAYS EQUAL the last valid error message */
        };

        public enum IqMode
        {
            Stream,
            Block,

            NumIqMode
        };

        public enum MaxBufferAction
        {
            Flush,                /**< Discard data in sensor FIFO and continue acquiring data */
            StopAcquistion        /**< Stop acquiring data, but keep sending data until FIFO is empty */
        };

        public enum DataProtocol
        {
            UDP = 0,
            TCP = 1,

            NumProtocol
        };

        public enum salDecimation
        {
            salDECIMATION_NONE,     /**< No decimation supported */
            salDECIMATION_BY_2,     /**< Decimation by two supported */
            salDECIMATION_BY_5,     /**< Decimation by five supported */
            salDECIMATION_VARIABLE, /**< Arbitrary decimation supported */
            salDECIMATION_UNKNOWN   /**< Decimation capability unknown */
        }

        public enum salIeee1588State // Caution: only valid when 1588 is in operation
        {
            PTP_POWERUP = 0,
            PTP_INITIALIZING = 1,
            PTP_FAULTY = 2,
            PTP_DISABLED = 3,
            PTP_LISTENING = 4,
            PTP_PRE_MASTER = 5,
            PTP_MASTER = 6,
            PTP_PASSIVE = 7,
            PTP_UNCALIBRATED = 8,
            PTP_SLAVE = 9
        };

        public enum SensorAttribute
        {
            COMPLEX_SAMPLE_RATE_MAX,     /**< returns salFloat64 (Hertz) */

            DECIMATION_MAX,              /**< returns salInt32 */
            DECIMATION_TYPE,             /**< returns salDecimation enum */

            FREQ_SPAN_FULL,             /**< returns valid analog freq span at full span salFloat64 (Hertz) */
            FREQ_SPAN_DECIMATING,       /**< returns (sample rate)/(valid freq span) when decimating */

            MEASURABLE_FREQ_MIN,        /**< returns salFloat64 Hertz */
            MEASURABLE_FREQ_MAX,        /**< returns salFloat64 Hertz */

            CENTER_FREQ_MIN,            /**< returns salFloat64 Hertz */
            CENTER_FREQ_MAX,            /**< returns salFloat64 Hertz */
            CENTER_FREQ_RESOLUTION,     /**< returns salFloat64 Hertz */

            RESAMPLER_CAPABILITY,       /**< returns salInt32; value is 1 if resampling is supported, 0 if not */

            ATTENUATION_MIN,            /**< returns salFloat64 */
            ATTENUATION_MAX,            /**< returns salFloat64 */
            ATTENUATION_STEP,           /**< returns salFloat64 */

            PREAMPLIFIER_CAPABILITY,    /**< returns salInt32 */

            IQ_CHANNELS_MAX,           /**< returns salInt32 */

            IQ_SAMPLES_MIN,            /**< returns salInt32 */
            IQ_SAMPLES_MAX_16BIT,      /**< returns salInt32 */
            IQ_SAMPLES_MAX_32BIT,      /**< returns salInt32 */

            SAMPLES_PER_XFER_MAX_TCP, /**< returns salInt32 */
            SAMPLES_PER_XFER_MAX_UDP, /**< returns salInt32 */

            MODEL_NUMBER,               /**< returns (char *) */
            SERIAL_NUMBER,              /**< returns (char *) */

            SENSOR_NAME,               /**< returns character array terminated by NULL; value is the SMS name of the sensor */
            SENSOR_HOSTNAME,            /**< returns character array terminated by NULL; value is the hostname of the sensor */
            DATE,                     /** < returns character array terminated by NULL; value is sensor time and date (e.g. Wed Jul 15 16:14:20 UTC 2009) */
            FFT_POINTS_MIN,           /**< returns salInt32; returns the minimum size of FFT that can be requested from the sensor */
            FFT_POINTS_MAX,           /**< returns salInt32; returns the maximum size of FFT that can be requested from the sensor */
            ATTRIBUTE_DMA_HW,      /**< returns salInt32; if non-zero, this sensor has DMA hardware, which allows higher data transfer rates */

            ATTRIBUTE_LO_ADJ_MODE,         /** [in|out] sets/gets current sensor LO adjustment mode */
            ATTRIBUTE_TIME_SYNC_MODE,      /** [out]  returns salInt32; value is one of the ::salTimeSync enumerated values indicating the current time sysc source */
            ATTRIBUTE_TUNER_FIFO_BYTES,    /** [out] get the size in bytes of the sensor's high speed FIFO (uint64) */
            ATTRIBUTE_DMA_BUFFER_BYTES,    /** [out] get the size in bytes of the sensor's DMA buffer (uint64) */

            ATTRIBUTE_IEEE1588_STATE,      /**< [out] gets current IEEE1588 state (int32, see salIeee1588State) */
            ATTRIBUTE_IEEE1588_DOMAIN,     /**< [in|out] sets/gets current IEEE1588 domain (uint32) */
            ATTRIBUTE_IEEE1588_PRIORITY1,  /**< [in|out] sets/gets current IEEE1588 priority1 (uint32) */
            ATTRIBUTE_IEEE1588_PRIORITY2,  /**< [in|out] sets/gets current IEEE1588 priority2 (uint32) */

            ATTRIBUTE_SENSOR_VARIANCE,     /**< [out] gets current "overall" sensor time-sync variance (double, sec^2) */
            ATTRIBUTE_SENSOR_OFFSET,       /**< [out] gets current "overall" sensor time-sync "offset from master" (double, sec) */
            ATTRIBUTE_IEEE1588_VARIANCE,   /**< [out] gets current sensor IEEE1588 variance (double, sec^2) */
            ATTRIBUTE_IEEE1588_OFFSET,     /**< [out] gets current sensor IEEE1588 "offset from master" (double, sec) */
            ATTRIBUTE_GPS_VARIANCE,        /**< [out] gets current sensor GPS variance (double, sec^2) */
            ATTRIBUTE_GPS_OFFSET,          /**< [out] gets current sensor GPS "offset from GPS module" (double, sec) */
            ATTRIBUTE_FPGA_VARIANCE,       /**< [out] gets current sensor FPGA variance (double, sec^2) */
            ATTRIBUTE_FPGA_OFFSET,         /**< [out] gets current sensor FPGA "offset from PHY or GPS module" (double, sec) */

            ATTRIBUTE_CPU_1MIN,            /**< [out] gets 1 minute CPU load (float) */
            ATTRIBUTE_CPU_5MIN,            /**< [out] gets 5 minute CPU load (float) */
            ATTRIBUTE_CPU_10MIN,           /**< [out] gets 10 minute CPU load (float) */

            ATTRIBUTE_VARIANCE_ALARM_THRESHOLD,  /**< [in|out] sets/gets current variance alarm threshold (double, sec^2) */
            ATTRIBUTE_OFFSET_ALARM_THRESHOLD,    /**< [in|out] sets/gets current time offset alarm threshold (double, sec) */
            ATTRIBUTE_CPU_ALARM_THRESHOLD,       /**< [in|out] sets/gets CPU alarm threshold (float, 0 to 1) */

            ATTRIBUTE_RF_TEMPERATURE,			/**< [out] RF Board temperature (double, deg C) */
            ATTRIBUTE_DIG_TEMPERATURE,			/**< [out] Digital Board temperature (double, deg C) */

            ATTRIBUTE_UP_TIME,					/**< [out] Up-time (float, sec) */
            ATTRIBUTE_IDLE_TIME,				/**< [out] Idle-time (float, sec) */

            ATTRIBUTE_TIME_ALARMS,				/**< [out] Time alarm bitfield (unit32, see salTimeAlarms) */
            ATTRIBUTE_SA_ALARMS,				/**< [out] Spectrum Analyzer alarm bitfield (unit32, see salSpectrumAnalyzerAlarms) */
            ATTRIBUTE_SYS_ALARMS,				/**< [out] System alarm bitfield (unit32, see salSystemAlarms) */
            ATTRIBUTE_INTEG_ALARMS,				/**< [out] Integrity alarm bitfield (unit32, see salIntegrityAlarms) */

            NUM_ATTRIBUTES
        };

        public enum SensorAttributeType
        {
            SensorAttribute_double,
            SensorAttribute_float,
            SensorAttribute_int32,
            SensorAttribute_uint32,
            SensorAttribute_uint64,
            SensorAttribute_String
        };

        // Caution: must be same size as SensorAttribute
        static public SensorAttributeType[] SensorAttributeTypes = new SensorAttributeType[] // true if Double, false if Int32
        {
            SensorAttributeType.SensorAttribute_double, // COMPLEX_SAMPLE_RATE_MAX,     /**< returns salFloat64 (Hertz) */

            SensorAttributeType.SensorAttribute_int32, // DECIMATION_MAX,              /**< returns salInt32 */
            SensorAttributeType.SensorAttribute_int32, // DECIMATION_TYPE,             /**< returns salDecimation enum */

            SensorAttributeType.SensorAttribute_double, // FREQ_SPAN_FULL,             /**< returns valid analog freq span at full span salFloat64 (Hertz) */
            SensorAttributeType.SensorAttribute_double, // FREQ_SPAN_DECIMATING,       /**< returns (sample rate)/(valid freq span) when decimating */

            SensorAttributeType.SensorAttribute_double, // MEASURABLE_FREQ_MIN,        /**< returns salFloat64 Hertz */
            SensorAttributeType.SensorAttribute_double, // MEASURABLE_FREQ_MAX,        /**< returns salFloat64 Hertz */

            SensorAttributeType.SensorAttribute_double, // CENTER_FREQ_MIN,            /**< returns salFloat64 Hertz */
            SensorAttributeType.SensorAttribute_double, // CENTER_FREQ_MAX,            /**< returns salFloat64 Hertz */
            SensorAttributeType.SensorAttribute_double, // CENTER_FREQ_RESOLUTION,     /**< returns salFloat64 Hertz */

            SensorAttributeType.SensorAttribute_int32, // RESAMPLER_CAPABILITY,       /**< returns salInt32; value is 1 if resampling is supported, 0 if not */

            SensorAttributeType.SensorAttribute_double, // ATTENUATION_MIN,            /**< returns salFloat64 */
            SensorAttributeType.SensorAttribute_double, // ATTENUATION_MAX,            /**< returns salFloat64 */
            SensorAttributeType.SensorAttribute_double, // ATTENUATION_STEP,           /**< returns salFloat64 */

            SensorAttributeType.SensorAttribute_int32, // PREAMPLIFIER_CAPABILITY,    /**< returns salInt32 */

            SensorAttributeType.SensorAttribute_int32, // IQ_CHANNELS_MAX,           /**< returns salInt32 */

            SensorAttributeType.SensorAttribute_int32, // IQ_SAMPLES_MIN,            /**< returns salInt32 */
            SensorAttributeType.SensorAttribute_int32, // IQ_SAMPLES_MAX_16BIT,      /**< returns salInt32 */
            SensorAttributeType.SensorAttribute_int32, // IQ_SAMPLES_MAX_32BIT,      /**< returns salInt32 */

            SensorAttributeType.SensorAttribute_int32, // SAMPLES_PER_XFER_MAX_TCP, /**< returns salInt32 */
            SensorAttributeType.SensorAttribute_int32, // SAMPLES_PER_XFER_MAX_UDP, /**< returns salInt32 */

            SensorAttributeType.SensorAttribute_String, // MODEL_NUMBER,               /**< returns (char *) */
            SensorAttributeType.SensorAttribute_String, // SERIAL_NUMBER,              /**< returns (char *) */

            SensorAttributeType.SensorAttribute_String, // SENSOR_NAME,               /**< returns character array terminated by NULL; value is the SMS name of the sensor */
            SensorAttributeType.SensorAttribute_String, // SENSOR_HOSTNAME,            /**< returns character array terminated by NULL; value is the hostname of the sensor */
            SensorAttributeType.SensorAttribute_String, // DATE,                     /** < returns character array terminated by NULL; value is sensor time and date (e.g. Wed Jul 15 16:14:20 UTC 2009) */
            SensorAttributeType.SensorAttribute_int32, // FFT_POINTS_MIN,           /**< returns salInt32; returns the minimum size of FFT that can be requested from the sensor */
            SensorAttributeType.SensorAttribute_int32, // FFT_POINTS_MAX,           /**< returns salInt32; returns the maximum size of FFT that can be requested from the sensor */
            SensorAttributeType.SensorAttribute_int32, // ATTRIBUTE_DMA_HW,      /**< returns salInt32; if non-zero, this sensor has DMA hardware, which allows higher data transfer rates */

            SensorAttributeType.SensorAttribute_int32, // ATTRIBUTE_LO_ADJ_MODE,         /** [in|out] sets/gets current sensor LO adjustment mode */
            SensorAttributeType.SensorAttribute_int32, // ATTRIBUTE_TIME_SYNC_MODE,      /** [out]  returns salInt32; value is one of the ::salTimeSync enumerated values indicating the current time sysc source */
            SensorAttributeType.SensorAttribute_uint64, // ATTRIBUTE_TUNER_FIFO_BYTES,    /** [out] get the size in bytes of the sensor's high speed FIFO (uint64) */
            SensorAttributeType.SensorAttribute_uint64, // ATTRIBUTE_DMA_BUFFER_BYTES,    /** [out] get the size in bytes of the sensor's DMA buffer (uint64) */
            SensorAttributeType.SensorAttribute_int32, // ATTRIBUTE_IEEE1588_STATE,       /**< [out] gets current IEEE1588 state (int32, see salIeee1588State) */
            SensorAttributeType.SensorAttribute_uint32, // ATTRIBUTE_IEEE1588_DOMAIN,     /**< [in|out] sets/gets current IEEE1588 domain (uint32) */
            SensorAttributeType.SensorAttribute_uint32, // ATTRIBUTE_IEEE1588_PRIORITY1,  /**< [in|out] sets/gets current IEEE1588 priority1 (uint32) */
            SensorAttributeType.SensorAttribute_uint32, // ATTRIBUTE_IEEE1588_PRIORITY2,  /**< [in|out] sets/gets current IEEE1588 priority2 (uint32) */

            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_SENSOR_VARIANCE,     /**< [out] gets current "overall" sensor time-sync variance (double, sec^2) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_SENSOR_OFFSET,       /**< [out] gets current "overall" sensor time-sync "offset from master" (double, sec) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_IEEE1588_VARIANCE,   /**< [out] gets current sensor IEEE1588 variance (double, sec^2) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_IEEE1588_OFFSET,     /**< [out] gets current sensor IEEE1588 "offset from master" (double, sec) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_GPS_VARIANCE,        /**< [out] gets current sensor GPS variance (double, sec^2) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_GPS_OFFSET,          /**< [out] gets current sensor GPS "offset from GPS module" (double, sec) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_FPGA_VARIANCE,       /**< [out] gets current sensor FPGA variance (double, sec^2) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_FPGA_OFFSET,         /**< [out] gets current sensor FPGA "offset from PHY or GPS module" (double, sec) */

            SensorAttributeType.SensorAttribute_float, // ATTRIBUTE_CPU_1MIN,            /**< [out] gets 1 minute CPU load (float) */
            SensorAttributeType.SensorAttribute_float, // ATTRIBUTE_CPU_5MIN,            /**< [out] gets 5 minute CPU load (float) */
            SensorAttributeType.SensorAttribute_float, // ATTRIBUTE_CPU_10MIN,           /**< [out] gets 10 minute CPU load (float) */

            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_VARIANCE_ALARM_THRESHOLD,  /**< [in|out] sets/gets current variance alarm threshold (double, sec^2) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_OFFSET_ALARM_THRESHOLD,    /**< [in|out] sets/gets current time offset alarm threshold (double, sec) */
            SensorAttributeType.SensorAttribute_float,  // ATTRIBUTE_CPU_ALARM_THRESHOLD,       /**< [in|out] sets/gets CPU alarm threshold (float, 0 to 1) */

            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_RF_TEMPERATURE,			/**< [out] RF Board temperature (double, deg C) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_DIG_TEMPERATURE,			/**< [out] Digital Board temperature (double, deg C) */

            SensorAttributeType.SensorAttribute_float, // ATTRIBUTE_UP_TIME,			/**< [out] Up-time (float, sec) */
            SensorAttributeType.SensorAttribute_float, // ATTRIBUTE_IDLE_TIME,			/**< [out] Idle-time (float, sec) */

            SensorAttributeType.SensorAttribute_uint32, // ATTRIBUTE_TIME_ALARMS,				/**< [out] Time alarm bitfield (unit32, see salTimeAlarms) */
            SensorAttributeType.SensorAttribute_uint32, // ATTRIBUTE_SA_ALARMS,				/**< [out] Spectrum Analyzer alarm bitfield (unit32, see salSpectrumAnalyzerAlarms) */
            SensorAttributeType.SensorAttribute_uint32, // ATTRIBUTE_SYS_ALARMS,				/**< [out] System alarm bitfield (unit32, see salSystemAlarms) */
            SensorAttributeType.SensorAttribute_uint32 // ATTRIBUTE_INTEG_ALARMS,				/**< [out] Integrity alarm bitfield (unit32, see salIntegrityAlarms) */

        };

        public enum IqAttribute
        {
            DELAY_MAX_SECONDS,      /**< returns salFloat64; value is maximum amount of data (in seconds) that will be buffered in the sensor before the data is discarded */
            DELAY_SECONDS,
            LAST_SEQUENCE_NUMBER,    /**< returns shInt32 */
            STATE,                   /**< returns shIqStates */
            USER_STREAM_ID,           /**< returns shInt32 */

            XFER_SAMPLES_MAX,    /**< returns salInt32 */
            XFER_BYTES_MAX,      /**< returns salInt32 */

            NUM_SAMPLES,             /**< returns salInt32  */
            SAMPLE_RATE,             /**< returns salFloat64 */
            CENTER_FREQUENCY,        /**< returns salFloat64 */
            ATTENUATION,             /**< returns salFloat64 */
            PREAMP,                  /**< returns salInt32  */
            ANTENNA,                 /**< returns salInt32 */
            IQ_MODE,                 /**< returns salInt32  */
            DATA_TYPE,               /**< returns salInt32  */

            NUM_ATTRIBUTES
        };

        public enum IqState
        {
            None,
            Init,		            /**< initial state (idle)      */
            Stopped,	            /**< stopped (idle)            */
            Streaming,		    /**< streaming, real time      */
            StreamingTransfer, /**< acquistion stopped, but there is still data in sensor memory */
            WaitingForTrigger,  /**< waiting for the trigger   */
            BlockAcquisition,    /**< acquiring a block of data */
            BlockTransfer,        /**< transfering a block       */

            NumIqState
        };



        public enum IqCommand
        {
            FlushBuffer,
            StopAcquisition
        };

        public enum Localization
        {
            English
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct salFlowControl
        {
            public Int32 pacingPolicy;        // Pacing policy, 0 == disabled (no pacing), 1 == wait when full policy, 2 == flush when full policy
            public float maxBacklogSeconds;   // Max backlogSeconds, 0 == disabled
            public float maxBytesPerSec;      // TX data rate threshold, 0 == disabled
            public Int32 maxBacklogBytes;     // Max bytes threshold, 0 == disabled
            public Int32 maxBacklogMessages;  // Max messages threshold, 0 == disabled
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct salBacklogStatus
        {
            public salFlowControl flowControlStatus;
            public UInt64 backlogBytes;      // Number of bytes waiting to be transferred
            public UInt64 discardBytes;      // Number of bytes discarded because of backlog condition
            public UInt32 backlogMessages;   // Number of messages waiting to be transferred
            public UInt64 discardMessages;   // Number of messages discarded because of backlog condition
            public float rxBytesPerSec;      // incoming data rate from the measurement HW
            public float txBytesPerSec;      // TX data rate leaving the sensor
            public float backlogSeconds;	  // Backlog in seconds
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct Gps
        {
            public UInt32 version;                    // GPS descriptor version 
            public UInt32 reserved1;                  // Reserved for future use
            public UInt32 reserved2;                  // Reserved for future use
            public UInt32 valid;                      // bitmap indicates which GPS values are valid
            public UInt32 changeIndicator;            // bitmap indicates which GPS values have changed

            public double latitude;                   // Sensor latitude in degrees; -90 (South) to +90 (North)
            public double longitude;                  // Sensor longitude in degrees; -180 (West) to +180 (East)
            public double altitude;                   // Sensor altitude in meters 
            public double speed;                      // Sensor speed in meters/second over ground
            public double heading;                    // Sensor orientation with respect to true North in decimal degrees 
            public double trackAngle;                 // Sensor direction of travel with respect to true North in decimal degrees
            public double magneticVariation;          // Magnetic North variation from true North in decimal degrees; -180 (West) to +180 (East)

            public const int GPS_INDICATOR_LATITUDE = 0x01;
            public const int GPS_INDICATOR_LONGITUDE = 0x02;
            public const int GPS_INDICATOR_ALTITUDE = 0x04;
            public const int GPS_INDICATOR_SPEED = 0x08;
            public const int GPS_INDICATOR_HEADING = 0x10;
            public const int GPS_INDICATOR_TRACK_ANGLE = 0x20;
            public const int GPS_INDICATOR_MAGNETIC_VARIATION = 0x40;

            public bool IsLatitudeValid
            {
                get { return (valid & GPS_INDICATOR_LATITUDE) != 0; }
            }
            public bool IsLongitudeValid
            {
                get { return (valid & GPS_INDICATOR_LONGITUDE) != 0; }
            }
            public bool IsAltitudeValid
            {
                get { return (valid & GPS_INDICATOR_ALTITUDE) != 0; }
            }
            public bool IsSpeedValid
            {
                get { return (valid & GPS_INDICATOR_SPEED) != 0; }
            }
            public bool IsHeadingValid
            {
                get { return (valid & GPS_INDICATOR_HEADING) != 0; }
            }
            public bool IsTrackAngleValid
            {
                get { return (valid & GPS_INDICATOR_TRACK_ANGLE) != 0; }
            }
            public bool IsMagneticVariationValid
            {
                get { return (valid & GPS_INDICATOR_MAGNETIC_VARIATION) != 0; }
            }

            public bool LatitudeChanged
            {
                get { return (changeIndicator & GPS_INDICATOR_LATITUDE) != 0; }
            }
            public bool LongitudeChanged
            {
                get { return (changeIndicator & GPS_INDICATOR_LONGITUDE) != 0; }
            }
            public bool AltitudeChanged
            {
                get { return (changeIndicator & GPS_INDICATOR_ALTITUDE) != 0; }
            }
            public bool SpeedChanged
            {
                get { return (changeIndicator & GPS_INDICATOR_SPEED) != 0; }
            }
            public bool HeadingChanged
            {
                get { return (changeIndicator & GPS_INDICATOR_HEADING) != 0; }
            }
            public bool TrackAngleChanged
            {
                get { return (changeIndicator & GPS_INDICATOR_TRACK_ANGLE) != 0; }
            }
            public bool MagneticVariationChanged
            {
                get { return (changeIndicator & GPS_INDICATOR_MAGNETIC_VARIATION) != 0; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct IqDataHeader
        {
            public UInt32 version;
            public UInt32 sequenceNumber;
            public UInt32 numSamples;
            public DataType dataType;

            public IntPtr streamIdentifier;
            public IntPtr userWorkspace;
            public UInt32 userStreamId;

            public UInt32 timestampSec;
            public UInt32 timestampNsec;

            public UInt32 stateEventIndicator;

            public UInt32 rfStatus;
            public UInt32 changeIndicator;

            public AntennaType antenna;
            public double bandwidth;
            public double centerFrequency;
            public double scaleToVolts;
            public double dataFullScale;
            public double attenuation;
            public double sampleRate;
            public double temperature;

            public Gps gps;

            public IntPtr pSamples;

            // IQ stateEventIndicator bit values
            public const int salSTATE_SAMPLE_LOSS = 0x01;
            public const int salSTATE_OVER_RANGE = 0x02;
            public const int salSTATE_BLOCK_MEASUREMENT_ERROR = 0x04;
            public const int salSTATE_LAST_BLOCK = 0x10;
            public const int salSTATE_REF_OSC_ADJUSTED = 0x20;
            public const int salSTATE_CPU_OVERLOAD = 0x100;
            public const int salSTATE_SYNC_PROBLEM = 0x200;

            // changeIndicator bit values
            public const int salCHANGE_BANDWIDTH = 0x01;
            public const int salCHANGE_RF_REF_FREQ = 0x02;
            public const int salCHANGE_SCALE_TO_VOLTS = 0x04;
            public const int salCHANGE_DATA_FULL_SCALE = 0x08;
            public const int salCHANGE_ATTENUATION = 0x10;
            public const int salCHANGE_SAMPLE_RATE = 0x20;
            public const int salCHANGE_TEMPERATURE = 0x40;
            public const int salCHANGE_ANTENNA = 0x80;


            public DateTime TimeUtc
            {
                get { return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timestampSec); }
            }

            public DateTime TimeLocal
            {
                get { return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timestampSec).ToLocalTime(); }
            }
            public double TimeFractionalSeconds
            {
                get { return (double)timestampNsec / 1e9; }
            }

            public bool IsLastBlock
            {
                get { return (stateEventIndicator & salSTATE_LAST_BLOCK) != 0; }
            }
            public bool IsOverRange
            {
                get { return (stateEventIndicator & IqDataHeader.salSTATE_OVER_RANGE) != 0; }
            }
            public bool IsMeasurementError
            {
                get { return (stateEventIndicator & salSTATE_BLOCK_MEASUREMENT_ERROR) != 0; }
            }
            public bool IsSampleLoss
            {
                get { return (stateEventIndicator & salSTATE_SAMPLE_LOSS) != 0; }
            }
            public bool CenterFrequencyChanged
            {
                get { return (changeIndicator & salCHANGE_RF_REF_FREQ) != 0; }
            }
            public bool BandwidthChanged
            {
                get { return (changeIndicator & salCHANGE_BANDWIDTH) != 0; }
            }
            public bool ScaleToVoltsChanged
            {
                get { return (changeIndicator & salCHANGE_SCALE_TO_VOLTS) != 0; }
            }
            public bool DataFullScaleChanged
            {
                get { return (changeIndicator & salCHANGE_DATA_FULL_SCALE) != 0; }
            }
            public bool AttenuationChanged
            {
                get { return (changeIndicator & salCHANGE_ATTENUATION) != 0; }
            }
            public bool AntennaChanged
            {
                get { return (changeIndicator & salCHANGE_ANTENNA) != 0; }
            }
            public bool SampleRateChanged
            {
                get { return (changeIndicator & salCHANGE_SAMPLE_RATE) != 0; }
            }
            public bool TemperatureChanged
            {
                get { return (changeIndicator & salCHANGE_TEMPERATURE) != 0; }
            }

            public UInt32 BytesPerSample
            {
                get
                {
                    switch (dataType)
                    {
                        case DataType.Complex16: return 4;
                        case DataType.Complex32: return 8;
                        default: return 0;
                    }
                }
            }

        } ;


        [StructLayout(LayoutKind.Sequential)]
        public struct IqArg
        {
            public UInt32 reserved1;
            public UInt32 reserved2;
            public UInt32 reserved3;
            public UInt32 id;  // user-defined ID
        } ;

        [StructLayout(LayoutKind.Sequential)]
        public struct SensorCapabilities
        {
            public Int32 supportsFrequencyData;         /**< if non-zero, sensor supports the frequency data interface */
            public Int32 supportsTimeData;              /**< if non-zero, sensor supports the time data interface */
            public Int32 fftMinBlocksize;       /**< the minimum FFT blocksize for this sensor */
            public Int32 fftMaxBlocksize;       /**< the maximum FFT blocksize for this sensor */
            public Int32 maxDecimations;        /**< maximum number of sample rate decimations */
            public Int32 hasDmaHardware;         /**< if non-zero, this sensor has DMA hardware, which allows higher data transfer rates */
            public UInt64 rfFifoBytes;
            public UInt64 dmaBufferBytes;
            private Int32 reserved1;              /**< reserved for future use */
            private Int32 reserved2;              /**< reserved for future use */
            private Int32 reserved3;              /**< reserved for future use */
            public double maxSampleRate;        /**< the maximum FFT blocksize for this sensor */
            public double maxSpan;              /**< the maximum valid measurement span for this sensor */
            public double sampleRateToSpanRatio; /**< the ratio of sample rate to valid frequency span */
            public double minFrequency;         /**< the minumum measurable frequency */
            public double maxFrequency;         /**< the maximum measurable frequency */
            public double fReserved1;           /**< reserved for future use */
            private double fReserved2;           /**< reserved for future use */
            private double fReserved3;           /**< reserved for future use */
            private double fReserved4;           /**< reserved for future use */
            private double fReserved5;           /**< reserved for future use */
            private double fReserved6;           /**< reserved for future use */
            private double fReserved7;           /**< reserved for future use */

        };

        [StructLayout(LayoutKind.Sequential)]
        public struct IqParameters
        {
            public UInt32 numSamples;           /**<	Number of points (in)*/
            public double sampleRate;           /**<	Sample rate in Hz (in)*/
            public double centerFrequency;      /**<	Center frequency in Hz (in)*/
            public double attenuation;         /**<	IF attenuation in dB (in)*/
            public UInt32 preamp;              /**<    pre-amp */
            public AntennaType antenna;             /**<    which antenna */
            public IqMode iqMode;              /**<    IQ Streaming or IQ Block Mode */
            public DataType dataType;          /**<    complex or real */
            public double maxBufferSeconds;    /**<    Size (in bytes) of the Node RAM buffer for IQ block mode */
            public MaxBufferAction maxBufferAction; /**<    For real-time streaming, what to do if maxBufferSeconds is exceeded */
        } ;


        [StructLayout(LayoutKind.Sequential)]
        public struct SensorInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string macAddress;          /**< Media access control address as a string (for example "123456789abc"). */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string modelNumber;         /**< Model number of sensor (for example "N6841A").*/
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string serialNumber;        /**< Serial number of sensor (for example "A-N6841A-50001").*/
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string hostName;            /**< Hostname of the  sensor.*/
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string ipAddress;           /**< Internet protocol address of the sensor (for example "192.168.1.101").*/
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string smsAddress;          /**< Host name or IP address of last SMS that this sensor was assigned to.*/
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string revision;            /**< Firmware and FPGA revision information.*/
        } ;







        #endregion



        #region Frequency data functions

        /**********************************************************/
        /*  Frequency data functions                              */
        /**********************************************************/

        public const UInt32 FREQ_DATA_MIN_VERSION = 0x10002;

        public const int FFT_POINTS_MIN = 8;
        public const int FFT_POINTS_MAX = 16384;

        public enum WindowType
        {
            Window_hann,         /**< Hann window (sometimes called the Hanning window)*/
            Window_gaussTop,     /**< Gausstop window  */
            Window_flatTop,      /**< Flattop window  */
            Window_uniform       /**< Uniform window */
        }

        public enum AverageType
        {
            Average_off,         /**< No averaging */
            Average_rms,         /**< RMS averaging */
            Average_peak,        /**< Peak-hold averaging */
        }

        public enum FftDataType
        {
            FftData_db,         /**< dBm data from sensor, 2 bytes/bin */
            FftData_mag         /**< v^2 data from sensor, 4 bytes/bin */
        }

        public enum AmplitudeType
        {
            salAmplitudeType_float32_dBm /**< data is returned as 32 bit floats in units of dBm (assuming 50 ohm load) */
        }
        public enum SweepCommand
        {
            SweepCommand_stop,         /**< Stop a sweep when the sweep is finished */
            SweepCommand_abort,        /**< Stop a sweep as soon as possible*/
            SweepCommand_flush         /**< Flush the sweep backlog */
        }

        public enum SweepStatus
        {
            SweepStatus_stopped,               /**< Sweep is waiting to start  */
            SweepStatus_running,            /**< Sweep is running */
        }

        public enum MonitorMode
        {
            MonitorMode_off,            /**< Do not use monitor mode */
            MonitorMode_on,             /**< If there is an FFT measurement running on the sensor,
                                              send data in "eavesdrop mode" */
        }

        public enum OverlapType
        {
            OverlapType_on,
            OverlapType_off,
        }

        // salSweepReturnDataControl bit values
        // These values control what FFT data is returned to the user
        public const uint salSWEEP_DATA_SAMPLE_LOSS = 0x01;    /**< Indicates that this data block is not contiguous with previous block */
        public const uint salSWEEP_DATA_OVER_RANGE = 0x02;    /**< RF Sensor input overload */
        public const uint salSWEEP_DATA_BLOCK_MEASUREMENT_ERROR = 0x04;    /**< Measurement hardware error */
        public const uint salSWEEP_DATA_SETUP_NOT_USER = 0x08;    /**< The measurement setup is different than requested */
        public const uint salSWEEP_DATA_LAST_BLOCK = 0x10;    /**< This is the last block of data for the current measurement */
        public const uint salSWEEP_DATA_OSCILLATOR_ADJUSTED = 0x20;    /**< If set; the sensor clock reference oscillator was adjusted during the measurement  */
        public const uint salSWEEP_DATA_SEGMENT_TIMEOUT = 0x40;    /**< If set; synchronized FFT segment was not completed in scheduled time */
        public const uint salSWEEP_DATA_CPU_OVERLOAD = 0x80;    /**< If set, the sensor's CPU is compute bound */
        public const uint salSWEEP_DATA_SYNC_PROBLEM = 0x100;   /**< If set, the sensor's synchronization is suspect */
        public const uint salSWEEP_IGNORE_SYNC = 0x1000;  /**< If set, this segment will begin immediately (rather than being time-triggered) */
        public const uint salSWEEP_DATA_SEGMENT_NO_ARRAY = 0x4000;  /**< If set; the data array will not be transferred (i.e. header info only) */
        public const uint salSWEEP_DATA_SEGMENT_SILENT = 0x8000;  /**< If set; silent mode (i.e. no transfer) */

        // FFT SWEEP_FLAGS bit values
        // These values represent the valid bits int the FFT header sweepFlags field.
        public const int salSWEEP_MEAS_ERROR = 0x0001; /**< Measurement hardware error */
        public const int salSWEEP_SETUP_NOT_USER = 0x0002;  /**< setup changed by differnt measurement operation */
        public const int salSWEEP_SEGMENT_TOO_LATE = 0x0004;  /**< FFT segment too late */
        public const int salSWEEP_END_OF_DATA = 0x0008;  /**< This is the last block of data for the current measurement; measurement may have terminated early */
        public const int salSWEEP_MONITOR_MODE = 0x0010;  /**< Monitor mode FFT */
        public const int salSWEEP_REF_OSC_ADJUSTED = 0x0020;  /**< If set; the sensor clock reference oscillator was adjusted during the measurement  */
        public const int salSWEEP_OVERLOAD_DETECTED = 0x0040;  /**< Overload detected */
        public const int salSWEEP_FREQ_OUT_OF_BOUNDS = 0x0080; /**< Center frequency out of bounds, value clamped to valid range */
        public const int salSWEEP_CONNECTION_ERROR = 0x1000;  /**< Connection problem to sensor */
        public const int salSWEEP_LAST_SEGMENT = 0x4000;  /**< This is the last block of data for the current measurement */
        public const int salSWEEP_STOPPING = 0x8000;  /**< FFT sweep is stopping */
        public const int salSWEEP_MISSING_DATA = 0x10000; /**< Gap in FFT data */
        public const int salSWEEP_CPU_OVERLOAD = 0x20000; /**< If set; the sensor's CPU is compute bound */
        public const int salSWEEP_SYNC_PROBLEM = 0x40000;  /**< If set; the sensor's synchronization is suspect */

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct FrequencySegment
        {
            public AntennaType antenna;          /**< Antenna input for this segment */
            public Int32 preamp;           /**< Preamp input state (0=off; otherwise, on) */
            public UInt32 numFftPoints;     /**< FFT points; must be power of 2 between ::SAL_FFT_POINTS_MIN and ::SAL_FFT_POINTS_MAX */
            public AverageType averageType;      /**< Average type for this segment */
            public UInt32 numAverages;      /**< Number of averages for this segment */

            public UInt32 firstPoint;       /**< Index of first point to return; must be less than numFftPoints */
            public UInt32 numPoints;        /**< Number of points to return; must be less than or equal to numFftPoints */
            public UInt32 repeatAverage;    /**< If true, repeat the measurement until duration has elapsed */
            public double attenuation;      /**< Input attenuation in dB for this segment */
            public double centerFrequency;  /**< Center frequency of RF data */
            public double sampleRate;       /**< Sample rate of RF data */
            public double duration;         /**< Time interval (sec) between the start of this segment and the start of the next segment */
            public double mixerLevel;       /**< Mixer level in dB; range is -10 to 10 dB, 0 dB gives best compromise between SNR and distortion. */
            public OverlapType overlapType; /**< 0 means use overlap averaging; 1 means do not overlap */
            public FftDataType dataType;    /**< FFT Data type for this segment */
            public Int32 noTunerChange;     /**< Set this to non-zero value if you do not want to modify tuner for this segment. */
            public UInt32 noDataTransfer;   /**< Set this to non-zero value to control return data for this segment. See salSweepReturnDataControl. */
            public double reserved3;

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct SweepParms
        {
            public UInt32 numSweeps;        /**< Number of sweeps to perform; 0 means sweep until a stop command is sent */
            public UInt32 numSegments;      /**< Number of segments in the sweep */
            public WindowType window;       /**< Window applied to time record before performing FFT  */
            public IntPtr userWorkspace;    /**< User-defined value that will be returned with each data message */
            public DataType dataType;       /**< Data type for returned power spectrum; */
            public Int32 reserved1;         /**< reserved */
            public Int32 syncSweepEnable;   /**< Set to non-zero when performing synchronous sweeps. */
            public double sweepInterval;    /**< Interval between end of last sweep and start of next one, in seconds. */
            public UInt32 syncSweepSec;     /**< "sec" start time for first segment (synchrounous sweep only). */
            public UInt32 syncSweepNSec;    /**< "nsec" start time for first segment (synchrounous sweep only). */
            public MonitorMode monitorMode; /**< Enable/disable monitor mode */
            public double monitorInterval;  /**< When monitorMode is salMonitorMode_on, send results back at this interval */
            public IntPtr reserved;         /**< Used internally */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct SegmentData
        {
            public IntPtr userWorkspace;    /**< User-defined value set in :: salSweepParms */
            public UInt32 segmentIndex;     /**< 0-based index of this segment in the segmentTable  */
            public UInt32 sequenceNumber;   /**< starts at 0; incremented by 1 for each frequency result  */
            public UInt32 sweepIndex;       /**< starts at 0; incremented by 1 at the end of a sweep */

            public UInt32 timestampSec;   /**< Integer seconds part of timestamp of first time point in this segment */
            public UInt32 timestampNSec;  /**< Fractional seconds part of timestamp of first time point in this segment */
            public UInt32 timeQuality;    /**< Measure of time quality of timestamp */
            public Location location;     /**< Sensor location when this segment was measured */

            public double startFrequency;  /**< Frequency of first point returnded by this measurement */
            public double frequencyStep;   /**< Frequency spacing in Hertz of frequency data */
            public UInt32 numPoints;       /**< Number of frequency points returned by this measurement */
            public UInt32 overload;        /**< If not 0, the sensor input overloaded during this segment */
            public AmplitudeType dataType; /**< Data type of returned amplitude data */
            public UInt32 lastSegment;     /**< If not zero, this is the last segment before measurement stops */

            public WindowType window;         /**< Window used for this measurement */
            public AverageType averageType;   /**< Average type used in this measurement */
            public UInt32 numAverages;        /**< Number of averages used in this measurement */
            public double fftDuration;        /**< Duration of one FFT result */


            public double averageDuration;    /**< Duration of this complete measurement (all numAverages)  */
            public UInt32 isMonitor;

            public SalError errorNum;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ERROR_STRING)]
            public string errorInfo;
            public UInt32 sweepFlags;      /**< Mask of indicators for various conditions (see ::salSWEEP_FLAGS). */
            public UInt32 timeAlarms;
            public double sampleRate;
            public IntPtr measHandle;         /**< Handle to the running sweep (see salStartSweep()) */
            public IntPtr sensorHandle;       /**< Connection handle to the sensor */
            public IntPtr reserved;


            public bool IsMonitor() { return isMonitor != 0; }
            public bool IsMissingData
            {
                //returns true if single-segment sweep has gaps
                get { return (sweepFlags & salSWEEP_MISSING_DATA) != 0; }
            }
            public bool IsSegmentTimeout
            {
                get { return (sweepFlags & salSWEEP_SEGMENT_TOO_LATE) != 0; }
            }
            public bool IsTimeQuestionable
            {
                get { return (timeAlarms & (UInt32)TimeSyncAlarmBits.TimeQuestionable) != 0; }
            }
            public bool IsClockNotSet
            {
                get { return (timeAlarms & (UInt32)TimeSyncAlarmBits.ClockSet) != 0; }
            }

            public bool IsLastBlock
            {
                get { return (sweepFlags & (UInt32)salSWEEP_LAST_SEGMENT) != 0; }
            }
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct SweepComputationParms
        {
            public double startFrequency;   /**< Start frequency for the sweep (Hz) */
            public double stopFrequency;    /**< Stop frequency for the sweep (Hz) */
            public double rbw;              /**< Resolution band-width (Hz) */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct SweepComputationResults
        {
            public double stepFreq;						/**< Computed desired FFT bin size (converted from rbw and window) */
            public double fftBinSize;						/**< Actual FFT bin size (some power of 2) */
            public double actualRbw;						/**< Actual RBW (related to fftBinSize by window type) */
            public double tunerSampleRate;				    /**< Actual tuner sample rate (Hz) */
            public UInt32 fftBlockSize;					/**< FFT size */
            public double nyquistFactor;					/**< Either 1.4 or 1.28 depending on tunerSampleRate */
            public UInt32 numBinsReturned;				    /**< Number of FFT bins returned in each segment */
            public UInt32 numBinsReturnedLastSegment;		/**< Number of FFT bins returned in the last segment */
            public UInt32 firstPointIdx;					/**< Index of first FFT bin returned */
            public UInt32 firstPointIdxLastSegment;		/**< Index of first FFT bin returned in the last segment */
            public UInt32 numSegments;					    /**< Number of FFT segments to cover the span */
            public double centerFrequencyFirstSegment;	    /**< Center frequency of the first segment */
            public double centerFrequencyLastSegment;		/**< Center frequency of the last segment */
        }


        #endregion

        #region

        [DllImport("Kernel32.dll")]
       private static extern bool AllocConsole();

        [DllImport("kernel32.dll",
            EntryPoint ="GetStdHandle",
            SetLastError =true,
            CharSet =CharSet.Auto,
            CallingConvention =CallingConvention.StdCall)]
       private static extern IntPtr GetStdHandle(int nStdHandle);

       private const int STD_OUTPUT_HANDLE = -11;

        public static void setOutput()
        {
            AllocConsole();
           IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
           SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
           FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
           Encoding encoding = System.Text.Encoding.GetEncoding(Console.OutputEncoding.CodePage);
           StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
            standardOutput.AutoFlush =true;
           Console.SetOut(standardOutput);

        }

        #endregion




        // ===================== SAL_SEGMENT_CALLBACK( ) ==================================
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int SAL_SEGMENT_CALLBACK(ref SegmentData dataHeader, IntPtr data);




       //private const String AGSAL_DLL_NAME_X64 = @"AgilentDll_x64.dll";
        private const String AGSAL_DLL_NAME_Win32 = @"AgilentDll.dll";



        /*
         * 
_DLLMETER_API bool GetSensorCapabilities(_OUT_ salSensorCapabilities *  capabilities);//salGetSensorCapabilities 
_DLLMETER_API void SendStopWbqexCmd(); //salSendSweepCommand
_DLLMETER_API bool GetWbqexData(_OUT_ salSegmentData* dataHdr,	_OUT_ salFloat32*  pAmplitude,_IN_ salUInt32  userDataBufferBytes);//salGetSegmentData
_DLLMETER_API bool Close();
         * 
         */



        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "Connect", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool Connect(string ip);


        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "SendWbqexCmd", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static int SendWbqexCmd(double start, double end, double rbw, SAL_SEGMENT_CALLBACK myHandler);


        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "GetSensorCapabilities", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool GetSensorCapabilities(out SensorCapabilities capabilities);


        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "SendStopWbqexCmd", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static void SendStopWbqexCmd();



        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "GetWbqexData", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static bool GetWbqexData(out SegmentData dataHeader, float[] data, UInt32 maxDataBytes);


        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "Close", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static void Close();

        public static bool IsUseSensor = false;


        //rmtp section

        public const int MAX_ERROR_STRING_RMTP = 512;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct rmtpSegmentData
        {
            public double startFrequency;  /**< Frequency of first point returnded by this measurement */
            public double frequencyStep;   /**< Frequency spacing in Hertz of frequency data */
            public UInt32 numPoints;       /**< Number of frequency points returned by this measurement */

            public int errorNum;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ERROR_STRING_RMTP)]
            public string errorInfo;

            public double frameStartFrequency;
            public int frameIndex;
            public int frameNumPoints;

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct rmtpMsgData
        {
            public int msgType;
            public int MsgLength;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ERROR_STRING_RMTP)]
            public string MsgInfo;

        }

        // ===================== SAL_SEGMENT_CALLBACK( ) ==================================
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int RMTP_SEGMENT_CALLBACK(ref rmtpSegmentData dataHeader, IntPtr data);

        // ===================== SAL_SEGMENT_CALLBACK( ) ==================================
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int Msg_CALLBACK(ref rmtpMsgData msg);

        private const String _DLL_NAME_X64 = "RmtpDll_x64.dll";
        private const String _DLL_NAME_Win32 = "RmtpDll.dll";
        
        [DllImport(_DLL_NAME_Win32, EntryPoint = "RMTPConnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern int RMTPConnect_Win32(string ip, int port, string username, string pwd, Msg_CALLBACK msgHandler, RMTP_SEGMENT_CALLBACK dataHandler);
        
        [DllImport(_DLL_NAME_X64, EntryPoint = "RMTPConnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern int RMTPConnect_X64(string ip, int port, string username, string pwd, Msg_CALLBACK msgHandler, RMTP_SEGMENT_CALLBACK dataHandler);

        public static int RMTPConnect(string ip, int port, string username, string pwd, Msg_CALLBACK msgHandler, RMTP_SEGMENT_CALLBACK dataHandler)
        {
            if (IntPtr.Size == 8)
                return RMTPConnect_X64(ip, port, username, pwd, msgHandler, dataHandler); // Call 64-bit DLL
            else
                return RMTPConnect_Win32(ip, port, username, pwd, msgHandler, dataHandler); // Call 32-bit DLL
        }

        [DllImport(_DLL_NAME_Win32, EntryPoint = "RMTPSendStopWbqexCmd", CallingConvention = CallingConvention.Cdecl)]
        private static extern int RMTPSendStopWbqexCmd_Win32();

        [DllImport(_DLL_NAME_X64, EntryPoint = "RMTPSendStopWbqexCmd", CallingConvention = CallingConvention.Cdecl)]
        private static extern int RMTPSendStopWbqexCmd_X64();

        public static int RMTPSendStopWbqexCmd()
        {
            if (IntPtr.Size == 8)
               return RMTPSendStopWbqexCmd_X64(); // Call 64-bit DLL
            else
               return RMTPSendStopWbqexCmd_Win32(); // Call 32-bit DLL
        }

        [DllImport(_DLL_NAME_Win32, EntryPoint = "RMTPSendWbqexCmd", CallingConvention = CallingConvention.Cdecl)]
        private static extern int RMTPSendWbqexCmd_Win32(double startFreq, double endFreq, double rbw);

        [DllImport(_DLL_NAME_X64, EntryPoint = "RMTPSendWbqexCmd", CallingConvention = CallingConvention.Cdecl)]
        private static extern int RMTPSendWbqexCmd_X64(double startFreq, double endFreq, double rbw);

        public static int RMTPSendWbqexCmd(double startFreq, double endFreq, double rbw)
        {
            if (IntPtr.Size == 8)
                return RMTPSendWbqexCmd_X64(startFreq, endFreq, rbw); // Call 64-bit DLL
            else
                return RMTPSendWbqexCmd_Win32(startFreq, endFreq, rbw); // Call 32-bit DLL
        }
        //rmtp end

    }
}
