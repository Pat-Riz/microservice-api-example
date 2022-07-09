/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{js,jsx,ts,tsx}"],
  theme: {
    extend: {
      animation: {
        "spin-slow": "spin 3s linear infinite",
        "enter-right": "enter 1s ease-in",
        fadeIn: "hide 0.6s ease-in 0.7s backwards",
        "fadeIn-quick": "hide 0.3s ease-in",
      },
      keyframes: {
        enter: {
          "0%": { opacity: 0, transform: "translateX(30rem)" },
          "80%": { transform: "translateX(-2rem)" },
          "100%": { opacity: 1, transform: "translateX(0)" },
        },
        hide: {
          "0%": { opacity: 0 },
          "100%": { opacity: 1 },
        },
      },
    },
  },
  plugins: [],
};
