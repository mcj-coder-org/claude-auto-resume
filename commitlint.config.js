export default {
  extends: ['@commitlint/config-conventional'],
  rules: {
    'type-enum': [
      2,
      'always',
      ['feat', 'fix', 'perf', 'refactor', 'docs', 'test', 'build', 'ci', 'chore', 'revert', 'style'],
    ],
    'scope-case': [2, 'always', 'lower-case'],
    'subject-case': [2, 'always', 'lower-case'],
    'subject-empty': [2, 'never'],
    'subject-full-stop': [2, 'never', '.'],
    'header-max-length': [2, 'always', 100],
    'references-empty': [2, 'never'],
  },
  parserPreset: {
    parserOpts: {
      issuePrefixes: ['#', 'GH-', 'AB#'],
    },
  },
  plugins: [
    {
      rules: {
        'references-empty': ({ references }) => {
          const valid = references && references.length > 0;
          return [valid, 'commit must reference a work item (e.g., "Refs: #123")'];
        },
      },
    },
  ],
};
