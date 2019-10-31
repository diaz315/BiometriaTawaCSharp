using Suprema;
using System;
using System.Device.Location;

namespace BiometriaTawaCSharp
{
    class CLocation
    {
        public void GetLocationProperty()
        {
            GetLocationEvent();
        }

        private void GetLocationEvent()
        {
            GeoCoordinateWatcher watcher;

            watcher = new GeoCoordinateWatcher();
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
            watcher.TryStart(false, TimeSpan.FromMilliseconds(2000));

        }

        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            var coordenadas = e.Position.Location.Latitude + "" + e.Position.Location.Longitude;
            Huella.txtCoordenada.Text = coordenadas;
        }

    }
}