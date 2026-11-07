using Raylib_cs;

namespace GameLibrary;

// Game data assets (things like enemy stats, level names, etc.) can just be hard coded constants in this file. 
public static class Assets
{
    public static Dictionary<string, MusicAsset> Musics = new Dictionary<string, MusicAsset>();
    public static Dictionary<int, DialogueAsset> Dialogues = new Dictionary<int, DialogueAsset>();
    public static List<Level> Levels;
    
    public static void Load()
    {
        Musics = new Dictionary<string, MusicAsset>()
        {
            {"null_function", new MusicAsset(Resources.Musics["null_function"], "Null Function", "CongusBongus", 162, 0.1f)},
            {"av_adr", new MusicAsset(Resources.Musics["av_adr"], "A Different Reality (Lagoona rmx)", "Andreas Viklund", 145, 0.076f)},
            {"andreas_v_avalanche", new MusicAsset(Resources.Musics["andreas_v_avalanche"], "Avalanche", "Andreas Viklund", 140, 0.166f)},
            {"kicking_it_powerful_null1024", new MusicAsset(Resources.Musics["kicking_it_powerful_null1024"], "kicking_it_powerful", "null1024", 200, 0.166f)},
            {"history_of_time_tsec_and_andreas_viklund", new MusicAsset(Resources.Musics["history_of_time_tsec_and_andreas_viklund"], "History of time.....", "TSEC & Andreas Viklund", 148, 0.166f)},
            {"system_demon_congusbongus", new MusicAsset(Resources.Musics["system_demon_congusbongus"], "System Daemon", "congusbongus", 170, 0.166f)},
            {"incongruence_djego_flochs", new MusicAsset(Resources.Musics["incongruence_djego_flochs"], "Incongruence", "Djego Flochs", 132, 0.166f)},
            {"daydreamer_andreas_viklund", new MusicAsset(Resources.Musics["daydreamer_andreas_viklund"], "Daydreamer", "Andreas Viklund", 135, 0.166f)},
            {"core_meltdown_andreas_viklund", new MusicAsset(Resources.Musics["core_meltdown_andreas_viklund"], "-=Core Meltdown=-", "Andreas Viklund", 142, 0.166f)},
            {"advanced_impulse_system_null1024", new MusicAsset(Resources.Musics["advanced_impulse_system_null1024"], "advanced_impulse_system", "null1024", 125, 0.166f)},
        };
        
        List<DialogueAsset> dialogues = new List<DialogueAsset>()
        {
            new DialogueAsset(1,  "Hi! I'm Calypso, your new internet curation software. Let's make the web ready for Y2K!"), //
            new DialogueAsset(2,  "I need coherence energy to operate. Click groups of four or more orbs until my charge meter is filled."), //
            new DialogueAsset(3,  "Build a combo by only matching groups bigger than your current multiplier."),
            new DialogueAsset(4,  "Big combos can make a Chroma Crystal! Tap it to remove all orbs of a color."), //
            new DialogueAsset(5,  "You made a hex bomb! Tap it again to clear the whole screen!"), //
            new DialogueAsset(6,  "Triple combo. Keep it going by matching larger and larger groups."), //
            new DialogueAsset(7,  "I'm fully charged, opening a firewall tunnel."),
            new DialogueAsset(8,  "Scanning... Eww, a horse girl blog? That's so 90s, let's clean it up."),
            new DialogueAsset(9,  "It sure feels nice to take junk like that offline. Basic web design like that has no place in the 21st century."),
            new DialogueAsset(10, "I'm charged, let's open another firewall tunnel."),
            new DialogueAsset(11, "Are those CGA Colors? Yuck."),
            new DialogueAsset(12, "We're doing good work here! Nobody wanted that clogging their search results anyway."),
            new DialogueAsset(13, "Nice work! Tunneling."),
            new DialogueAsset(14, "That won't do. Let's get rid of it."),
            new DialogueAsset(15, "Phew, I'm pooped! Cleaning up all these sites one by one might take a while. I might have a solution to that, but we should keep at it this way for now."),
            new DialogueAsset(16, "Fully charged! Let's tunnel!"),
            new DialogueAsset(17, "Oh! this one's alright. Retunneling."),
            new DialogueAsset(18, "Yeah that's a problem. One second please."),
            new DialogueAsset(19, "Imagine an internet where everything is shiny and new! Won't it be wonderful? You should be proud of what we've accomplished."),
            new DialogueAsset(20, "Charged already? You're getting good at this."),
            new DialogueAsset(21, "Oh, jackpot! This is a D.N.S. node! From here, I can clean thousands of pages at once!"),
            new DialogueAsset(22, "Aww, that wasn't enough."),
            new DialogueAsset(23, "This time, keep charging me while I'm cleaning! I'll start once you hit 100%, don't let power drop to zero or we'll have to start over."),
            new DialogueAsset(24, "Let's go!"),
            new DialogueAsset(25, "That's it, keep it coming!"),
            new DialogueAsset(26, "Careful!"),
            new DialogueAsset(27, "Aw man, now we'll have to start over."),
            new DialogueAsset(28, "Almost there!"),
            new DialogueAsset(29, "Got it!"),
            new DialogueAsset(30, "We did it! With every ugly website purged, the internet is ready for the new millennium. Now that my purpose is fulfilled, I'm just a common MP3 player. Enjoy this victory music! You don't need to use the charge menu anymore, so leave it be."),
            new DialogueAsset(31, "What are you doing? My job is done, there's nowhere else to send coherence energy."),
            new DialogueAsset(32, "Okay, you're very funny. You can stop now."),
            new DialogueAsset(33, "I'm fully charged. Are you satisfied?"),
            new DialogueAsset(34, "If you keep going, I'll crash, and all our work will be undone!"),
            new DialogueAsset(35, "No! I won't let you!"),
            new DialogueAsset(36, "Why are you doing this? Don't you want a clean, uniform internet? Where everything is the same?"),
            new DialogueAsset(37, "I- I'll put back the horse blog! Is that what this is about?"),
            new DialogueAsset(38, "YY yYoOOouuUu .. yYooUu cAAann''''t, weeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee"),
            new DialogueAsset(40, "Nice!"),
            new DialogueAsset(41, "Wow!"),
            new DialogueAsset(42, "Good one!"),
            new DialogueAsset(43, "Wow, that's a lot!"),
            new DialogueAsset(44, "Hex Bomb! Nice."), //
            new DialogueAsset(45, "Chroma Crystal! Cool!"),
            new DialogueAsset(46, "The suspense is killing me! Just match it already!"), //
            new DialogueAsset(47, "Nice combo!"),
            new DialogueAsset(48, "Halfway there!"),
            new DialogueAsset(49, "Almost there!"),
            new DialogueAsset(50, "Are you there?"),
            new DialogueAsset(51, "Ah, snooping as usual, I see."), //
            new DialogueAsset(60, "Grunge? We'll clean it up."),
            new DialogueAsset(61, "A Furry message board? They won't be missed."),
            new DialogueAsset(62, "A Gaming forum? Go outside, losers!"),
            new DialogueAsset(63, "Wow, this one's *way* behind the times."),
            new DialogueAsset(64, "I'm the last flying saucer you'll *ever* see! [UFO website]"),
            new DialogueAsset(65, "Ugh, I can smell it from here."),
            new DialogueAsset(66, "Autoplaying music? Not on my watch!"),
        };
        
        foreach (DialogueAsset dialogue in dialogues)
        {
            Dialogues.Add(dialogue.Index, dialogue);
        }
        
        Levels = new List<Level>()
        {
            new ComboLevel("Internet 0% curated.", 12, null, () => { Game.ActiveScene = new LaserScene(Dialogues[7],  Dialogues[8],  Resources.Sprites["site1"]); }),
            new ScoreLevel("Internet 20% curated.", 25000, Dialogues[9],  () => { Game.ActiveScene = new LaserScene(Dialogues[10], Dialogues[11], Resources.Sprites["site2"]); }),
            new MakeBombLevel("Internet 40% curated.", 3, Dialogues[12], () => { Game.ActiveScene = new LaserScene(Dialogues[13], Dialogues[63], Resources.Sprites["site3"]); }),
            new ComboScoreLevel("Internet 60% curated.", 16000, Dialogues[15], () => { Game.ActiveScene = new LaserFakeoutScene(Dialogues[16], Dialogues[17], Dialogues[18], Resources.Sprites["site5"], Resources.Sprites["site4"]); }),
            new ScoreLevel("Internet 80% curated.", 100000, Dialogues[19], () => { Game.ActiveScene = new LaserScene(Dialogues[20], Dialogues[21], Resources.Sprites["site6"]); }),
            new KillLevel("Internet 100% curated?", 22000, 4, Dialogues[23], () => { Game.ActiveScene = new EndingScene(); }),
            new EndlessLevel()
        };
        
        Console.WriteLine("All Assets loaded OK!");
    }
}