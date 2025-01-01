
document.addEventListener("DOMContentLoaded", function () {
    LoadDataTables();
})


const LoadDataTables = function () {
    $('#tblData').DataTable({
        ajax: { url: '/admin/product/getall' },
        columns: [
            {
                title: 'No.',
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
                    return `<a href="https://www.isbnsearch.org/isbn/${data}">${data}</a>`;
                }
            },
            { data: 'price', title: 'price' },
            { data: 'author', title: 'author' },
            { data: 'category.name', title: 'category name' },
            {
                data: 'id',
                title: 'Actions',
                render: function (d) {
                    return `
                    	<div class="btn-group d-flex justify-content-center" role="group">
							<a class="mx-4 " href="/admin/product/Upsert?id=${d}">
								<i class="bi bi-pencil-square fs-4" style="cursor: pointer"></i>
							</a>

							<a class="text-danger mx-4" href="/admin/product/Delete?id=${d}">
								<i class="bi bi-trash fs-4" style="cursor: pointer"></i>
							</a>
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
    });
}
