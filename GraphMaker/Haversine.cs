#define HAVERSINE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphMaker
{
    public enum DIRECTION { NORTH = 0, SOUTH, EAST, WEST };
/*
                          -1   straight line distance
surface distance = 2 R sin   (------------------------)
                                       2 R
*/
    class LatLongPoint
    {
        double latitude; // in degrees only (convert minutes to fraction of degree -> (minutes / 60 * 100)
        double longitude;
        DIRECTION latitude_direction;
        DIRECTION longitude_direction;
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; } // convert (latitude, longitude) to Cartesian (x, y, z)

        public const double PI = 3.141592;
        public const double RADIUS_OF_EARTH = 6367.0; // not exact, and the earth isn't a sphere either

        public double phi;
        public double theta;

        public LatLongPoint(double latitude, DIRECTION lat_dir, double longitude, DIRECTION lon_dir)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.latitude_direction = lat_dir;
            this.longitude_direction = lon_dir;
            CalculatePhiAndTheta();
            CalculateXYZ();
        }

        public void CalculatePhiAndTheta()
        {
            if (latitude_direction == DIRECTION.NORTH)
                phi = 90.0 - latitude;
            else // == LATLONG.SOUTH
                phi = 90.0 + latitude;

            if (longitude_direction == DIRECTION.EAST)
                theta = longitude;
            else // == LATLONG.WEST
                theta = (-1 * longitude);

            phi = DegreesToRadians(phi); // convert phi and theta to radians
            theta = DegreesToRadians(theta);
        }

        public void CalculateXYZ()
        {
            x = RADIUS_OF_EARTH * Math.Cos(theta) * Math.Sin(phi);
            y = RADIUS_OF_EARTH * Math.Sin(theta) * Math.Sin(phi);
            z = RADIUS_OF_EARTH * Math.Cos(phi);
            Console.WriteLine("x: {0}\ny: {1}\nz: {2}", x, y, z);
        }

        public static double DegreesToRadians(double degrees)
        {
            double radians;
            radians = (degrees * PI / 180);
            return radians;
        }

        public static double StraightLineDistance(LatLongPoint a, LatLongPoint b)
        {
            double distance = 0.0;
            double xd, yd, zd;

            xd = b.x - a.x; // find a vector from a -> b
            yd = b.y - a.y;
            zd = b.z - a.z;

            distance = Math.Sqrt((xd * xd) + (yd * yd) + (zd * zd)); // distance = magnitude of the vector

            return distance;
        }

        public static double SurfaceDistance(double straight_line_distance)
        {
            double distance = 0.0;

            distance = 2 * RADIUS_OF_EARTH * Math.Asin(straight_line_distance / (2 * RADIUS_OF_EARTH));

            return distance;
        }

        public static double Haversine(LatLongPoint PointA, LatLongPoint PointB)
        {
            double dLat = PointB.latitude - PointA.latitude;
            double dLon = PointB.longitude - PointA.longitude;
            double lat1 = PointA.latitude;
            double lat2 = PointB.latitude;

            /* This bit (commented) requires conversion to radians first, the next bit (in use) includes (degrees / (180/PI))
            // http://www.movable-type.co.uk/scripts/latlong.html
            double a = (Math.Sin(dLat / 2.0) * Math.Sin(dLat / 2.0)) +
                    Math.Sin(dLon / 2.0) * Math.Sin(dLon / 2.0) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));*/

            // http://www.meridianworlddata.com/Distance-Calculation.asp
            double a = Math.Sin(lat1/57.2958) * Math.Sin(lat2/57.2958) + Math.Cos(lat1/57.2958) * Math.Cos(lat2/57.2958) * Math.Cos(dLon/57.2958);
            double c = Math.Atan(Math.Sqrt(1 - (a * a)) / a);
            double distance = (double)RADIUS_OF_EARTH * c;

            return distance;
        }
    }
}
