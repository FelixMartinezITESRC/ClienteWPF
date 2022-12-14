using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;
using Timers.Models;
using Timers.Services;

namespace Timers.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Propiedades_y_campos
        public ObservableCollection<Vuelos> Vuelos { get; set; } = new ObservableCollection<Vuelos>();
        private readonly VuelosService vuelosService = new VuelosService();
        private DispatcherTimer dispatcherTimer = new();
        private string error="";
        public string Error
        {
            get { return error; }
            set { error = value; ActualizarPropiedad(nameof(Error)); }
        }

        #endregion

        #region Método constructor
        public MainViewModel()
        {
            vuelosService.Error += VuelosService_Error;
            CargarVuelos();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(15);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
        }
        #endregion

        #region Métodos
        private async void ActualizarVuelo(Vuelos v)
        {
            if (v != null)
            {
                if (await vuelosService.Update(v))
                {
                    
                }           
            }
        }
        private async void EliminarVuelo(Vuelos v)
        {
            if (v != null)
            {
                if (await vuelosService.Delete(v))
                {
                    
                }
            }
        }
        public TimeSpan Diferencia(DateTime fecha)
        {
            return fecha - DateTime.Now;
        }
        private async void CargarVuelos()
        {
            Vuelos.Clear();
            var vuelos = await vuelosService.GetByDate();
            vuelos.ForEach(v => Vuelos.Add(v));
        }
        public void ActualizarPropiedad(string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Suscripciones a eventos
        private void VuelosService_Error(List<string> obj)
        {
            Error = string.Join("\n", obj);
        }
        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {

            for (int i = 0; i < Vuelos.Count; i++)
            {
                if (Vuelos[i].HorarioSalida.Date == DateTime.Now.Date)
                {
                    if ((int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) < 4 && (int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) > 3)
                    {
                        if (Vuelos[i].Estado!="En camino")
                        {
                            Vuelos[i].Estado = "En camino";
                            ActualizarVuelo(Vuelos[i]);
                        }                      
                    }
                    else if ((int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) <= 3 && (int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) > 2)
                    {
                        if (Vuelos[i].Estado != "Aterrizo")
                        {
                            Vuelos[i].Estado = "Aterrizo";
                            ActualizarVuelo(Vuelos[i]);
                        }
                    }
                    else if ((int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) <= 2 && (int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) > 1)
                    {
                        if (Vuelos[i].Estado != "Abordando")
                        {
                            Vuelos[i].Estado = "Abordando";
                            ActualizarVuelo(Vuelos[i]);
                        }
                    }
                    else if ((int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) <= 1 && (int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) > -2)
                    {
                        if (Vuelos[i].Estado != "Despego")
                        {
                            Vuelos[i].Estado = "Despego";
                            ActualizarVuelo(Vuelos[i]);
                        }
                    }
                    else if ((int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) <= -2)
                    {
                        EliminarVuelo(Vuelos[i]);
                        Vuelos.RemoveAt(i);
                    }
                }
            }
            CargarVuelos();
        }
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion   
    }
}
