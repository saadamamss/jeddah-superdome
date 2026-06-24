/**
 * .eslintrc.js
 *
 * ESLint configuration file.
 */

module.exports = {
  root: true,
  env: {
    node: true,
  },
  parser: 'vue-eslint-parser',
  parserOptions: {
    parser: '@typescript-eslint/parser',
    ecmaVersion: 2022,
    sourceType: 'module',
  },
  extends: [
    'vuetify',
    './.eslintrc-auto-import.json',
  ],
  rules: {
    'vue/multi-word-component-names': 'off',
    '@typescript-eslint/no-unused-vars': ['warn', { argsIgnorePattern: '^_' }],
    '@typescript-eslint/no-explicit-any': 'error',
    '@typescript-eslint/explicit-function-return-type': 'off',
    '@typescript-eslint/explicit-module-boundary-types': 'off',
    '@typescript-eslint/no-non-null-assertion': 'error',
    eqeqeq: ['error', 'always'],
    'func-call-spacing': 'off',
    'vue/attributes-order': 'off',
    'vue/custom-event-name-casing': 'off',
  },
  overrides: [
    {
      files: ['src/@core/**', 'src/@layouts/**', 'src/@iconify/**', 'src/@index.vue'],
      rules: {
        '@typescript-eslint/no-explicit-any': 'off',
        '@typescript-eslint/no-unused-vars': 'off',
        'no-unused-vars': 'off',
        camelcase: 'off',
        eqeqeq: 'off',
        'vue/attributes-order': 'off',
        'vue/require-prop-type-constructor': 'off',
        'vue/require-component-is': 'off',
        'vue/require-toggle-inside-transition': 'off',
        'no-async-promise-executor': 'off',
        'sonarjs/cognitive-complexity': 'off',
        'array-callback-return': 'off',
        'func-call-spacing': 'off',
        'no-useless-escape': 'off',
        'no-return-await': 'off',
        'vue/no-template-shadow': 'off',
        'vue/no-unused-vars': 'off',
      },
    },
  ],
}
