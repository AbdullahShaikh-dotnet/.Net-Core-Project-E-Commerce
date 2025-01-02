document.addEventListener("DOMContentLoaded", function () {
    LoadDataTables();
})

let DT;

const LoadDataTables = function () {
    const tableElement = document.querySelector('.tblData');
    const url = tableElement.dataset?.url;
    DT = new DataTable(tableElement, {
        ajax: { url },
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
            { data: 'title', title: 'title' },
            {
                data: 'isbn', title: 'isbn', render: function (data) {
                    return `<a class="link-danger" target="_blank" href="https://www.isbnsearch.org/isbn/${data}">${data}</a>`;
                }
            },
            { data: 'price', title: 'price' },
            { data: 'author', title: 'author' },
            { data: 'category.name', title: 'category name' },
            {
                title: 'Image',
                data: 'imageURL',
                render: function (d) {
                    return d ? `<a href="${d}" target="_blank">
                                    <img src=${d} width="auto" height="30" />
                                </a>` : '';
                },
                className: 'text-center',
                orderable: false,
                searchable: false
            },
            {
                data: 'id',
                title: 'Actions',
                render: function (d) {
                    return `
                    	<div class="btn-group d-flex justify-content-center" role="group">
							<a class="mx-4 " href="/admin/product/Upsert?id=${d}">
								<i class="bi bi-pencil-square fs-4" style="cursor: pointer"></i>
							</a>

                            <i class="text-danger mx-4 bi bi-trash fs-4 btnDelete"
                            data-url='/admin/product/Delete?id=${d}' style="cursor: pointer"></i>
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
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
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
