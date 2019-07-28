using NOAAWeatherServiceContract;
using NOAAServiceReference;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NOAAWeatherService
{
    public class NOAAWeatherServiceManager : INOAAWeatherServiceManager
    {
        public void Dispose()
        {
            
        }

        public async Task<string> GetCurrent(string zipCode)
        {
            try
            {
                string latlist = string.Empty;
                
                XmlDocument doc = new XmlDocument();
                
                ndfdXMLPortTypeClient NOAA = new ndfdXMLPortTypeClient();
                //NOAA = new ndfdXMLPortTypeClient(ndfdXMLPortTypeClient.EndpointConfiguration.ndfdXMLPort, NOAA.Endpoint.Address);

                await NOAA.OpenAsync();


                latlist = await NOAA.LatLonListZipCodeAsync(zipCode);
                doc.LoadXml(latlist);

                string latlong = doc.SelectSingleNode("dwml").SelectSingleNode("latLonList").InnerText;

                string[] split = latlong.Split(',');
                string lat = split[0];
                string lon = split[1];

                string forecast = await NOAA.NDFDgenAsync(Convert.ToDecimal(lat), Convert.ToDecimal(lon), NOAAServiceReference.productType.timeseries, DateTime.Now, DateTime.Now, NOAAServiceReference.unitType.e, null);
                
                doc.LoadXml(forecast);

                await NOAA.CloseAsync();

                string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string weatherFolder = string.Format(@"{0}\{1}", appDataFolder, "WeatherApp");
                DirectoryInfo directoryInfo = new DirectoryInfo(weatherFolder);
                if (!directoryInfo.Exists)
                    Directory.CreateDirectory(weatherFolder);

                string currentFileName = string.Format(@"{0}\Current{1}.xml", weatherFolder, latlong);


                FileInfo forecastFile = new FileInfo(currentFileName);
                if (forecastFile.Exists)
                    File.Delete(currentFileName);

                doc.Save(currentFileName);

                return currentFileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetForecast(string zipCode)
        {
            try
            {
                string latlist = string.Empty;

                XmlDocument doc = new XmlDocument();
                NOAAServiceReference.ndfdXMLPortTypeClient NOAA = new NOAAServiceReference.ndfdXMLPortTypeClient();

                latlist = await NOAA.LatLonListZipCodeAsync(zipCode);
                doc.LoadXml(latlist);

                string latlong = doc.SelectSingleNode("dwml").SelectSingleNode("latLonList").InnerText;

                string[] split = latlong.Split(',');
                string lat = split[0];
                string lon = split[1];

                string forecast = await NOAA.NDFDgenByDayAsync(Convert.ToDecimal(lat), Convert.ToDecimal(lon), DateTime.Now, "7", NOAAServiceReference.unitType.e, NOAAServiceReference.formatType.Item12hourly);
                doc.LoadXml(forecast);

                string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string weatherFolder = string.Format(@"{0}\{1}", appDataFolder, "WeatherApp");
                DirectoryInfo directoryInfo = new DirectoryInfo(weatherFolder);
                if (!directoryInfo.Exists)
                    Directory.CreateDirectory(weatherFolder);


                string forecastFileName = string.Format(@"{0}\Forecast{1}.xml", weatherFolder, latlong);

                FileInfo forecastFile = new FileInfo(forecastFileName);
                if (forecastFile.Exists)
                    File.Delete(forecastFileName);

                doc.Save(forecastFileName);

                return forecastFileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
