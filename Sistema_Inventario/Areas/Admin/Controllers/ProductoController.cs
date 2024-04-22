using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;

namespace Sistema_Inventario.Areas.Admin.Controllers
{
    [Area("Admin")] //Le decimos a que Area pertecene
    public class ProductoController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        public ProductoController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo= unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult>Upsert(int? id)
        {
            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Categoria"),
                MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Marca")
            };
            if (id == null)
            {
                //Crear nuevo Producto
                return View(productoVM);
            }
            else
            {
                //Vamos a actualizar a producto
                productoVM.Producto = await _unidadTrabajo.Producto.obtener(id.GetValueOrDefault());
                if (productoVM.Producto == null)
                {
                    return NotFound();
                }
                return View(productoVM);
            }
        }

        
        
        #region API
        [HttpGet]
        public async Task<IActionResult> obtenerTodos()
        {
            var todos=await _unidadTrabajo.Producto.ObtenerTodos(incluirPropiedades:"Categoria,Marca");
            return Json(new {data=todos});  //data es el nombre que tiene que tener la tabla por defecto para crear el JSON
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var productoDB = await _unidadTrabajo.Producto.obtener(id);
            if (productoDB == null)
            {
                return Json(new { success = false, message = "Error al borrar Producto." });
            }
            _unidadTrabajo.Producto.Remover(productoDB);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Producto borrado exitósamente." });
        }


        [ActionName("ValidarNombre")]
        public async Task<IActionResult>ValidarNombre(string serie,int id = 0)
        {
            bool valor=false;
            var lista = await _unidadTrabajo.Producto.ObtenerTodos();
            if (id == 0)
            {
                valor=lista.Any(b=>b.NumeroSerie.ToLower().Trim()== serie.ToLower().Trim());
            }else
            {
                valor = lista.Any(b=>b.NumeroSerie.ToLower().Trim()== serie.ToLower().Trim() && b.Id!=id);
            }
            if (valor)
            {
                return Json(new {success=true});
            }
			return Json(new { success = false });
		}

        #endregion
    }
}
