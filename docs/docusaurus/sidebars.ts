import type { SidebarsConfig } from "@docusaurus/plugin-content-docs";

const sidebars: SidebarsConfig = {
  docsSidebar: [
    "intro",
    {
      type: "category",
      label: "Getting Started",
      items: ["getting-started/installation", "getting-started/quick-start"],
    },
    {
      type: "category",
      label: "Guides",
      items: ["guides/configuration", "guides/usage"],
    },
    {
      type: "category",
      label: "Contributing",
      items: [
        "contributing/development-setup",
        "contributing/coding-standards",
        "contributing/testing",
      ],
    },
    {
      type: "category",
      label: "Architecture",
      items: ["architecture/overview", "architecture/decisions"],
    },
  ],
};

export default sidebars;
