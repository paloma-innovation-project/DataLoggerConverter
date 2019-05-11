using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDrive
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = DataLoggerConverter.Converter.GetLoggerData(@"C:\Users\MORIMOTOS\Desktop\新しいフォルダー (2)\間欠　40℃　最大　Ver.28　3段点火.csv");
        }
    }
}
