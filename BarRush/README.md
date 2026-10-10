# Bar Rush (Cities: Skylines 2 mod prototype)

Goal: turn entertainment venues into bars, have sims linger around them later,
trigger a rush at midnight, run DJ nights with an on-screen announcement, and
swap a bar's company after a crime.

## Status

None of this has been compiled or run in the game. Lines tagged `VERIFY` in the
`.cs` files name game types and fields to check in a decompiler.

| Piece | File | State |
| --- | --- | --- |
| Mod entry point, in-game options page | `Mod.cs`, `Setting.cs` | Written from the standard template pattern |
| Midnight rush | `BarRushSystem.cs` | Written, needs the clock and leisure types confirmed |
| Stay longer | `Outings.cs` | Only applies to outings this mod starts, not vanilla outings |
| Attendance cap (never more than half the city out at once) | `Outings.cs` | Written; shared by the rush and DJ events |
| DJ events with random names | `DjEventSystem.cs`, `DjNames.cs` | Written; sims are not yet steered to the announced bar |
| DJ popup (DJ, bar, sim count) | `DjAnnouncementUISystem.cs`, `ui/DjPopup.tsx` | Written; needs the UI mod setup below |
| Blue tab button that opens the Bars panel (all bars + events happening now) | `BarListUISystem.cs`, `ActiveEvents.cs`, `ui/BarPanel.tsx` | Written; needs the UI mod setup below; the button's mount point may need moving |
| Thank-you panel with Exit button when a save loads | `BarListUISystem.cs`, `ui/BarPanel.tsx` | Written with two load-detection routes; can be turned off in options |
| Crime changes a bar's company | `CrimeCompanySwapSystem.cs` | Written; the game picks the new tenant, which may not be a bar |
| Bars = entertainment businesses | `BarFinder.cs` | Written, needs the company/prefab types confirmed |
| Sims stay outside bars | | Not implemented (see the note in `BarRushSystem.cs`) |
| Entertainment venues look like bars | | Not implemented (assets, separate from the code mod) |

## About the attendance cap

The request said attendance should stay "below 2 times below" the population.
This is read as fewer than half the population. The cap covers everything this
mod has running at once (midnight rush plus every DJ event), and the options
page lets you lower it from 50% (default 25%).

## Setup

1. In Cities: Skylines 2, open Options > Modding and install the modding toolchain.
2. Use the in-game mod template in Visual Studio 2022 or Rider to create a
   project (enable the settings option).
3. Replace the generated `Mod.cs` and `Setting.cs` with these, and add the other
   `.cs` files. If the template's namespace differs, rename `BarRush` to match.
4. In the `.csproj`, add `<LangVersion>9</LangVersion>` (the template targets
   net472 and defaults to an older C# that rejects `using` declarations).
5. For the popup, generate the UI part of the project with the template's UI
   option (`npx create-csii-ui-mod`), then copy `ui/DjPopup.tsx` and `ui/BarPanel.tsx` into its `src`
   folder and use `ui/index.tsx` as its entry point. The binding group name is
   `BarRush` on both sides.
6. Decompile `Cities2_Data\Managed\Game.dll` with ILSpy or dotPeek, then resolve
   each `VERIFY` comment.
7. Build, launch the game, and check the log for the "Midnight bar rush",
   "DJ event" and "Crime at a bar" lines.

## If the panel does not open on load

Check the game's log for these lines, which say where it stopped:

- "Bar panel system created." means the C# side loaded. If it is missing, the
  mod itself did not load or `BarListUISystem` was not registered in `Mod.cs`.
- "Bar panel: load callback fired" or "city detected from the frame counter"
  means a load was noticed. If neither appears, neither detection route worked
  and the `OnGameLoadingComplete` signature needs checking in a decompiler.
- "Bar panel: opened with the thank-you message." means the C# side did its job.
  If you still see nothing, the UI half is not built into the mod, or its mount
  point (`GameTopLeft` in `ui/index.tsx`) is wrong. See setup step 5.
- If the options page has "Don't open Bars panel on load" turned on, the
  panel stays closed, though the blue tab button still opens it.

The blue tab button is pinned to the top-centre of the screen and the panel opens
just under it, so where the game mounts the component does not change where they
appear. To move them, edit the `top`, `left` and `marginLeft` values in
`ui/BarPanel.tsx`. If nothing shows at the top at all, the UI half is not being
loaded (setup step 5) or the mount point name in `ui/index.tsx` is wrong for your
game version.

Options saved by an older build can also hide features. The mod now resets the
options to defaults when it sees a saved file that predates the newer ones.

## Next steps

1. Get it compiling and confirm sims actually leave home when an outing starts.
2. Steer outing sims to the specific bar named in the DJ popup.
3. Tackle "sims stay outside", which is the hard part.
