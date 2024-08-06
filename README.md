# ASCII-Clock

A C# program that displays an analog clock in a terminal. Comes with configurable symbols for each hand and the dial, hand lengths and dial radius.

<p align='center' height='24em'>
    <img src='http://stopa-averyanov.com/gifs/ascii_clock_loop_white.gif'>
</p>

It is also responsive to the changes of the window dimensions/font size in real time.

<p align='center' height='24em'>
    <img src='http://stopa-averyanov.com/gifs/ascii_clock_resize.gif'>
</p>

## Getting started

Requires .NET 8.0 (but will probably work with older versions). Visit [https://dotnet.microsoft.com/en-us/download](https://dotnet.microsoft.com/en-us/download) to get it.

To build, use

```shell
$ dotnet build
```

## Configuration

When launched, the program generates a `parameters.json` file or reads it if it exists. It contains the following keys:

* `dialSymbol` — the symbol used to draw the circle of the dial
* `secondHandSymbol` — the symbol used to draw the second hand
* `minuteHandSymbol` — the symbol used to draw the minute hand
* `hourHandSymbol` — the symbol used to draw the hour hand
* `dialRadius` — the radius of the dial relative to the window (0 is 0, 1 is half the window height)
* `secondHandLength` — the length of the second hand relative to the window
* `minuteHandLength` — the length of the minute hand relative to the window
* `hourHandLength` — the length of the hour hand relative to the window
* `smooth` — controls whether the hands go smoothly or have a step to them (`true` by default)
* `renderLastLine` — controls whether to output the last line of the framebuffer to the terminal. Can fix jitter on some terminals (like Windows' cmd.exe)