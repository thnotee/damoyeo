using System;
using System.Collections.Generic;
using System.Text;

namespace Damoyeo.Data.DataAccess
{
    public class Logger : ILogger
    {
        public string Test(string message)
        {
            return "테스트성공";
        }
    }
}
