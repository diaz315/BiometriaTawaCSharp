using System;
using System.Device.Location;

namespace BiometriaTawaCSharp
{
    class CLocation
    {
        public static string GetLocationProperty()
        {
            string cordenadas = "";
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
            // Do not suppress prompt, and wait 1000 milliseconds to start.
            watcher.TryStart(false, TimeSpan.FromMilliseconds(1000));

            GeoCoordinate coord = watcher.Position.Location;

            if (coord.IsUnknown != true)
            {
                cordenadas="Latitud: {0}, Longitud: {1}"+coord.Latitude+coord.Longitude;
            }
            else
            {
                cordenadas="N/A";
            }
            return cordenadas;
        }
    }
}