using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;


//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace NawigacjaPszczola
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class Koordynaty : Page
    {
        public Koordynaty()
        {
            this.InitializeComponent();
            GdzieJaNaMapie();
        }

        private void back(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

       async private void szukaj(object sender, RoutedEventArgs e)
        {
            daneGeograficzne.opisCelu = txAdres.Text;
            if (daneGeograficzne.opisCelu == "")
            {
                return;
            }
            var wynik = await MapLocationFinder.FindLocationsAsync(daneGeograficzne.opisCelu, new Geopoint(daneGeograficzne.pktStartowy));
            if(wynik.Status == MapLocationFinderStatus.Success)
            {
                try
                {
                    double dlg = wynik.Locations[0].Point.Position.Longitude;
                    double szer = wynik.Locations[0].Point.Position.Latitude;
                    tbDlg.Text = $"dlg.: {dlg:F4}";
                    tbSzer.Text = $"szer.: {szer:F4}";
                    daneGeograficzne.pktDocelowy = wynik.Locations[0].Point.Position;
                    MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteAsync(
                        new Geopoint(daneGeograficzne.pktStartowy),
                        new Geopoint(daneGeograficzne.pktDocelowy),
                        MapRouteOptimization.Time,
                        MapRouteRestrictions.None);
                        daneGeograficzne.distance = (routeResult.Route.LengthInMeters / 1000).ToString();
                }
                catch (Exception)
                {
                    var dlg = new Windows.UI.Popups.MessageDialog(wynik.Status.ToString(), daneGeograficzne.opisCelu);
                    await dlg.ShowAsync();
                    daneGeograficzne.opisCelu = wynik.Status.ToString();
                }                                
            }
            else
            {
                var dlg = new Windows.UI.Popups.MessageDialog(wynik.Status.ToString(), daneGeograficzne.opisCelu);
                await dlg.ShowAsync();
                daneGeograficzne.opisCelu = wynik.Status.ToString();
            }
        }

        async void GdzieJaNaMapie()
        {
            Geolocator mojGPS = new Geolocator { DesiredAccuracy = PositionAccuracy.High };
            Geoposition mojeZGPS = await mojGPS.GetGeopositionAsync();

            double dlg = mojeZGPS.Coordinate.Point.Position.Longitude;
            double szer = mojeZGPS.Coordinate.Point.Position.Latitude;

            tbGPS.Text = $"dlg.: {dlg:F4} szer.: {szer:F4}";
            
            BasicGeoposition geoposition = new BasicGeoposition
            {
                Latitude = szer,
                Longitude = dlg
            };
            daneGeograficzne.pktStartowy = geoposition;

        }
    }
}
