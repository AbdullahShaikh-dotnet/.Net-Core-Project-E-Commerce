const toast = {
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
    }

};