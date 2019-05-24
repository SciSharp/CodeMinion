using System;
using System.Collections.Generic;
using System.Text;

namespace Numpy.Models
{
    public class Shape
    {
        public int[] Dimensions { get; set; }

        public Shape(params int[] shape)
        {
            this.Dimensions = shape;
        }
    }
}
