document.addEventListener("DOMContentLoaded", function () {
    LoadGridAsync();
    showLoader_Cards();
    LoadDataTables();
    const searchInput = document.querySelector(".dt-search input");
    if (searchInput) {
        searchInput.placeholder = "Search";
    }
    totalCountOrderCategory_Wise();
    setActiveFilter();
})

let DT;

const LoadDataTables = function (status = 'all') {
    const statusValue = location.hash.split('#')[1] || 'all';

    // Destroy existing DataTable if it exists
    if (DT) {
        DT.destroy();
    }

    const tableElement = document.querySelector('.tblData');
    const RootPath = tableElement.dataset?.root;
    const GetURL = `${RootPath}${tableElement.dataset?.geturl}?Status=${statusValue}`;
    const EditURL = RootPath + tableElement.dataset?.editurl;
    const DeleteURL = RootPath + tableElement.dataset?.deleteurl;

    DT = new DataTable(tableElement, {
        ajax: {
            url: GetURL,
        },
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
                    const editLink = `
                        <div class="flex justify-center text-center">
                            <a class="mx-4 " href="${EditURL + data.id}">
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="size-5">
                                <path stroke-linecap="round" stroke-linejoin="round" d="m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L10.582 16.07a4.5 4.5 0 0 1-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 0 1 1.13-1.897l8.932-8.931Zm0 0L19.5 7.125M18 14v4.75A2.25 2.25 0 0 1 15.75 21H5.25A2.25 2.25 0 0 1 3 18.75V8.25A2.25 2.25 0 0 1 5.25 6H10" /></svg>
                            </a>
                        </div>`;
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
                        return `
                        <div class="flex justify-between text-center">
                            <a class="mx-4 " href="/customer/Invoice/DownloadInvoice?OrderID=${data.id}">
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="size-5">
                                <path stroke-linecap="round" stroke-linejoin="round" d="M3 16.5v2.25A2.25 2.25 0 0 0 5.25 21h13.5A2.25 2.25 0 0 0 21 18.75V16.5M16.5 12 12 16.5m0 0L7.5 12m4.5 4.5V3" /></svg>
                            </a>

                            <a class="mx-4 " href="/customer/Invoice/GenerateInvoice?OrderID=${data.id}" target="_blank">
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="size-5">
                                  <path stroke-linecap="round" stroke-linejoin="round" d="M2.036 12.322a1.012 1.012 0 0 1 0-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178Z" />
                                  <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z" />
                                </svg>
                            </a>
                        </div>
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
        scrollCollapse: true,
        scrollY: '70vh',
    });
}

const ReloadTable = function (selector) {
    LoadDataTables(); // Completely reinitialize the table instead of just reloading
    totalCountOrderCategory_Wise(); // Also refresh the counts
}

const totalCountOrderCategory_Wise = () => {
    fetch("/Admin/Order/GetOrderCategoryCount")
        .then(res => res.json())
        .then(data => {
            Object.keys(data).forEach(el => {
                const elem = document.getElementById(el);
                if (elem) elem.innerHTML = data[el];
            });
        })
        .catch(err => console.error("Error loading order counts:", err));
};

const showLoader_Cards = () => {
    document.querySelectorAll(".countCards").forEach(el => {
        const loaderSVG = `
            <svg class="mr-3 -ml-1 size-5 animate-spin text-gray-500" xmlns="http://www.w3.org/2000/svg" fill="none"
            viewBox="0 0 24 24"><circle class="opacity-10" cx="12" cy="12" r="10"
            stroke="currentColor" stroke-width="4"></circle><path class="opacity-75"
            fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0
            12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
            </path></svg>`;
        document.getElementById(el.id).innerHTML = loaderSVG;
    });
};

const LoadGridAsync = () => {
    window.addEventListener('hashchange', () => {
        ReloadTable(); // Call ReloadTable which will reinitialize everything
        setActiveFilter();
    });
};

function setActiveFilter() {

    const status = (window.location.hash.substring(1) || 'all').toLowerCase();
    const filterButtons = document.querySelectorAll('.tabLinks');

    filterButtons.forEach(link => {
        link.classList.remove(
            'active',
            'border-gray-800', 'text-gray-800',
            'border-blue-600', 'text-blue-600',
            'border-amber-500', 'text-amber-500',
            'border-green-600', 'text-green-600',
            'border-purple-600', 'text-purple-600',
            'border-red-600', 'text-red-600',
            'border-slate-600', 'text-slate-600'
        );

        link.classList.add(
            'border-transparent',
            'text-gray-500',
            'hover:text-gray-700',
            'hover:border-gray-300'
        );
    });

    // Find the active button
    const activeButton = document.getElementById(`filter-${status}`)

    if (!activeButton) return;

    const activeColor = activeButton.dataset?.activecolor;
    activeButton.classList.remove(
        'border-transparent',
        'text-gray-500',
        'hover:text-gray-700',
        'hover:border-gray-300'
    );

    const Border = activeColor.split(' ')[0];
    const Text = activeColor.split(' ')[1];

    activeButton.classList.add(
        'active',
        Border,
        Text
    );

}
