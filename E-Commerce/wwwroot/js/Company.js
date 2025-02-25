document.addEventListener("DOMContentLoaded", function () {
    LoadDataTables();
    const searchInput = document.querySelector(".dt-search input");
    if (searchInput) {
        searchInput.placeholder = "Search";
    }
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
            { data: 'name', title: 'Company Name', orderable: true },
            { data: 'streetAddress', title: 'Street Address' },
            { data: 'city', title: 'City' },
            { data: 'state', title: 'State' },
            { data: 'postalCode', title: 'Postal Code' },
            { data: 'phoneNumber', title: 'Phone Number' },
            {
                data: 'id',
                title: 'Actions',
                render: function (d) {
                    return `
                    	<div class="flex justify-between text-center">
							<a class="mx-4 " href="${EditURL + d}">
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="size-6">
                                <path stroke-linecap="round" stroke-linejoin="round" d="m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L10.582 16.07a4.5 4.5 0 0 1-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 0 1 1.13-1.897l8.932-8.931Zm0 0L19.5 7.125M18 14v4.75A2.25 2.25 0 0 1 15.75 21H5.25A2.25 2.25 0 0 1 3 18.75V8.25A2.25 2.25 0 0 1 5.25 6H10" /></svg>
							</a>


                            <button class="text-red-500 btnDelete" data-url='${DeleteURL + d}'>
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="size-6">
                                <path stroke-linecap="round" stroke-linejoin="round" d="m14.74 9-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0" /></svg>
                            </button>

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
        scrollCollapse: true,
        scrollY: '80vh',
        drawCallback: () => {
            // This function is triggered after every table redraw (e.g., after data is loaded)
            const deleteButtons = document.querySelectorAll('.btnDelete');
            deleteButtons.forEach(button => {
                button.addEventListener('click', (e) => {
                    e.preventDefault();
                    const URL = e.currentTarget.dataset.url;
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
                toast.success(data.message);
                ReloadTable('.tblData');
            });

    });
}

const ReloadTable = function (selector) {
    DT.ajax.reload();
}
