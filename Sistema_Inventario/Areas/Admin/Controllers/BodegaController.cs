using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace Sistema_Inventario.Areas.Admin.Controllers
{
    [Area("Admin")] //Le decimos a que Area pertecene
    public class CategoriaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        public CategoriaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo= unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult>Upsert(int? id)
        {
            Categoria categoria=new Categoria();
            if (id == null)
            {
                //Crear nueva bodega
                categoria.Estado = true;
                return View(categoria);
            }
            categoria = await _unidadTrabajo.Categoria.obtener(id.GetValueOrDefault()); //Nos aseguramos que la información llegue correctamente
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Evita que se pueda clonar 
        public async Task<IActionResult>Upsert(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                if (categoria.Id == 0)
                {
                    await _unidadTrabajo.Categoria.Agregar(categoria);
                    TempData[DS.Exitosa] = "Categoria creada exitósamente";
                }
                else
                {
                    _unidadTrabajo.Categoria.Actualizar(categoria);
					TempData[DS.Exitosa] = "Categoria actualizada exitósamente";
				}
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al guardar la categoria";
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var categoriaDB = await _unidadTrabajo.Categoria.obtener(id);
            if (categoriaDB == null)
            {
                return Json(new { success = false, message = "Error al borrar Categoria." });
            }
            _unidadTrabajo.Categoria.Remover(categoriaDB);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Categoria borrada exitósamente." });
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> obtenerTodos()
        {
            var todos=await _unidadTrabajo.Categoria.ObtenerTodos();
            return Json(new {data=todos});  //data es el nombre que tiene que tener la tabla por defecto para crear el JSON
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult>ValidarNombre(string nombre,int id = 0)
        {
            bool valor=false;
            var lista = await _unidadTrabajo.Categoria.ObtenerTodos();
            if (id == 0)
            {
                valor=lista.Any(b=>b.Nombre.ToLower().Trim()==nombre.ToLower().Trim());
            }else
            {
                valor = lista.Any(b=>b.Nombre.ToLower().Trim()==nombre.ToLower().Trim() && b.Id!=id);
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
