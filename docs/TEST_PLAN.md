# Test plan and actual results

Environment: Windows 10 x64; Visual Studio 2026 Community 18; .NET SDK 10.0.401; target net8.0-windows with installed WindowsDesktop 10.0.12. Release build: **PASS, 0 warnings, 0 errors**. Solution opened in Visual Studio; accessibility tree confirmed title and PokemonMemoryBattle debug target. Actual executed checks: **43 PASS**, recorded in TEST_RESULTS.txt. These are model/service checks and real STA WinForms handler/timer tests; they are not a claim of manual desktop clicks. Runtime form screenshots were captured by DrawToBitmap and visually inspected.

Run `tests/Run-Tests.ps1` on Windows. The test runner opens real forms and processes their message loop with Application.DoEvents only within the test harness; production gameplay uses Application.Run and WinForms Timer. Pump uses elapsed wall time, not Thread.Sleep. Tests isolate score writes in a temporary directory. The first short countdown assertion was corrected to wait over one active second rather than relying on incidental UI overhead; final run passes.

| Case | Steps | Expected | Actual |
|---|---|---|---|
| Decks | Reset Easy, Normal, Hard; group creature IDs | 4/6/16 cards; exactly two each | PASS |
| Shuffle | Compare seeds 2 and 3 | Different valid permutations | PASS |
| Start/input | Click Ready, Start, duplicate same card, rapid extra clicks | Ready ignored; duplicate ignored; maximum two during mismatch | PASS |
| Scoring/win | Select every known pair in all modes | +100/pair plus seconds×5; Won prevents input | PASS |
| Penalty | First mismatch at score zero | Score remains zero | PASS |
| Countdown | Pump real WinForms clock beyond one active second | Remaining decreases | PASS |
| Pause mismatch | Mismatch, pause, pump 1100ms, resume | Both clocks frozen while paused; delay completes after resume | PASS |
| Restart | Restart before mismatch Timer fires, pump past delay | Fresh Ready grid remains concealed | PASS |
| Timeout | Real board with one-second test difficulty | Lost at zero | PASS |
| UI generation | Open 2×2; Normal/Hard at 900×640 | Cards fit with positive usable dimensions | PASS |
| Save/name | Blank rejected; GameOver text box receives UI Tester, Save PerformClick | File gets chosen name once; button disabled | PASS |
| Leaderboard | Save tied scores, reload, bind grid | Score↓ moves↑ time↑ date↑; saved rows displayed | PASS |
| Missing assets | Resource service points to empty temporary root | Warnings collected; drawn fallback / skipped audio | PASS |
| Invalid assets | Load bad PNG and WAV | Catch and continue | PASS |
| Corrupt rows | Append bad JSON, null name, invalid score row | Warn; skip bad lines | PASS |
| Missing scores | Load nonexistent thanhtichs.txt | Warn; empty file created | PASS |
| Access failure | Primary thanhtichs.txt path is a directory | Switch to temporary writable fallback; save succeeds | PASS |
| Sound/close | Toggle off/on; Feedback; close forms and dispose players | No thrown error; resources released | PASS for calls/cleanup; audible output NOT VERIFIED |
| Help/menu/about | Instantiate, Show, pump and close actual forms | Visible forms, live controls | PASS |
| Modal warning | Rename img/bg or file in isolated compiled copy; launch and acknowledge MessageBox | Human-readable warning and playable fallback | NOT VERIFIED visually; production warning callback and catches tested |
| Physical display modes | Run at 1366×768 and 1920×1080, 100/150/200% DPI | No clipped controls, readable board | NOT VERIFIED at native screen modes; 900×640 layout checked |
| Human video/mic | Record 3–5 minutes with a member speaking | Complete narrated video | FAIL: no recording produced |

## Reproducible manual resource trap
1. Build Release. Copy the complete net8.0-windows output to a disposable demo folder. Never rename assets in the source tree during grading.
2. Close that copy. Rename img to img.saved and launch it. Open Start Game. Observe warnings for missing folder and images. Acknowledge; Start and finish Easy using labelled fallback cards.
3. Close; restore img. Repeat with bg renamed; acknowledge audio/card-back warnings. Gameplay must continue silently with drawn backs.
4. Restore bg. Rename thanhtichs.txt to thanhtichs.saved; open Leaderboard. Expect warning and a recreated empty file. Save a new named result and reload it.
5. To test a corrupt file, add one invalid JSON line in the disposable copy. Expect skipped-line warning and valid rows retained.
6. Restore assets after recording. For mid-game removal, note images and WAVs already loaded are cached; close and reopen the affected form/app to force another disk read. The existing board should keep running.

## Verification limits
The desktop automation helper could enumerate apps and read accessibility trees but capture timed out and mouse input returned unavailable geometry. Therefore no manual mouse-based live workflow, audible playback, or modal screenshot is claimed. App-level DrawToBitmap renders and real event-handler/Timer checks succeeded. A real presentation-machine smoke test remains necessary.
