let datatable;
$(document).ready(function () {
    loadDataTable();  //Sirve para leer la tabla
});

function loadDataTable() {
    datatable = $('#tblDatos').DataTable({
        //Seccion de ajax para el´pluggin
        "ajax": { "url": "/Admin/Usuario/obtenerTodos" },
        "columns": [
            { "data": "email"},
            { "data": "nombres" },
            { "data": "apellidos" },
            { "data": "phoneNumber" },
            { "data": "role" },
            {
                "data": {
                    id:"id",lockoutEnd:"lockoutEnd"
                },
                "render": function (data) {  //Alt + 96 para estos simbolos
                    let hoy = new Date().getTime();
                    let bloqueo = new Date(data.lockoutEnd).getTime();
                    if (bloqueo > hoy) {
                        //Usuario esta bloqueado
                        return ` 
                        <div class="text-center">
                            <a onclick=BloquearDesbloquear('${data.id}') class="btn btn-danger text-white" 
                            style="cursor:pointer;width:150px">
                            <i class="bi bi-unlock-fill"></i>
                            </a>
                            Desbloquear
                        </div>
                    `;
                    } else {
                        return ` 
                        <div class="text-center">
                            <a onclick=BloquearDesbloquear('${data.id}') class="btn btn-success text-white" 
                            style="cursor:pointer;width:150px">
                            <i class="bi bi-lock-fill"></i>
                            Bloquear
                            </a>
                        </div>
                    `;
                    }

                    
                }, "width": "20%"
            }
        ],
        language: {
            url: '//cdn.datatables.net/plug-ins/1.10.25/i18n/Spanish.json',
            search: "Búsqueda:",
            paginate: {
                previous: "Antes",
                next: "Después",
            },
            info: "Mostrando resultados del 1 al 2 de 2 resultados",
            lengthMenu:"Ver resultados"      
        },
    });
}

function BloquearDesbloquear(id) {
    $.ajax({
        type: "Post",
        url: '/Admin/Usuario/BloquearDesbloquear',
        data: JSON.stringify(id),
        contentType:"application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                datatable.ajax.reload();
            } else {
                toastr.error(data.message);
            }
        }

    });
}