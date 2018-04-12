﻿using MapKit;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TMapViews.Models;
using TMapViews.Models.Interfaces;
using TMapViews.Models.Models;
using UIKit;

namespace TMapViews.iOS
{
    public class BindingMKMapView : MKMapView
    {
        public BindingMKMapView()
        {
            var mapClickRecognizer = new UITapGestureRecognizer(OnMapClicked);
            AddGestureRecognizer(mapClickRecognizer);
            var mapLongClickRecognizer = new UILongPressGestureRecognizer(OnMapLongClick);
            AddGestureRecognizer(mapLongClickRecognizer);
        }

        private bool _shouldShowPins = true;

        public bool ShouldShowPins
        {
            get => _shouldShowPins;
            set
            {
                _shouldShowPins = value;
                UpdatePins();
            }
        }

        private double _zoomLevel = 0.2;

        public double ZoomLevel
        {
            get => _zoomLevel;
            set
            {
                _zoomLevel = value;
                CenterMap();
            }
        }

        private Binding2DLocation _centerMapLocation;

        public Binding2DLocation CenterMapLocation
        {
            get => _centerMapLocation;
            set
            {
                _centerMapLocation = value;
                CenterMap();
            }
        }

        private ObservableCollection<IBindingMapAnnotation> _annotationSource;

        public ObservableCollection<IBindingMapAnnotation> AnnotationSource
        {
            get => _annotationSource;
            set
            {
                _annotationSource = value;
                UpdatePins();
            }
        }

        public new BindingMKMapViewDelegate Delegate { get => base.Delegate as BindingMKMapViewDelegate; set => base.Delegate = value; }

        /// <summary>
        /// Executes when user taps a location on the map. Passes a <paramref name="I2DLocation"/>
        /// </summary>
        public ICommand MapClick { get; set; }

        /// <summary>
        /// Executes when user performs a long tap on a location on the map.
        /// Passes a <paramref name="I2DLocation"/>
        /// </summary>
        public ICommand MapLongClick { get; set; }

        public override MKMapType MapType { get => base.MapType; set => base.MapType = value; }
        public override bool ShowsUserLocation { get => base.ShowsUserLocation; set => base.ShowsUserLocation = value; }

        private void CenterMap()
        {
            if (CenterMapLocation != null)
            {
                var region = new MKCoordinateRegion
                {
                    Center = CenterMapLocation.ToCLLocationCoordinate2D(),
                    Span = new MKCoordinateSpan(ZoomLevel, ZoomLevel)
                };
                SetRegion(region, true);
            }
        }

        private void UpdatePins()
        {
            RemoveAnnotations(Annotations);
            if (ShouldShowPins)
            {
                foreach (var pin in AnnotationSource)
                    AddBindingAnnotation(pin);
            }
        }

        private void AddBindingAnnotation(IBindingMapAnnotation pin)
        {
            AddAnnotation(new BindingMKAnnotation(pin));
        }

        private void OnMapClicked(UITapGestureRecognizer gesture)
        {
            var loc = ConvertPoint(gesture.LocationInView(this), this).ToBinding2DLocation();
            if (MapClick?.CanExecute(loc) ?? false)
                MapClick.Execute(loc);
        }

        private void OnMapLongClick(UILongPressGestureRecognizer gesture)
        {
            var loc = ConvertPoint(gesture.LocationInView(this), this).ToBinding2DLocation();
            if (MapLongClick?.CanExecute(loc) ?? false)
                MapLongClick.Execute(loc);
        }
    }
}