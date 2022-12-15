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
        public List<Vuelos> vuelosNuevos { get; set; } = new List<Vuelos>();
        private readonly VuelosService vuelosService = new VuelosService();
        private DispatcherTimer dispatcherTimer = new();
        private string error = "";
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
            Bajar();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(15);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
        }
        #endregion

        #region Métodos


        private async void Bajar()
        {
            vuelosNuevos.Clear();
            var vuelos = await vuelosService.GetByDate();
            vuelos.ForEach(v => vuelosNuevos.Add(v));
            if (vuelosNuevos.Count != Vuelos.Count)
            {
                CargarVuelos();
            }
            for (int i = 0; i < Vuelos.Count; i++)
            {
                if (Vuelos[i].Estado != vuelosNuevos[i].Estado || Vuelos[i].HorarioSalida != vuelosNuevos[i].HorarioSalida || Vuelos[i].Destino != vuelosNuevos[i].Destino || Vuelos[i].PuertaSalida != vuelosNuevos[i].PuertaSalida)
                {
                    CargarVuelos();
                }
            }
        }
        private async void ActualizarVuelo(Vuelos v)
        {
            if (v != null)
            {
                if (await vuelosService.Update(v))
                {
                    Error = "";
                }
            }
        }
        private async void EliminarVuelo(Vuelos v)
        {
            if (v != null)
            {
                if (await vuelosService.Delete(v))
                {
                    Error = "";
                }
            }
        }
        public TimeSpan Diferencia(DateTime fecha)
        {
            return fecha - DateTime.Now;
        }
        private void CargarVuelos()
        {
            Vuelos.Clear();
            vuelosNuevos.ForEach
                (v => Vuelos.Add(v));
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
            Bajar();
            for (int i = 0; i < Vuelos.Count; i++)
            {
                if ((int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) < 4 && (int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) > 3)
                {
                    if (Vuelos[i].Estado != "En camino")
                    {
                        Vuelos[i].Estado = "En camino";
                        ActualizarVuelo(Vuelos[i]);
                        CargarVuelos();
                        break;
                    }
                }
                else if ((int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) <= 3 && (int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) > 2)
                {
                    if (Vuelos[i].Estado != "Aterrizo")
                    {
                        Vuelos[i].Estado = "Aterrizo";
                        ActualizarVuelo(Vuelos[i]);
                        CargarVuelos();
                        break;
                    }
                }
                else if ((int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) <= 2 && (int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) > 1)
                {
                    if (Vuelos[i].Estado != "Abordando")
                    {
                        Vuelos[i].Estado = "Abordando";
                        ActualizarVuelo(Vuelos[i]);
                        CargarVuelos();
                        break;
                    }
                }
                else if ((int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) <= 1 && (int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) > 0)
                {
                    if (Vuelos[i].Estado != "Despego")
                    {
                        Vuelos[i].Estado = "Despego";
                        ActualizarVuelo(Vuelos[i]);
                        CargarVuelos();
                        break;
                    }
                }
                else if ((int)(Diferencia(Vuelos[i].HorarioSalida).TotalMinutes) <= 0 || Vuelos[i].Estado=="Cancelado")
                {
                    EliminarVuelo(Vuelos[i]);
                    CargarVuelos();
                    break;
                }

            }
        }
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion   
    }
}
