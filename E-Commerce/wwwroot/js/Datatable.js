document.addEventListener("DOMContentLoaded", function () {
    initializeDataTable('.tblData');
});

let DT;

// Generic function to initialize DataTable
const initializeDataTable = function (tableSelector) {
    const tableElement = document.querySelector(tableSelector);
    if (!tableElement) return;

    const RootPath = tableElement.dataset?.root;
    const GetURL = RootPath + tableElement.dataset?.geturl;
    const EditURL = RootPath + tableElement.dataset?.editurl;
    const DeleteURL = RootPath + tableElement.dataset?.deleteurl;
    const configKey = tableElement.dataset?.configKey;

    // Load configuration dynamically
    const tableConfig = getTableConfig(configKey);

    if (!tableConfig) {
        console.error(`No configuration found for key: ${configKey}`);
        return;
    }

    DT = new DataTable(tableElement, {
        ajax: { url: GetURL },
        columns: generateColumns(tableConfig.columns, EditURL, DeleteURL),
        select: true,
        processing: true,
        info: true,
        drawCallback: () => setupDeleteHandlers('.btnDelete')
    });
};

// Define reusable configurations for all tables
const getTableConfig = function (key) {
    const configs = {
        company: {
            columns: [
                { data: 'name', title: 'Name' },
                { data: 'streetAddress', title: 'Street Address' },
                { data: 'city', title: 'City' },
                { data: 'state', title: 'State' },
                { data: 'postalCode', title: 'Postal Code' },
                { data: 'phoneNumber', title: 'Phone Number' }
            ]
        },
        product: {
            columns: [
                { data: 'title', title: 'Title' },
                {
                    data: 'isbn', title: 'ISBN',
                    render: (data) => `<a class="link-danger" target="_blank" href="https://www.isbnsearch.org/isbn/${data}">${data}</a>`
                },
                { data: 'price', title: 'Price' },
                { data: 'author', title: 'Author' },
                { data: 'category.name', title: 'Category Name' },
                {
                    title: 'Image', data: 'imageURL',
                    render: (d) => d ? `<a href="${d}" target="_blank"><img src=${d} width="auto" height="30" /></a>` : '',
                    className: 'text-center',
                    orderable: false,
                    searchable: false
                }
            ]
        },
        orders: {
            columns: [
                { data: 'orderId', title: 'Order ID' },
                { data: 'customerName', title: 'Customer Name' },
                { data: 'orderDate', title: 'Order Date' },
                { data: 'status', title: 'Status' },
                { data: 'totalAmount', title: 'Total Amount' }
            ]
        }
        // Add more configurations as needed
    };

    return configs[key] || null;
};

// Generate columns dynamically
const generateColumns = function (columns, EditURL, DeleteURL) {
    const commonColumns = [
        {
            title: 'Sr. No.',
            data: null,
            render: (data, type, row, meta) => meta.row + 1,
            className: 'text-center',
            orderable: false,
            searchable: false
        },
        {
            data: 'id',
            title: 'Actions',
            render: (d) => createActionButtons(d, EditURL, DeleteURL),
            orderable: false,
            searchable: false
        }
    ];

    return [...columns, ...commonColumns];
};

// Create action buttons HTML
const createActionButtons = function (id, EditURL, DeleteURL) {
    return `
        <div class="btn-group d-flex justify-content-center" role="group">
            <a class="mx-4" href="${EditURL + id}">
                <i class="bi bi-pencil-square fs-4" style="cursor: pointer"></i>
            </a>
            <i class="text-danger mx-4 bi bi-trash fs-4 btnDelete"
               data-url='${DeleteURL + id}' style="cursor: pointer"></i>
        </div>
    `;
};

// Set up delete button event handlers
const setupDeleteHandlers = function (selector) {
    document.querySelectorAll(selector).forEach(button => {
        button.addEventListener('click', (e) => {
            const URL = e.target.dataset.url;
            showDeleteConfirmation(URL);
        });
    });
};

// Delete confirmation dialog
const showDeleteConfirmation = function (url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (!result.isConfirmed) return;

        fetch(url, { method: "POST" })
            .then(response => response.json())
            .then(data => {
                toastr.success(data.message);
                reloadTable();
            });
    });
};

// Reload DataTable
const reloadTable = function () {
    DT.ajax.reload();
};
