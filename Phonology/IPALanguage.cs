using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public class IPALanguage
    {
        public string Code { get; }
        public string FriendlyName { get; }

        public IPALanguage(string code, string friendlyName)
        {
            Code = code;
            FriendlyName = friendlyName;
        }

        public CultureInfo? Culture
        {
            get
            {
                try
                {
                    return CultureInfo.GetCultureInfo(Code.Replace('_', '-'));
                } catch (CultureNotFoundException)
                {
                    return null;
                }
            }
        }
    }
}
