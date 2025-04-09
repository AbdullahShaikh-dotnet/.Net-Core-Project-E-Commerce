class FileUpload {
    constructor(container) {
        this.container = container;
        this.previewId = container.dataset.previewId;
        this.inputId = container.dataset.inputId;
        this.errorId = container.dataset.errorId;
        this.maxFileCount = parseInt(container.dataset.maxFileCount);
        this.maxFileSizeMB = parseInt(container.dataset.maxFileSize);
        this.acceptedTypes = container.dataset.acceptedTypes;

        this.fileInput = document.getElementById(this.inputId);
        this.previewContainer = document.getElementById(this.previewId);
        this.errorContainer = document.getElementById(this.errorId);
        this.filesArray = [];

        this.initialize();
    }

    initialize() {
        this.fileInput.addEventListener('change', (e) => this.handleFileChange(e));
        this.fileInput.addEventListener('dragover', (e) => e.preventDefault());
        this.fileInput.addEventListener('drop', (e) => {
            e.preventDefault();
            this.fileInput.files = e.dataTransfer.files;
            this.handleFileChange(e);
        });
    }

    handleFileChange(e) {
        const newFiles = Array.from(this.fileInput.files);
        const validationResult = this.validateFiles(newFiles);

        if (!validationResult.isValid) {
            this.showError(validationResult.message);
            this.fileInput.value = '';
            return;
        }

        this.hideError();
        this.filesArray = this.filesArray.concat(newFiles);
        this.updatePreview();
        this.fileInput.value = '';
    }

    validateFiles(files) {
        // Check file count
        if (this.filesArray.length + files.length > this.maxFileCount) {
            return {
                isValid: false,
                message: `Maximum ${this.maxFileCount} files allowed. You're trying to add ${files.length} files to existing ${this.filesArray.length}.`
            };
        }

        for (const file of files) {
            // Check file size
            const fileSizeMB = file.size / (1024 * 1024);
            if (fileSizeMB > this.maxFileSizeMB) {
                return {
                    isValid: false,
                    message: `File "${file.name}" exceeds maximum size of ${this.maxFileSizeMB}MB.`
                };
            }

            // Check file type
            if (this.acceptedTypes !== '*' && !this.isFileTypeValid(file)) {
                return {
                    isValid: false,
                    message: `File "${file.name}" is not an accepted file type.`
                };
            }
        }

        return { isValid: true, message: '' };
    }

    isFileTypeValid(file) {
        if (this.acceptedTypes === '*') return true;

        const acceptedExtensions = this.acceptedTypes.split(',');
        const fileExtension = '.' + file.name.split('.').pop().toLowerCase();

        return acceptedExtensions.some(ext =>
            ext.trim().toLowerCase() === fileExtension ||
            file.type.match(new RegExp(ext.trim().replace('*', '.*'), 'i')));
    }

    showError(message) {
        this.errorContainer.textContent = message;
        this.errorContainer.classList.remove('hidden');
        this.errorContainer.classList.add('block');
    }

    hideError() {
        this.errorContainer.textContent = '';
        this.errorContainer.classList.remove('block');
        this.errorContainer.classList.add('hidden');
    }

    updatePreview() {
        this.previewContainer.innerHTML = '';

        if (this.filesArray.length > 0) {
            this.previewContainer.classList.remove('hidden');

            this.filesArray.forEach((file, index) => {
                const reader = new FileReader();
                reader.onload = (e) => {
                    const previewItem = document.createElement('div');
                    previewItem.className = 'relative';

                    const fileSizeMB = (file.size / (1024 * 1024)).toFixed(2);

                    previewItem.innerHTML = `
                        <div class="h-[60px] w-full border rounded-md overflow-hidden relative group">
                            <img src="${e.target.result}" class="h-full w-full object-cover" />
                            <div class="absolute inset-0 bg-black bg-opacity-0 group-hover:bg-opacity-30 transition-all duration-200 flex items-center justify-center opacity-0 group-hover:opacity-100">
                                <span class="text-white text-xs font-medium bg-black bg-opacity-50 px-2 py-1 rounded">
                                    ${fileSizeMB} MB
                                </span>
                            </div>
                            <button type="button" data-index="${index}" 
                                class="remove-file-btn absolute top-0.5 right-0.5 bg-white rounded-md p-0.5 hover:bg-red-100 transition-colors border border-gray-200">
                                <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3 text-red-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                                </svg>
                            </button>
                        </div>
                        <p class="text-[10px] text-gray-600 truncate w-full" title="${file.name}">
                            ${file.name}
                        </p>
                    `;

                    previewItem.querySelector('.remove-file-btn').addEventListener('click', () => this.removeFile(index));
                    this.previewContainer.appendChild(previewItem);
                };
                reader.readAsDataURL(file);
            });
        } else {
            this.previewContainer.classList.add('hidden');
        }

        this.updateFileInput();
    }

    removeFile(index) {
        this.filesArray.splice(index, 1);
        this.updatePreview();
    }

    //updateFileInput() {
    //    const dataTransfer = new DataTransfer();
    //    this.filesArray.forEach(file => dataTransfer.items.add(file));
    //    this.fileInput.files = dataTransfer.files;
    //}


    updateFileInput() {
        // Create a new file input element instead of using DataTransfer
        const newInput = document.createElement('input');
        newInput.type = 'file';
        newInput.name = this.fileInput.name;
        newInput.multiple = this.fileInput.multiple;
        newInput.hidden = true;

        // Replace the old input with the new one
        this.fileInput.parentNode.insertBefore(newInput, this.fileInput);
        this.fileInput.remove();
        this.fileInput = newInput;

        // Store files in a way that survives form submission
        this.filesArray.forEach(file => {
            const fileList = new DataTransfer();
            fileList.items.add(file);
            this.fileInput.files = fileList.files;
        });
    }
}
