
//function controlFromInput(fromSlider, fromInput, toInput, controlSlider) {
//    const [from, to] = getParsed(fromInput, toInput);
//    fillSlider(fromInput, toInput, '#C6C6C6', '#1e293b', controlSlider);
//    if (from > to) {
//        fromSlider.value = to;
//        fromInput.value = to;
//    } else {
//        fromSlider.value = from;
//    }
//}

//function controlToInput(toSlider, fromInput, toInput, controlSlider) {
//    const [from, to] = getParsed(fromInput, toInput);
//    fillSlider(fromInput, toInput, '#C6C6C6', '#1e293b', controlSlider);
//    setToggleAccessible(toInput);
//    if (from <= to) {
//        toSlider.value = to;
//        toInput.value = to;
//    } else {
//        toInput.value = from;
//    }
//}

//function controlFromSlider(fromSlider, toSlider, fromInput) {
//    const [from, to] = getParsed(fromSlider, toSlider);
//    fillSlider(fromSlider, toSlider, '#C6C6C6', '#1e293b', toSlider);
//    if (from > to) {
//        fromSlider.value = to;
//        fromInput.value = to;
//    } else {
//        fromInput.value = from;
//    }
//}

//function controlToSlider(fromSlider, toSlider, toInput) {
//    const [from, to] = getParsed(fromSlider, toSlider);
//    fillSlider(fromSlider, toSlider, '#C6C6C6', '#1e293b', toSlider);
//    setToggleAccessible(toSlider);
//    if (from <= to) {
//        toSlider.value = to;
//        toInput.value = to;
//    } else {
//        toInput.value = from;
//        toSlider.value = from;
//    }
//}

//function getParsed(currentFrom, currentTo) {
//    const from = parseInt(currentFrom.value, 10);
//    const to = parseInt(currentTo.value, 10);
//    return [from, to];
//}

//function fillSlider(from, to, sliderColor, rangeColor, controlSlider) {
//    const rangeDistance = to.max - to.min;
//    const fromPosition = from.value - to.min;
//    const toPosition = to.value - to.min;
//    controlSlider.style.background = `linear-gradient(
//                      to right,
//                      ${sliderColor} 0%,
//                      ${sliderColor} ${(fromPosition) / (rangeDistance) * 100}%,
//                      ${rangeColor} ${((fromPosition) / (rangeDistance)) * 100}%,
//                      ${rangeColor} ${(toPosition) / (rangeDistance) * 100}%,
//                      ${sliderColor} ${(toPosition) / (rangeDistance) * 100}%,
//                      ${sliderColor} 100%)`;
//}

//function setToggleAccessible(currentTarget) {
//    const toSlider = document.querySelector('#toSlider');
//    if (Number(currentTarget.value) <= 0) {
//        toSlider.style.zIndex = 2;
//    } else {
//        toSlider.style.zIndex = 0;
//    }
//}

//const fromSlider = document.querySelector('#fromSlider');
//const toSlider = document.querySelector('#toSlider');
//const fromInput = document.querySelector('#fromInput');
//const toInput = document.querySelector('#toInput');
//const fromValue = document.getElementById("fromValue");
//const toValue = document.getElementById("toValue");
//const comboxSortBy = document.getElementById("comboxSortBy");


//fillSlider(fromSlider, toSlider, '#C6C6C6', '#1e293b', toSlider);
//setToggleAccessible(toSlider);

//fromSlider.oninput = () => {
//    controlFromSlider(fromSlider, toSlider, fromInput);
//    fromValue.textContent = fromSlider.value;
//}

//toSlider.oninput = () => {
//    controlToSlider(fromSlider, toSlider, toInput);
//    toValue.textContent = toSlider.value;
//}

//fromInput.oninput = () => {
//    controlFromInput(fromSlider, fromInput, toInput, toSlider);
//    fromValue.textContent = fromSlider.value;
//}

//toInput.oninput = () => {
//    controlToInput(toSlider, fromInput, toInput, toSlider);
//    toValue.textContent = toSlider.value;
//}


//const dualRangeinit = function () {
//    controlFromSlider(fromSlider, toSlider, fromInput);
//    controlToSlider(fromSlider, toSlider, toInput);
//    controlFromInput(fromSlider, fromInput, toInput, toSlider);
//    controlToInput(toSlider, fromInput, toInput, toSlider);
//}



//const prevPage = function () {
//    let page = parseInt(this.dataset.page);
//    if (page > 0) loadPage(page, this);
//    updatePagination();
//}


//const nextPage = function () {
//    let page = parseInt(this.dataset.page);
//    let totalPages = parseInt(document.getElementById("hfTotalPages").value);
//    if (page <= totalPages) loadPage(page, this);
//    updatePagination();
//}


//// Load filter state from localStorage for Mobile
//function loadFilterState() {
//    const savedState = localStorage.getItem('productFilterState');
//    if (savedState) {
//        try {
//            const state = JSON.parse(savedState);

//            // Apply state to desktop
//            document.getElementById('fromInput').value = state.fromValue;
//            document.getElementById('fromSlider').value = state.fromValue;
//            document.getElementById('toInput').value = state.toValue;
//            document.getElementById('toSlider').value = state.toValue;
//            document.getElementById('comboxSortBy').value = state.sortBy;

//            // Apply state to mobile
//            document.getElementById('mobileFromInput').value = state.fromValue;
//            // document.getElementById('mobileFromSlider').value = state.fromValue;
//            document.getElementById('mobileToInput').value = state.toValue;
//            // document.getElementById('mobileToSlider').value = state.toValue;
//            document.getElementById('mobileSortBy').value = state.sortBy;

//        } catch (e) {
//            console.error('Error loading filter state', e);
//        }
//    }
//}


//async function GetCategories() {
//    const response = await fetch('/Customer/Home/GetFiltersData');
//    const data = await response.json();
//    if (!data.success) throw new Error('Filter API response failed');

//    const categoriesFilterHTML = constructCategoriesHTML(data.d.category);

//    const responseData = {
//        categoriesFilterHTML,
//        maxPrice: data.d.maxPrice,
//        minPrice: data.d.minPrice
//    }

//    return responseData;
//}


//const constructCategoriesHTML = function (categoriesObject) {
//    return categoriesObject.map(category => {
//        const checkboxId = `check-vertical-list-group-${category.id}`;
//        return `
//            <nav class="flex min-w-[240px] flex-col gap-1 px-2 py-1">
//                <div role="button" class="flex w-full items-center rounded-md p-0 transition-all hover:bg-slate-100 focus:bg-slate-100 active:bg-slate-100">
//                    <label for="${checkboxId}" class="flex w-full cursor-pointer items-center px-3 py-2.5">
//                        <div class="inline-flex items-center">
//                            <label class="flex items-center cursor-pointer relative" for="${checkboxId}">
//                                <input type="checkbox" data-id="${category.id}" data-name="${category.name}" name="category"
//                                       class="peer h-5 w-5 cursor-pointer transition-all appearance-none rounded border border-slate-300 checked:bg-slate-800 checked:border-slate-800"
//                                       id="${checkboxId}" />
//                                <span class="absolute text-white opacity-0 peer-checked:opacity-100 top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2">
//                                    <svg xmlns="http://www.w3.org/2000/svg" class="h-3.5 w-3.5" viewBox="0 0 20 20" fill="currentColor" stroke="currentColor" stroke-width="1">
//                                        <path fill-rule="evenodd"
//                                              d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
//                                              clip-rule="evenodd"></path>
//                                    </svg>
//                                </span>
//                            </label>
//                            <label class="cursor-pointer ml-2 text-slate-600 text-sm" for="${checkboxId}">
//                                ${category.name}
//                            </label>
//                        </div>
//                    </label>
//                </div>
//            </nav>`;
//    }).join('');
//}


//async function GetDesktopFilterData() {
//    try {
//        const categoryList = document.getElementById('liCategory');

//        //categoryList.innerHTML = "<b class='flex justify-center'> Loading.... </b>"


//        categoryList.innerHTML = `<div class="flex items-center justify-center overflow-hidden">
//                        <svg class="mr-3 -ml-1 size-5 animate-spin text-gray-500" xmlns="http://www.w3.org/2000/svg" fill="none"
//                        viewBox="0 0 24 24"><circle class="opacity-10" cx="12" cy="12" r="10"
//                        stroke="currentColor" stroke-width="4"></circle><path class="opacity-75"
//                        fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0
//                        12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
//                        </path></svg></div>`;


//        const { categoriesFilterHTML, maxPrice, minPrice } = await GetCategories();
//        categoryList.innerHTML = categoriesFilterHTML

//        const roundedMax = Math.ceil(parseFloat(maxPrice) / 100) * 100;
//        Object.assign(fromSlider, { max: roundedMax, value: minPrice });
//        Object.assign(toSlider, { max: roundedMax, value: roundedMax });
//        Object.assign(fromInput, { max: roundedMax, value: minPrice });
//        Object.assign(toInput, { max: roundedMax, value: roundedMax });

//        fromValue.textContent = minPrice;
//        toValue.textContent = roundedMax;
//        comboxSortBy.options.selectedIndex = 0;

//        dualRangeinit();
//    } catch (error) {
//        console.error('Error fetching categories:', error);
//    }
//}


//async function GetProductData(e) {
//    const filterButton = e.currentTarget;
//    btnLoaderShow(filterButton);

//    try {
//        const formData = collectFilterData();
//        const response = await fetch(filterButton.dataset?.url, {
//            method: "POST",
//            body: formData,
//            headers: { "X-Requested-With": "XMLHttpRequest" }
//        });

//        const html = await response.text();
//        const productContainer = document.getElementById("productListContainer");
//        productContainer.innerHTML = html;

//        updatePagination();
//    } catch (error) {
//        console.error("Error fetching products:", error);
//    } finally {
//        btnLoaderHide(filterButton);
//    }
//}


//async function loadPage(page, element) {
//    if (element) btnLoaderShow(element);

//    const ProductList_Container = document.getElementById("productListContainer");
//    ProductList_Container.innerHTML = `
//                    <div class="col-span-full flex flex-col items-center justify-center space-y-4 py-12">
//                    <div class="relative size-20">
//                        <div class="absolute inset-0 rounded-full border-4 border-gray-100"></div>
//                        <div class="absolute inset-0 rounded-full border-4 border-gray-500 border-t-transparent animate-spin"></div>
//                        <div class="absolute inset-2 rounded-full border-4 border-gray-300 border-b-transparent animate-spin animation-delay-150"></div>
//                    </div>
//                        <p class="text-sm text-gray-500">Discovering amazing products for you</p>
//                </div>
//    `;



//    try {
//        const formData = collectFilterData();
//        formData.append("CurrentPage", page);

//        const response = await fetch(`/`, {
//            method: "POST",
//            body: formData,
//            headers: { "X-Requested-With": "XMLHttpRequest" }
//        });

//        const html = await response.text();
//        ProductList_Container.innerHTML = html;
//        updatePagination(page);
//    } catch (error) {
//        console.error("Error fetching page:", error);
//    } finally {
//        if (element) btnLoaderHide(element);
//    }
//}

//function collectFilterData() {
//    const formData = new FormData();
//    const selectedCategories = [...document.querySelectorAll('input[name="category"]:checked')]
//        .map(el => el.dataset?.name);

//    if (selectedCategories.length) selectedCategories.forEach(cat => formData.append("SelectedCategories", cat));
//    if (fromInput.value) formData.append("MinPrice", fromInput.value);
//    if (toInput.value) formData.append("MaxPrice", toInput.value);
//    if (comboxSortBy.value) formData.append("SortBy", comboxSortBy.value);

//    return formData;
//}

//function updatePagination(currentPage = parseInt(document.getElementById("hfcurrentPage")?.value)) {
//    const totalPages = parseInt(document.getElementById("hfTotalPages")?.value);
//    currentPage = parseInt(currentPage); // Ensure it's a number

//    // Update page display
//    document.getElementById("totalPages").innerText = totalPages;
//    document.getElementById("currentPage").innerText = currentPage;

//    // Get buttons
//    const previousPage = document.getElementById("prevPageBtn");
//    const nextPage = document.getElementById("nextPageBtn");

//    // Update data attributes
//    previousPage.dataset.page = currentPage - 1;
//    nextPage.dataset.page = currentPage + 1;

//    // Set disabled states - IMPORTANT: Use strict comparisons
//    previousPage.disabled = (currentPage <= 1);
//    nextPage.disabled = (currentPage >= totalPages);

//    // Visual feedback
//    previousPage.classList.toggle('opacity-50', currentPage <= 1);
//    previousPage.classList.toggle('cursor-not-allowed', currentPage <= 1);
//    nextPage.classList.toggle('opacity-50', currentPage >= totalPages);
//    nextPage.classList.toggle('cursor-not-allowed', currentPage >= totalPages);
//}


//const btnLoaderShow = (element) => {
//    element.dataset.originalContent = element.innerHTML;
//    element.disabled = true;

//    element.innerHTML = `<svg class="size-5 animate-spin text-gray-500" xmlns="http://www.w3.org/2000/svg" fill="none"
//                             viewBox="0 0 24 24">
//                            <circle class="opacity-10" cx="12" cy="12" r="10"
//                                    stroke="currentColor" stroke-width="4"></circle>
//                            <path class="opacity-75"
//                                  fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0
//                        12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
//                            </path>
//                        </svg>`;
//};


//const btnLoaderHide = (element) => {
//    element.disabled = false;
//    element.innerHTML = element.dataset?.originalContent;
//};



//// Mobile Filters
//// Function to sync filters from mobile to desktop
//function syncMobileToDesktop() {
//    const mobileFromInput = document.getElementById('mobileFromInput');
//    const mobileToInput = document.getElementById('mobileToInput');
//    const mobileSortBy = document.getElementById('mobileSortBy');

//    // Sync to desktop
//    document.getElementById('fromInput').value = mobileFromInput.value;
//    document.getElementById('fromSlider').value = mobileFromInput.value;
//    document.getElementById('toInput').value = mobileToInput.value;
//    document.getElementById('toSlider').value = mobileToInput.value;
//    document.getElementById('comboxSortBy').value = mobileSortBy.value;

//    // Update the slider visuals
//    controlFromInput(fromSlider, fromInput, toInput, toSlider);
//    controlToInput(toSlider, fromInput, toInput, toSlider);
//}

//const mobileFilterButton = async function () {
//    mobileFilterPopup.classList.remove('hidden');
//    setTimeout(() => {
//        filterPanel.classList.remove('translate-x-full');
//    }, 10);
//    // Sync desktop filters to mobile
//    await syncDesktopToMobile();
//}


//// Close filter popup
//function closeFilterPopup() {
//    filterPanel.classList.add('translate-x-full');
//    filterPanel.addEventListener('transitionend', () => {
//        mobileFilterPopup.classList.add('hidden');
//    }, { once: true });
//}


//// Handle window resize events
//window.addEventListener('resize', function () {
//    // If we resize to desktop view and the mobile filter is open, close it
//    const mobileFilterPopup = document.getElementById('mobileFilterPopup');
//    if (window.innerWidth >= 768 && !mobileFilterPopup.classList.contains('hidden')) {
//        closeFilterPopup();
//    }
//});


//// Update the mobileApplyFilter function
//const mobileApplyFilter = function () {
//    // 1. Sync mobile values to desktop inputs first
//    document.getElementById('fromInput').value = document.getElementById('mobileFromInput').value;
//    document.getElementById('toInput').value = document.getElementById('mobileToInput').value;
//    document.getElementById('comboxSortBy').value = document.getElementById('mobileSortBy').value;

//    // 2. Sync checkbox states
//    const mobileChecked = [...document.querySelectorAll('#mobileLiCategory input[name="category"]:checked')];
//    document.querySelectorAll('#liCategory input[name="category"]').forEach(checkbox => {
//        checkbox.checked = mobileChecked.some(mobileCheck => mobileCheck.dataset.id === checkbox.dataset.id);
//    });

//    // 3. Update sliders
//    controlFromInput(fromSlider, fromInput, toInput, toSlider);
//    controlToInput(toSlider, fromInput, toInput, toSlider);

//    // 4. Save state and apply
//    saveFilterState();
//    document.getElementById('btnApplyFilter').click();
//    closeFilterPopup();
//}

//// Update the saveFilterState function
//function saveFilterState() {
//    const state = {
//        fromValue: document.getElementById('fromInput').value,
//        toValue: document.getElementById('toInput').value,
//        sortBy: document.getElementById('comboxSortBy').value
//    };
//    localStorage.setItem('productFilterState', JSON.stringify(state));
//}


//// Update your syncDesktopToMobile function
//async function syncDesktopToMobile() {
//    const { categoriesFilterHTML } = await GetCategories();
//    const mobileCategories = document.getElementById('mobileLiCategory');
//    mobileCategories.innerHTML = categoriesFilterHTML;

//    // Sync price range
//    const fromInput = document.getElementById('fromInput');
//    const toInput = document.getElementById('toInput');
//    document.getElementById('mobileFromInput').value = fromInput.value;
//    document.getElementById('mobileToInput').value = toInput.value;

//    // Sync sort by
//    document.getElementById('mobileSortBy').value = document.getElementById('comboxSortBy').value;

//    // Sync checked categories
//    const desktopChecked = [...document.querySelectorAll('#liCategory input[name="category"]:checked')];
//    desktopChecked.forEach(checkbox => {
//        const mobileCheckbox = document.querySelector(`#mobileLiCategory input[data-id="${checkbox.dataset.id}"]`);
//        if (mobileCheckbox) mobileCheckbox.checked = true;
//    });
//}













// Range Slider Module
class RangeSlider {
    constructor(config) {
        this.elements = {
            fromSlider: document.querySelector(config.fromSlider),
            toSlider: document.querySelector(config.toSlider),
            fromInput: document.querySelector(config.fromInput),
            toInput: document.querySelector(config.toInput),
            fromValue: document.querySelector(config.fromValue),
            toValue: document.querySelector(config.toValue)
        };

        this.colors = {
            base: config.baseColor || '#C6C6C6',
            range: config.rangeColor || '#1e293b'
        };

        this.init();
    }

    init() {
        this.fillSlider();
        this.setToggleAccessible();
        this.bindEvents();
    }

    bindEvents() {
        const { fromSlider, toSlider, fromInput, toInput, fromValue, toValue } = this.elements;

        fromSlider.addEventListener('input', () => {
            this.controlFromSlider();
            fromValue.textContent = fromSlider.value;
        });

        toSlider.addEventListener('input', () => {
            this.controlToSlider();
            toValue.textContent = toSlider.value;
        });

        fromInput.addEventListener('input', () => {
            this.controlFromInput();
            fromValue.textContent = fromSlider.value;
        });

        toInput.addEventListener('input', () => {
            this.controlToInput();
            toValue.textContent = toSlider.value;
        });
    }

    getParsed(currentFrom, currentTo) {
        const from = parseInt(currentFrom.value, 10);
        const to = parseInt(currentTo.value, 10);
        return [from, to];
    }

    fillSlider() {
        const { fromSlider, toSlider } = this.elements;
        const { base, range } = this.colors;

        const rangeDistance = toSlider.max - toSlider.min;
        const fromPosition = fromSlider.value - toSlider.min;
        const toPosition = toSlider.value - toSlider.min;

        toSlider.style.background = `linear-gradient(
      to right,
      ${base} 0%,
      ${base} ${(fromPosition) / (rangeDistance) * 100}%,
      ${range} ${((fromPosition) / (rangeDistance)) * 100}%,
      ${range} ${(toPosition) / (rangeDistance) * 100}%,
      ${base} ${(toPosition) / (rangeDistance) * 100}%,
      ${base} 100%)`;
    }

    setToggleAccessible() {
        const { toSlider } = this.elements;
        if (Number(toSlider.value) <= 0) {
            toSlider.style.zIndex = 2;
        } else {
            toSlider.style.zIndex = 0;
        }
    }

    controlFromSlider() {
        const { fromSlider, toSlider, fromInput } = this.elements;
        const [from, to] = this.getParsed(fromSlider, toSlider);

        this.fillSlider();
        if (from > to) {
            fromSlider.value = to;
            fromInput.value = to;
        } else {
            fromInput.value = from;
        }
    }

    controlToSlider() {
        const { fromSlider, toSlider, toInput } = this.elements;
        const [from, to] = this.getParsed(fromSlider, toSlider);

        this.fillSlider();
        this.setToggleAccessible();
        if (from <= to) {
            toSlider.value = to;
            toInput.value = to;
        } else {
            toInput.value = from;
            toSlider.value = from;
        }
    }

    controlFromInput() {
        const { fromSlider, fromInput, toInput, toSlider } = this.elements;
        const [from, to] = this.getParsed(fromInput, toInput);

        this.fillSlider();
        if (from > to) {
            fromSlider.value = to;
            fromInput.value = to;
        } else {
            fromSlider.value = from;
        }
    }

    controlToInput() {
        const { toSlider, fromInput, toInput } = this.elements;
        const [from, to] = this.getParsed(fromInput, toInput);

        this.fillSlider();
        this.setToggleAccessible();
        if (from <= to) {
            toSlider.value = to;
            toInput.value = to;
        } else {
            toInput.value = from;
        }
    }

    updateValues(min, max) {
        const { fromSlider, toSlider, fromInput, toInput, fromValue, toValue } = this.elements;

        Object.assign(fromSlider, { max, value: min });
        Object.assign(toSlider, { max, value: max });
        Object.assign(fromInput, { max, value: min });
        Object.assign(toInput, { max, value: max });

        fromValue.textContent = min;
        toValue.textContent = max;

        this.fillSlider();
    }
}

// Product Filter Module
class ProductFilter {
    constructor() {
        this.elements = {
            categoryList: document.getElementById('liCategory'),
            mobileCategoryList: document.getElementById('mobileLiCategory'),
            productContainer: document.getElementById('productListContainer'),
            filterButton: document.getElementById('btnApplyFilter'),
            prevPageBtn: document.getElementById('prevPageBtn'),
            nextPageBtn: document.getElementById('nextPageBtn'),
            sortBySelect: document.getElementById('comboxSortBy'),
            mobileSortBy: document.getElementById('mobileSortBy'),
            mobileFilterBtn: document.getElementById('mobileFilterBtn'),
            mobileFilterPopup: document.getElementById('mobileFilterPopup'),
            filterPanel: document.getElementById('filterPanel'),
            mobileApplyBtn: document.getElementById('mobileApplyBtn'),
            mobileCloseBtn: document.getElementById('mobileCloseBtn'),
            cartCountElement: document.getElementById('cart-count-LoginPartial')
        };

        this.rangeSlider = new RangeSlider({
            fromSlider: '#fromSlider',
            toSlider: '#toSlider',
            fromInput: '#fromInput',
            toInput: '#toInput',
            fromValue: '#fromValue',
            toValue: '#toValue',
            baseColor: '#C6C6C6',
            rangeColor: '#1e293b'
        });

        this.init();
    }

    init() {
        this.loadFilterState();
        this.bindEvents();
        this.loadDesktopFilterData();
        this.handleWindowResize();
        this.updatePagination();
        this.bindProductActions();
    }

    bindEvents() {
        const {
            filterButton, prevPageBtn, nextPageBtn, mobileFilterBtn,
            mobileApplyBtn, mobileCloseBtn
        } = this.elements;

        if (filterButton) filterButton.addEventListener('click', this.getProductData.bind(this));
        if (prevPageBtn) prevPageBtn.addEventListener('click', this.prevPage.bind(this));
        if (nextPageBtn) nextPageBtn.addEventListener('click', this.nextPage.bind(this));
        if (mobileFilterBtn) mobileFilterBtn.addEventListener('click', this.openMobileFilter.bind(this));
        if (mobileApplyBtn) mobileApplyBtn.addEventListener('click', this.applyMobileFilter.bind(this));
        if (mobileCloseBtn) mobileCloseBtn.addEventListener('click', this.closeMobileFilter.bind(this));
    }

    bindProductActions() {
        // Bind wishlist buttons
        const wishListButtons = document.querySelectorAll('.btnWishlist');
        wishListButtons.forEach(btn => btn.addEventListener('click', this.addToWishList.bind(this)));

        // Bind add to cart buttons
        const addToCartButtons = document.querySelectorAll('.btnAddtocart');
        addToCartButtons.forEach(btn => btn.addEventListener('click', this.addToCart.bind(this)));
    }

    // Cart and Wishlist functionality
    addToCart(event) {
        event.preventDefault();

        const button = event.currentTarget;
        const isUserLoggedIn = button.dataset?.isauthenticated === "true";

        if (!isUserLoggedIn) {
            window.location.href = '/Identity/Account/Login';
            return;
        }

        const productId = button.dataset?.productid;
        if (!productId) {
            console.error('Product ID not found');
            return;
        }

        this.showButtonLoader(button);

        fetch(`/Customer/Home/AddtoCart?ProductID=${encodeURIComponent(productId)}`, {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                if (data.success) {
                    this.reloadCartCount();
                    toast.success('Item added to cart', 5);
                } else {
                    console.error('Failed to add product to cart:', data.message || 'Unknown error');
                    toast.error('Failed to add item to cart', 5);
                }
            })
            .catch(error => {
                console.error('Error adding product to cart:', error);
                toast.error('Error adding item to cart', 5);
            })
            .finally(() => {
                this.hideButtonLoader(button);
            });
    }

    reloadCartCount() {
        const { cartCountElement } = this.elements;

        if (!cartCountElement) {
            console.warn('Cart count element not found');
            return;
        }

        fetch('/Customer/Home/ReloadCartCount', {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' },
            cache: 'no-store' // Prevent caching
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.text();
            })
            .then(data => {
                cartCountElement.innerHTML = data;
            })
            .catch(error => {
                console.error('Error reloading cart count:', error);
            });
    }

    addToWishList(event) {
        event.preventDefault();

        const button = event.currentTarget;
        const isUserLoggedIn = button.dataset?.isauthenticated === "true";

        if (!isUserLoggedIn) {
            window.location.href = '/Identity/Account/Login';
            return;
        }

        const productId = button.dataset?.productid;
        if (!productId) {
            console.error('Product ID not found');
            return;
        }

        this.showButtonLoader(button);

        const URL = `/Customer/Wishlist/AddToWishlist?ProductID=${encodeURIComponent(productId)}`;

        fetch(URL, {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                if (data.success) {
                    toast.success('Item added to wishlist', 5);
                } else {
                    console.error('Failed to add product to wishlist:', data.message || 'Unknown error');
                    toast.error('Failed to add item to wishlist', 5);
                }
            })
            .catch(error => {
                console.error('Error adding product to wishlist:', error);
                toast.error('Error adding item to wishlist', 5);

            })
            .finally(() => {
                this.hideButtonLoader(button);
            });

    }

    async loadDesktopFilterData() {
        try {
            const { categoryList } = this.elements;

            if (!categoryList) return;

            categoryList.innerHTML = this.getLoadingSpinner();

            const { categoriesFilterHTML, maxPrice, minPrice } = await this.getCategories();
            categoryList.innerHTML = categoriesFilterHTML;

            const roundedMax = Math.ceil(parseFloat(maxPrice) / 100) * 100;
            this.rangeSlider.updateValues(minPrice, roundedMax);

            const { sortBySelect } = this.elements;
            if (sortBySelect) sortBySelect.options.selectedIndex = 0;

        } catch (error) {
            console.error('Error fetching categories:', error);
        }
    }

    async getCategories() {
        const response = await fetch('/Customer/Home/GetFiltersData');
        const data = await response.json();

        if (!data.success) throw new Error('Filter API response failed');

        return {
            categoriesFilterHTML: this.constructCategoriesHTML(data.d.category),
            maxPrice: data.d.maxPrice,
            minPrice: data.d.minPrice
        };
    }

    constructCategoriesHTML(categories) {
        return categories.map(category => {
            const checkboxId = `check-vertical-list-group-${category.id}`;
            return `
        <nav class="flex min-w-[240px] flex-col gap-1 px-2 py-1">
          <div role="button" class="flex w-full items-center rounded-md p-0 transition-all hover:bg-slate-100 focus:bg-slate-100 active:bg-slate-100">
            <label for="${checkboxId}" class="flex w-full cursor-pointer items-center px-3 py-2.5">
              <div class="inline-flex items-center">
                <label class="flex items-center cursor-pointer relative" for="${checkboxId}">
                  <input type="checkbox" data-id="${category.id}" data-name="${category.name}" name="category"
                         class="peer h-5 w-5 cursor-pointer transition-all appearance-none rounded border border-slate-300 checked:bg-slate-800 checked:border-slate-800"
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

    async getProductData(e) {
        debugger;
        const filterButton = e.currentTarget;
        this.showButtonLoader(filterButton);

        try {
            const formData = this.collectFilterData();
            const response = await fetch(filterButton.dataset?.url, {
                method: "POST",
                body: formData,
                headers: { "X-Requested-With": "XMLHttpRequest" }
            });

            const html = await response.text();
            const { productContainer } = this.elements;

            if (productContainer) {
                productContainer.innerHTML = html;
                this.updatePagination();
                // Rebind product actions after content update
                this.bindProductActions();
            }
        } catch (error) {
            console.error("Error fetching products:", error);
        } finally {
            this.hideButtonLoader(filterButton);
        }
    }

    prevPage(e) {
        const button = e.currentTarget;
        const page = parseInt(button.dataset.page);

        if (page > 0) {
            this.loadPage(page, button);
            this.updatePagination();
        }
    }

    nextPage(e) {
        const button = e.currentTarget;
        const page = parseInt(button.dataset.page);
        const totalPages = parseInt(document.getElementById("hfTotalPages")?.value);

        if (page <= totalPages) {
            this.loadPage(page, button);
            this.updatePagination();
        }
    }

    async loadPage(page, element) {
        if (element) this.showButtonLoader(element);

        const { productContainer } = this.elements;
        if (productContainer) {
            productContainer.innerHTML = this.getPageLoadingSpinner();
        }

        try {
            const formData = this.collectFilterData();
            formData.append("CurrentPage", page);

            const response = await fetch(`/`, {
                method: "POST",
                body: formData,
                headers: { "X-Requested-With": "XMLHttpRequest" }
            });

            const html = await response.text();
            if (productContainer) {
                productContainer.innerHTML = html;
                this.updatePagination(page);
                // Rebind product actions after content update
                this.bindProductActions();
            }
        } catch (error) {
            console.error("Error fetching page:", error);
        } finally {
            if (element) this.hideButtonLoader(element);
        }
    }

    collectFilterData() {
        const formData = new FormData();
        const selectedCategories = [...document.querySelectorAll('input[name="category"]:checked')]
            .map(el => el.dataset?.name);

        const fromInput = document.getElementById('fromInput');
        const toInput = document.getElementById('toInput');
        const sortBySelect = document.getElementById('comboxSortBy');

        if (selectedCategories.length) {
            selectedCategories.forEach(cat => formData.append("SelectedCategories", cat));
        }

        if (fromInput?.value) formData.append("MinPrice", fromInput.value);
        if (toInput?.value) formData.append("MaxPrice", toInput.value);
        if (sortBySelect?.value) formData.append("SortBy", sortBySelect.value);

        return formData;
    }

    updatePagination(currentPage) {
        const totalPagesEl = document.getElementById("hfTotalPages");
        const currentPageEl = document.getElementById("hfcurrentPage");

        if (!totalPagesEl || !currentPageEl) return;

        const totalPages = parseInt(totalPagesEl.value);
        currentPage = currentPage || parseInt(currentPageEl.value);

        // Update page display
        const totalPagesDisplay = document.getElementById("totalPages");
        const currentPageDisplay = document.getElementById("currentPage");

        if (totalPagesDisplay) totalPagesDisplay.innerText = totalPages;
        if (currentPageDisplay) currentPageDisplay.innerText = currentPage;

        // Get buttons
        const { prevPageBtn, nextPageBtn } = this.elements;

        if (!prevPageBtn || !nextPageBtn) return;

        // Update data attributes
        prevPageBtn.dataset.page = currentPage - 1;
        nextPageBtn.dataset.page = currentPage + 1;

        // Set disabled states
        prevPageBtn.disabled = (currentPage <= 1);
        nextPageBtn.disabled = (currentPage >= totalPages);

        // Visual feedback
        prevPageBtn.classList.toggle('opacity-50', currentPage <= 1);
        prevPageBtn.classList.toggle('cursor-not-allowed', currentPage <= 1);
        nextPageBtn.classList.toggle('opacity-50', currentPage >= totalPages);
        nextPageBtn.classList.toggle('cursor-not-allowed', currentPage >= totalPages);
    }

    showButtonLoader(element) {
        element.dataset.originalContent = element.innerHTML;
        element.disabled = true;

        element.innerHTML = `<svg class="size-5 animate-spin text-gray-500" xmlns="http://www.w3.org/2000/svg" fill="none"
                           viewBox="0 0 24 24">
                          <circle class="opacity-10" cx="12" cy="12" r="10"
                                  stroke="currentColor" stroke-width="4"></circle>
                          <path class="opacity-75"
                                fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0
                      12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                          </path>
                      </svg>`;
    }

    hideButtonLoader(element) {
        element.disabled = false;
        element.innerHTML = element.dataset?.originalContent;
    }

    getLoadingSpinner() {
        return `<div class="flex items-center justify-center overflow-hidden">
              <svg class="mr-3 -ml-1 size-5 animate-spin text-gray-500" xmlns="http://www.w3.org/2000/svg" fill="none" 
              viewBox="0 0 24 24"><circle class="opacity-10" cx="12" cy="12" r="10" 
              stroke="currentColor" stroke-width="4"></circle><path class="opacity-75" 
              fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 
              12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
              </path></svg></div>`;
    }

    getPageLoadingSpinner() {
        return `<div class="col-span-full flex flex-col items-center justify-center space-y-4 py-12">
              <div class="relative size-20">
                <div class="absolute inset-0 rounded-full border-4 border-gray-100"></div>
                <div class="absolute inset-0 rounded-full border-4 border-gray-500 border-t-transparent animate-spin"></div>
                <div class="absolute inset-2 rounded-full border-4 border-gray-300 border-b-transparent animate-spin animation-delay-150"></div>
              </div>
              <p class="text-sm text-gray-500">Discovering amazing products for you</p>
            </div>`;
    }

    // Mobile Filter Methods
    openMobileFilter() {
        const { mobileFilterPopup, filterPanel } = this.elements;

        if (!mobileFilterPopup || !filterPanel) return;

        mobileFilterPopup.classList.remove('hidden');
        setTimeout(() => {
            filterPanel.classList.remove('translate-x-full');
        }, 10);

        this.syncDesktopToMobile();
    }

    closeMobileFilter() {
        const { mobileFilterPopup, filterPanel } = this.elements;

        if (!filterPanel) return;

        filterPanel.classList.add('translate-x-full');

        if (filterPanel && mobileFilterPopup) {
            filterPanel.addEventListener('transitionend', () => {
                mobileFilterPopup.classList.add('hidden');
            }, { once: true });
        }
    }

    applyMobileFilter() {
        this.syncMobileToDesktop();
        this.saveFilterState();

        const { filterButton } = this.elements;
        if (filterButton) filterButton.click();

        this.closeMobileFilter();
    }

    syncMobileToDesktop() {
        const mobileFromInput = document.getElementById('mobileFromInput');
        const mobileToInput = document.getElementById('mobileToInput');
        const mobileSortBy = document.getElementById('mobileSortBy');

        if (!mobileFromInput || !mobileToInput || !mobileSortBy) return;

        // Sync to desktop
        const fromInput = document.getElementById('fromInput');
        const toInput = document.getElementById('toInput');
        const sortBySelect = document.getElementById('comboxSortBy');
        const fromSlider = document.getElementById('fromSlider');
        const toSlider = document.getElementById('toSlider');

        if (fromInput) fromInput.value = mobileFromInput.value;
        if (fromSlider) fromSlider.value = mobileFromInput.value;
        if (toInput) toInput.value = mobileToInput.value;
        if (toSlider) toSlider.value = mobileToInput.value;
        if (sortBySelect) sortBySelect.value = mobileSortBy.value;

        // Sync checkbox states
        const mobileChecked = [...document.querySelectorAll('#mobileLiCategory input[name="category"]:checked')];
        document.querySelectorAll('#liCategory input[name="category"]').forEach(checkbox => {
            checkbox.checked = mobileChecked.some(mobileCheck => mobileCheck.dataset.id === checkbox.dataset.id);
        });

        // Update sliders
        if (this.rangeSlider) {
            this.rangeSlider.controlFromInput();
            this.rangeSlider.controlToInput();
        }
    }

    async syncDesktopToMobile() {
        try {
            const { categoriesFilterHTML } = await this.getCategories();
            const { mobileCategoryList } = this.elements;

            if (mobileCategoryList) {
                mobileCategoryList.innerHTML = categoriesFilterHTML;
            }

            // Sync price range
            const fromInput = document.getElementById('fromInput');
            const toInput = document.getElementById('toInput');
            const sortBySelect = document.getElementById('comboxSortBy');
            const mobileFromInput = document.getElementById('mobileFromInput');
            const mobileToInput = document.getElementById('mobileToInput');
            const mobileSortBy = document.getElementById('mobileSortBy');

            if (mobileFromInput && fromInput) mobileFromInput.value = fromInput.value;
            if (mobileToInput && toInput) mobileToInput.value = toInput.value;
            if (mobileSortBy && sortBySelect) mobileSortBy.value = sortBySelect.value;

            // Sync checked categories
            const desktopChecked = [...document.querySelectorAll('#liCategory input[name="category"]:checked')];
            desktopChecked.forEach(checkbox => {
                const mobileCheckbox = document.querySelector(`#mobileLiCategory input[data-id="${checkbox.dataset.id}"]`);
                if (mobileCheckbox) mobileCheckbox.checked = true;
            });
        } catch (error) {
            console.error('Error syncing to mobile:', error);
        }
    }

    loadFilterState() {
        const savedState = localStorage.getItem('productFilterState');
        if (!savedState) return;

        try {
            const state = JSON.parse(savedState);

            // Apply state to desktop
            const fromInput = document.getElementById('fromInput');
            const toInput = document.getElementById('toInput');
            const sortBySelect = document.getElementById('comboxSortBy');
            const fromSlider = document.getElementById('fromSlider');
            const toSlider = document.getElementById('toSlider');

            if (fromInput) fromInput.value = state.fromValue;
            if (fromSlider) fromSlider.value = state.fromValue;
            if (toInput) toInput.value = state.toValue;
            if (toSlider) toSlider.value = state.toValue;
            if (sortBySelect) sortBySelect.value = state.sortBy;

            // Apply state to mobile
            const mobileFromInput = document.getElementById('mobileFromInput');
            const mobileToInput = document.getElementById('mobileToInput');
            const mobileSortBy = document.getElementById('mobileSortBy');

            if (mobileFromInput) mobileFromInput.value = state.fromValue;
            if (mobileToInput) mobileToInput.value = state.toValue;
            if (mobileSortBy) mobileSortBy.value = state.sortBy;

        } catch (e) {
            console.error('Error loading filter state', e);
        }
    }

    saveFilterState() {
        const fromInput = document.getElementById('fromInput');
        const toInput = document.getElementById('toInput');
        const sortBySelect = document.getElementById('comboxSortBy');

        if (!fromInput || !toInput || !sortBySelect) return;

        const state = {
            fromValue: fromInput.value,
            toValue: toInput.value,
            sortBy: sortBySelect.value
        };

        localStorage.setItem('productFilterState', JSON.stringify(state));
    }

    handleWindowResize() {
        window.addEventListener('resize', () => {
            const { mobileFilterPopup } = this.elements;

            if (window.innerWidth >= 768 && mobileFilterPopup && !mobileFilterPopup.classList.contains('hidden')) {
                this.closeMobileFilter();
            }
        });
    }
}