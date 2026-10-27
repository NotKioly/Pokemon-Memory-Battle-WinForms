# Requirements checklist

Primary source: lecturer document “Lich bao cao + huong dan.docx”, read in full including tables. Course 2611COMP101901. No original WIN_T4_TrucXanh source was supplied. This table distinguishes implementation from real-world submission and manual verification. PASS means implemented and verified as stated; FAIL means a deliverable or supplied information is absent; NOT VERIFIED means an external/manual confirmation remains.

## Conflicts and interpretation
The lecturer calls Section 2 7.0 points while its sub-items total 8.0; the overall labelled sections also do not sum consistently to 10. All listed criteria are retained without inventing weights. The selected three-person group is not identified; assignment roster names are not copied into placeholders. Pokémon art/music is replaced by original creature assets to satisfy redistribution rules. .NET 8 remains the target; major-runtime roll-forward permits the installed .NET 10 desktop runtime. UI is implemented with C# drawing/layout; students must review and be able to explain the assisted code, respecting the lecturer's warning against relying on generated UI without ownership. The exact group presentation slot cannot be inferred from group size.

| ID | Requirement | Implementation and relevant classes/methods | Verification | Status |
|---|---|---|---|---|
| L01 | Submit solution and complete source with Visual Studio gitignore | PokemonMemoryBattle.sln; .gitignore; src | dotnet build; inspect git tracked files | PASS |
| L02 | Source data img/ and matching card art | img/0.png–7.png; GameForm.ResetGame; ResourceManager.LoadImage | All three decks and cloned paths | PASS |
| L03 | bg/no-image.png and true.wav / false.wav | bg/; AudioManager constructor and Feedback | Inspect copied output; execute sound calls | PASS |
| L04 | Sample thanhtichs.txt using System.IO | thanhtichs.txt; LeaderboardService.Load/Save | Save/reload in temporary directory | PASS |
| L05 | 3–5 minute full narrated demo including resource failure | docs/VIDEO_RECORDING.md | Real member records and reviews final video | FAIL |
| L06 | Public GitHub or lecturer permission and complete commit | README; docs/DELIVERY.md; remote main | Verify remote tree after push | NOT VERIFIED |
| L07 | Enter repository URL in class Giữa kì column before 27/10/2026 23:59 | docs/DEMO_SCRIPT.md | Group confirms sheet submission separately | NOT VERIFIED |
| L08 | Attend 29/10/2026 from 20:00, max 10 minutes and arrive 10 minutes early | docs/DEMO_SCRIPT.md | Group confirms actual slot, equipment and attendance | NOT VERIFIED |
| L09 | Help and About with full name MSSV class and responsibilities | HelpForm; TEAM_CONTRIBUTIONS.md | Placeholders present; real identities not supplied | FAIL |
| L10 | Show board, score, click count, countdown and controls | GameForm.RefreshGame; MemoryCardControl.OnPaint | Runtime screenshots and handler tests | PASS |
| L11 | Start Pause Restart End Game smooth selection and small grid | GameForm; GameManager; Difficulty.All | Real timers and 2×2 completion | PASS |
| L12 | Background and event audio plus live toggle | AudioManager.Play/Toggle; GameForm sound handler | Playback calls pass; speaker audibility needs human | NOT VERIFIED |
| L13 | End game name popup and sorted DataGridView leaderboard | GameOverForm; LeaderboardForm | Name-entry save handler and grid rows tested | PASS |
| L14 | try/catch resource trap shows MessageBox and avoids crash | ResourceManager; AudioManager; LeaderboardService; MainMenuForm.Warn | Injected missing/corrupt resources pass; visual modal needs manual check | NOT VERIFIED |
| L15 | Explain generated Controls/Layout and matching/shuffling lock | GameForm.ResetGame; GameManager.Select/Reset | CODE_EXPLANATION and code inspection | PASS |
| L16 | Improve sample Sleep freeze, countdown, Noname | Timer handlers; GameOverForm | No Thread.Sleep in application; no original sample inspected | PASS |
| L17 | Individual Q&A Event Handler IO SoundPlayer Timer, mic/camera | DEMO_SCRIPT; TEAM_CONTRIBUTIONS | Prepared answers; actual rehearsal and viva pending | NOT VERIFIED |
| U01 | All required menu actions and difficulty selector | MainMenuForm constructor | All buttons have handlers; real form launch | PASS |
| U02 | Gameplay Start Pause Resume Restart Menu Sound | GameForm constructor / public action methods | Handler and timer tests | PASS |
| U03 | Result screen win/loss score moves remaining name save replay leaderboard | GameOverForm constructor | Real form name Save; navigation inspection | PASS |
| U04 | Ready Playing Paused Won Lost state machine | GameManager; GameState | Model and WinForms terminal tests | PASS |
| U05 | Programmatic paired grid and Fisher–Yates | GameManager.Reset; GameForm.ResetGame | Pair multiplicity and seeded shuffle tests | PASS |
| U06 | Only two selections; ignore revealed matched and terminal clicks | GameManager.Select; InputLocked | Rapid 50-click, duplicate and terminal tests | PASS |
| U07 | Nonblocking mismatch delay and no Thread.Sleep UI | GameForm flip Timer; Stopwatch mismatch | Message loop continues during pause/mismatch tests | PASS |
| U08 | Easy 2×2 Normal 2×3 Hard 4×4 playable | Difficulty.All | Each model wins; real Easy wins; Normal/Hard forms generated | PASS |
| U09 | Match points, optional penalty and victory time bonus floor zero | ScoreManager; GameManager.Select | Exact score equation and penalty tests | PASS |
| U10 | Countdown pause end reset and timeout | GameForm clock; GameManager.Tick | Real countdown, paused timer, restart and timeout | PASS |
| U11 | Rapid clicks pause pending flip restart pending event audio cleanup | GameManager.Select; GameForm.ResetGame/FormClosed; AudioManager.Dispose | Executed runtime checks plus disposal inspection | PASS |
| U12 | Original redistributable artwork and music | img; bg; ASSET_LICENSE; tools/generate_assets.py | Generated assets no downloaded commercial media | PASS |
| U13 | Relative paths and resource copy for build/publish | csproj Content; AppContext.BaseDirectory | Release output and publish inspect | PASS |
| U14 | Missing img/bg/cardback/audio/score safe fallback | ResourceManager.LoadImage/CheckDirectories; services | Actual injected failures with recorded warning callbacks | PASS |
| U15 | Invalid lines values null names access denial directories | LeaderboardService.Read/Load/Save | Corruption and blocked target fallback tests | PASS |
| U16 | Missing/invalid audio safely skipped no overlap preferred | AudioManager.Play/Feedback | Invalid WAV and missing audio executed; StopAll before feedback | PASS |
| U17 | Name validation 1–24, blank rejected, save once | LeaderboardService.ValidateName; GameOverForm | Blank-name test and real save handler | PASS |
| U18 | Sort score descending moves/time/date tie break | LeaderboardService.Read | Known ordered data checked | PASS |
| U19 | Writable fallback preserving bundled sample | LeaderboardService.Fallback; source thanhtichs.txt | Blocked primary path redirects to temporary fallback | PASS |
| U20 | Clean small classes no external UI packages | src structure; csproj | Source inspection | PASS |
| U21 | 15 beginner code topics explained with real source | docs/CODE_EXPLANATION.md | Compare class/method references against source | PASS |
| U22 | Actual build no compile errors and Visual Studio opens | Solution; build command | SDK10 Release 0 warnings/0 errors; VS18 title/debug target observed | PASS |
| U23 | Runtime tests explicitly separated from static/manual checks | docs/TEST_PLAN.md; TEST_RESULTS.txt | 43 executed checks and unresolved manual checks documented | PASS |
| U24 | Vietnamese seven chapter report and genuine code extracts | docs/REPORT.md; DOCX report | Extract implementation lines; render and inspect pages | NOT VERIFIED |
| U25 | 10 minute script Q&A plus separate recording checklist | DEMO_SCRIPT; VIDEO_RECORDING | Document inspection | PASS |
| U26 | Professional navy UI custom buttons hover pressed spacing | Theme; MemoryCardControl; forms | Actual form captures visually inspected | PASS |
| U27 | 1366×768 and 1920×1080 usability | Theme.Form; TableLayoutPanel Dock/percent styles | Minimum 900×640 grids tested; native screen modes not changed | NOT VERIFIED |
| U28 | Final audit and truthful outstanding issues | REQUIREMENTS; TEST_PLAN; DELIVERY | Review source document again against all rows | PASS |
