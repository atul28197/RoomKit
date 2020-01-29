﻿using System;
using System.Collections.Generic;
using Elements;
using Elements.Geometry;
using GeometryEx;

namespace RoomKit
{
    /// <summary>
    /// A data structure recording room characteristics.
    /// </summary>
    public class Room
    {
        #region Constructors

        /// <summary>
        /// Default constructor creates a 1.0 x 1.0 x 1.0 white cube on the zero plane with the SW corner at the origin.
        /// </summary>
        public Room()
        {
            Color = Palette.White;
            Department = "";
            DesignArea = 1.0;
            DesignRatio = 1.0;
            Elevation = 0.0;
            Height = 1.0;
            Name = "";
            Number = "";
            Perimeter = Polygon.Rectangle(Vector3.Origin, new Vector3(1.0, 1.0));
            Placed = false;
            Suite = "";
            SuiteID = "";
            UniqueID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Constructor creates a Room by a positive sizing vector with the SW corner at the origin.
        /// </summary>
        public Room(Vector3 size)
        {
            if (size.X <= 0.0 || size.Y <= 0.0 || size.Z <= 0.0)
            {
                throw new ArgumentOutOfRangeException(Messages.NEGATIVE_VALUE_EXCEPTION);
            }
            Color = Palette.White;
            Department = "";
            DesignArea = size.X * size.Y;
            DesignRatio = size.X / size.Y;
            Elevation = 0.0;
            Height = size.Z;
            Name = "";
            Number = "";
            Perimeter = Polygon.Rectangle(Vector3.Origin, new Vector3(size.X, size.Y));
            Placed = false;
            Suite = "";
            SuiteID = "";
            UniqueID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Constructor creates a Room by a positive area and a positive x : y dimension ratio and a positive height with the SW corner at the origin.
        /// </summary>
        public Room(double area, double ratio, double height)
        {
            if (area <= 0.0 || ratio <= 0.0 || height <= 0.0)
            {
                throw new ArgumentOutOfRangeException(Messages.NEGATIVE_VALUE_EXCEPTION);
            }
            Color = Palette.White;
            Department = "";
            DesignArea = area;
            DesignRatio = ratio;
            Elevation = 0.0;
            Height = height;
            Name = "";
            Number = "";
            Perimeter = Shaper.RectangleByArea(area, ratio);
            Placed = false;
            Suite = "";
            SuiteID = "";
            UniqueID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Constructor creates a Room by two points, a positive width, and a positive height.
        /// </summary>
        public Room(Vector3 start, Vector3 end, double width, double height)
        {
            if (width <= 0.0 || height <= 0.0)
            {
                throw new ArgumentOutOfRangeException(Messages.NEGATIVE_VALUE_EXCEPTION);
            }
            Color = Palette.White;
            Department = "";
            DesignArea = start.DistanceTo(end) * width;
            DesignRatio = start.DistanceTo(end) / width;
            Elevation = start.Z;
            Height = height;
            Name = "";
            Number = "";
            Perimeter = new Line(start, end).Thicken(width);
            Placed = false;
            Suite = "";
            SuiteID = "";
            UniqueID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Constructor creates a Room by a line, a positive width, and a positive height.
        /// </summary>
        public Room(Line line, double width, double height)
        {
            if (width <= 0.0 || height <= 0.0)
            {
                throw new ArgumentOutOfRangeException(Messages.NEGATIVE_VALUE_EXCEPTION);
            }
            Color = Palette.White;
            Department = "";
            DesignArea = line.Length() * width;
            DesignRatio = line.Length() / width;
            Elevation = 0.0;
            Height = height;
            Name = "";
            Number = "";
            Perimeter = line.Thicken(width);
            Placed = false;
            Suite = "";
            SuiteID = "";
            UniqueID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Constructor creates a Room by a Polygon and a positive height.
        /// </summary>
        public Room(Polygon polygon, double height)
        {
            if (height <= 0.0)
            {
                throw new ArgumentOutOfRangeException(Messages.NEGATIVE_VALUE_EXCEPTION);
            }
            if (polygon.IsClockWise())
            {
                polygon = polygon.Reversed();
            }
            Color = Palette.White;
            Department = "";
            DesignArea = polygon.Area();
            var compass = polygon.Compass();
            DesignRatio = compass.SizeX / compass.SizeY;
            Elevation = 0.0;
            Height = height;
            Name = "";
            Number = "";
            Perimeter = polygon;
            Placed = false;
            Suite = "";
            SuiteID = "";
            UniqueID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Constructor creates a Room from another Room.
        /// </summary>
        public Room(Room room)
        {

            Color = room.Color;
            Department = room.Suite;
            DesignArea = room.DesignArea;
            DesignRatio = room.DesignRatio;
            Elevation = room.Elevation;
            Height = room.Height;
            Name = room.Name;
            Number = room.Number;
            Perimeter = room.Perimeter;
            Placed = false;
            Suite = room.Suite;
            SuiteID = room.SuiteID;
            UniqueID = Guid.NewGuid().ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The area of the Room's Perimeter;
        /// </summary>
        public double Area
        {
            get
            {
                return Perimeter.Area();
            }
        }

        /// <summary>
        /// Color of the Room. 
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Material of the Room. 
        /// </summary>
        public Material ColorAsMaterial
        {
            get
            {
                return new Material(this.Color, 0.0, 0.0, Guid.NewGuid(), Guid.NewGuid().ToString());
            }
        }

        /// <summary>
        /// Arbitrary string identifier to link this Room to a department.
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Intended area for this Room after placement.
        /// </summary>
        public double DesignArea { get; set; }

        /// <summary>
        /// Intended x :y ratio for this Room after placement.
        /// </summary>
        public double DesignRatio { get; set; }

        /// <summary>
        /// Vertical position of the Room's lowest plane relative to the ground plane.
        /// </summary> 
        public double Elevation { get; set; }

        /// <summary>
        /// Vertical height of the room above its elevation.
        /// </summary>
        private double height;
        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                if (value > 0.0)
                {
                    height = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(Messages.NEGATIVE_VALUE_EXCEPTION);
                }
            }
        }

        /// <summary>
        /// Arbitrary string identifier for this Room instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Arbitrary string identifier for this Room instance.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Polygon perimeter of the Room.
        /// </summary>
        private Polygon perimeter;
        public Polygon Perimeter
        {
            get
            {
                return perimeter;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(Messages.PERIMETER_NULL_EXCEPTION);
                }
                if (value.IsClockWise())
                {
                    perimeter = value.Reversed();
                }
                else
                {
                    perimeter = value;
                }
            }
        }

        /// <summary>
        /// Profile perimeter of the Room.
        /// </summary>
        public Profile PerimeterAsProfile
        {
            get
            {
                return new Profile(perimeter);
            }
        }


        /// <summary>
        /// Whether the Room has been placed in a location with a perimeter.
        /// </summary>
        public bool Placed { get; set; }

        /// <summary>
        /// Arbitrary string identifier to link this Room to a Suite instance.
        /// </summary>
        public string Suite { get; set; }

        /// <summary>
        /// Arbitrary string identifier to link this Room to a Suite instance.
        /// </summary>
        public string SuiteID { get; set; }

        /// <summary>
        /// UUID for this instance set on initialization.
        /// </summary>
        public string UniqueID { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Moves the Room Perimeter between the specifed relative points.
        /// </summary>
        /// <param name="from">Vector3 anchor point.</param> 
        /// <param name="to">Vector3 destination point.</param> 
        /// <returns>
        /// None.
        /// </returns>
        public void MoveFromTo(Vector3 from, Vector3 to)
        {
            perimeter = perimeter.MoveFromTo(from, to);
        }

        /// <summary>
        /// Rotates the Room Perimeter in the horizontal plane around the supplied pivot point.
        /// </summary>
        /// <param name="pivot">Vector3 point around which the Room Perimeter will be rotated.</param> 
        /// <param name="angle">Angle in degrees to rotate the Perimeter.</param> 
        /// <returns>
        /// None.
        /// </returns>
        public void Rotate(Vector3 pivot, double angle)
        {
            perimeter = perimeter.Rotate(pivot, angle);
        }

        #endregion
    }
}
