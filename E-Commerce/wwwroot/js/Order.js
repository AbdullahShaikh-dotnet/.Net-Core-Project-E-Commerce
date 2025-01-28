document.addEventListener("DOMContentLoaded", function () {
    LoadDataTables();
})

let DT;

const LoadDataTables = function () {
    const params = new URLSearchParams(window.location.search);
    const statusValue = params.get('status');


    const tableElement = document.querySelector('.tblData');
    const RootPath = tableElement.dataset?.root;
    const GetURL = `${RootPath}${tableElement.dataset?.geturl}?Status=${statusValue}`;
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
            { data: 'orderTotal', title: 'Order Total' },
            {
                data: 'id',
                title: 'Actions',
                render: function (d) {
                    return `
                    	<div class="btn-group d-flex justify-content-center" role="group">
							<a class="mx-4 " href="${EditURL + d}">
								<i class="bi bi-pencil-square fs-4" style="cursor: pointer"></i>
							</a>

                            <a class="mx-4 text-decoration-none" href="/customer/Invoice/GenerateInvoice?OrderID=${d}"> 
                            <i class="bi bi-download"></i>
                            Invoice
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

const ReloadTable = function (selector) {
    DT.ajax.reload();
}
