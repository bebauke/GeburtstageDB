using Geburtstage.Services;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geburtstage.ViewModels
{
    internal class PlotViewModel : BaseViewModel
    {
        /// <summary>
        /// Erstellt ein Bar-Chart mit der Anzahl der Geburtstage pro Monat.
        /// </summary>
        /// <param name="birthdaysPerMonth">Eine Liste mit der Anzahl der Geburtstage pro Monat.</param>
        /// <returns>Ein ScottPlot.Plot-Objekt, das das Diagramm darstellt.</returns>
        public Plot CreateBirthdayPlot()
        {
            string[] labels = { "Jan", "Feb", "Mär", "Apr", "Mai", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dez" };
            var birthdaysPerMonth = GetBirthdaysPerMonth();

            double[] values = new double[12];
            for (int i = 0; i < 12; i++)
            {
                values[i] = birthdaysPerMonth[i];
            }

            var plt = new Plot();
            plt.Title("Geburtstage pro Monat");
            plt.XLabel("Monat");
            plt.YLabel("Anzahl der Geburtstage");

            var barPlot = plt.Add.Bars(values);

            var j = 0;
            foreach (var bar in barPlot.Bars)
            {
                bar.Label = labels[j] + ": " + bar.Value.ToString();
                j++;
            }

            barPlot.ValueLabelStyle.Bold = true;
            barPlot.ValueLabelStyle.FontSize = 12;

            double[] tickPositions = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            plt.Axes.Bottom.SetTicks(tickPositions, labels);

            plt.Axes.SetLimitsX(left: 0, right: 13);
            plt.Axes.Margins(bottom: 0.2);

            return plt;
        }

        public List<int> GetBirthdaysPerMonth()
        {
            var birthdaysPerMonth = new List<int>(new int[12]); // Liste mit 12 Nullen initialisieren
            var _personService = new PersonService();
            var persons = _personService.Get();

            foreach (var person in persons)
            {
                var month = person.DateOfBirth.Month;
                birthdaysPerMonth[month - 1]++;
            }

            return birthdaysPerMonth;
        }

    }
}

