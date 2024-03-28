using System.Collections.Generic;
using UnityEngine;

namespace LunacidAP.Data
{
    public class LoreDialogue
    {
        private static Vector3 neutral = new Vector3(0, 0, 0);

        public static Dictionary<Goal, string> GoalHint = new(){
            {Goal.AnyEnding, "Seek either the slumbering creature of moonlight, or the accursed Gates of Tartarus; they are the last existing door."},
            {Goal.EndingA, "Seek the slumbering creature of moonlight, they are the last existing door."},
            {Goal.EndingB, "Seek the accursed Gates of Tartarus, they are the last existing door."},
            {Goal.EndingCD, "Seek only the tranquil waters and contemplation, for it is the only escape."},
            {Goal.EndingE, "Seek the sumbling creature of moonlight, and the magicks of this world; they are the last existing door."}
        };

        public static Dialog.Line[] CliveLine = new[]{
            new Dialog.Line(){
                value = "Hey /c!",
                NXT = 3,
                NEW_SAID = 1,
                exprssion = neutral,
                LOAD_LINE = 0,
                special = 0,
            },
            new Dialog.Line(){
                value = "How ya been?",
                NXT = 3,
                NEW_SAID = 1,
                exprssion = neutral,
                LOAD_LINE = 0,
                special = 0,
            },
            new Dialog.Line(){
                value = "Well Hi!",
                NXT = 3,
                NEW_SAID = 1,
                exprssion = neutral,
                LOAD_LINE = 0,
                special = 0,
            },
            new Dialog.Line(){
                value = "Hey sonny, you look a bit lost.  Did you ever ask me about what you were looking for?",
                NXT = 7,
                NEW_SAID = 1,
                exprssion = new Vector3(0, 100, 0),
                LOAD_LINE = 0,
                special = 3,
            },
            new Dialog.Line(){
                value = "Be careful out there.",
                NXT = -1,
                NEW_SAID = 1,
                exprssion = neutral,
                LOAD_LINE = 0,
                special = 3,
            },
            new Dialog.Line(){
                value = "Stay fleshy /c.",
                NXT = -1,
                NEW_SAID = 1,
                exprssion = neutral,
                LOAD_LINE = 0,
                special = 3,
            },
            new Dialog.Line(){
                value = "Oh, my bad, another time then.",
                NXT = -1,
                NEW_SAID = 1,
                exprssion = neutral,
                LOAD_LINE = 0,
                special = 3,
            },
            new Dialog.Line(){
                value = "Hah well out with it.  Might have a tale about it.",
                NXT = 2,
                NEW_SAID = 3,
                exprssion = neutral,
                LOAD_LINE = 0,
                special = 3,
            },
            new Dialog.Line(){
                value = "Hmm...",
                NXT = 2,
                NEW_SAID = 3,
                exprssion = neutral,
                LOAD_LINE = 0,
                special = 3,
            },
            new Dialog.Line(){
                value = "I'm not sure if that kind of thing exists...does it?",
                NXT = 2,
                NEW_SAID = 3,
                exprssion = neutral,
                LOAD_LINE = 0,
                special = 0,
            },
            new Dialog.Line(){
                value = "Could repeat it to me later.",
                NXT = -1,
                NEW_SAID = 3,
                exprssion = neutral,
                LOAD_LINE = 0,
                special = 0,
            },
            
            new Dialog.Line(){
                value = "I'm not sure if that kind of thing exists...does it?",
                NXT = 2,
                NEW_SAID = 3,
                exprssion = neutral,
                LOAD_LINE = 0,
                special = 0,
            },
            
            new Dialog.Line(){
                value = "I'm not sure if that kind of thing exists...does it?",
                NXT = 2,
                NEW_SAID = 3,
                exprssion = neutral,
                LOAD_LINE = 0,
                special = 0,
            },
            
            new Dialog.Line(){
                value = "I'm not sure if that kind of thing exists...does it?",
                NXT = 2,
                NEW_SAID = 3,
                exprssion = neutral,
                LOAD_LINE = 0,
                special = 0,
            },
        };

    }
}