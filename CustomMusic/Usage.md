# What is this?
The setting "Random Music" lets you import your own songs into the game which are shuffled, similar to how Ocarina of Time and Majora's Mask have custom music for their randomizers.

# How does it work?
The files should satisfy the following:
- Be in the mp3 format.
- (Optional) Have metadata regarding the song name, the album, and the artist.

Place these in this folder, and run the generate_tracks.py script, which requires tinytag.  This will generate a Tracks.json file.  This will generate a dictionary of file name to title to be used in the game.  If there is no metadata for the file other than the name, the game will merely print the file's name as a reference in-game.

# Does this play the game's original music?
Unfortunately, I am the bad dev, and I don't know how to grab the assets from the game to play these as they aren't in the Resources folder to be loaded.  If you want the original game's music, I suggest grabbing the Lunacid's OST and putting the files in this folder to be played.  Its very weird but doable.  If you think you have an idea of grabbing the files from the AudioClip files internally without much hassle let me know; I'd rather do that.

# The tracks are too loud!
This is something that varies between music and is better solved on your own.  I used MP3Gain for that purpose.  For linux users you can get QMP3Gain for a GUI.  I suggest ~89% volume, but you can modify the music to your liking.