# Bar Rush: start here

This folder holds the whole mod, organized so you can copy it into your project.
It is not a drop-in mod: Cities: Skylines 2 only loads mods that have been built
(compiled), so the files below go into a project that you build. Building puts
the finished mod into the game's own Mods folder for you.

## What is in here

- `Code/` is the C# part of the mod.
  - `Mod.cs` and `Setting.cs` are the entry point and the options page.
  - `Systems/` holds the parts that do things: the midnight rush, DJ nights, the
    crime company swap, the Bars panel's backend, and the DJ popup's backend.
  - `Helpers/` holds shared pieces: finding bars, the attendance cap, DJ names,
    and the list of live events.
- `UI/` is the on-screen part: the blue tab button, the Bars panel, and the DJ
  popup (all `.tsx` files).
- `README.md` has the full status list and a troubleshooting checklist.

## If you already have a project from the game's template

1. Copy everything inside `Code/` into your project, keeping the sub-folders.
   Replace your `Mod.cs` and `Setting.cs` with the ones here. Delete any older
   copies of these files elsewhere in the project so nothing is defined twice.
2. Copy the three files in `UI/` into the `src` folder of your UI project and
   make sure `index.tsx` is the one the UI project uses as its entry point.
   Delete `BarListPanel.tsx` if you still have it from an earlier version.
3. Open your project's `.csproj` and add `<LangVersion>9</LangVersion>` near the
   top (inside the first `PropertyGroup`) if it is not there already.
4. Build the project (and the UI project, if it builds separately).
5. Start the game, make sure the mod is enabled, and load a save.

## If you are starting from scratch

1. In the game, install the modding toolchain from the Options menu.
2. In Visual Studio 2022 or Rider, create a project from the game's mod
   template, with the settings option turned on.
3. Create the UI part with the template's UI option (`npx create-csii-ui-mod`).
4. Then follow the steps above, starting at step 1.

## When something does not show up

Look at the game's log for lines that start with "Bar panel". The README has a
short checklist that says what each line means. Parts of this mod were written
without access to the game's own code, so the first build may show errors; the
spots most likely to need a tweak are marked `VERIFY` in the code.
