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



//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace NawigacjaPszczola
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void koordynatyMapa(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Koordynaty));
        }

        private void trybMapy(object sender, RoutedEventArgs e)
        {
            var ab = sender as AppBarButton;
            FontIcon ficon = new FontIcon {FontFamily = FontFamily.XamlAutoFontFamily };
            if(mojaMapa.Style == MapStyle.Aerial3DWithRoads)
            {
                mojaMapa.Style = MapStyle.Road;
                ab.Label = "Satelita";
                ficon.Glyph = "S";
                ab.Icon = ficon;

            }
            else
            {
                mojaMapa.Style = MapStyle.Aerial3DWithRoads;
                ab.Label = "Mapa";
                ficon.Glyph = "M";
                ab.Icon = ficon;
            }
        }

        private void pomMapa(object sender, RoutedEventArgs e)
        {
            mojaMapa.ZoomLevel--;
        }

        private void powMapa(object sender, RoutedEventArgs e)
        {
            mojaMapa.ZoomLevel++;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (daneGeograficzne.opisCelu == null) return;
            var pkt = new Geopoint(daneGeograficzne.pktStartowy);
            MapIcon start = new MapIcon()
            {
                Location = pkt,
                Title = "Tu jesteś!"
            };
            mojaMapa.MapElements.Add(start);
            var pktCel = new Geopoint(daneGeograficzne.pktDocelowy);
            MapIcon cel = new MapIcon()
            {
                Location = pktCel,
                Title = daneGeograficzne.opisCelu
            };
            mojaMapa.MapElements.Add(cel);
            Trasa(pkt, pktCel);

            var komunikat = new Windows.UI.Popups.MessageDialog(daneGeograficzne.distance + " km", $"Odległość do {daneGeograficzne.opisCelu} to:");
            await komunikat.ShowAsync();

            MapPolyline trasaLotem = new MapPolyline()
            {
                StrokeColor = Windows.UI.Colors.Black,
                StrokeThickness = 3,
                StrokeDashed = true,
                Path = new Geopath(new List<BasicGeoposition> { daneGeograficzne.pktStartowy,daneGeograficzne.pktDocelowy })
                
            };
            mojaMapa.MapElements.Add(trasaLotem);
            await mojaMapa.TrySetViewAsync(new Geopoint(daneGeograficzne.pktStartowy), 8);

        }
        async void Trasa(Geopoint start, Geopoint stop)
        {
            var wynik = await MapRouteFinder.GetDrivingRouteAsync(start, stop);
            if (wynik.Status == MapRouteFinderStatus.Success)
            {
                var trasa = wynik.Route;
                var trasaNaMape = new MapRouteView(wynik.Route)
                {
                    RouteColor = Windows.UI.Colors.Blue
                };
                mojaMapa.Routes.Add(trasaNaMape);
                await mojaMapa.TrySetViewBoundsAsync(trasa.BoundingBox, new Thickness(25), MapAnimationKind.Bow);
                
            }
            else
            {
                var dlg = new Windows.UI.Popups.MessageDialog(wynik.Status.ToString(), daneGeograficzne.opisCelu);
                await dlg.ShowAsync();
            }

        }

    }
}
