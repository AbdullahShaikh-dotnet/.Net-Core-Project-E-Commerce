﻿const toast = {
    success: (message, timeout = 5000) => {
        iziToast.success({
            title: 'Success',
            titleColor: '#064e3b', // Tailwind 'emerald-900'
            message: message,
            position: 'topCenter',
            theme: 'light',
            backgroundColor: '#d1fae5', // Tailwind 'emerald-100'
            messageColor: '#065f46', // Tailwind 'emerald-700'
            timeout: timeout * 1000, // Convert seconds to milliseconds
            layout: 1,
            close: true,
            progressBarColor: '#10b981', // Tailwind 'emerald-500'
            transitionIn: 'fadeInUp',
            transitionOut: 'fadeOut',
            drag: true
        });
    },

    error: (message, timeout = 5000) => {
        iziToast.error({
            title: 'Error',
            titleColor: '#7f1d1d', // Tailwind 'red-900'
            message: message,
            position: 'topCenter',
            theme: 'light',
            backgroundColor: '#fee2e2', // Tailwind 'red-100'
            messageColor: '#991b1b', // Tailwind 'red-700'
            timeout: timeout * 1000,
            layout: 1,
            close: true,
            progressBarColor: '#ef4444', // Tailwind 'red-500'
            transitionIn: 'fadeInUp',
            transitionOut: 'fadeOut',
            drag: true
        });
    },

    warning: (message, timeout = 5000) => {
        iziToast.warning({
            title: 'Warning',
            titleColor: '#78350f', // Tailwind 'amber-900'
            message: message,
            position: 'topCenter',
            theme: 'light',
            backgroundColor: '#fef3c7', // Tailwind 'amber-100'
            messageColor: '#b45309', // Tailwind 'amber-700'
            timeout: timeout * 1000,
            layout: 1,
            close: true,
            progressBarColor: '#f59e0b', // Tailwind 'amber-500'
            transitionIn: 'fadeInUp',
            transitionOut: 'fadeOut',
            drag: true
        });
    },

    info: (message, timeout = 5000) => {
        iziToast.info({
            title: 'Info',
            titleColor: '#1e3a8a', // Tailwind 'blue-900'
            message: message,
            position: 'topCenter',
            theme: 'light',
            backgroundColor: '#dbeafe', // Tailwind 'blue-100'
            messageColor: '#1e40af', // Tailwind 'blue-700'
            timeout: timeout * 1000,
            layout: 1,
            close: true,
            progressBarColor: '#3b82f6', // Tailwind 'blue-500'
            transitionIn: 'fadeInUp',
            transitionOut: 'fadeOut',
            drag: true
        });
    },

    custom: (message, title = 'Info', timeout = 5000) => {
        iziToast.info({
            title: title,
            titleColor: '#1e1e1e',
            message: message,
            position: 'bottomRight',
            theme: 'light',
            backgroundColor: '#fff',
            messageColor: '#1e1e1e',
            timeout: timeout * 1000,
            layout: 2,
            close: true,
            progressBarColor: '#1e1e1e',
            transitionIn: 'fadeInUp',
            transitionOut: 'fadeOut',
            drag: true,
            animateInside: true,
            displayMode: 0,
            maxWidth: "500px",
            closeOnClick: true,
            progressBarEasing: 'linear',
            iconUrl: "/images/Icons/Bell-Icon.svg"
        });
    },

    basic: (message, position = 'bottom-right') => {
        const existingToasts = document.querySelectorAll('.custom-basic-toast');
        existingToasts.forEach(t => t.remove());

        // Map position to Tailwind classes
        const positionClasses = {
            'top-left': 'top-5 left-5',
            'top-right': 'top-5 right-5',
            'bottom-left': 'bottom-5 left-5',
            'bottom-right': 'bottom-5 right-5'
        };

        const posClass = positionClasses[position] || positionClasses['bottom-right'];
        const toast = document.createElement('div');
        toast.className = `custom-basic-toast fixed ${posClass} bg-gray-800 text-white px-4 py-2 rounded shadow-lg z-50 transition-all duration-300 ease-in-out opacity-0 translate-y-2`;
        toast.textContent = message;

        document.body.appendChild(toast);
        requestAnimationFrame(() => {
            toast.classList.remove('opacity-0', 'translate-y-2');
            toast.classList.add('opacity-100', 'translate-y-0');
        });
        setTimeout(() => {
            toast.classList.remove('opacity-100', 'translate-y-0');
            toast.classList.add('opacity-0', 'translate-y-2');

            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }

};