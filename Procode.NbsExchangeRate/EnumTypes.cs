using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Procode.NbsExchangeRate
{
    public enum ReturnDataType
    {
        Html = 0,
        Csv = 1,
        Ascii = 2,
        Xls = 3
    }
     
    public enum RateType
    {
        Foreign = 1, // za devize
        Cash = 2, // za efektivu
        AverageRate = 3, // za srednji kurs
    }
}
