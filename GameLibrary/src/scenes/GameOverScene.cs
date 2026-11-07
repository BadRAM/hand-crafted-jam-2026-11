// using Raylib_cs;
//
// namespace GameLibrary;
//
// public class GameOverScene : Scene
// {
//     private Music _cutsceneAudio = Resources.Musics["self_destruct_aborted"];
//
//     private string _script =
// """
// Having averted the lockdown and
// consumed the entire crew of EFF
// Mill Bay, the mysterious
// creature boarded a lifeboat and
// was jettisoned safely to earth.
// By the time Mill Bay's black 
// box recorder was recovered and 
// the cause of her demise became 
// known, the creature had 
// vanished into the wilderness. 
// We can only hope that it cannot
// propagate by itself.
//
//
// Disaster aboard EFF Mill Bay
// features the voice talents of:
// (In order of appearance)
//
// Saskia "Sky" Cseh as
//   Captain Janet Windlass
//
// "Rune" as
//   Earth Traffic Controller
//
// Luke Vaughan as
//   Petty Officer Richard Irwin
//
// Nick Mehrer as
//   Lt Cmdr. Brewster Scott
//
// Ciara Sanders as
//   Ens. Kathleen Reisman
//
// David Mehrer as
//   Chief Eng. Andrew Payton
//
// Gracey Mehrer as
//   Ship Announcement System
//
//
// And was made possible by the
// significant other talents of:
//
// AbrahamGames1102 - Art
// David Mehrer & Lostyy - Sound
// Luke Vaughan - Code
//
//
//
//
//
//
//
//
//
//
//        Thanks for Playing!
// """.ReplaceLineEndings("\n");
//
//     public GameOverScene()
//     {
//         Time.Reset();
//         _cutsceneAudio.Looping = false;
//         Raylib.PlayMusicStream(_cutsceneAudio);
//     }
//     
//     public override void Update()
//     {
//         Raylib.UpdateMusicStream(_cutsceneAudio);
//         Raylib.ClearBackground(Color.Black);
//         Raylib.DrawTexture(Resources.Sprites["menubg"], 0, 0, Color.White);
//         Resources.Sprites["menubg"].Draw(0, 0);
//         Raylib.DrawRectangle(35, 0, 250, 240, new Color(0,0,0,200));
//         ImGui.DrawText(_script, 37, (int)(240 - Math.Min(Time.Scaled, 80) * 10), 10);
//     }
// }