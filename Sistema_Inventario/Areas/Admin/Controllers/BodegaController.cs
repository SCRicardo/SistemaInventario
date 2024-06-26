﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace Sistema_Inventario.Areas.Admin.Controllers
{
    [Area("Admin")] //Le decimos a que Area pertecene
    [Authorize(Roles =DS.Role_Admin)]
    public class BodegaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        public BodegaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo= unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult>Upsert(int? id)
        {
            Bodega bodega=new Bodega();
            if (id == null)
            {
                //Crear nueva bodega
                bodega.Estado = true;
                return View(bodega);
            }
            bodega = await _unidadTrabajo.Bodega.obtener(id.GetValueOrDefault()); //Nos aseguramos que la información llegue correctamente
            if (bodega == null)
            {
                return NotFound();
            }
            return View(bodega);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Evita que se pueda clonar 
        public async Task<IActionResult>Upsert(Bodega bodega)
        {
            if (ModelState.IsValid)
            {
                if (bodega.Id == 0)
                {
                    await _unidadTrabajo.Bodega.Agregar(bodega);
                    TempData[DS.Exitosa] = "Bodega creada exitósamente";
                }
                else
                {
                    _unidadTrabajo.Bodega.Actualizar(bodega);
					TempData[DS.Exitosa] = "Bodega actualizada exitósamente";
				}
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al guardar la bodega";
            return View(bodega);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var bodegaDB = await _unidadTrabajo.Bodega.obtener(id);
            if (bodegaDB == null)
            {
                return Json(new { success = false, message = "Error al borrar Bodega." });
            }
            _unidadTrabajo.Bodega.Remover(bodegaDB);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Bodega borrada exitósamente." });
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> obtenerTodos()
        {
            var todos=await _unidadTrabajo.Bodega.ObtenerTodos();
            return Json(new {data=todos});  //data es el nombre que tiene que tener la tabla por defecto para crear el JSON
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult>ValidarNombre(string nombre,int id = 0)
        {
            bool valor=false;
            var lista = await _unidadTrabajo.Bodega.ObtenerTodos();
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
