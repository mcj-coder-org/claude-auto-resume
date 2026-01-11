import { themes as prismThemes } from "prism-react-renderer";
import type { Config } from "@docusaurus/types";
import type * as Preset from "@docusaurus/preset-classic";

const config: Config = {
  title: "Claude Auto Resume",
  tagline: "Automatic resume functionality for Claude CLI",
  favicon: "img/favicon.ico",

  url: "https://mcj-coder-org.github.io",
  baseUrl: "/claude-auto-resume/",

  organizationName: "mcj-coder-org",
  projectName: "claude-auto-resume",

  onBrokenLinks: "throw",
  onBrokenMarkdownLinks: "warn",

  i18n: {
    defaultLocale: "en-GB",
    locales: ["en-GB"],
  },

  presets: [
    [
      "classic",
      {
        docs: {
          sidebarPath: "./sidebars.ts",
          editUrl:
            "https://github.com/mcj-coder-org/claude-auto-resume/tree/main/docs/docusaurus/",
        },
        blog: false,
        theme: {
          customCss: "./src/css/custom.css",
        },
      } satisfies Preset.Options,
    ],
  ],

  themeConfig: {
    navbar: {
      title: "Claude Auto Resume",
      items: [
        {
          type: "docSidebar",
          sidebarId: "docsSidebar",
          position: "left",
          label: "Documentation",
        },
        {
          href: "https://github.com/mcj-coder-org/claude-auto-resume",
          label: "GitHub",
          position: "right",
        },
      ],
    },
    footer: {
      style: "dark",
      links: [
        {
          title: "Docs",
          items: [
            {
              label: "Getting Started",
              to: "/docs/intro",
            },
          ],
        },
        {
          title: "More",
          items: [
            {
              label: "GitHub",
              href: "https://github.com/mcj-coder-org/claude-auto-resume",
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} McjCoderOrg. Built with Docusaurus.`,
    },
    prism: {
      theme: prismThemes.github,
      darkTheme: prismThemes.dracula,
      additionalLanguages: ["csharp", "powershell", "bash", "json", "yaml"],
    },
  } satisfies Preset.ThemeConfig,
};

export default config;
