﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows.Media;

namespace CO_IA.UI.Collection.Chart
{
    public class LineGraphViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets or sets the point data source.
        /// </summary>
        /// <value>The point data source.</value>
        public CompositeDataSource PointDataSource
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the line graph.</value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public Color Color
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        /// <value>The entity id.</value>
        public Guid EntityId
        {
            get;
            set;
        }

        public bool LineAndMarker
        {
            get;
            set;
        }

        public int Thickness
        {
            get;
            set;
        }
    }
}
