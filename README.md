# DotFed

This is primarily going to be used to experiment in C#, Go and Zig while also being extremely efficent for my choices of tech stack. 


This is all going to be hosted via Kind then Kubernetes. I will write the full setup on how I did all of this and my considerations on my blog.

# Tech stack


Backend: I am using C# Razor files with a Minimal API because of the speed and less design patterns that I find unnatural.


Frontend: HTMX + Hyperscript + Twind for general use and then Zig WebAssembly for a client side worker.


Worker: Currently it is being written in Go as it has good support with RabbitMQ, JSON and Postgres while being fast but I plan on switching to Zig by making my own implementations in all of these pain points.


Mobile: Expo + Zig WASM


API: A C# Minimal API because of the speed and ease of use.


Devops: Github Actions + ghcr.io + Kubernetes.


# More info / Questions?
I am #DotRacc on basically all platforms (Discord + Snapchat + Instagram)

I am also writing a blog post on how I did all of this and my thought process behind the code. This will be on [DotKey](https://dotkey.dev) when it is ready.





P.S I already own DotFed.com, I am fully committed to making this very complicated project work.

