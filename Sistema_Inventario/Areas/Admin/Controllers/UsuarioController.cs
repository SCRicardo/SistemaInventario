using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Inventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Utilidades;

namespace Sistema_Inventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]
    public class UsuarioController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly ApplicationDbContext _db;

        public UsuarioController(IUnidadTrabajo unidadTrabajo, ApplicationDbContext db)
        {
            _unidadTrabajo = unidadTrabajo;
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region APIS
        [HttpGet]
        public async Task<IActionResult> obtenerTodos()
        {
            var usuarioLista = await _unidadTrabajo.UsuarioAplicacion.ObtenerTodos();
            var userRole=await _db.UserRoles.ToListAsync();
            var roles=await _db.Roles.ToListAsync();
            foreach(var usuario in usuarioLista)
            {
                var roleId = userRole.FirstOrDefault(u=>u.UserId== usuario.Id).RoleId;
                usuario.Role = roles.FirstOrDefault(u=>u.Id==roleId).Name;
            }
            return Json(new {data=usuarioLista});
        }

        [HttpPost]
        public async Task<IActionResult> BloquearDesbloquear([FromBody]string id)
        {
            var usuario = await _unidadTrabajo.UsuarioAplicacion.obtenerPrimero(u=>u.Id==id);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Error de usuario." });
            }
            if (usuario.LockoutEnd != null && usuario.LockoutEnd>DateTime.Now)
            {
                //Usuario está bloqueado y para desbloquearlo tendremos que ponerle la fecha actual
                usuario.LockoutEnd= DateTime.Now;
            }
            else
            {
                //El usuario esta desbloqueado y se requiere bloquear sumandole 1000 años a la fecha actual
                usuario.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Operación exitosa." });
        }

        #endregion
    }
}
