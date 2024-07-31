using System;
using System.Linq;
using System.Windows;
using Geburtstage.Services;
using Geburtstage.ViewModels;
using ScottPlot;

namespace Geburtstage.Views
{
    public partial class PlotView : Window
    {
        public PlotView()
        {
            InitializeComponent();
            PlotViewModel plotViewModel = new PlotViewModel();
            var plt = plotViewModel.CreateBirthdayPlot();
            // Zuweisen des Plots zum WpfPlot-Steuerelement
            foreach (var pltable in plt.GetPlottables())
            {
                wpfPlot1.Plot.Add.Plottable(pltable);
            }

            wpfPlot1.UpdateLayout(); // Plot rendern
        }
    }
}
