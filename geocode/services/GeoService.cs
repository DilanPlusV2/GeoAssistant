using geocode.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geocode.services
{
    public class GeoService
    {
        private const double RadioTierra = 6371; // km

        public double CalcularDistancia(Barrio a, Barrio b)
        {
            if (a.Lat == null || a.Lng == null || b.Lat == null || b.Lng == null)
                return double.MaxValue;

            double lat1 = GradosARadianes(a.Lat.Value);
            double lon1 = GradosARadianes(a.Lng.Value);
            double lat2 = GradosARadianes(b.Lat.Value);
            double lon2 = GradosARadianes(b.Lng.Value);

            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;

            double h = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Pow(Math.Sin(dLon / 2), 2);

            double d = 2 * RadioTierra * Math.Asin(Math.Sqrt(h));

            return d;
        }

        private double GradosARadianes(double grados)
        {
            return grados * Math.PI / 180;
        }
    }
}
