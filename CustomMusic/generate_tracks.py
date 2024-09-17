from typing import Dict

from tinytag import TinyTag
import os
import json


def generate_tracks():
    songs = [song for song in os.listdir() if os.path.isfile(song) and ".mp3" in song]
    output: Dict[str, str] = {}
    for song in songs:
        tag = TinyTag.get(song)
        full_track = tag.title
        if tag.album != "":
            full_track = tag.album + ": " + full_track
        if tag.artist != "":
            full_track += " - " + tag.artist
        output[song.replace(".mp3","")] = full_track
    with open('Tracks.json', 'w') as fp:
        json.dump(output, fp)


if __name__ == '__main__':
    generate_tracks()
