using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Timers.Models;

namespace Timers.Services
{
    public class VuelosService
    {
        #region Vuelo service
        HttpClient cliente = new HttpClient
        {
            BaseAddress = new Uri("https://equipo9.sistemas19.com/")
        };

        //Obtener los vuelos ordenados por fecha.
        public async Task<List<Vuelos>> GetByDate()
        {
            List<Vuelos>? listaVuelos = null;

            var respuesta = await cliente.GetAsync("api/vuelos");

            if (respuesta.IsSuccessStatusCode)
            {
                var json = await respuesta.Content.ReadAsStringAsync();
                listaVuelos=JsonConvert.DeserializeObject<List<Vuelos>>(json);
            }

            return listaVuelos!=null?listaVuelos:new List<Vuelos>();
        }

        //Borrar un vuelo especificado.
        public async Task<bool> Delete(Vuelos v)
        {
            var response = await cliente.DeleteAsync("api/vuelos/"+v.Id);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errores = await response.Content.ReadAsStringAsync();
                NotificarErrorJson(errores);
                return false;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NotificarError("No se encontró el vuelo que desea eliminar.");
            }
            return true;
        }

        //Actualizar un vuelo especificado.
        public async Task<bool> Update(Vuelos v)
        {
            //Validar

            var json = JsonConvert.SerializeObject(v);
            var response = await cliente.PutAsync("api/vuelos", new StringContent(json, Encoding.UTF8,
                "application/json"));
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errores = await response.Content.ReadAsStringAsync();
                NotificarErrorJson(errores);
                return false;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NotificarError("No se encontro el vuelo que desea actualizar.");
            }
            return true;
        }

        public event Action<List<string>>? Error;

        void NotificarError(string mensaje)
        {
            Error?.Invoke(new List<string> { mensaje });
        }
        private void NotificarErrorJson(string json)
        {
            List<string>? listaErrores = JsonConvert.DeserializeObject<List<string>>(json);
            if (listaErrores != null)
            {
                Error?.Invoke(listaErrores);
            }
        }
        #endregion
    }
}
