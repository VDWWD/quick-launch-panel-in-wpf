# Quick Launch Panel in WPF

Are you using a lot of the same programs or open the same files over and over again? Usually you create a shortcut on the desktop. But now you can also put them in a simple WPF program that is always on top of other programs so you can trigger your shortcut with one simple click.

----------

The extra libraries needed to run this app are included as an embedded resource so the app can run as a single .exe file in a folder without dll's.
These are loaded in the App.xaml.cs > Main() with AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>

This project also uses the 2 following library (https://www.nuget.org/packages/StickyWindows.WPF/0.3.0-unstable0009 and https://www.nuget.org/packages/PixiEditor.ColorPicker) but is not installed with the package manager.
To include libraries as resource they need to be strongly named but StickyWindows was not when installing from nuget. And PixiEditor did not work from nuget but did when compiling it myself.

More info: https://www.vanderwaal.eu/mini-projecten/quick-launch-panel-in-wpf

&nbsp;

<img src="https://www.vanderwaal.eu/files/quick-launch-panel-in-wpf.jpg">
