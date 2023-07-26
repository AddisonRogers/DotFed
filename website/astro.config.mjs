import { defineConfig } from 'astro/config';
import svelte from '@astrojs/svelte';
import tailwind from "@astrojs/tailwind";
import sitemap from "@astrojs/sitemap";
import purgecss from "astro-purgecss";

import deno from "@astrojs/deno";

// https://astro.build/config
export default defineConfig({
  // Enable Svelte to support Svelte components.
  integrations: [svelte(), tailwind(), sitemap(), purgecss()],
  output: "server",
  adapter: deno({})
});