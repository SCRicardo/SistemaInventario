using SistemaInventario.Modelos.Especificaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio.IRepositorio
{
    public interface IRepositorio<T>where T:class
    {
        Task<T> obtener(int id);  //Busqueda directamente por id
        Task<IEnumerable<T>> ObtenerTodos(
            Expression<Func<T, bool>> filtro = null, //Sirve para ver si viene o no informacion
            Func<IQueryable<T>,IOrderedQueryable<T>>orderBy=null, //Sirve para Ordenar
            string incluirPropiedades=null,
            bool isTracking=true  //Sirve para reservar un pequeño espacio en memoria 
            );
        PagesList<T> ObtenerTodosPaginado(Parametros parametros,
            Expression<Func<T, bool>> filtro = null, //Sirve para ver si viene o no informacion
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, //Sirve para Ordenar
            string incluirPropiedades = null,
            bool isTracking = true  //Sirve para reservar un pequeño espacio en memoria }
            );

        Task<T> obtenerPrimero(
            Expression<Func<T, bool>> filtro = null,  //Sirve para ver si viene o no informacion
            string incluirPropiedades = null,
            bool isTracking = true  //Sirve para reservar un pequeño espacio en memoria 
            );

        Task Agregar(T entidad);
        void Remover(T entidad);
        void RemoverRango(IEnumerable<T> entidad);
    }
}
