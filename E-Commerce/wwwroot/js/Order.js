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
                data: null,
                title: 'Actions',
                className: 'text-center',
                render: function (data) {
                    const editLink = `<a class="mx-4" href="${EditURL + data.id}">
                                        <i class="bi bi-pencil-square fs-4" style="cursor: pointer"></i>
                                      </a>`;
                    return editLink;
                },
                orderable: false,
                searchable: false
            },
            {
                data: null,
                title: 'Invoice',
                className: 'text-center',
                render: function (data) {
                    if (data.invoiceNumber) {
                        return `<div class="btn-group d-flex justify-content-center" role="group">
                        <a class="mx-4 fs-5 text-decoration-none" title="Download Invoice"
                        href="/customer/Invoice/DownloadInvoice?OrderID=${data.id}">
                            <i class="bi bi-download"></i>
                        </a>

                        <a class="mx-4 fs-5 text-decoration-none" target="_blank" title="View Invoice"
                        href="/customer/Invoice/GenerateInvoice?OrderID=${data.id}">
                            <i class="bi bi-eye"></i>
                        </a>
                               `;
                    }
                    return '';
                },
                orderable: false,
                searchable: false,
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
