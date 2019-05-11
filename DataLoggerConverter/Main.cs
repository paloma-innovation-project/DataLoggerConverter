using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLoggerConverter
{
    public class Converter
    {
        public static LoggerData GetLoggerData(string path)
        {
            var t = new LoggerData();
            var enc = System.Text.Encoding.GetEncoding("shift-jis");
            string str = System.IO.File.ReadAllText(path, enc);
            var p = GetChString(str);
            var u = GetBodyStrings(str);
            UInt16 cnt = 0;
            SetInitial(ref t, str);
            foreach (var f in p)
            {
                var w = new LoggerData.LoggerOneData();
                string[] g = f.Split(',');
                g[0] = g[0].Replace("CH", "");
                w.CHNo = UInt16.Parse(g[0]);
                w.Name = g[1];
                switch (g[2])
                {
                    case "TEMP":
                        w.Unit = LoggerData.LoggerOneData.TypeUnit.TEMP;
                        break;
                    case "DC":
                        w.Unit = LoggerData.LoggerOneData.TypeUnit.DC;
                        break;
                    default:
                        break;
                }
                foreach (var y in u)
                {
                    string[] q = y.Split(',');
                    var v = new LoggerData.LoggerOneData.PointData();
                    v.No = ulong.Parse(q[0]);
                    v.DateTime = DateTime.Parse(q[1]);
                    v.Value = double.Parse(q[cnt+3]);
                    v.SamplingMillisSecond = ((v.No - 1) * t.SamplingMillisSecond);
                    w._DataValue.Add(v);
                }
                t._Data.Add(w);
                cnt++;
            }

            SetInitial(ref t, str);
            return t;
        }

        private static List<string> GetBodyStrings(string text)
        {
            List<string> str = new List<string>();
            string[] del = { "\r\n" };
            string[] arr = text.Split(del, StringSplitOptions.None);
            foreach (var f in arr)
            {
                str.Add(f);
            }
            ulong cnt = 0;
            while(true)
            {
                string[] s = str[0].Split(',');
                if (s[0] == "NO.")
                {
                    break;
                }
                str.RemoveAt(0);
                cnt++;
                if (cnt >= 1000)
                {
                    break;
                }
            }
            str.RemoveAt(0);
            return str;
                
        }
        private static void SetInitial(ref LoggerData data,string text)
        {
            string[] del = { "\r\n" };
            string[] arr = text.Split(del, StringSplitOptions.None);
            foreach (var f in arr)
            {
                try
                {
                    string[] s = f.Split(',');
                    switch (s[0])
                    {
                        case "ベンダ":
                            data.Vendor = s[1];
                            break;
                        case "モデル":
                            data.Model = s[1];
                            break;
                        case "バージョン":
                            data.Version = s[1];
                            break;
                        case "開始時刻":
                            data.StartDateTime = DateTime.Parse(s[1] + " " + s[2]);
                            break;
                        case "終了時刻":
                            data.EndDateTime = DateTime.Parse(s[1] + " " + s[2]);
                            break;
                        case "測定間隔":
                            switch (s[1])
                            {
                                case "100ms":
                                    data.SamplingMillisSecond = 100;
                                    break;
                                case "250ms":
                                    data.SamplingMillisSecond = 250;
                                    break;
                                case "1s":
                                    data.SamplingMillisSecond = 1000;
                                    break;
                                default:
                                    data.SamplingMillisSecond = 0;
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {

                }
                

            }
        }
        /// <summary>
        /// csvのテキストの中から
        /// CH1,入水温,TEMP,TC_T,Off,100.000000,0.000000,ﾟC
        /// こんな感じの部分を抜き出す
        /// </summary>
        /// <returns></returns>
        private static List<string> GetChString(string text)
        {
            List<string> str = new List<string>();
            string[] del = { "\r\n" };
            string[] arr = text.Split(del, StringSplitOptions.None);
            foreach (var f in arr)
            {
                str.Add(f);
            }
            List<string> rstr = new List<string>();
            bool b = false;
            foreach (string f in str)
            {
                if (f == "Logic/Pulse,Pulse")
                {
                    b = false;
                }
                if (b)
                {
                    rstr.Add(f);
                }
                if (f == "CH,信号名,入力,レンジ,フィルタ,スパン")
                {
                    b = true;
                }

            }
            return rstr;
        }
    }
    public class LoggerData
    {
        #region サンプリングタイム
        private ulong _SamplingMs;
        private int myVar;

        /// <summary>
        /// サンプリングタイム(ms)
        /// </summary>
        public ulong SamplingMillisSecond
        {
            get
            {
                return _SamplingMs;
            }
            set
            {
                _SamplingMs = value;
            }
        }
        public double SamplingSecond
        {
            get
            {
                double d = (double)_SamplingMs;
                d = d / 1000;
                return d;
            }
            set
            {
                double d = value;
                d = d * 1000;
                _SamplingMs = (ulong)d;
            }
        }
        #endregion

        public string Vendor { get; set; }
        public string Model { get; set; }
        public string Version { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        internal List<LoggerOneData> _Data = new List<LoggerOneData>();
        public List<LoggerOneData> Data { get { return _Data; } }

        public class LoggerOneData
        {
            public UInt16 CHNo { get; set; }
            public string Name { get; set; }
            public enum TypeUnit
            {
                TEMP,DC
            }
            public TypeUnit Unit { get; set; }
            internal List<PointData> _DataValue = new List<PointData>();
            public List<PointData> DataValue { get { return _DataValue; } }
            public class PointData
            {
                #region サンプリングタイム
                private ulong _SamplingMs;
                private int myVar;

                /// <summary>
                /// サンプリングタイム(ms)
                /// </summary>
                public ulong SamplingMillisSecond
                {
                    get
                    {
                        return _SamplingMs;
                    }
                    set
                    {
                        _SamplingMs = value;
                    }
                }
                public double SamplingSecond
                {
                    get
                    {
                        double d = (double)_SamplingMs;
                        d = d / 1000;
                        return d;
                    }
                    set
                    {
                        double d = value;
                        d = d * 1000;
                        _SamplingMs = (ulong)d;
                    }
                }
                #endregion
                public ulong No { get; set; }
                public DateTime DateTime { get; set; }
                public double Value { get; set; }
            }
        }
    }
}
