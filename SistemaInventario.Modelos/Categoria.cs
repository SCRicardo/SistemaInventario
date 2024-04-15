using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo Nombre es requerido")]
        [MaxLength(60, ErrorMessage = "El campo Nombre no es más de 60 caracteres")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo Descripcion es requerido")]
        [MaxLength(100, ErrorMessage = "El campo Descripcion no es más de 100 caracteres")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "El campo Estado es requerido")]
        public bool Estado { get; set; }
    }
}
