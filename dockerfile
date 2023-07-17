FROM scratch as ZIG
WORKDIR /src/wasm_modules



FROM oven/bun as JS
WORKDIR /
COPY ["package.json", "package-lock.json*", "./"]
RUN bun install --production
COPY . .
run vite build
WORKDIR /build
CMD [ "bun", "run start" ]
