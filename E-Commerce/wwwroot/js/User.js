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
    const LockUnlockURL = RootPath + tableElement.dataset?.lockunlockurl;
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
            { data: 'name', title: 'User Name', orderable: true },
            { data: 'email', title: 'Email' },
            { data: 'phoneNumber', title: 'Phone Number' },
            { data: 'role', title: 'User Role' },
            { data: 'company.name', title: 'Company Name' },
            {
                data: { id: "id", lockoutEnd: "lockoutEnd" },
                title: 'Actions',
                "render": function (d) {

                    let today = new Date().getTime();
                    let lockout = new Date(d.lockoutEnd).getTime();

                    if (lockout > today) {
                        return `
                    	<div class="flex justify-around text-center">
							<a class="flex font-light item-center text-red-500 border border-red-200 p-1 rounded btnLockUnlock" data-url='${LockUnlockURL}' data-id='${d.id}'>
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="0.7" stroke="currentColor" class="size-5">
                                  <path stroke-linecap="round" stroke-linejoin="round" d="M16.5 10.5V6.75a4.5 4.5 0 1 0-9 0v3.75m-.75 11.25h10.5a2.25 2.25 0 0 0 2.25-2.25v-6.75a2.25 2.25 0 0 0-2.25-2.25H6.75a2.25 2.25 0 0 0-2.25 2.25v6.75a2.25 2.25 0 0 0 2.25 2.25Z" />
                                </svg>
                                <span class="px-1">Lock</span>
							</a>


                            <button class="flex item-center btnDelete" data-url='${LockUnlockURL + d.id}'>
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="0.7" stroke="currentColor" class="size-6">
                                  <path stroke-linecap="round" stroke-linejoin="round" d="M9 12.75 11.25 15 15 9.75m-3-7.036A11.959 11.959 0 0 1 3.598 6 11.99 11.99 0 0 0 3 9.749c0 5.592 3.824 10.29 9 11.623 5.176-1.332 9-6.03 9-11.622 0-1.31-.21-2.571-.598-3.751h-.152c-3.196 0-6.1-1.248-8.25-3.285Z" />
                                </svg>
                                <span class="px-1">Permission</span>
                            </button>

						</div>
                    `
                    } else {
                        return `
                    	<div class="flex justify-around item-center text-center">
							<button class="flex font-light item-center text-emerald-600 border border-emerald-400 p-1 rounded btnLockUnlock" data-url='${LockUnlockURL}' data-id='${d.id}'>
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="0.7" stroke="currentColor" class="size-5">
                                  <path stroke-linecap="round" stroke-linejoin="round" d="M13.5 10.5V6.75a4.5 4.5 0 1 1 9 0v3.75M3.75 21.75h10.5a2.25 2.25 0 0 0 2.25-2.25v-6.75a2.25 2.25 0 0 0-2.25-2.25H3.75a2.25 2.25 0 0 0-2.25 2.25v6.75a2.25 2.25 0 0 0 2.25 2.25Z" />
                                </svg>
                                <span class="px-1">Unlock</span>
							</button>


                            <button class="btnDelete flex item-center" data-url='${LockUnlockURL + d.id}'>
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="0.7" stroke="currentColor" class="size-6">
                                  <path stroke-linecap="round" stroke-linejoin="round" d="M9 12.75 11.25 15 15 9.75m-3-7.036A11.959 11.959 0 0 1 3.598 6 11.99 11.99 0 0 0 3 9.749c0 5.592 3.824 10.29 9 11.623 5.176-1.332 9-6.03 9-11.622 0-1.31-.21-2.571-.598-3.751h-.152c-3.196 0-6.1-1.248-8.25-3.285Z" />
                                </svg>
                                <span class="px-1">Permission</span>
                            </button>

						</div>
                    `
                    }


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
            const LockUnlockButton = document.querySelectorAll('.btnLockUnlock');
            LockUnlockButton.forEach(button => {
                button.addEventListener('click', (e) => {
                    e.preventDefault();
                    const URL = e.currentTarget.dataset.url;
                    const ID = e.currentTarget.dataset.id;
                    const messageText = e.currentTarget.innerText.toLowerCase() == "unlock" ? "Lock" : "Unlock";
                    LockUnlockConfirmation(URL, ID, messageText);
                });
            });
        }
    });
}

const LockUnlockConfirmation = function (url, id, messageText) {
    Swal.fire({
        title: "Confirmation",
        text: `Are you Sure you want to ${messageText}!`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: `Yes, ${messageText} it!`
    }).then((result) => {
        if (!result.isConfirmed) return;

        fetch(url, {
            method: "POST",
            body: JSON.stringify(id),
            headers: {
                "Content-Type": "application/json"
            },
        }).then(response => response.json())
            .then(data => {
                if (data.success)
                    toast.success(data.message, 5);
                else
                    toast.error(data.message, 5);
                ReloadTable('.tblData');
            });

    });
}

const ReloadTable = function (selector) {
    DT.ajax.reload();
}
