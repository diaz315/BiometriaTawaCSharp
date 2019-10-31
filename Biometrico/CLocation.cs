using System;
using System.Device.Location;

namespace BiometriaTawaCSharp
{
    class CLocation
    {
        private static double latitud;
        private static double longitud;
        public static string GetLocationProperty()
        {
            string cordenadas = "";
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
            watcher.Start(true);

            cordenadas = latitud + "-" + longitud;

            return cordenadas;
        }
        private static void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            latitud = e.Position.Location.Latitude;
            longitud = e.Position.Location.Longitude;
        }
    }
}