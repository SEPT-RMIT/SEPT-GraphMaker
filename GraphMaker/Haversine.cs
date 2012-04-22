using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphMaker
{
    public enum DIRECTION { NORTH = 0, SOUTH, EAST, WEST }; // the direction of the lat/long measurement (i.e. 56 degrees north)
/*
                          -1   straight line distance
surface distance = 2 R sin   (------------------------)
                                       2 R
*/
    class LatLongPoint
    {
        double latitude; // in degrees only (convert minutes to fraction of degree -> (minutes / 60 * 100)
        double longitude;
        DIRECTION latitude_direction; // the direction of the lat/long measurement (i.e. 56 degrees north)
        DIRECTION longitude_direction;
        public double x { get; set; } // convert (latitude, longitude) to Cartesian (x, y, z)
        public double y { get; set; }
        public double z { get; set; } 

        public const double PI = 3.141592;
        public const double RADIUS_OF_EARTH = 6367.0; // not exact, and the earth isn't a sphere either

        public double phi;
        public double theta;

        /**
         * Constructor creates a LatLongPoint with latitude and longitude in degrees (ONLY, must convert minutes)
         * relative to either N/S and E/W. The point is converted to Cartesian (x,y,z) 3d coordinates
         */ 
        public LatLongPoint(double latitude, DIRECTION lat_dir, double longitude, DIRECTION lon_dir)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.latitude_direction = lat_dir;
            this.longitude_direction = lon_dir;
            CalculatePhiAndTheta();
            CalculateXYZ(); // convert lat/long to Cartesian (x, y, z) spherical coordinates
        }

        /**
         * Phi and Theta are used in the conversion from lat/long to spherical coordinates
         */ 
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

        /**
         * Calculate Cartesian (x, y, z) 3d coordinates using Phi and Theta
         */ 
        public void CalculateXYZ()
        {
            x = RADIUS_OF_EARTH * Math.Cos(theta) * Math.Sin(phi);
            y = RADIUS_OF_EARTH * Math.Sin(theta) * Math.Sin(phi);
            z = RADIUS_OF_EARTH * Math.Cos(phi);
            Console.WriteLine("x: {0}\ny: {1}\nz: {2}", x, y, z);
        }

        /**
         * Convert degrees to radians - note degrees only, convert minutes to fractions of a degree (minutes / 60 * 100)
         * before passing to this function
         */ 
        public static double DegreesToRadians(double degrees)
        {
            double radians;
            radians = (degrees * PI / 180);
            return radians;
        }

        /**
         * Calculate the straight line distance from one 3d Cartesian coordinate to another, this straight line goes
         * THROUGH the sphere of the earth, not over the surface, and is used in the SurfaceDistance() function
         */ 
        public static double StraightLineDistance(LatLongPoint a, LatLongPoint b)
        {
            double distance = 0.0;
            double xd, yd, zd;

            xd = b.x - a.x; // find a vector from a -> b by subtracting the coordinates of a from the coordinates of b
            yd = b.y - a.y;
            zd = b.z - a.z;

            distance = Math.Sqrt((xd * xd) + (yd * yd) + (zd * zd)); // distance = magnitude of the vector

            return distance;
        }
        /**
         * Find the distance over the earth's surface, given a straight line distance from one point to another.
         * 
         * I haven't figured out how to derive this function, it seems to be a simplified Haversine and produces a fairly
         * accurate calculation.
         */ 
        public static double SurfaceDistance(double straight_line_distance)
        {
            double distance = 0.0;

            distance = 2 * RADIUS_OF_EARTH * Math.Asin(straight_line_distance / (2 * RADIUS_OF_EARTH));

            return distance;
        }

        /**
         * Calculate the great circle distance from one lat/long point to another using implementation of Haversine
         */ 
        public static double Haversine(LatLongPoint PointA, LatLongPoint PointB)
        {
            double dLat = PointB.latitude - PointA.latitude; // dLat is the difference in latitude
            double dLon = PointB.longitude - PointA.longitude; // dLon is the difference in longitude
            double lat1 = PointA.latitude; // saves space in formula for readability
            double lat2 = PointB.latitude; // "     "     "  "       "   " 

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
