import { defineConfig } from 'astro/config';
import preact from '@astrojs/preact';
import mdx from "@astrojs/mdx";

import tailwind from "@astrojs/tailwind";

// https://astro.build/config
export default defineConfig({
  integrations: [
    // Enable Preact to support Preact JSX components.
    preact(),
    , mdx(), tailwind()],
  site: `https://Joa-Launcher.github.io`,
  base: `Api-Documentation`
});
