﻿using System;
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
            var t = DataLoggerConverter.Converter.GetLoggerData(@"..\..\..\sample.csv");
        }
    }
}
