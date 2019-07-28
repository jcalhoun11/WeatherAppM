using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherAppM
{
    public class AppConnectivity
    {
        public static bool IsConnected()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
