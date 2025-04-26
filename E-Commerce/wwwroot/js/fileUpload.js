/**
 * FileUploadManager - A class to manage file uploads with validation and preview
 *
 * Features:
 * - Multiple upload controls on the same page
 * - File validation (extension, size, count)
 * - File preview with thumbnails
 * - Material Design UI
 * - Delete functionality
 */
class FileUploadManager {
    /**
     * Constructor for the file upload manager
     * @@param {HTMLElement} container - The container element for the upload component
     */
    constructor(container) {
        // Element references
        this.container = container;
        this.fileInput = container.querySelector('.file-upload-input');
        this.uploadLabel = container.querySelector('.file-upload-label');
        this.filesContainer = container.querySelector('.selected-files-container');
        this.errorContainer = container.querySelector('.error-container');

        // Configuration from data attributes
        this.maxSizeMB = parseInt(container.dataset.maxSize || 10);
        this.maxSizeBytes = this.maxSizeMB * 1024 * 1024;
        this.allowedExtensions = (container.dataset.allowedExtensions || 'jpg,jpeg,png,gif').split(',');
        this.maxFiles = parseInt(container.dataset.maxFiles || 10);

        // State
        this.selectedFiles = [];

        // Initialize the component
        this.initialize();
    }

    /**
     * Initializes event listeners and setup
     */
    initialize() {

        // Bind methods to this instance
        this.handleFileSelection = this.handleFileSelection.bind(this);
        this.handleDragEnter = this.handleDragEnter.bind(this);
        this.handleDragLeave = this.handleDragLeave.bind(this);
        this.handleDrop = this.handleDrop.bind(this);
        this.preventDefaults = this.preventDefaults.bind(this);

        // Add event listeners
        this.fileInput.addEventListener('change', this.handleFileSelection);

        // Drag and drop events
        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            this.uploadLabel.addEventListener(eventName, this.preventDefaults, false);
        });

        this.uploadLabel.addEventListener('dragenter', this.handleDragEnter);
        this.uploadLabel.addEventListener('dragleave', this.handleDragLeave);
        this.uploadLabel.addEventListener('drop', this.handleDrop);
    }

    /**
     * Prevent default events
     * @@param {Event} e - The event object
     */
    preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    /**
     * Handle drag enter event
     */
    handleDragEnter() {
        this.uploadLabel.classList.add('border-blue-500', 'bg-blue-50');
    }

    /**
     * Handle drag leave event
     */
    handleDragLeave() {
        this.uploadLabel.classList.remove('border-blue-500', 'bg-blue-50');
    }

    /**
     * Handle drop event
     * @@param {DragEvent} e - The drag event object
     */
    handleDrop(e) {
        this.uploadLabel.classList.remove('border-blue-500', 'bg-blue-50');
        const dt = e.dataTransfer;
        const files = dt.files;
        this.processFiles(files);
    }

    /**
     * Handle file selection from input
     */
    handleFileSelection() {
        const files = this.fileInput.files;
        this.processFiles(files);
    }

    /**
     * Process selected files with validation
     * @@param {FileList} files - The FileList object
     */
    processFiles(files) {
        // Clear any existing errors
        this.clearErrors();

        // Check if adding these files would exceed the maximum
        if (this.selectedFiles.length + files.length > this.maxFiles) {
            this.showError(`You can only upload a maximum of ${this.maxFiles} files.`);
            return;
        }

        // Track validation failures
        let validationFailed = false;

        // Process each file
        for (let i = 0; i < files.length; i++) {
            const file = files[i];

            // Validate file extension
            const extension = this.getFileExtension(file.name).toLowerCase();
            if (!this.allowedExtensions.includes(extension)) {
                this.showError(`File "${file.name}" has an invalid extension. Allowed: ${this.allowedExtensions.join(', ')}`);
                validationFailed = true;
                continue;
            }

            // Validate file size
            if (file.size > this.maxSizeBytes) {
                this.showError(`File "${file.name}" exceeds the maximum size of ${this.maxSizeMB}MB.`);
                validationFailed = true;
                continue;
            }

            // Check for duplicate files
            if (this.isDuplicateFile(file)) {
                // this.showError(`File "${file.name}" is already selected.`);
                validationFailed = true;
                continue;
            }

            // Add file to selected files
            this.selectedFiles.push(file);
        }

        // Update the display and input
        if (!validationFailed) {
            this.displaySelectedFiles();
            this.updateFileInput();
        }
    }

    /**
     * Check if a file is already selected (based on name and size)
     * @@param {File} file - The file to check
     * @@returns {boolean} - True if duplicate
     */
    isDuplicateFile(file) {
        return this.selectedFiles.some(existing =>
            existing.name === file.name && existing.size === file.size);
    }

    /**
     * Get the file extension from a filename
     * @@param {string} filename - The filename
     * @@returns {string} - The extension without the dot
     */
    getFileExtension(filename) {
        return filename.slice((filename.lastIndexOf('.') - 1 >>> 0) + 2);
    }

    /**
     * Show an error message
     * @@param {string} message - The error message
     */
    showError(message) {
        toast.error(message, 10)
    }

    /**
     * Clear all error messages
     */
    clearErrors() {
        this.errorContainer.innerHTML = '';
    }

    /**
     * Display the selected files
     */
    displaySelectedFiles() {
        // Clear the container
        this.filesContainer.innerHTML = '';

        if (this.selectedFiles.length === 0) {
            return;
        }

        // Create a header for selected files
        const header = document.createElement('div');
        header.className = 'text-sm font-medium text-gray-700 mb-2 flex items-center justify-between';

        const headerText = document.createElement('span');
        headerText.textContent = `Selected Files (${this.selectedFiles.length}/${this.maxFiles})`;
        header.appendChild(headerText);

        // Add a "Clear all" button if there are multiple files
        if (this.selectedFiles.length > 1) {
            const clearAllBtn = document.createElement('button');
            clearAllBtn.type = 'button';
            clearAllBtn.className = 'text-xs text-red-500 hover:text-red-700 hover:underline focus:outline-none';
            clearAllBtn.textContent = 'Clear all';
            clearAllBtn.addEventListener('click', () => {
                this.selectedFiles = [];
                this.displaySelectedFiles();
                this.updateFileInput();
            });
            header.appendChild(clearAllBtn);
        }

        this.filesContainer.appendChild(header);

        // Add each file to the display
        this.selectedFiles.forEach((file, index) => {
            const fileItem = this.createFileItem(file, index);
            this.filesContainer.appendChild(fileItem);
        });
    }

    /**
     * Create a file item element
     * @@param {File} file - The file object
     * @@param {number} index - The index in the array
     * @@returns {HTMLElement} - The file item element
     */
    createFileItem(file, index) {
        // Create file item container
        const fileItem = document.createElement('div');
        fileItem.className = 'flex items-center justify-between p-3 bg-white rounded-lg shadow-sm border border-gray-100 transition-all hover:shadow-md relative overflow-hidden';

        // Left side with file info
        const fileInfo = document.createElement('div');
        fileInfo.className = 'flex items-center space-x-3 flex-1 min-w-0';

        // File icon or thumbnail
        const fileIcon = document.createElement('div');
        fileIcon.className = 'flex-shrink-0';

        if (file.type.startsWith('image/')) {
            // Create thumbnail for images
            const imgContainer = document.createElement('div');
            imgContainer.className = 'w-10 h-10 rounded bg-gray-100 flex items-center justify-center overflow-hidden';

            // Add loading indicator
            const loadingIndicator = document.createElement('div');
            loadingIndicator.className = 'w-5 h-5 border-2 border-gray-200 border-t-blue-500 rounded-full animate-spin';
            imgContainer.appendChild(loadingIndicator);

            // Create a thumbnail preview
            const reader = new FileReader();
            reader.onload = function (e) {
                imgContainer.innerHTML = `<img src="${e.target.result}" class="object-cover w-full h-full" alt="Thumbnail">`;
            };
            reader.readAsDataURL(file);

            fileIcon.appendChild(imgContainer);
        } else {
            // Generic file icon for non-images
            fileIcon.innerHTML = `
                                            <div class="w-10 h-10 rounded bg-gray-100 flex items-center justify-center">
                                                <svg class="w-6 h-6 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
                                                </svg>
                                            </div>
                                        `;
        }

        // File details
        const details = document.createElement('div');
        details.className = 'flex flex-col min-w-0';

        const fileName = document.createElement('span');
        fileName.className = 'text-sm font-medium text-gray-700 truncate';
        fileName.title = file.name;
        fileName.textContent = file.name;

        const fileDetails = document.createElement('div');
        fileDetails.className = 'flex items-center text-xs text-gray-500';

        // File size
        const fileSize = document.createElement('span');
        fileSize.textContent = this.formatFileSize(file.size);

        // File type badge
        const fileType = document.createElement('span');
        fileType.className = 'ml-2 px-1.5 py-0.5 bg-gray-100 rounded text-gray-600 uppercase text-[10px]';
        fileType.textContent = this.getFileExtension(file.name);

        fileDetails.appendChild(fileSize);
        fileDetails.appendChild(fileType);

        details.appendChild(fileName);
        details.appendChild(fileDetails);

        fileInfo.appendChild(fileIcon);
        fileInfo.appendChild(details);

        // Delete button
        const deleteButton = document.createElement('button');
        deleteButton.className = 'flex-shrink-0 ml-3 p-1.5 rounded-full text-gray-400 hover:text-red-500 hover:bg-red-50 transition-colors focus:outline-none focus:ring-2 focus:ring-red-200 relative';
        deleteButton.setAttribute('type', 'button');
        deleteButton.setAttribute('aria-label', 'Delete file');
        deleteButton.innerHTML = `
                                        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                                        </svg>
                                    `;

        // Add delete functionality with material ripple effect
        deleteButton.addEventListener('click', (e) => {
            // Create ripple effect
            const rect = deleteButton.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            const ripple = document.createElement('span');
            ripple.style.left = `${x}px`;
            ripple.style.top = `${y}px`;
            ripple.className = 'absolute w-full h-full bg-red-100 rounded-full opacity-50 scale-0 animate-ripple';
            deleteButton.appendChild(ripple);

            // Remove the file from our array
            this.removeFile(index);

            // Clean up ripple after animation
            setTimeout(() => {
                if (ripple.parentNode) {
                    ripple.parentNode.removeChild(ripple);
                }
            }, 500);
        });

        // Assemble the file item
        fileItem.appendChild(fileInfo);
        fileItem.appendChild(deleteButton);

        return fileItem;
    }

    /**
     * Remove a file from the selected files
     * @@param {number} index - The index of the file to remove
     */
    removeFile(index) {
        // Remove file from array
        this.selectedFiles.splice(index, 1);

        // Update the display
        this.displaySelectedFiles();

        // Update the file input
        this.updateFileInput();
    }

    /**
     * Format file size to human-readable string
     * @@param {number} bytes - The file size in bytes
     * @@returns {string} - Formatted file size
     */
    formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';

        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));

        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
    }

    /**
     * Update the file input with current selected files
     */
    updateFileInput() {
        try {
            // Create a new DataTransfer object
            const dataTransfer = new DataTransfer();

            // Add each file to the DataTransfer object
            this.selectedFiles.forEach(file => {
                dataTransfer.items.add(file);
            });

            // Set the new files to the input
            this.fileInput.files = dataTransfer.files;

            // Dispatch change event for form validation
            const event = new Event('change', { bubbles: true });
            this.fileInput.dispatchEvent(event);
        } catch (error) {
            console.error('Error updating file input:', error);
            // Fallback for browsers that don't support DataTransfer
            this.showError('Your browser does not fully support file management. Please try a modern browser.');
        }
    }
}

/**
 * FileUploadRegistry - Manages multiple file upload instances
 */
class FileUploadRegistry {
    constructor() {
        this.uploadManagers = [];
        this.initialize();
    }

    initialize() {
        // Find all file upload containers on the page
        const containers = document.querySelectorAll('.file-upload-container');

        // Create a manager for each container
        containers.forEach(container => {
            this.uploadManagers.push(new FileUploadManager(container));
        });
    }

    // Get a specific upload manager by its container element
    getManagerByContainer(container) {
        return this.uploadManagers.find(manager => manager.container === container);
    }
}