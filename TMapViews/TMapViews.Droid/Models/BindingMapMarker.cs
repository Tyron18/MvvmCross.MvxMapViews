﻿using TMapViews.Models;

namespace TMapViews.Droid.Models
{
    public class BindingMapMarker : Java.Lang.Object, IBindingMapAnnotation
    {
        public virtual I2DLocation Location { get; set; }
    }
}