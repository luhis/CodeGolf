module.exports = {
    env: {
        browser: true
    },
    plugins: ["functional"],
    extends: [
        "plugin:react/recommended",
        "plugin:@typescript-eslint/recommended",
        "prettier/@typescript-eslint",
        //"plugin:prettier/recommended",
        "plugin:functional/external-recommended",
        "plugin:functional/recommended"
    ],
    parser: "@typescript-eslint/parser",
    parserOptions: {
        ecmaFeatures: {
            jsx: true
        },
        ecmaVersion:  2018,
        project: "tsconfig.json",
        sourceType: "module",
    },
    rules: {
        "react/no-unknown-property": ["error", { ignore: ["class"] }],
        "@typescript-eslint/explicit-function-return-type": "off",
        "@typescript-eslint/no-unused-vars": ["error", { "varsIgnorePattern": "[\_]+" }],
        "react/prop-types": "off",
        "@typescript-eslint/no-unused-vars": ["error", { "argsIgnorePattern": "^_" }],

        "@typescript-eslint/no-floating-promises": "error",
        "functional/no-return-void": "off",
        "functional/prefer-type-literal": "off",
        "functional/no-expression-statement": "off",
        "functional/functional-parameters": "off",
        "functional/no-conditional-statement": "off",
        "functional/no-mixed-type": "off",
        "functional/no-try-statement": "off",
        "functional/no-throw-statement": "off",

        "no-multi-spaces": "error"
    },
    settings: {
        react: {
            pragma: "h",
            version: "detect"
        },
    },
    overrides: [
        {
            files: ["*.js"],
            rules: {
                "@typescript-eslint/explicit-function-return-type": "off",
            }
        }
    ]
};
