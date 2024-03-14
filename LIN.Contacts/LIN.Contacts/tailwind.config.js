/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["D:/LIN Services/Components/LIN.Emma.UI/**/*{html,razor,js,cs}", "../**/*{html,razor,js,cs}"],
    theme: {
        screens: {
            'sm': '640px',
            'md': '768px',
            'dl': '910px',
            'lg': '1024px',
            'xl': '1280px',
            '2xl': '1536px',
        },
        extend: {

            colors: {

                'current': {
                    '50': '#f4f6fb',
                    '100': '#e8ecf6',
                    '200': '#ccd8eb',
                    '300': '#9fb8da',
                    '400': '#6b92c5',
                    '500': '#446ea6',
                    '600': '#365b93',
                    '700': '#2d4a77',
                    '800': '#283f64',
                    '900': '#263754',
                    '950': '#192438'
                }
            }        },
  },
    plugins: []
}

