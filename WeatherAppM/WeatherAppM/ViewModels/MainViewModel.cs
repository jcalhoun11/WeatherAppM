using NOAAWeatherService;
using NOAAWeatherServiceContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Xamarin.Forms;

namespace WeatherAppM.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Properties
        private double _Direction;
        public double Direction
        {
            get { return _Direction; }
            set { _Direction = value; OnChanged(); }
        }


        #endregion

        #region Commands
        private Command _GetLocationCommand;
        public Command GetLocationCommand
        {
            get
            {
                if (_GetLocationCommand == null)
                    _GetLocationCommand = new Command(param => GetLocation(param));

                return _GetLocationCommand;
            }
        }
        #endregion
        public MainViewModel()
        {

        }

        private void GetLocation(object param)
        {
            if (param is string && AppConnectivity.IsConnected())
            {
                using (INOAAWeatherServiceManager manager = new NOAAWeatherServiceManager())
                {
                    try
                    {
                        string zipCode = param.ToString();
                        string currentFile = manager.GetCurrent(zipCode).Result;

                        //CurrentConditionProvider.Source = new Uri(currentFile);
                        //CurrentConditionProvider.XPath = "dwml/data";

                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(currentFile);
                        XmlNode node = xmlDocument.SelectSingleNode("dwml/data/parameters/direction/value");
                        Direction = Convert.ToDouble(node.InnerText);

                        //CurrentConditionProvider.Refresh();

                        string forecastFile = manager.GetForecast(zipCode).Result;

                        //ForecastProvider.Source = new Uri(forecastFile);
                        //ForecastProvider.XPath = "dwml/data";

                        //ForecastProvider.Refresh();



                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        //Handle
                    }
                }
            }
        }
    }
}
