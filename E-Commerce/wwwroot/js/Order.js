document.addEventListener("DOMContentLoaded", function () {
    LoadDataTables();
})

let DT;

const LoadDataTables = function () {
    const tableElement = document.querySelector('.tblData');
    const RootPath = tableElement.dataset?.root;
    const GetURL = RootPath + tableElement.dataset?.geturl;
    const EditURL = RootPath + tableElement.dataset?.editurl;
    const DeleteURL = RootPath + tableElement.dataset?.deleteurl;
    DT = new DataTable(tableElement, {
        ajax: { url: GetURL },
        columns: [
            {
                title: 'Sr. No.',
                data: null,
                render: function (data, type, row, meta) {
                    return meta.row + 1;
                },
                className: 'text-center',
                orderable: false,
                searchable: false
            },
            { data: 'id', title: 'Order ID' },
            { data: 'name', title: 'User Name' },
            { data: 'phoneNumber', title: 'Phone Number' },
            { data: '_ApplicationUser.email', title: 'Email' },
            { data: 'orderStatus', title: 'Order Status' },
            { data: 'orderTotal', title:'Order Total' },
            {
                data: 'id',
                title: 'Actions',
                render: function (d) {
                    return `
                    	<div class="btn-group d-flex justify-content-center" role="group">
							<a class="mx-4 " href="${EditURL + d}">
								<i class="bi bi-pencil-square fs-4" style="cursor: pointer"></i>
							</a>

                            <i class="text-danger mx-4 bi bi-trash fs-4 btnDelete d-none"
                            data-url='${DeleteURL + d}' style="cursor: pointer"></i>
						</div>
                    `;
                },
                orderable: false,
                searchable: false
            },
        ],
        select: true,
        processing: true,
        info: true,
        drawCallback: () => {
            // This function is triggered after every table redraw (e.g., after data is loaded)
            const deleteButtons = document.querySelectorAll('.btnDelete');
            deleteButtons.forEach(button => {
                button.addEventListener('click', (e) => {
                    const URL = e.target.dataset.url
                    DeleteConfirmation(URL);
                });
            });
        }
    });
}

const DeleteConfirmation = function (url) {
    Swal.fire({
        title: "Are you sure ?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (!result.isConfirmed) return;

        fetch(url, {
            method: "POST",
        }).then(response => response.json())
            .then(data => {
                toastr.success(data.message)
                ReloadTable('.tblData');
            });

    });
}

const ReloadTable = function (selector) {
    DT.ajax.reload();
}
