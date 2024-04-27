using System.Net.Http;
using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SeriesCollection SeriesCollection_temp { get; set; }
        public string[] Labels_temp { get; set; }
        public Func<double, string> YFormatter_temp { get; set; }


        public SeriesCollection SeriesCollection_humidity { get; set; }
        public string[] Labels_humidity { get; set; }
        public Func<double, string> YFormatter_humidity { get; set; }


        public SeriesCollection SeriesCollection_precip { get; set; }
        public string[] Labels_precip { get; set; }
        public Func<double, string> YFormatter_precip { get; set; }


        public SeriesCollection SeriesCollection_wind { get; set; }
        public string[] Labels_wind { get; set; }
        public Func<double, string> YFormatter_wind { get; set; }

        public MainWindow()
        {
            InitializeComponent();

        }

        public void DrawChart()
        {
            YFormatter_temp = value => value.ToString("F2") + " °C";
            YFormatter_humidity = value => value.ToString("F2") + " %";
            YFormatter_precip = value => value.ToString("F2") + " mm";
            YFormatter_wind = value => value.ToString("F2") + " km/h";

            DataContext = this;
        }
        public async Task<JObject> FetchWeatherData(string location, string startDate, string endDate = null)
        {
            var apiKey = "3KAJKHWT3UEMRQWF2ABKVVVZE";
            var baseUrl = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/";
            var url = string.IsNullOrEmpty(endDate)
                ? $"{baseUrl}{location}/{startDate}?key={apiKey}"
                : $"{baseUrl}{location}/{startDate}/{endDate}?key={apiKey}";

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JObject.Parse(content);
            }
        }
        public async Task GetWeatherSeries(string location, string startDate, string endDate = null)
        {

            var data = await FetchWeatherData(location, startDate, endDate);
            var points_temp = new ChartValues<double>();
            var points_humidity = new ChartValues<double>();
            var points_precip = new ChartValues<double>();
            var points_wind = new ChartValues<double>();

            var daysData = data["days"] as JArray;  // Ensure daysData is treated as a JArray
            if (daysData != null)
            {
                Labels_temp = new string[daysData.Count];
                Labels_humidity = new string[daysData.Count];
                Labels_precip = new string[daysData.Count];
                Labels_wind = new string[daysData.Count];

                for (int i = 0; i < daysData.Count; i++)
                {
                    Labels_temp[i] = (string)daysData[i]["datetime"];
                    Labels_humidity[i] = (string)daysData[i]["datetime"];
                    Labels_precip[i] = (string)daysData[i]["datetime"];
                    Labels_wind[i] = (string)daysData[i]["datetime"];
                    
                    points_temp.Add((double)daysData[i]["temp"]);
                    points_humidity.Add((double)daysData[i]["humidity"]);
                    points_precip.Add((double)daysData[i]["precip"]);
                    points_wind.Add((double)daysData[i]["windspeed"]);

                }
                SeriesCollection_temp = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "",
                        Values = points_temp,
                        PointGeometry = null,
                        StrokeThickness = 1,                        
                    }
                };

                SeriesCollection_humidity = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "",
                        Values = points_humidity,
                        PointGeometry = null,
                        StrokeThickness = 1,
                    }
                };

                SeriesCollection_precip = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "",
                        Values = points_precip,
                        PointGeometry = null,
                        StrokeThickness = 1,
                    }
                };

                SeriesCollection_wind = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "",
                        Values = points_wind,
                        PointGeometry = null,
                        StrokeThickness = 1,
                    }
                };
            }

        }

        public async void onSubmitClick(object sender, RoutedEventArgs e)
        {
            string location = Location.Text;
            string StartDate = startDate.Text;
            string EndDate = endDate.Text;
            
            try
            {
                await GetWeatherSeries(location, StartDate, EndDate);
                DrawChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        
    }
}