OpenSIMKit-Mono
===============

This repo represents the development of OpenSIMKit using the Mono. The tools used in development include:

* [The Mono Runtime](http://www.mono-project.com/Main_Page). The Mono Runtime allows us to create and execute applications across various platforms or operating systems.
* [MonoDevelop](http://monodevelop.com/). MonoDevelop is being used for project management and as a coding tool.
* [The Glade UI Designer](http://glade.gnome.org/). We use Glade to create interfaces and embed them as GTK# resources on MonoDevelop using [Mono's Glade libraries](http://www.mono-project.com/GtkSharpBeginnersGuide#Your_First_Glade.23_Application).
* We have embedded [COMEX libraries for Mono](https://code.google.com/p/comex-project/). This is to allow us to work with [PC/SC](http://pcsclite.alioth.debian.org/) libraries as well as the serial ports (on Linux / Windows / Unix systems).

## How the tools work together

Glade is used to design the UI. The UI is stored as an XML and embedded within a MonoDevelop GTK# project as a resource. We store these UI definitions within the glade-gui folder, and store the logic that processes the various UI elements within the "User Interface" folder.

COMEX is placed within its own folder but its libraries are called within the application. We had modified COMEX to work with the Mono Framework without much effort. We intend to build the logging framework from scratch, currently we default to console logging.

Also the reference to System.Xml library also has to be embedded within the application's references.