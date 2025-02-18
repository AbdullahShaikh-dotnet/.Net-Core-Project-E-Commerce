/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml',
        './Areas/**/*.cshtml',
        './Views/Shared/*.cshtml',
        "./wwwroot/js/**/*.js"
    ],
    theme: {
        extend: {},
    },
    plugins: [require('@tailwindcss/line-clamp')],
}

