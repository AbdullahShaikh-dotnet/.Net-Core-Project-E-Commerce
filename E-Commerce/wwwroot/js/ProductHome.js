
function controlFromInput(fromSlider, fromInput, toInput, controlSlider) {
    const [from, to] = getParsed(fromInput, toInput);
    fillSlider(fromInput, toInput, '#C6C6C6', '#1e293b', controlSlider);
    if (from > to) {
        fromSlider.value = to;
        fromInput.value = to;
    } else {
        fromSlider.value = from;
    }
}

function controlToInput(toSlider, fromInput, toInput, controlSlider) {
    const [from, to] = getParsed(fromInput, toInput);
    fillSlider(fromInput, toInput, '#C6C6C6', '#1e293b', controlSlider);
    setToggleAccessible(toInput);
    if (from <= to) {
        toSlider.value = to;
        toInput.value = to;
    } else {
        toInput.value = from;
    }
}

function controlFromSlider(fromSlider, toSlider, fromInput) {
    const [from, to] = getParsed(fromSlider, toSlider);
    fillSlider(fromSlider, toSlider, '#C6C6C6', '#1e293b', toSlider);
    if (from > to) {
        fromSlider.value = to;
        fromInput.value = to;
    } else {
        fromInput.value = from;
    }
}

function controlToSlider(fromSlider, toSlider, toInput) {
    const [from, to] = getParsed(fromSlider, toSlider);
    fillSlider(fromSlider, toSlider, '#C6C6C6', '#1e293b', toSlider);
    setToggleAccessible(toSlider);
    if (from <= to) {
        toSlider.value = to;
        toInput.value = to;
    } else {
        toInput.value = from;
        toSlider.value = from;
    }
}

function getParsed(currentFrom, currentTo) {
    const from = parseInt(currentFrom.value, 10);
    const to = parseInt(currentTo.value, 10);
    return [from, to];
}

function fillSlider(from, to, sliderColor, rangeColor, controlSlider) {
    const rangeDistance = to.max - to.min;
    const fromPosition = from.value - to.min;
    const toPosition = to.value - to.min;
    controlSlider.style.background = `linear-gradient(
                      to right,
                      ${sliderColor} 0%,
                      ${sliderColor} ${(fromPosition) / (rangeDistance) * 100}%,
                      ${rangeColor} ${((fromPosition) / (rangeDistance)) * 100}%,
                      ${rangeColor} ${(toPosition) / (rangeDistance) * 100}%,
                      ${sliderColor} ${(toPosition) / (rangeDistance) * 100}%,
                      ${sliderColor} 100%)`;
}

function setToggleAccessible(currentTarget) {
    const toSlider = document.querySelector('#toSlider');
    if (Number(currentTarget.value) <= 0) {
        toSlider.style.zIndex = 2;
    } else {
        toSlider.style.zIndex = 0;
    }
}

const fromSlider = document.querySelector('#fromSlider');
const toSlider = document.querySelector('#toSlider');
const fromInput = document.querySelector('#fromInput');
const toInput = document.querySelector('#toInput');
const fromValue = document.getElementById("fromValue");
const toValue = document.getElementById("toValue");
const comboxSortBy = document.getElementById("comboxSortBy");


fillSlider(fromSlider, toSlider, '#C6C6C6', '#1e293b', toSlider);
setToggleAccessible(toSlider);

fromSlider.oninput = () => {
    controlFromSlider(fromSlider, toSlider, fromInput);
    fromValue.textContent = fromSlider.value;
}

toSlider.oninput = () => {
    controlToSlider(fromSlider, toSlider, toInput);
    toValue.textContent = toSlider.value;
}

fromInput.oninput = () => {
    controlFromInput(fromSlider, fromInput, toInput, toSlider);
    fromValue.textContent = fromSlider.value;
}

toInput.oninput = () => {
    controlToInput(toSlider, fromInput, toInput, toSlider);
    toValue.textContent = toSlider.value;
}


const dualRangeinit = function () {
    controlFromSlider(fromSlider, toSlider, fromInput);
    controlToSlider(fromSlider, toSlider, toInput);
    controlFromInput(fromSlider, fromInput, toInput, toSlider);
    controlToInput(toSlider, fromInput, toInput, toSlider);
}


// Function to add a product to the cart
function addToCart(productId, event) {
    event.preventDefault();

    const isUserLoggedIn = event.currentTarget.dataset?.isauthenticated; // Ensure this is properly rendered as a boolean string
    if (isUserLoggedIn !== "true") {
        window.location.href = '/Identity/Account/Login';
        return; // Prevent further execution if not logged in
    }

    fetch(`/Customer/Home/AddtoCart?ProductID=${encodeURIComponent(productId)}`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                reloadCartCount(); // Reload cart count if item was added successfully
                toast.success('Item added to cart', 5); // Show success message
            } else {
                console.error('Failed to add product to cart.');
            }
        })
        .catch(error => console.error('Error adding product to cart:', error));
}

// Function to reload the cart count
function reloadCartCount() {
    fetch('/Customer/Home/ReloadCartCount', {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    })
        .then(response => response.text())
        .then(data => {
            document.getElementById('cart-count-LoginPartial').innerHTML = data;
        })
        .catch(error => console.error('Error reloading cart count:', error));
}


document.addEventListener('DOMContentLoaded', () => {
    GetDesktopFilterData();

    document.getElementById("prevPageBtn").addEventListener("click", function () {
        let page = parseInt(this.dataset.page);
        if (page > 0) loadPage(page, this);
    });

    document.getElementById("nextPageBtn").addEventListener("click", function () {
        let page = parseInt(this.dataset.page);
        let totalPages = parseInt(document.getElementById("hfTotalPages").value);
        if (page <= totalPages) loadPage(page, this);
    });


    // For Mobile Filter
    loadFilterState();
});


// Load filter state from localStorage for Mobile
function loadFilterState() {
    const savedState = localStorage.getItem('productFilterState');
    if (savedState) {
        try {
            const state = JSON.parse(savedState);

            // Apply state to desktop
            document.getElementById('fromInput').value = state.fromValue;
            document.getElementById('fromSlider').value = state.fromValue;
            document.getElementById('toInput').value = state.toValue;
            document.getElementById('toSlider').value = state.toValue;
            document.getElementById('comboxSortBy').value = state.sortBy;

            // Apply state to mobile
            document.getElementById('mobileFromInput').value = state.fromValue;
            // document.getElementById('mobileFromSlider').value = state.fromValue;
            document.getElementById('mobileToInput').value = state.toValue;
            // document.getElementById('mobileToSlider').value = state.toValue;
            document.getElementById('mobileSortBy').value = state.sortBy;

        } catch (e) {
            console.error('Error loading filter state', e);
        }
    }
}


async function GetCategories() {
    const response = await fetch('/Customer/Home/GetFiltersData');
    const data = await response.json();
    if (!data.success) throw new Error('Filter API response failed');

    const categoriesFilterHTML = constructCategoriesHTML(data.d.category);

    const responseData = {
        categoriesFilterHTML,
        maxPrice: data.d.maxPrice,
        minPrice: data.d.minPrice
    }

    return responseData;
}


const constructCategoriesHTML = function (categoriesObject) {
    return categoriesObject.map(category => {
        const checkboxId = `check-vertical-list-group-${category.id}`;
        return `
            <nav class="flex min-w-[240px] flex-col gap-1 px-2 py-1">
                <div role="button" class="flex w-full items-center rounded-md p-0 transition-all hover:bg-slate-100 focus:bg-slate-100 active:bg-slate-100">
                    <label for="${checkboxId}" class="flex w-full cursor-pointer items-center px-3 py-2.5">
                        <div class="inline-flex items-center">
                            <label class="flex items-center cursor-pointer relative" for="${checkboxId}">
                                <input type="checkbox" data-id="${category.id}" data-name="${category.name}" name="category"
                                       class="peer h-5 w-5 cursor-pointer transition-all appearance-none rounded shadow hover:shadow-md border border-slate-300 checked:bg-slate-800 checked:border-slate-800"
                                       id="${checkboxId}" />
                                <span class="absolute text-white opacity-0 peer-checked:opacity-100 top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2">
                                    <svg xmlns="http://www.w3.org/2000/svg" class="h-3.5 w-3.5" viewBox="0 0 20 20" fill="currentColor" stroke="currentColor" stroke-width="1">
                                        <path fill-rule="evenodd"
                                              d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                                              clip-rule="evenodd"></path>
                                    </svg>
                                </span>
                            </label>
                            <label class="cursor-pointer ml-2 text-slate-600 text-sm" for="${checkboxId}">
                                ${category.name}
                            </label>
                        </div>
                    </label>
                </div>
            </nav>`;
    }).join('');
}



async function GetDesktopFilterData() {
    try {
        const { categoriesFilterHTML, maxPrice, minPrice } = await GetCategories();
        const categoryList = document.getElementById('liCategory');
        categoryList.innerHTML = categoriesFilterHTML

        const roundedMax = Math.ceil(parseFloat(maxPrice) / 100) * 100;
        Object.assign(fromSlider, { max: roundedMax, value: minPrice });
        Object.assign(toSlider, { max: roundedMax, value: roundedMax });
        Object.assign(fromInput, { max: roundedMax, value: minPrice });
        Object.assign(toInput, { max: roundedMax, value: roundedMax });

        fromValue.textContent = minPrice;
        toValue.textContent = roundedMax;
        comboxSortBy.options.selectedIndex = 0;

        dualRangeinit();
    } catch (error) {
        console.error('Error fetching categories:', error);
    }
}

async function GetProductData(e) {
    const filterButton = e.currentTarget;
    btnLoaderShow(filterButton);

    try {
        const formData = collectFilterData();
        const response = await fetch(filterButton.dataset?.url, {
            method: "POST",
            body: formData,
            headers: { "X-Requested-With": "XMLHttpRequest" }
        });

        const html = await response.text();
        const productContainer = document.getElementById("productListContainer");
        productContainer.innerHTML = html;

        updatePagination();
    } catch (error) {
        console.error("Error fetching products:", error);
    } finally {
        btnLoaderHide(filterButton);
    }
}

async function loadPage(page, element) {
    if (element) btnLoaderShow(element);

    try {
        const formData = collectFilterData();
        formData.append("CurrentPage", page);

        const response = await fetch(`/`, {
            method: "POST",
            body: formData,
            headers: { "X-Requested-With": "XMLHttpRequest" }
        });

        const html = await response.text();
        document.getElementById("productListContainer").innerHTML = html;
        updatePagination(page);
    } catch (error) {
        console.error("Error fetching page:", error);
    } finally {
        if (element) btnLoaderHide(element);
    }
}

function collectFilterData() {
    const formData = new FormData();
    const selectedCategories = [...document.querySelectorAll('input[name="category"]:checked')]
        .map(el => el.dataset?.name);

    if (selectedCategories.length) selectedCategories.forEach(cat => formData.append("SelectedCategories", cat));
    if (fromInput.value) formData.append("MinPrice", fromInput.value);
    if (toInput.value) formData.append("MaxPrice", toInput.value);
    if (comboxSortBy.value) formData.append("SortBy", comboxSortBy.value);

    return formData;
}

function updatePagination(currentPage = parseInt(document.getElementById("hfcurrentPage")?.value)) {
    const totalPages = parseInt(document.getElementById("hfTotalPages")?.value);
    document.getElementById("totalPages").innerText = totalPages;
    document.getElementById("currentPage").innerText = currentPage;

    document.getElementById("prevPageBtn").dataset.page = currentPage - 1;
    document.getElementById("nextPageBtn").dataset.page = currentPage + 1;

    document.getElementById("prevPageBtn").disabled = currentPage <= 1;
    document.getElementById("nextPageBtn").disabled = currentPage >= totalPages;
}

const btnLoaderShow = (element) => {
    element.dataset.originalContent = element.innerHTML;
    element.disabled = true;
    element.innerHTML = `<img src="/images/spinner.svg" width="20" height="20" />`;
};

const btnLoaderHide = (element) => {
    element.disabled = false;
    element.innerHTML = element.dataset?.originalContent;
};




// Mobile Filters





// Function to sync filters from mobile to desktop
function syncMobileToDesktop() {
    const mobileFromInput = document.getElementById('mobileFromInput');
    const mobileToInput = document.getElementById('mobileToInput');
    const mobileSortBy = document.getElementById('mobileSortBy');

    // Sync to desktop
    document.getElementById('fromInput').value = mobileFromInput.value;
    document.getElementById('fromSlider').value = mobileFromInput.value;
    document.getElementById('toInput').value = mobileToInput.value;
    document.getElementById('toSlider').value = mobileToInput.value;
    document.getElementById('comboxSortBy').value = mobileSortBy.value;

    // Update the slider visuals
    controlFromInput(fromSlider, fromInput, toInput, toSlider);
    controlToInput(toSlider, fromInput, toInput, toSlider);
}

const mobileFilterButton = async function () {
    mobileFilterPopup.classList.remove('hidden');
    setTimeout(() => {
        filterPanel.classList.remove('translate-x-full');
    }, 10);
    // Sync desktop filters to mobile
    await syncDesktopToMobile();
}


// Close filter popup
function closeFilterPopup() {
    filterPanel.classList.add('translate-x-full');
    filterPanel.addEventListener('transitionend', () => {
        mobileFilterPopup.classList.add('hidden');
    }, { once: true });
}


// Handle window resize events
window.addEventListener('resize', function () {
    // If we resize to desktop view and the mobile filter is open, close it
    const mobileFilterPopup = document.getElementById('mobileFilterPopup');
    if (window.innerWidth >= 768 && !mobileFilterPopup.classList.contains('hidden')) {
        closeFilterPopup();
    }
});


// Update the mobileApplyFilter function
const mobileApplyFilter = function () {
    // 1. Sync mobile values to desktop inputs first
    document.getElementById('fromInput').value = document.getElementById('mobileFromInput').value;
    document.getElementById('toInput').value = document.getElementById('mobileToInput').value;
    document.getElementById('comboxSortBy').value = document.getElementById('mobileSortBy').value;

    // 2. Sync checkbox states
    const mobileChecked = [...document.querySelectorAll('#mobileLiCategory input[name="category"]:checked')];
    document.querySelectorAll('#liCategory input[name="category"]').forEach(checkbox => {
        checkbox.checked = mobileChecked.some(mobileCheck => mobileCheck.dataset.id === checkbox.dataset.id);
    });

    // 3. Update sliders
    controlFromInput(fromSlider, fromInput, toInput, toSlider);
    controlToInput(toSlider, fromInput, toInput, toSlider);

    // 4. Save state and apply
    saveFilterState();
    document.getElementById('btnApplyFilter').click();
    closeFilterPopup();
}

// Update the saveFilterState function
function saveFilterState() {
    const state = {
        fromValue: document.getElementById('fromInput').value,
        toValue: document.getElementById('toInput').value,
        sortBy: document.getElementById('comboxSortBy').value
    };
    localStorage.setItem('productFilterState', JSON.stringify(state));
}


// Update your syncDesktopToMobile function
async function syncDesktopToMobile() {
    const { categoriesFilterHTML } = await GetCategories();
    const mobileCategories = document.getElementById('mobileLiCategory');
    mobileCategories.innerHTML = categoriesFilterHTML;

    // Sync price range
    const fromInput = document.getElementById('fromInput');
    const toInput = document.getElementById('toInput');
    document.getElementById('mobileFromInput').value = fromInput.value;
    document.getElementById('mobileToInput').value = toInput.value;

    // Sync sort by
    document.getElementById('mobileSortBy').value = document.getElementById('comboxSortBy').value;

    // Sync checked categories
    const desktopChecked = [...document.querySelectorAll('#liCategory input[name="category"]:checked')];
    desktopChecked.forEach(checkbox => {
        const mobileCheckbox = document.querySelector(`#mobileLiCategory input[data-id="${checkbox.dataset.id}"]`);
        if (mobileCheckbox) mobileCheckbox.checked = true;
    });
}