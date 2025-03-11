
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


// Cache DOM elements to avoid repeated DOM lookups
const toggleFilterBtn = document.getElementById("toggleFilter");
const closeFilterBtn = document.getElementById("closeFilter");
const mobileFilters = document.getElementById("mobileFilters");

// Event listener for toggling the filter visibility
toggleFilterBtn.addEventListener("click", () => {
    mobileFilters.classList.remove("translate-x-full");
});

// Event listener for closing the filter
closeFilterBtn.addEventListener("click", () => {
    mobileFilters.classList.add("translate-x-full");
});

// Function to add a product to the cart
function addToCart(productId, event) {
    event.preventDefault();

    const isUserLoggedIn = event.currentTarget.dataset?.isauthenticated; // Ensure this is properly rendered as a boolean string
    if (!isUserLoggedIn) {
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
    GetFilterData();

    document.getElementById("prevPageBtn").addEventListener("click", function () {
        let page = parseInt(this.dataset.page);
        if (page > 0) loadPage(page, this);
    });

    document.getElementById("nextPageBtn").addEventListener("click", function () {
        let page = parseInt(this.dataset.page);
        let totalPages = parseInt(document.getElementById("hfTotalPages").value);
        if (page <= totalPages) loadPage(page, this);
    });
});

async function GetFilterData() {
    try {
        const response = await fetch('/Customer/Home/GetFiltersData');
        const data = await response.json();
        if (!data.success) throw new Error('Filter API response failed');

        const categoryList = document.getElementById('liCategory');
        categoryList.innerHTML = data.d.category.map(category => {
            const checkboxId = `check-vertical-list-group-${category.id}`;
            return `
                <nav class="flex min-w-[240px] flex-col gap-1 px-2 py-1">
                                    <div role="button" class="flex w-full items-center rounded-md p-0 transition-all hover:bg-slate-100 focus:bg-slate-100 active:bg-slate-100">
                                        <label for="${checkboxId}" class="flex w-full cursor-pointer items-center px-3 py-2.5">
                                            <div class="inline-flex items-center">
                                                <label class="flex items-center cursor-pointer relative" for="${checkboxId}">
                                                    <input type="checkbox" data-id="${category.id}" data-name="${category.name}" name="category"
                                                           class="peer h-5 w-5 cursor-pointer transition-all appearance-none rounded duration-200 hover:shadow border border-slate-300 checked:bg-slate-800 checked:border-slate-800"
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

        const roundedMax = Math.ceil(parseFloat(data.d.maxPrice) / 100) * 100;
        Object.assign(fromSlider, { max: roundedMax, value: data.d.minPrice });
        Object.assign(toSlider, { max: roundedMax, value: roundedMax });
        Object.assign(fromInput, { max: roundedMax, value: data.d.minPrice });
        Object.assign(toInput, { max: roundedMax, value: roundedMax });

        fromValue.textContent = data.d.minPrice;
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

        const notFoundDiv = document.getElementById('productNotFoundDiv');
        notFoundDiv.classList.toggle('hidden', !!html.trim());
        productContainer.classList.toggle('hidden', !html.trim());

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
