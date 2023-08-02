# DotFed

This is primarily going to be showing off my skill in the Dotnet eco system rather than a fully functioning website.

I am using Razor pages + Minimal API made by a wonderful developer in the dotnet team.

HTMX + Hyperscript + Twind for any clientside rendering and such.

For data that is being accessed I am running a postgresSQL database which is used alongside a RabbitMQ messaging service which messages a worker service to update information.



This is all going to be hosted via Podman-Compose then later when I am ready K8s. 



Stuff that can be seen in the code so far:
- Worker service is almost fully done just missing web scraping and connection to the database (5 - 10 lines of code)
- A basic routing system for the minimal API
- A working prototype of HTMX in Razor Pages
- Like 60 commits showing my pain and misery as I go through all the different ways to do this.


# More info / Questions?
I am #DotRacc on basically all platforms (Discord + Snapchat + Instagram)

I am also writing a blog post on how I did all of this and my thought process behind the code. This will be on [DotKey](https://dotkey.dev) when it is ready.



P.S I already own DotFed.com, I am fully committed to making this very complicated project work.

