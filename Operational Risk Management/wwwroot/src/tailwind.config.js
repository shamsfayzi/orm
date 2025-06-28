/** @type {import('tailwindcss').Config} */
module.exports = {
    darkMode: "light",
    content: [
        "../../Views/**/*.{cshtml,js}",
        "../../Areas/**/*.{cshtml,js}",
        './*.{html,js}',
        './node_modules/preline/dist/*.js',
    ],
    theme: {
        extend: {
            colors: {
                orange: {
                    50: '#fff2e6',
                    100: '#ffddb3',
                    200: '#ffc180',
                    300: '#ffa34d',
                    400: '#ff8626',
                    500: '#ff6a00', 
                    600: '#e66000',
                    700: '#cc5500',
                    800: '#b34a00',
                    900: '#993f00',
                },
                yellow: {
                    50: '#FFFDE7',
                    100: '#FFF9C4',
                    200: '#FFF59D',
                    300: '#FFF176',
                    400: '#FFEE58',
                    500: '#FFEB3B',
                    600: '#FDD835',
                    700: '#FBC02D',
                    800: '#F9A825',
                    900: '#F57F17',
                },
                blue: {
                    50: '#E0F2FF',
                    100: '#B3D4FF',
                    200: '#80B4FF',
                    300: '#4D93FF',
                    400: '#2679FF',
                    500: '#005FD4',
                    600: '#0048A7',
                    700: '#00327A',
                    800: '#001C4D',
                    900: '#005294',
                },
                green: {
                    50: '#E0F7EC',
                    100: '#B3ECCF',
                    200: '#80E0B0',
                    300: '#4DD490',
                    400: '#26C879',
                    500: '#00BD61',
                    600: '#00994F',
                    700: '#00753E',
                    800: '#004F2C',
                    900: '#00A94F',
                }
            },
            width: {
                '50': '14rem',  // 48px or 3rem
                '10': '2.5rem',
                '22': '22rem'
            },
            height: {
                '10': '2.5rem',
            },
            margin: {
                '1': "1rem",
                '2': "2rem",
                '3': "3rem",
                '4': "4rem",
                '5': "5rem",
            },
            gap: {
                '3': '0.75rem',
                '4': '1rem',
                '5': '1.25rem',
            },
            justifyContent: {
                'right': 'flex-end',
                'left': 'flex-start',
            },
            padding: {
                'ps': {
                    '1': '0.25rem',
                    '2': '0.5rem',
                    '3': '0.75rem',
                    '4': '1rem',
                    '5': '1.25rem',
                    '6': '1.5rem',
                    '7': '1.75rem',
                    '8': '2rem',
                    '9': '2.25rem',
                    '10': '2.5rem',
                },
                'pe': {
                    '1': '0.25rem',
                    '2': '0.5rem',
                    '3': '0.75rem',
                    '4': '1rem',
                    '5': '1.25rem',
                    '6': '1.5rem',
                    '7': '1.75rem',
                    '8': '2rem',
                    '9': '2.25rem',
                    '10': '2.5rem',
                },
                'pt': {
                    '1': '0.25rem',
                    '2': '0.5rem',
                    '3': '0.75rem',
                    '4': '1rem',
                    '5': '1.25rem',
                    '6': '1.5rem',
                    '7': '1.75rem',
                    '8': '2rem',
                    '9': '2.25rem',
                    '10': '2.5rem',
                },
                'pb': {
                    '1': '0.25rem',
                    '2': '0.5rem',
                    '3': '0.75rem',
                    '4': '1rem',
                    '5': '1.25rem',
                    '6': '1.5rem',
                    '7': '1.75rem',
                    '8': '2rem',
                    '9': '2.25rem',
                    '10': '2.5rem',
                },
            },
            borderRadius: {
                'l-lg': '0.5rem 0 0 0.5rem', // Left-rounded large
                'r-lg': '0 0.5rem 0.5rem 0', // Right-rounded large
            },

        }
    },
    safelist: [
        {
            pattern: /orange-(50|100|200|300|400|500|600|700|800|900)/,
        },
        {
            pattern: /yellow-(50|100|200|300|400|500|600|700|800|900)/,
        },
        {
            pattern: /blue-(50|100|200|300|400|500|600|700|800|900)/,
        },
        {
            pattern: /green-(50|100|200|300|400|500|600|700|800|900)/,
        },
        {
            pattern: /red-(50|100|200|300|400|500|600|700|800|900)/,
        },
    ],
    plugins: [
        require('@tailwindcss/forms'),
        require('preline/plugin'),
    ],
};
